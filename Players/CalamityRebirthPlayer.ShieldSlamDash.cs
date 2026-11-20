using CalamityRebirth.Content.Particles;
using CalamityRebirth.Content.Projectiles.Typeless;
using CalamityRebirth.Enums;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Players
{
    /// <summary>
    /// 阿斯加德系列盾牌冲刺、被动防御与联机表现。
    /// </summary>
    public partial class CalamityRebirthPlayer : ModPlayer
    {
        /// <summary>冲刺速度高于过程上限时的每帧衰减。</summary>
        private const float ShieldSpeedDeceleration = 0.985f;
        /// <summary>冲刺速度落入跑步区间后的每帧衰减。</summary>
        private const float ShieldRunSpeedDeceleration = 0.94f;
        /// <summary>基础冲刺的过程速度上限。</summary>
        private const float DefaultMidDashSpeed = 12f;
        /// <summary>冲刺结束后的冷却帧数。</summary>
        private const int ShieldSlamCooldown = 30;
        /// <summary>方向键双击判定窗口。</summary>
        private const int DoubleTapWindow = 15;
        /// <summary>冲刺命中后玩家获得的碰撞免疫帧数。</summary>
        private const int ShieldHitImmunityFrames = 12;
        /// <summary>冲刺命中后敌人至少获得的冲刺免疫帧数。</summary>
        private const int MinShieldNPCImmunityFrames = 12;
        /// <summary>远端冲刺表现的最大兜底帧数。</summary>
        private const int RemoteDashMaxFrames = 120;
        /// <summary>本帧是否装备阿斯加德之英勇。</summary>
        public bool asgardsValor;
        /// <summary>本帧是否装备阿斯加德之庇护。</summary>
        public bool asgardianAegis;
        /// <summary>本帧是否装备华丽盾。</summary>
        public bool ornateShield;
        /// <summary>本帧是否装备海之盾。</summary>
        public bool shieldOfTheOcean;
        /// <summary>本帧是否装备极乐之庇护。</summary>
        public bool elysianAegis;
        /// <summary>本帧是否允许切换守护状态。</summary>
        public bool elysianAegisPower;
        /// <summary>守护状态是否开启。</summary>
        public bool elysianGuard;
        /// <summary>当前护盾充能，五为充满、零为耗尽。</summary>
        public float shieldInvinc = 5f;
        /// <summary>本帧是否装备阿巴顿。</summary>
        public bool abaddon;
        /// <summary>阿巴顿暴击爆炸的剩余冷却帧数。</summary>
        private int abaddonCritCooldown;
        /// <summary>当前生效的盾牌冲刺类型。</summary>
        public ShieldSlamDash shieldSlamDash;
        /// <summary>本地玩家累计的屏幕震动强度。</summary>
        public float GeneralScreenShakePower;
        /// <summary>本地冲刺已经持续的帧数。</summary>
        private int shieldSlamDashElapsed;
        /// <summary>本地冲刺剩余冷却帧数。</summary>
        private int shieldSlamDashCooldown;
        /// <summary>左右方向键的双击窗口，正数表示右键、负数表示左键。</summary>
        private int shieldSlamDashTimeMod;
        /// <summary>本次冲刺是否已经触发首次震屏。</summary>
        private bool shieldSlamDashScreenShook;
        /// <summary>本地冲刺的表现相位。</summary>
        private int shieldSlamDashTime;
        /// <summary>远端玩家正在表现的冲刺类型。</summary>
        private ShieldSlamDash remoteShieldSlamDash;
        /// <summary>远端冲刺表现已经持续的帧数。</summary>
        private int remoteShieldSlamElapsed;
        /// <summary>远端冲刺的表现相位。</summary>
        private int remoteShieldSlamTime;
        /// <summary>仅以更高优先级覆盖当前冲刺，避免饰品更新顺序改变顶点结果。</summary>
        public void ProvideShieldSlamDash(ShieldSlamDash dash)
        {
            if (dash > shieldSlamDash)
                shieldSlamDash = dash;
        }
        /// <summary>在每帧装备效果重置时清空阿斯加德系列标记与当前冲刺。</summary>
        public void ResetShieldSlamEffects()
        {
            asgardsValor = false;
            asgardianAegis = false;
            ornateShield = false;
            shieldOfTheOcean = false;
            elysianAegis = false;
            elysianAegisPower = false;
            abaddon = false;
            shieldSlamDash = ShieldSlamDash.None;
        }
        /// <summary>死亡时清空全部冲刺计时、冷却与远端表现状态。</summary>
        public void ResetShieldSlamDashState()
        {
            shieldSlamDashElapsed = 0;
            shieldSlamDashCooldown = 0;
            shieldSlamDashTimeMod = 0;
            shieldSlamDashScreenShook = false;
            shieldSlamDashTime = 0;
            remoteShieldSlamDash = ShieldSlamDash.None;
            remoteShieldSlamElapsed = 0;
            remoteShieldSlamTime = 0;
            GeneralScreenShakePower = 0f;
            elysianGuard = false;
            shieldInvinc = 5f;
            abaddonCritCooldown = 0;
        }
        /// <summary>结算盾牌冲刺链全部常驻效果。</summary>
        public void ApplyShieldSlamAccessoryEffects()
        {
            if (abaddonCritCooldown > 0)
                abaddonCritCooldown--;
            if (ornateShield)
                ApplyOrnateShieldEffects();
            if (shieldOfTheOcean && Collision.DrownCollision(
                Player.position, Player.width, Player.height, Player.gravDir))
                Player.statDefense += 5;
            if (abaddon)
            {
                Player.GetCritChance(DamageClass.Generic) += 8;
                AddImmunity(Player, "CalamityMod", "BrimstoneFlames");
                AddImmunity(Player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
                AddImmunity(Player, "CalRD", "BrimstoneFlames");
            }
            if (asgardsValor)
                ApplyAsgardsValorEffects();
            if (elysianAegis)
                ApplyElysianAegisEffects();
            if (elysianAegisPower)
                UpdateElysianGuard();
            else
                elysianGuard = false;
            if (asgardianAegis)
                ApplyAsgardianAegisEffects();
        }
        /// <summary>应用华丽盾的生命、再生、残血防御与冲刺禁用。</summary>
        private void ApplyOrnateShieldEffects()
        {
            Player.dashType = 0;
            Player.lifeRegen += 2;
            Player.statLifeMax2 += 20;
            if (Player.statLife < Player.statLifeMax2 * 0.25f)
                Player.statDefense += 4;
        }
        /// <summary>应用阿斯加德之英勇的生命、移动与减益免疫。</summary>
        private void ApplyAsgardsValorEffects()
        {
            Player.dashType = 0;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.statLifeMax2 += 50;
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Weak] = true;
            Player.buffImmune[BuffID.BrokenArmor] = true;
            Player.buffImmune[BuffID.Bleeding] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Slow] = true;
            Player.buffImmune[BuffID.Confused] = true;
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.Darkness] = true;
            Player.buffImmune[BuffID.WindPushed] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.Daybreak] = true;
            Player.buffImmune[BuffID.OnFire] = true;
            Player.buffImmune[BuffID.OnFire3] = true;
            AddValorImmunities(Player);
            ApplySubmergedEndurance();
        }
        /// <summary>应用极乐之庇护的生命、恢复、火焰免疫与守护状态标记。</summary>
        private void ApplyElysianAegisEffects()
        {
            Player.dashType = 0;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.statLifeMax2 += 30;
            Player.lifeRegen += 3;
            Player.buffImmune[BuffID.CursedInferno] = true;
            Player.buffImmune[BuffID.ShadowFlame] = true;
            Player.buffImmune[BuffID.Daybreak] = true;
            Player.buffImmune[BuffID.OnFire] = true;
            Player.buffImmune[BuffID.OnFire3] = true;
            AddImmunity(Player, "CalamityMod", "HolyFlames");
            AddImmunity(Player, "CalamityMod", "BrimstoneFlames");
            AddImmunity(Player, "CalamityMod", "WeakPetrification");
            AddImmunity(Player, "CalamityMod", "Nightwither");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "GlacialState");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "HolyLight");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "WhisperingDeath");
            AddImmunity(Player, "CalRD", "GlacialState");
            AddImmunity(Player, "CalRD", "HolyFlames");
            AddImmunity(Player, "CalRD", "HolyLight");
            AddImmunity(Player, "CalRD", "BrimstoneFlames");
        }
        /// <summary>按护盾充能更新守护状态的伤害、暴击、仇恨、防御与移动惩罚。</summary>
        private void UpdateElysianGuard()
        {
            bool changed = false;
            if (elysianGuard)
            {
                float previous = shieldInvinc;
                shieldInvinc = Math.Max(0f, shieldInvinc - 0.08f);
                changed = previous != shieldInvinc;
                if (changed && shieldInvinc <= 0f && Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI);
                float consumed = 5f - shieldInvinc;
                Player.GetDamage(DamageClass.Generic) += consumed * 0.03f;
                Player.GetCritChance(DamageClass.Generic) += (int)(consumed * 2f);
                Player.aggro += (int)(consumed * 220f);
                Player.statDefense += (int)(consumed * 4f);
                Player.moveSpeed *= 0.85f;
                if (Player.mount.Active)
                    elysianGuard = false;
            }
            else
            {
                float previous = shieldInvinc;
                shieldInvinc = Math.Min(5f, shieldInvinc + 0.08f);
                changed = previous != shieldInvinc;
                if (changed && shieldInvinc >= 5f && Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI);
            }
            if (!changed)
                return;
            if (Main.rand.NextBool(2))
                SpawnGuardChargeDust(DustID.CopperCoin, 1.5707964f, 4f, 5, 11);
            if (Main.rand.NextBool(2))
                SpawnGuardChargeDust(DustID.GoldCoin, -1.5707964f, 2f, 12, 12);
        }
        /// <summary>生成守护充能时环绕玩家的金币色粒子。</summary>
        private void SpawnGuardChargeDust(int dustType, float angle, float speed, int minRadius, int maxRadius)
        {
            Vector2 direction = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
            Dust dust = Dust.NewDustPerfect(Player.Center - direction * maxRadius, dustType);
            dust.noGravity = true;
            dust.position = Player.Center - direction * Main.rand.Next(minRadius, maxRadius + 1);
            dust.velocity = direction.RotatedBy(angle) * speed;
            dust.scale = 0.5f + Main.rand.NextFloat();
            dust.fadeIn = 0.5f;
        }
        /// <summary>应用阿斯加德之庇护的生命、恢复、残血防御与广泛减益免疫。</summary>
        private void ApplyAsgardianAegisEffects()
        {
            Player.dashType = 0;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.statLifeMax2 += 40;
            Player.lifeRegen += 5;
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Weak] = true;
            Player.buffImmune[BuffID.BrokenArmor] = true;
            Player.buffImmune[BuffID.Bleeding] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Slow] = true;
            Player.buffImmune[BuffID.Confused] = true;
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.Darkness] = true;
            Player.buffImmune[BuffID.WindPushed] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.Daybreak] = true;
            Player.buffImmune[BuffID.OnFire] = true;
            Player.buffImmune[BuffID.OnFire3] = true;
            Player.buffImmune[BuffID.CursedInferno] = true;
            Player.buffImmune[BuffID.ShadowFlame] = true;
            Player.buffImmune[BuffID.Venom] = true;
            Player.buffImmune[BuffID.Webbed] = true;
            Player.buffImmune[BuffID.Blackout] = true;
            AddAsgardianImmunities(Player);
            ApplySubmergedEndurance();
            if (Player.statLife < Player.statLifeMax2 * 0.25f)
                Player.statDefense += 10;
        }
        /// <summary>为阿斯加德之英勇追加对应分支的减益免疫。</summary>
        private static void AddValorImmunities(Player player)
        {
            AddImmunity(player, "CalamityMod", "SearingLava");
            AddImmunity(player, "CalamityMod", "HolyFlames");
            AddImmunity(player, "CalamityMod", "BrimstoneFlames");
            AddImmunity(player, "CalamityModClassicPreTrailer", "GlacialState");
            AddImmunity(player, "CalamityModClassicPreTrailer", "HolyLight");
            AddImmunity(player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
            AddImmunity(player, "CalRD", "GlacialState");
            AddImmunity(player, "CalRD", "HolyFlames");
            AddImmunity(player, "CalRD", "HolyLight");
            AddImmunity(player, "CalRD", "BrimstoneFlames");
        }
        /// <summary>为阿斯加德之庇护追加广泛的外部减益免疫。</summary>
        private static void AddAsgardianImmunities(Player player)
        {
            AddImmunity(player, "CalamityMod", "HolyFlames");
            AddImmunity(player, "CalamityMod", "BrimstoneFlames");
            AddImmunity(player, "CalamityMod", "WeakPetrification");
            AddImmunity(player, "CalamityMod", "Nightwither");
            AddImmunity(player, "CalamityMod", "GodSlayerInferno");
            AddImmunity(player, "CalamityMod", "ArmorCrunch");
            AddImmunity(player, "CalamityMod", "SulphuricPoisoning");
            AddImmunity(player, "CalamityMod", "BrainRot");
            AddImmunity(player, "CalamityMod", "BurningBlood");
            AddImmunity(player, "CalamityModClassicPreTrailer", "GlacialState");
            AddImmunity(player, "CalamityModClassicPreTrailer", "HolyLight");
            AddImmunity(player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
            AddImmunity(player, "CalamityModClassicPreTrailer", "WhisperingDeath");
            AddImmunity(player, "CalamityModClassicPreTrailer", "GodSlayerInferno");
            AddImmunity(player, "CalamityModClassicPreTrailer", "ArmorCrunch");
            AddImmunity(player, "CalRD", "GlacialState");
            AddImmunity(player, "CalRD", "HolyFlames");
            AddImmunity(player, "CalRD", "HolyLight");
            AddImmunity(player, "CalRD", "BrimstoneFlames");
            AddImmunity(player, "CalRD", "WhisperingDeath");
            AddImmunity(player, "CalRD", "GodSlayerInferno");
            AddImmunity(player, "CalRD", "ArmorCrunch");
        }
        /// <summary>向玩家写入指定外部减益的免疫。</summary>
        private static void AddImmunity(Player player, string modName, string buffName)
        {
            int type = CalamityCompatUtil.GetBuffType(modName, buffName);
            if (type > 0)
                player.buffImmune[type] = true;
        }
        /// <summary>在完整溺水碰撞环境中提供十二百分点伤害减免。</summary>
        private void ApplySubmergedEndurance()
        {
            if (Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir))
                Player.endurance += 0.12f;
        }
        /// <summary>每帧执行盾牌冲刺状态机。</summary>
        public void ShieldSlamDashMovement()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                UpdateRemoteShieldSlamVisuals();
                return;
            }
            if (shieldSlamDashCooldown > 0)
                shieldSlamDashCooldown--;
            if (shieldSlamDashTimeMod != 0)
                shieldSlamDashTimeMod -= shieldSlamDashTimeMod > 0 ? 1 : -1;
            bool startedThisFrame = false;
            if (shieldSlamDashElapsed <= 0)
            {
                UpdateShieldSlamInput();
                if (shieldSlamDashElapsed <= 0)
                    return;
                startedThisFrame = true;
            }
            BlockVanillaDash();
            if (startedThisFrame)
                return;
            ResolveShieldSlamHits();
            UpdateShieldSlamMotion();
        }
        /// <summary>把本地累积的震动强度转换为屏幕偏移并逐渐衰减。</summary>
        public override void ModifyScreenPosition()
        {
            if (Player.whoAmI != Main.myPlayer || GeneralScreenShakePower <= 0f)
                return;
            Main.screenPosition += Main.rand.NextVector2Circular(GeneralScreenShakePower, GeneralScreenShakePower);
            GeneralScreenShakePower = MathHelper.Clamp(GeneralScreenShakePower - 0.185f, 0f, 20f);
        }
        /// <summary>守护状态额外降低跑动加速度与最大跑速。</summary>
        public void ApplyShieldSlamRunSpeedEffects()
        {
            if (!elysianGuard)
                return;
            Player.runAcceleration *= 0.85f;
            Player.maxRunSpeed *= 0.85f;
        }
        /// <summary>读取守护状态快捷键；骑乘时禁止开启。</summary>
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ShieldSlamKeybinds.ElysianHotKey?.JustPressed == true
                && elysianAegisPower && !Player.mount.Active)
                elysianGuard = !elysianGuard;
        }
        /// <summary>更新远端玩家的拖尾表现，结束包丢失时自动过期。</summary>
        private void UpdateRemoteShieldSlamVisuals()
        {
            if (Main.dedServ || remoteShieldSlamElapsed <= 0)
                return;
            if (!Player.active || Player.dead)
            {
                ReceiveShieldSlamDash(ShieldSlamDash.None);
                return;
            }
            SpawnShieldSlamTrailDust(remoteShieldSlamDash, ref remoteShieldSlamTime);
            if (++remoteShieldSlamElapsed > RemoteDashMaxFrames)
                ReceiveShieldSlamDash(ShieldSlamDash.None);
        }
        /// <summary>接收其他玩家的冲刺开始或结束广播。</summary>
        public void ReceiveShieldSlamDash(ShieldSlamDash dash)
        {
            remoteShieldSlamDash = dash;
            remoteShieldSlamElapsed = dash == ShieldSlamDash.None ? 0 : 1;
            remoteShieldSlamTime = 0;
        }
        /// <summary>接收其他玩家的撞击广播并在目标位置生成表现粒子。</summary>
        public void ReceiveShieldSlamDashHit(ShieldSlamDash dash, int npcIndex)
        {
            if (npcIndex < 0 || npcIndex >= Main.maxNPCs || !Main.npc[npcIndex].active)
                return;
            SpawnShieldSlamHitParticles(dash, Main.npc[npcIndex].Center);
        }
        /// <summary>广播本地冲刺开始或结束。</summary>
        private void SendShieldSlamDashPacket(ShieldSlamDash dash)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write(MsgShieldSlamDash);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)dash);
            if (Main.netMode == NetmodeID.Server)
                packet.Send(-1, Player.whoAmI);
            else
                packet.Send();
        }
        /// <summary>广播本地撞击，供其他客户端同步粒子表现。</summary>
        private void SendShieldSlamHitPacket(ShieldSlamDash dash, int npcIndex)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write(MsgShieldSlamDashHit);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)dash);
            packet.Write((short)npcIndex);
            if (Main.netMode == NetmodeID.Server)
                packet.Send(-1, Player.whoAmI);
            else
                packet.Send();
        }
        /// <summary>读取左右方向键十五帧内的同向双击。</summary>
        private void UpdateShieldSlamInput()
        {
            if (shieldSlamDash == ShieldSlamDash.None || shieldSlamDashCooldown > 0 || Player.mount.Active)
                return;
            bool rightInput = Player.controlRight && Player.releaseRight;
            bool leftInput = Player.controlLeft && Player.releaseLeft;
            if (rightInput)
            {
                if (shieldSlamDashTimeMod > 0)
                {
                    shieldSlamDashTimeMod = 0;
                    StartShieldSlamDash(1);
                }
                else
                    shieldSlamDashTimeMod = DoubleTapWindow;
            }
            else if (leftInput)
            {
                if (shieldSlamDashTimeMod < 0)
                {
                    shieldSlamDashTimeMod = 0;
                    StartShieldSlamDash(-1);
                }
                else
                    shieldSlamDashTimeMod = -DoubleTapWindow;
            }
        }
        /// <summary>设置水平冲刺速度，并在斜前方有实心块时减半。</summary>
        private void StartShieldSlamDash(int direction)
        {
            ShieldSlamProfile profile = GetShieldSlamProfile(shieldSlamDash);
            Player.velocity = new Vector2(direction * profile.StartSpeed, Player.velocity.Y);
            Point upwardPoint = (Player.Center + new Vector2(direction * Player.width / 2f + 2f
                , Player.gravDir * -Player.height / 2f + Player.gravDir * 2f)).ToTileCoordinates();
            Point aheadPoint = (Player.Center + new Vector2(direction * Player.width / 2f + 2f, 0f))
                .ToTileCoordinates();
            if (WorldGen.SolidOrSlopedTile(upwardPoint.X, upwardPoint.Y)
                || WorldGen.SolidOrSlopedTile(aheadPoint.X, aheadPoint.Y))
                Player.velocity.X *= 0.5f;
            Player.dashDelay = -1;
            shieldSlamDashElapsed = 1;
            shieldSlamDashTime = 0;
            shieldSlamDashScreenShook = false;
            SendShieldSlamDashPacket(shieldSlamDash);
        }
        /// <summary>更新拖尾、朝向和两级速度衰减，并在速度落入跑步区间时结束冲刺。</summary>
        private void UpdateShieldSlamMotion()
        {
            SpawnShieldSlamTrailDust(shieldSlamDash, ref shieldSlamDashTime);
            Player.vortexStealthActive = false;
            if (Player.velocity.X != 0f)
                Player.ChangeDir(Math.Sign(Player.velocity.X));
            ShieldSlamProfile profile = GetShieldSlamProfile(shieldSlamDash);
            float runSpeed = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
            if (Math.Abs(Player.velocity.X) > profile.MidDashSpeed)
                Player.velocity.X *= ShieldSpeedDeceleration;
            else if (Math.Abs(Player.velocity.X) > runSpeed)
                Player.velocity.X *= ShieldRunSpeedDeceleration;
            else
                EndShieldSlamDash(runSpeed);
        }
        /// <summary>按冲刺类型生成护盾染色的拖尾尘。</summary>
        private void SpawnShieldSlamTrailDust(ShieldSlamDash dash, ref int time)
        {
            switch (dash)
            {
                case ShieldSlamDash.AsgardianAegis:
                {
                    time += 2;
                    float radiusFactor = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(2f, 2.5f, time, true));
                    for (int i = 0; i < 3; i++)
                    {
                        float angle = Player.velocity.ToRotation() + time / 5f;
                        float radius = (15f + MathF.Cos(time / 3f) * 12f) * radiusFactor;
                        Vector2 position = Player.Center - Player.velocity * 2f;
                        position += angle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * radius;
                        Dust ring = Dust.NewDustPerfect(position, Main.rand.NextBool(5)
                            ? DustID.GiantCursedSkullBolt : 295);
                        ring.alpha = 220;
                        ring.noGravity = true;
                        ring.velocity = Player.velocity * 0.8f;
                        ring.scale = Main.rand.NextFloat(1.7f, 2f);
                        ring.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        Dust trail = Dust.NewDustPerfect(
                            Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                                + Player.velocity * 1.5f
                            , Main.rand.NextBool(8) ? 180 : 295
                            , -Player.velocity.RotatedByRandom(MathHelper.ToRadians(30f))
                                * Main.rand.NextFloat(0.1f, 0.8f)
                            , 0, default, Main.rand.NextFloat(1.7f, 1.9f));
                        trail.alpha = 170;
                        trail.noGravity = true;
                        trail.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                    }
                    break;
                }
                case ShieldSlamDash.ElysianAegis:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Dust sacred = Dust.NewDustPerfect(
                            Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                                - Player.velocity * 1.2f
                            , Main.rand.NextBool(8) ? DustID.FireworkFountain_Yellow : 162
                            , -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f))
                                * Main.rand.NextFloat(0.1f, 0.8f)
                            , 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                        sacred.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        sacred.noGravity = sacred.type != DustID.FireworkFountain_Yellow;
                        sacred.fadeIn = 0.5f;
                        if (sacred.type == DustID.FireworkFountain_Yellow)
                        {
                            sacred.scale = Main.rand.NextFloat(0.8f, 1.2f);
                            sacred.velocity += new Vector2(0f, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                        }
                        Dust gold = Dust.NewDustPerfect(Player.Center
                            + Main.rand.NextVector2Circular(6f, 6f) - Player.velocity * 2f
                            , DustID.GoldFlame);
                        gold.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                        gold.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        gold.noGravity = true;
                    }
                    break;
                }
                case ShieldSlamDash.AsgardsValor:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Player.position + Vector2.UnitY * 4f
                            , Player.width, Player.height - 8
                            , Main.rand.NextBool() ? DustID.CrimsonTorch : DustID.OrangeTorch
                            , 0f, 0f, 0, default, 1.2f);
                        dust.velocity = -Player.velocity * Main.rand.NextFloat(0.1f, 0.75f);
                        dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                        dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        dust.noGravity = true;
                        if (Main.rand.NextBool())
                            dust.fadeIn = 0.1f;
                    }
                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustPerfect(Player.Center
                            + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                            - Player.velocity * 1.7f
                            , DustID.FireworkFountain_Yellow
                            , -Player.velocity * Main.rand.NextFloat(0.15f, 0.4f), 0, default, 0.5f);
                        dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                    }
                    break;
                }
                case ShieldSlamDash.OrnateShield:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust ice = Dust.NewDustPerfect(
                            Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                                - Player.velocity * 1.7f
                            , Main.rand.NextBool(8) ? 223 : 180
                            , -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f))
                                * Main.rand.NextFloat(0.1f, 0.8f)
                            , 0, default, Main.rand.NextFloat(0.6f, 0.8f));
                        ice.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        ice.noGravity = true;
                        ice.fadeIn = 0.5f;
                        if (ice.type == 180)
                            ice.scale = Main.rand.NextFloat(1.6f, 2.2f);
                    }
                    break;
                }
            }
        }
        /// <summary>复位原版冲刺状态、写入短冷却并结束广播。</summary>
        private void EndShieldSlamDash(float runSpeed)
        {
            shieldSlamDashCooldown = ShieldSlamCooldown;
            Player.dashDelay = 0;
            if (Player.velocity.X < 0f)
                Player.velocity.X = -runSpeed;
            else if (Player.velocity.X > 0f)
                Player.velocity.X = runSpeed;
            shieldSlamDashElapsed = 0;
            shieldSlamDashTime = 0;
            SendShieldSlamDashPacket(ShieldSlamDash.None);
        }
        /// <summary>在玩家前方构造略向外扩的冲刺命中框。</summary>
        private void ResolveShieldSlamHits()
        {
            ShieldSlamProfile profile = GetShieldSlamProfile(shieldSlamDash);
            if (profile.Damage <= 0)
                return;
            Rectangle hitArea = new(
                (int)(Player.position.X + Player.velocity.X * 0.5f - 4f),
                (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4f),
                Player.width + 8, Player.height + 8);
            int direction = Player.velocity.X == 0f ? Player.direction : Math.Sign(Player.velocity.X);
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[npc.type])
                    continue;
                if (npc.dontTakeDamage || npc.friendly || npc.immune[Player.whoAmI] > 0)
                    continue;
                if (!hitArea.Intersects(npc.getRect()) || !(npc.noTileCollide || Player.CanHit(npc)))
                    continue;
                int damage = (int)Player.GetTotalDamage<MeleeDamageClass>().ApplyTo(profile.Damage);
                float knockback = Player.GetTotalKnockback<MeleeDamageClass>().ApplyTo(profile.Knockback);
                bool critical = Main.rand.Next(100) < Player.GetTotalCritChance<MeleeDamageClass>();
                Player.ApplyDamageToNPC(npc, damage, knockback, direction, critical, DamageClass.Melee, true);
                if (npc.immune[Player.whoAmI] < MinShieldNPCImmunityFrames)
                    npc.immune[Player.whoAmI] = MinShieldNPCImmunityFrames;
                if (Player.immuneTime < ShieldHitImmunityFrames)
                    Player.immuneTime = ShieldHitImmunityFrames;
                ApplyShieldSlamHitEffects(npc, profile);
            }
        }
        /// <summary>触发震屏、爆炸、粒子、减益与撞击广播。</summary>
        private void ApplyShieldSlamHitEffects(NPC npc, ShieldSlamProfile profile)
        {
            if (profile.ScreenShake > 0f && !shieldSlamDashScreenShook)
            {
                GeneralScreenShakePower = profile.ScreenShake;
                shieldSlamDashScreenShook = true;
            }
            if (shieldSlamDash == ShieldSlamDash.AsgardianAegis)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero
                    , ModContent.ProjectileType<CosmicDashExplosion>(), GetShieldExplosionDamage(profile)
                    , profile.ExplosionKnockback, Main.myPlayer, 3f, 0f);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityMod", "GodSlayerInferno", profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityModClassicPreTrailer", "GodSlayerInferno"
                    , profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalRD", "GodSlayerInferno", profile.DebuffFrames);
            }
            else if (shieldSlamDash == ShieldSlamDash.ElysianAegis)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero
                    , ModContent.ProjectileType<HolyExplosionSupreme>(), GetShieldExplosionDamage(profile)
                    , profile.ExplosionKnockback, Main.myPlayer, 1f, 0f);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityMod", "HolyFlames", profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityModClassicPreTrailer", "HolyLight"
                    , profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalRD", "HolyFlames", profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalRD", "HolyLight", profile.DebuffFrames);
            }
            else if (shieldSlamDash == ShieldSlamDash.OrnateShield && profile.DebuffFrames > 0)
                npc.AddBuff(BuffID.Frostburn2, profile.DebuffFrames);
            SpawnShieldSlamHitParticles(shieldSlamDash, npc.Center);
            if (shieldSlamDash != ShieldSlamDash.OrnateShield)
                SendShieldSlamHitPacket(shieldSlamDash, npc.whoAmI);
        }
        /// <summary>生成冲刺撞击的圆环、爆炸或三色尘环。</summary>
        private void SpawnShieldSlamHitParticles(ShieldSlamDash dash, Vector2 center)
        {
            switch (dash)
            {
                case ShieldSlamDash.AsgardianAegis:
                    ShieldSlamParticleSystem.SpawnRing(center, new Vector2(2f), 0f, 0.1f, 0.85f, 36);
                    ShieldSlamParticleSystem.SpawnExplosion(center, Color.Magenta
                        , Main.rand.NextFloat(-5f, 5f), 0.65f, 26);
                    break;
                case ShieldSlamDash.ElysianAegis:
                {
                    float scale = Main.rand.NextFloat(0.45f, 0.55f);
                    ShieldSlamParticleSystem.SpawnExplosion(center, Color.Gray * 0.6f
                        , Main.rand.NextFloat(-5f, 5f), scale + 0.07f, 20);
                    ShieldSlamParticleSystem.SpawnExplosion(center, Color.Orange
                        , Main.rand.NextFloat(-5f, 5f), scale, 20);
                    break;
                }
                case ShieldSlamDash.AsgardsValor:
                {
                    const int dustCount = 12;
                    float step = MathHelper.TwoPi / dustCount;
                    Vector2 start = Vector2.Normalize(new Vector2(-1f, -1f));
                    for (int i = 0; i < dustCount; i++)
                    {
                        Vector2 velocity = start.RotatedBy(step * i);
                        SpawnShieldHitDust(center, velocity, DustID.CrimsonTorch, 3f, 2.5f, default);
                        SpawnShieldHitDust(center, velocity, DustID.OrangeTorch, 5f, 2.2f, Color.Salmon);
                        SpawnShieldHitDust(center, velocity, DustID.IchorTorch, 7f, 1.9f, Color.SandyBrown);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(center, DustID.FireworkFountain_Yellow
                            , new Vector2(0f, -3.5f).RotatedByRandom(0.7f) * Main.rand.NextFloat(0.8f, 1.4f)
                            , 0, default, 1.2f);
                        dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                    }
                    SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.6f, PitchVariance = 0.3f }, center);
                    break;
                }
            }
        }
        /// <summary>生成一颗护盾染色的撞击尘。</summary>
        private void SpawnShieldHitDust(Vector2 center, Vector2 direction, int type
            , float speed, float scale, Color color)
        {
            Dust dust = Dust.NewDustPerfect(center, type, direction * speed, 0, default, scale);
            dust.noGravity = true;
            dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
            if (color != default)
                dust.color = color;
        }
        /// <summary>以通用伤害为底，叠加近战、远程、魔法与召唤中的最高职业加成计算爆炸伤害。</summary>
        private int GetShieldExplosionDamage(ShieldSlamProfile profile)
        {
            StatModifier damage = StatModifier.Default;
            StatModifier generic = Player.GetTotalDamage<GenericDamageClass>();
            damage.Base = generic.Base;
            damage *= generic.Multiplicative;
            damage.Flat = generic.Flat;
            float strongest = Math.Max(1f, Player.GetTotalDamage<MeleeDamageClass>().Additive);
            strongest = Math.Max(strongest, Player.GetTotalDamage<RangedDamageClass>().Additive);
            strongest = Math.Max(strongest, Player.GetTotalDamage<MagicDamageClass>().Additive);
            strongest = Math.Max(strongest, Player.GetTotalDamage<SummonDamageClass>().Additive * 0.75f);
            damage += strongest - 1f;
            return (int)damage.ApplyTo(profile.ExplosionDamage);
        }
        /// <summary>阿巴顿在弹幕暴击命中时生成一次短冷却硫磺爆炸。</summary>
        public void TrySpawnAbaddonCrit(Projectile projectile, NPC target, NPC.HitInfo hit)
        {
            if (!abaddon || !hit.Crit || abaddonCritCooldown > 0
                || Player.whoAmI != Main.myPlayer)
                return;
            abaddonCritCooldown = 15;
            int damage = DamageSoftCap(projectile.damage * 0.03f, 25);
            Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero
                , ModContent.ProjectileType<AbaddonCrit>(), damage, 0f, Player.whoAmI);
        }
        /// <summary>对超过上限的伤害按平方根曲线折算增长幅度。</summary>
        private static int DamageSoftCap(float damage, int cap)
        {
            if (damage < cap)
                return (int)damage;
            double ratio = Math.Sqrt(damage / cap) / 1.25 + 0.2;
            return (int)(cap * ratio);
        }
        /// <summary>阻止原版冲刺和克苏鲁之盾在盾牌冲刺期间抢占状态。</summary>
        private void BlockVanillaDash()
        {
            Player.dashType = 0;
            Player.eocHit = -1;
            if (Player.eocDash != 0)
                Player.eocDash = 0;
        }
        /// <summary>返回指定盾牌冲刺的完整数值档。</summary>
        private static ShieldSlamProfile GetShieldSlamProfile(ShieldSlamDash dash)
        {
            return dash switch
            {
                ShieldSlamDash.AsgardianAegis => new ShieldSlamProfile(
                    23.3f, 16f, 1000, 15f, 5f, 300, 1000, 20f),
                ShieldSlamDash.ElysianAegis => new ShieldSlamProfile(
                    21.5f, 14f, 500, 12f, 3.5f, 300, 500, 15f),
                ShieldSlamDash.AsgardsValor => new ShieldSlamProfile(
                    16.9f, DefaultMidDashSpeed, 200, 9f, 0f, 0, 0, 0f),
                ShieldSlamDash.OrnateShield => new ShieldSlamProfile(
                    16.9f, 12.5f, 50, 3f, 0f, 180, 0, 0f),
                _ => new ShieldSlamProfile(DefaultMidDashSpeed, DefaultMidDashSpeed, 0, 0f, 0f, 0, 0, 0f)
            };
        }
        /// <summary>单个盾牌冲刺的只读数值配置。</summary>
        private readonly struct ShieldSlamProfile
        {
            /// <summary>起手速度。</summary>
            public readonly float StartSpeed;
            /// <summary>过程速度上限。</summary>
            public readonly float MidDashSpeed;
            /// <summary>撞击基础伤害。</summary>
            public readonly int Damage;
            /// <summary>撞击基础击退。</summary>
            public readonly float Knockback;
            /// <summary>首次命中的屏幕震动强度。</summary>
            public readonly float ScreenShake;
            /// <summary>附加减益持续时间。</summary>
            public readonly int DebuffFrames;
            /// <summary>爆炸弹幕基础伤害。</summary>
            public readonly int ExplosionDamage;
            /// <summary>爆炸弹幕基础击退。</summary>
            public readonly float ExplosionKnockback;
            /// <summary>构造一档完整冲刺数值。</summary>
            public ShieldSlamProfile(float startSpeed, float midDashSpeed, int damage, float knockback
                , float screenShake, int debuffFrames, int explosionDamage, float explosionKnockback)
            {
                StartSpeed = startSpeed;
                MidDashSpeed = midDashSpeed;
                Damage = damage;
                Knockback = knockback;
                ScreenShake = screenShake;
                DebuffFrames = debuffFrames;
                ExplosionDamage = explosionDamage;
                ExplosionKnockback = explosionKnockback;
            }
        }
    }
}
