using CalamityRebirth.Players;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Content.Items.Accessories.Combat.Ranged;
using CalamityRebirth.Content.Items.Accessories.Combat.Rogue;
using CalamityRebirth.Content.Projectiles.Rogue;
using CalamityRebirth.Content.Projectiles.Typeless;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles
{
    /// <summary>
    /// 弹幕侧全局钩子，将玩家拥有的近战弹幕命中同步到 PvP 减益结算。
    /// </summary>
    internal class CalamityRebirthGlobalProjectile : GlobalProjectile
    {
        /// <summary>该弹幕是否由元素箭袋分裂产生，用于阻止子箭继续分裂。</summary>
        public bool elementalQuiverSplit;
        /// <summary>纳米技术的潜行穿甲是否已经应用到当前弹幕。</summary>
        private bool nanotechArmorPenApplied;
        /// <summary>分裂标记必须跟随每个弹幕实例单独保存。</summary>
        public override bool InstancePerEntity => true;
        /// <summary>
        /// 元素箭袋的箭矢分裂：友方箭类远程弹幕每个 AI 更新节拍约 0.5% 概率分裂。
        /// 仅在主人客户端生成，两枚子箭互为镜像，继承 50% 伤害并持续 60 帧。
        /// </summary>
        public override void AI(Projectile projectile)
        {
            TryElementalQuiverSplit(projectile);
            TryNanotechBlade(projectile);
        }
        /// <summary>远程弹幕消失时，龙蒿射手套有 12% 概率分裂出 2～3 枚生命能量。</summary>
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers
                || projectile.owner != Main.myPlayer || !projectile.friendly
                || !projectile.DamageType.CountsAsClass(DamageClass.Ranged)
                || projectile.type == ModContent.ProjectileType<TarraEnergy>())
                return;
            Player owner = Main.player[projectile.owner];
            if (!owner.GetModPlayer<CalamityRebirthPlayer>().tarraRanged || Main.rand.Next(100) >= 12)
                return;
            int count = Main.rand.Next(2, 4);
            int damage = Math.Min((int)(projectile.damage * 0.33f), 65);
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7f, 10f);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, velocity
                    , ModContent.ProjectileType<TarraEnergy>(), damage, 0f, projectile.owner);
            }
        }
        /// <summary>执行元素箭袋的分裂判定。</summary>
        private void TryElementalQuiverSplit(Projectile projectile)
        {
            if (elementalQuiverSplit || projectile.owner != Main.myPlayer
                || projectile.owner < 0 || projectile.owner >= Main.maxPlayers
                || !projectile.friendly || projectile.hostile || !projectile.arrow
                || !projectile.DamageType.CountsAsClass(DamageClass.Ranged))
                return;
            Player owner = Main.player[projectile.owner];
            if (!owner.GetModPlayer<CalamityRebirthPlayer>().elementalQuiver
                || Main.rand.Next(200) <= 198)
                return;
            float spread = 180f * 0.0174f;
            double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2f;
            int damage = (int)(projectile.damage * ElementalQuiver.ArrowSplitDamageMultiplier);
            int first = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center.X
                , projectile.Center.Y, (float)(Math.Sin(startAngle) * 8f)
                , (float)(Math.Cos(startAngle) * 8f), projectile.type, damage, projectile.knockBack
                , projectile.owner);
            int second = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center.X
                , projectile.Center.Y, (float)(-Math.Sin(startAngle) * 8f)
                , (float)(-Math.Cos(startAngle) * 8f), projectile.type, damage, projectile.knockBack
                , projectile.owner);
            MarkQuiverSplit(first);
            MarkQuiverSplit(second);
        }
        /// <summary>
        /// 纳米技术让盗贼弹幕每 30 个游戏节拍留下纳米刀刃；
        /// 潜行打击弹幕首次更新时额外获得 20 点穿甲。
        /// </summary>
        private void TryNanotechBlade(Projectile projectile)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers
                || projectile.owner != Main.myPlayer || !projectile.friendly || projectile.hostile
                || projectile.npcProj || projectile.trap || projectile.damage <= 0
                || !CRUtil.IsRogueDamageType(projectile.DamageType)
                || projectile.type == ModContent.ProjectileType<NanotechBlade>())
                return;
            Player owner = Main.player[projectile.owner];
            CalamityRebirthPlayer modPlayer = owner.GetModPlayer<CalamityRebirthPlayer>();
            if (!modPlayer.nanotech)
                return;
            if (!nanotechArmorPenApplied && CRUtil.IsStealthStrike(owner))
            {
                nanotechArmorPenApplied = true;
                projectile.ArmorPenetration += 20;
            }
            if (projectile.numUpdates != 0 || owner.miscCounter % 30 != 0)
                return;
            int blade = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center
                , Vector2.Zero, ModContent.ProjectileType<NanotechBlade>()
                , (int)(projectile.damage * 0.15f), 0f, projectile.owner);
            if (Main.projectile.IndexInRange(blade))
                Main.projectile[blade].DamageType = CRUtil.GetRogueDamageClass();
        }
        /// <summary>为刚生成的分裂子箭设置寿命、掉落限制与防递归标记。</summary>
        private static void MarkQuiverSplit(int projectileIndex)
        {
            if (projectileIndex < 0 || projectileIndex >= Main.maxProjectiles)
                return;
            Projectile child = Main.projectile[projectileIndex];
            child.timeLeft = ElementalQuiver.ArrowSplitLifetime;
            child.noDropItem = true;
            child.GetGlobalProjectile<CalamityRebirthGlobalProjectile>().elementalQuiverSplit = true;
        }
        /// <summary>
        /// PvP 中为元素箭袋装备者的非箭类远程弹幕应用同样的 5% 最终伤害乘数。
        /// 仅处理 PvP、有效主人、友好且非敌意的远程弹幕。
        /// </summary>
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP || projectile.owner < 0 || projectile.owner >= Main.maxPlayers
                || !projectile.friendly || projectile.hostile)
                return;
            Player owner = Main.player[projectile.owner];
            CalamityRebirthPlayer modPlayer = owner.GetModPlayer<CalamityRebirthPlayer>();
            if (modPlayer.silvaRanged && modPlayer.silvaCountdown > 0 && modPlayer.hasSilvaEffect
                && projectile.DamageType.CountsAsClass(DamageClass.Ranged))
                modifiers.FinalDamage *= 1.40f;
            if (modPlayer.silvaSummon && modPlayer.silvaCountdown <= 0 && modPlayer.hasSilvaEffect
                && projectile.DamageType.CountsAsClass(DamageClass.Summon))
                modifiers.FinalDamage *= 1.10f;
            if (!projectile.arrow && projectile.DamageType.CountsAsClass(DamageClass.Ranged)
                && modPlayer.elementalQuiver)
                modifiers.FinalDamage *= ElementalQuiver.NonArrowDamageMultiplier;
        }
        /// <summary>
        /// 仅在“玩家发射、命中另一名玩家”的可信所有权路径上执行。
        /// 敌意弹幕和无效 owner 直接跳过，避免越界读取玩家数组或错误施加玩家减益。
        /// </summary>
        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (projectile.hostile || projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;
            Player attacker = Main.player[projectile.owner];
            CalamityRebirthPlayer.TryApplyMeleeHitDebuffsOnPvP(attacker, target, projectile.DamageType);
            CalamityRebirthPlayer.TryApplyPotionHitDebuffsOnPvP(attacker, target);
            CalamityRebirthPlayer.TryApplyCrystalSkullLightningOnPvP(attacker, target, projectile.DamageType
                , info.Damage);
        }
        /// <summary>
        /// 烦恼项链使任意友方弹幕命中 NPC 时附加毒液，覆盖射手、法师、召唤和盗贼弹幕。
        /// </summary>
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.hostile || projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;
            Player attacker = Main.player[projectile.owner];
            if (attacker.GetModPlayer<CalamityRebirthPlayer>().vexation)
                target.AddBuff(BuffID.Venom, NecklaceofVexation.VenomDuration);
            TryNanotechFlares(projectile, target, attacker);
        }
        /// <summary>
        /// 纳米技术潜行打击前三次命中从屏幕上方召唤六枚灾厄 NanoFlare。
        /// 只有现代灾厄提供该弹幕，其他版本安全跳过。
        /// </summary>
        private static void TryNanotechFlares(Projectile projectile, NPC target, Player attacker)
        {
            CalamityRebirthPlayer modPlayer = attacker.GetModPlayer<CalamityRebirthPlayer>();
            if (!modPlayer.nanotech || modPlayer.nanotechFlareCooldown > 0
                || projectile.owner != Main.myPlayer || projectile.numHits >= 3
                || !CRUtil.IsRogueDamageType(projectile.DamageType)
                || !CRUtil.IsStealthStrike(attacker)
                || !ModContent.TryFind<ModProjectile>("CalamityMod", "NanoFlare", out ModProjectile nanoFlare))
                return;
            for (int i = 0; i < 6; i++)
            {
                Vector2 spawn = new(target.Center.X + Main.rand.Next(-201, 201)
                    , Main.screenPosition.Y - 600f - Main.rand.Next(50));
                Vector2 velocity = (target.Center - spawn) / 40f;
                Projectile.NewProjectile(projectile.GetSource_FromThis(), spawn, velocity, nanoFlare.Type
                    , (int)(projectile.damage * 0.05f), 3f, projectile.owner);
            }
            modPlayer.nanotechFlareCooldown = 15;
        }
    }
}
