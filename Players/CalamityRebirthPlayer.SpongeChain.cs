using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Players
{
    /// <summary>涡流虹吸石及其完整前置链的玩家属性与受击反击。</summary>
    public partial class CalamityRebirthPlayer : ModPlayer
    {
        /// <summary>是否装备远古化石。</summary>
        public bool ancientFossil;
        /// <summary>是否装备太古粉末。</summary>
        public bool archaicPowder;
        /// <summary>是否装备腐化之瓶。</summary>
        public bool corruptFlask;
        /// <summary>是否装备猩红之瓶。</summary>
        public bool crimsonFlask;
        /// <summary>是否装备辉光软泥。</summary>
        public bool radiantOoze;
        /// <summary>是否装备生命之露。</summary>
        public bool livingDew;
        /// <summary>是否装备蜜露。</summary>
        public bool honeyDew;
        /// <summary>是否装备百草天香瓶。</summary>
        public bool ambrosialAmpoule;
        /// <summary>是否装备法力凝胶。</summary>
        public bool manaJelly;
        /// <summary>是否装备生命凝胶。</summary>
        public bool lifeJelly;
        /// <summary>是否装备活力凝胶。</summary>
        public bool vitalJelly;
        /// <summary>是否装备绚彩凝胶。</summary>
        public bool grandGelatin;
        /// <summary>是否装备海螺壳。</summary>
        public bool seaShell;
        /// <summary>是否装备巨蟹甲壳。</summary>
        public bool crawCarapace;
        /// <summary>是否装备真菌甲壳。</summary>
        public bool fungalCarapace;
        /// <summary>是否装备巨型陆龟壳。</summary>
        public bool giantTortoiseShell;
        /// <summary>是否装备阿米迪亚斯之火花。</summary>
        public bool amidiasSpark;
        /// <summary>是否装备阴阳吸星石。</summary>
        public bool theAbsorber;
        /// <summary>是否装备巨型贝壳。</summary>
        public bool giantShell;
        /// <summary>是否装备涡流虹吸石。</summary>
        public bool sponge;
        /// <summary>是否具有蜂类弹幕减伤。</summary>
        public bool beeResist;
        /// <summary>甲壳爆发增益是否生效。</summary>
        public bool shellBoost;
        /// <summary>清空本帧涡流虹吸石链状态。</summary>
        public void ResetSpongeChainEffects()
        {
            ancientFossil = false;
            archaicPowder = false;
            corruptFlask = false;
            crimsonFlask = false;
            radiantOoze = false;
            livingDew = false;
            honeyDew = false;
            ambrosialAmpoule = false;
            manaJelly = false;
            lifeJelly = false;
            vitalJelly = false;
            grandGelatin = false;
            seaShell = false;
            crawCarapace = false;
            fungalCarapace = false;
            giantTortoiseShell = false;
            amidiasSpark = false;
            theAbsorber = false;
            giantShell = false;
            sponge = false;
            beeResist = false;
            shellBoost = false;
        }
        /// <summary>结算整条链的常驻面板、环境效果与静态再生。</summary>
        public void ApplySpongeChainEffects()
        {
            if (ancientFossil && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight))
                Player.pickSpeed -= 0.35f;
            if (archaicPowder && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight
                || Player.ZoneUnderworldHeight))
            {
                Player.statDefense += 3;
                Player.endurance += 0.07f;
                Player.pickSpeed -= 0.5f;
            }
            if (corruptFlask && Player.ZoneCorrupt)
                AddFlaskBonus();
            if (crimsonFlask && Player.ZoneCrimson)
                AddFlaskBonus();
            if (radiantOoze && !Main.dayTime)
            {
                Lighting.AddLight(Player.Center / 16f, 1f, 1f, 0.6f);
                Player.lifeRegen += 2;
            }
            if (livingDew && Player.ZoneJungle)
                AddJungleDewBonus();
            if (honeyDew)
            {
                if (Player.ZoneJungle)
                    AddJungleDewBonus();
                Player.buffImmune[BuffID.Poisoned] = true;
                Player.buffImmune[BuffID.Venom] = true;
                ApplyHoneyRegen();
                Player.lifeRegenTime += 2;
                Player.lifeRegen += 2;
            }
            if (manaJelly)
            {
                Player.statManaMax2 += 20;
                if (IsStandingStill())
                    Player.manaRegenBonus += 2;
            }
            if (lifeJelly)
            {
                Player.statLifeMax2 += 20;
                if (IsStandingStill())
                    Player.lifeRegen += 2;
            }
            if (vitalJelly)
            {
                Player.moveSpeed += 0.1f;
                Player.jumpSpeedBoost += 1f;
            }
            if (grandGelatin)
            {
                Player.moveSpeed += 0.1f;
                Player.jumpSpeedBoost += 1f;
                Player.statLifeMax2 += 20;
                Player.statManaMax2 += 20;
                if (IsStandingStill())
                {
                    Player.lifeRegen += 2;
                    Player.manaRegenBonus += 2;
                }
            }
            if (seaShell && IsSubmerged())
                AddSubmergedShellBonus();
            if (crawCarapace)
            {
                Player.endurance += 0.05f;
                Player.thorns = Math.Max(Player.thorns, 0.25f);
            }
            if (giantTortoiseShell)
            {
                Player.moveSpeed -= 0.1f;
                Player.thorns = Math.Max(Player.thorns, 0.25f);
            }
            if (theAbsorber)
            {
                Player.statLifeMax2 += 30;
                Player.statManaMax2 += 20;
                Player.moveSpeed += 0.12f;
                Player.jumpSpeedBoost += 1.2f;
                Player.thorns = 1f;
                Player.endurance += 0.06f;
                if (IsStandingStill())
                {
                    Player.lifeRegen += 6;
                    Player.manaRegenBonus += 2;
                }
                if (IsSubmerged())
                {
                    Player.statDefense += 5;
                    Player.endurance += 0.05f;
                    Player.moveSpeed += 0.2f;
                    Player.ignoreWater = true;
                }
            }
            if (giantShell)
                Player.moveSpeed -= 0.15f;
            if (ambrosialAmpoule)
                ApplyAmbrosialAmpouleEffects();
            if (sponge)
                ApplySpongeEffects();
            if (shellBoost)
            {
                Player.panic = false;
                Player.moveSpeed += 0.9f;
            }
        }
        /// <summary>应用百草天香瓶的减伤、挖掘、照明、再生、免疫和蜂蜜恢复。</summary>
        private void ApplyAmbrosialAmpouleEffects()
        {
            Player.endurance += 0.12f;
            Player.pickSpeed -= 0.5f;
            Lighting.AddLight(Player.Center / 16f, 1f, 1f, 0.6f);
            Player.lifeRegen += 4;
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Venom] = true;
            ApplyHoneyRegen();
            Player.lifeRegenTime += 2;
        }
        /// <summary>应用涡流虹吸石的生存、浸水、挖掘、照明与免疫。</summary>
        private void ApplySpongeEffects()
        {
            Player.lifeRegen += 6;
            Player.endurance += 0.18f;
            Player.statLifeMax2 += 30;
            Player.statManaMax2 += 30;
            Player.moveSpeed += 0.12f;
            Player.jumpSpeedBoost += 1.2f;
            Player.thorns = 1f;
            if (IsStandingStill())
            {
                Player.lifeRegen += 6;
                Player.manaRegenBonus += 2;
            }
            if (IsSubmerged())
            {
                Player.statDefense += 5;
                Player.endurance += 0.1f;
                Player.moveSpeed += 0.2f;
                Player.ignoreWater = true;
            }
            Player.pickSpeed -= 0.5f;
            Lighting.AddLight(Player.Center / 16f, 1f, 1f, 0.6f);
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Venom] = true;
            ApplyHoneyRegen();
            Player.lifeRegenTime += 2;
        }
        /// <summary>添加邪恶生态下的三点防御与七百分点减伤。</summary>
        private void AddFlaskBonus()
        {
            Player.statDefense += 3;
            Player.endurance += 0.07f;
        }
        /// <summary>添加丛林露水系列的再生、防御与减伤。</summary>
        private void AddJungleDewBonus()
        {
            Player.lifeRegen += 2;
            Player.statDefense += 5;
            Player.endurance += 0.1f;
        }
        /// <summary>添加浸水海螺壳的防御、减伤、移速与水体忽略。</summary>
        private void AddSubmergedShellBonus()
        {
            Player.statDefense += 3;
            Player.endurance += 0.05f;
            Player.moveSpeed += 0.15f;
            Player.ignoreWater = true;
        }
        /// <summary>在未泡蜂蜜且负再生时抵消四点，并保证结果不超过零。</summary>
        private void ApplyHoneyRegen()
        {
            if (Player.honey || Player.lifeRegen >= 0)
                return;
            Player.lifeRegen += 4;
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
        }
        /// <summary>判断玩家是否完全静止且没有物品挥动。</summary>
        private bool IsStandingStill()
        {
            return Math.Abs(Player.velocity.X) < 0.05f
                && Math.Abs(Player.velocity.Y) < 0.05f && Player.itemAnimation == 0;
        }
        /// <summary>判断玩家是否处于完整溺水碰撞环境。</summary>
        private bool IsSubmerged()
        {
            return Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir);
        }
        /// <summary>蜂类弹幕命中时降低至百分之七十五伤害。</summary>
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (beeResist && IsBeeProjectile(proj.type))
                modifiers.FinalDamage *= 0.75f;
        }
        /// <summary>判断原版与三个分支中的蜂刺类弹幕。</summary>
        private static bool IsBeeProjectile(int type)
        {
            if (type == ProjectileID.Stinger || type == ProjectileID.HornetStinger)
                return true;
            return IsNamedProjectile(type, "CalamityMod", "PlagueStingerGoliath")
                || IsNamedProjectile(type, "CalamityMod", "PlagueStingerGoliathV2")
                || IsNamedProjectile(type, "CalamityModClassicPreTrailer", "PlagueExplosion")
                || IsNamedProjectile(type, "CalamityModClassicPreTrailer", "PlagueStingerGoliath")
                || IsNamedProjectile(type, "CalamityModClassicPreTrailer", "PlagueStingerGoliathV2")
                || IsNamedProjectile(type, "CalRD", "PlagueExplosion")
                || IsNamedProjectile(type, "CalRD", "PlagueStingerGoliath")
                || IsNamedProjectile(type, "CalRD", "PlagueStingerGoliathV2");
        }
        /// <summary>比较指定外部模组弹幕类型。</summary>
        private static bool IsNamedProjectile(int type, string modName, string projectileName)
        {
            return ModContent.TryFind(modName, projectileName, out ModProjectile projectile)
                && projectile.Type == type;
        }
        /// <summary>在受伤结算前按阴阳吸星石或涡流虹吸石比例直接治疗。</summary>
        public void ApplySpongeAbsorbHeal(ref Player.HurtModifiers modifiers)
        {
            if (theAbsorber)
                HealBeforeDamage((int)modifiers.SourceDamage.Base / 20);
            if (sponge)
                HealBeforeDamage((int)modifiers.SourceDamage.Base / 16);
        }
        /// <summary>在受击后触发甲壳爆发。</summary>
        public void ApplySpongeChainOnHurt()
        {
            if ((theAbsorber || giantShell || sponge) && !Player.panic)
                Player.AddBuff(ModContent.BuffType<ShellBoost>(), 300);
        }
        /// <summary>在受击后生成火花与松露孢子反击。</summary>
        public void ApplySpongeChainPostHurt(Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Player.whoAmI != Main.myPlayer)
                return;
            if (amidiasSpark || theAbsorber)
                SpawnSparkBurst(Main.hardMode ? 36 : 6);
            if (sponge)
                SpawnSparkBurst(Main.hardMode ? 144 : 36);
            if (fungalCarapace || theAbsorber)
                SpawnSporeBurst((int)Player.GetDamage(DamageClass.Generic).ApplyTo(70));
            if (sponge)
                SpawnSporeBurst(448);
        }
        /// <summary>从玩家中心向八个方向生成近战火花。</summary>
        private void SpawnSparkBurst(int damage)
        {
            SoundEngine.PlaySound(SoundID.Item93, Player.position);
            SpawnBidirectionalBurst(Player.Center, damage, ModContent.ProjectileType<Spark>());
        }
        /// <summary>从玩家两侧生成八颗松露孢子。</summary>
        private void SpawnSporeBurst(int damage)
        {
            SoundEngine.PlaySound(SoundID.NPCHit45, Player.position);
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            for (int i = 0; i < 4; i++)
            {
                float x = Main.rand.NextBool(2) ? Player.Center.X + 100f : Player.Center.X - 100f;
                Vector2 origin = new(x, Player.Center.Y + Main.rand.Next(-100, 101));
                double angle = startAngle + deltaAngle * (i + i * i) / 2d + 32d * i;
                SpawnPair(origin, angle, damage, ProjectileID.TruffleSpore);
            }
        }
        /// <summary>在指定中心生成八向火花。</summary>
        private void SpawnBidirectionalBurst(Vector2 origin, int damage, int projectileType)
        {
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            for (int i = 0; i < 4; i++)
            {
                double angle = startAngle + deltaAngle * (i + i * i) / 2d + 32d * i;
                SpawnPair(origin, angle, damage, projectileType);
            }
        }
        /// <summary>沿给定角度的正反方向各生成一枚一百二十帧弹幕。</summary>
        private void SpawnPair(Vector2 origin, double angle, int damage, int projectileType)
        {
            float speedX = (float)Math.Sin(angle) * 5f;
            float speedY = (float)Math.Cos(angle) * 5f;
            int first = Projectile.NewProjectile(Entity.GetSource_FromThis(), origin, new Vector2(speedX, speedY)
                , projectileType, damage, 1.25f, Player.whoAmI);
            int second = Projectile.NewProjectile(Entity.GetSource_FromThis(), origin, new Vector2(-speedX, -speedY)
                , projectileType, damage, 1.25f, Player.whoAmI);
            if (Main.projectile.IndexInRange(first))
                Main.projectile[first].timeLeft = 120;
            if (Main.projectile.IndexInRange(second))
                Main.projectile[second].timeLeft = 120;
        }
        /// <summary>在伤害结算前直接治疗并限制到最大生命。</summary>
        private void HealBeforeDamage(int amount)
        {
            if (amount <= 0)
                return;
            Player.statLife += amount;
            Player.HealEffect(amount);
            if (Player.statLife > Player.statLifeMax2)
                Player.statLife = Player.statLifeMax2;
        }
    }
}
