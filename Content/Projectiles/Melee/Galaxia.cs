using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 银河弹：银河与鸿蒙方舟共用的彩虹追踪弹幕。
    /// 命中时根据生物群系、四柱、月亮事件和星陨环境施加不同减益、玩家增益与二次弹幕。
    /// </summary>
    internal sealed class Galaxia : ModProjectile
    {
        /// <summary>登记六格历史位置用于绘制彩虹残影。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        /// <summary>
        /// 银河弹具有 24×24 碰撞箱、三次穿透和 280 帧默认寿命；
        /// 无视地形与入水减速，并对同一 NPC 使用十帧本地冷却。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 50;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 280;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        /// <summary>
        /// 弹幕随速度旋转，逐渐变实，生成彩虹尘和星形残片，并追踪 1600 像素内最近敌人。
        /// ai[1] 为一时启用鸿蒙方舟群星的额外发光与高频特效。
        /// </summary>
        public override void AI()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            Projectile.alpha -= 15;
            int minimumAlpha = Projectile.Center.Y >= Projectile.ai[1] ? 0 : 150;
            if (Projectile.alpha < minimumAlpha)
                Projectile.alpha = minimumAlpha;
            float rotationStep = (System.Math.Abs(Projectile.velocity.X)
                + System.Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;
            Projectile.localAI[0] += rotationStep;
            Projectile.rotation += rotationStep;
            if (Main.rand.NextBool(8))
            {
                Vector2 offsetDirection = Vector2.UnitX.RotatedByRandom(MathHelper.PiOver2)
                    .RotatedBy(Projectile.velocity.ToRotation());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + offsetDirection * 12f
                    , DustID.RainbowTorch, offsetDirection * 0.66f, 150
                    , new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                dust.noGravity = true;
            }
            if (Main.rand.NextBool(24))
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center
                    , Projectile.velocity * 0.2f, 16, 1f);
                Main.gore[gore].velocity *= 0.66f;
                Main.gore[gore].velocity += Projectile.velocity * 0.3f;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.light = 0.9f;
                if (Main.rand.NextBool(5))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowTorch
                        , Projectile.velocity * 0.5f, 150
                        , new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.2f);
                    dust.noGravity = true;
                }
                if (Main.rand.NextBool(10))
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position
                        , Projectile.velocity * 0.2f, Main.rand.Next(16, 18), 1f);
                }
            }
            CRUtil.HomeInNPC(Projectile, 1600f, 30f, 20f);
        }
        /// <summary>命中 NPC 时执行完整的环境与事件效果表。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyHitEffects(target, target);
        }
        /// <summary>PvP 命中使用相同环境效果，目标玩家取代 NPC 作为减益承受者。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ApplyHitEffects(target, target);
        }
        /// <summary>绘制六格彩虹残影后由本方法完全接管本体绘制。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            CRUtil.DrawAfterimages(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);
            return false;
        }
        /// <summary>本体和残影使用随游戏时间变化的彩虹颜色，透明度跟随 alpha。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Projectile.alpha);
        }
        /// <summary>
        /// 执行月亮事件、主群系、星陨感染和二次弹幕。
        /// hitEntity 同时供 NPC 与 PvP 玩家构造 OnHit 来源，避免维护两份命中流程。
        /// </summary>
        private void ApplyHitEffects<T>(T target, Entity hitEntity)
        {
            Player owner = Main.player[Projectile.owner];
            if (Main.bloodMoon)
                owner.AddBuff(BuffID.Battle, 600);
            if (Main.snowMoon)
                owner.AddBuff(BuffID.RapidHealing, 600);
            if (Main.pumpkinMoon)
                owner.AddBuff(BuffID.WellFed, 600);
            else if (owner.ZoneJungle)
                ApplyJungle(target, hitEntity, owner, false);
            else if (owner.ZoneSnow)
                ApplySnow(target, hitEntity, owner, false);
            else if (owner.ZoneBeach)
                ApplyBeach(target, hitEntity, owner, false);
            else if (owner.ZoneCorrupt)
                ApplyCorruption(target, hitEntity, owner, false);
            else if (owner.ZoneCrimson)
                ApplyCrimson(target, hitEntity, owner, false);
            else if (owner.ZoneDungeon)
                ApplyDungeon(target, hitEntity, owner, false);
            else if (owner.ZoneDesert)
                ApplyDesert(target, hitEntity, owner, false);
            else if (owner.ZoneGlowshroom)
                ApplyGlowshroom(target, hitEntity, owner, false);
            else if (owner.ZoneUnderworldHeight)
                ApplyUnderworld(target, hitEntity, owner, false);
            else if (owner.ZoneHallow)
                ApplyHallow(target, hitEntity, owner, false);
            else if (owner.ZoneTowerNebula)
                ApplyNebula(target, hitEntity, owner, false);
            else if (owner.ZoneTowerStardust)
                ApplyStardust(target, hitEntity, owner, false);
            else if (owner.ZoneTowerSolar)
                ApplySolar(target, hitEntity, owner, false);
            else if (owner.ZoneTowerVortex)
                ApplyVortex(target, hitEntity, owner, false);
            else
                ApplyDefault(target, hitEntity, owner, false);
            if (CalamityCompatUtil.IsInAstralInfection(owner))
                ApplyAstral(target, hitEntity, owner);
        }
        /// <summary>丛林：毒液、瘟疫、荆棘与叶刃。</summary>
        private void ApplyJungle<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            AddVanillaBuff(target, BuffID.Venom, 1200);
            TryAddVersionedBuff(target, "Plague", 1200);
            owner.AddBuff(BuffID.Thorns, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.Leaf, Projectile.velocity);
        }
        /// <summary>雪原：极寒状态或霜火、温暖与冰锥。</summary>
        private void ApplySnow<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            if (!TryAddVersionedBuff(target, "GlacialState", 1200))
                AddVanillaBuff(target, BuffID.Frostburn, 1200);
            owner.AddBuff(BuffID.Warmth, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.IceBolt, Projectile.velocity);
        }
        /// <summary>海滩：深海重压、潮湿与低速度海泡。</summary>
        private void ApplyBeach<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            TryAddVersionedBuff(target, "CrushDepth", 1200);
            owner.AddBuff(BuffID.Wet, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.FlaironBubble, Projectile.velocity * 0.25f);
        }
        /// <summary>腐化：愤怒与诅咒焰。</summary>
        private void ApplyCorruption<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            owner.AddBuff(BuffID.Wrath, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.CursedFlameFriendly, Projectile.velocity, 1);
        }
        /// <summary>猩红：暴怒与黄金雨。</summary>
        private void ApplyCrimson<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            owner.AddBuff(BuffID.Rage, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.GoldenShowerFriendly, Projectile.velocity, 1);
        }
        /// <summary>地牢：霜火、危险感知与水流束。</summary>
        private void ApplyDungeon<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            AddVanillaBuff(target, BuffID.Frostburn, 1200);
            owner.AddBuff(BuffID.Dangersense, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.WaterBolt, Projectile.velocity, 1);
        }
        /// <summary>沙漠：圣火或圣光、耐力与黑曜石冲击波。</summary>
        private void ApplyDesert<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            TryAddVersionedBuff(target, "HolyFlames", 1200);
            owner.AddBuff(BuffID.Endurance, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.BlackBolt, Projectile.velocity);
        }
        /// <summary>发光蘑菇地：时之悲、洞穴探险与蘑菇弹。</summary>
        private void ApplyGlowshroom<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            TryAddVersionedBuff(target, "TemporalSadness", 1200);
            owner.AddBuff(BuffID.Spelunker, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.Mushroom, Projectile.velocity);
        }
        /// <summary>地狱：硫磺火、狱火与火球。</summary>
        private void ApplyUnderworld<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            TryAddVersionedBuff(target, "BrimstoneFlames", 1200);
            owner.AddBuff(BuffID.Inferno, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.BallofFire, Projectile.velocity);
        }
        /// <summary>神圣：圣火或圣光、心之共鸣与彩虹爆炸。</summary>
        private void ApplyHallow<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            TryAddVersionedBuff(target, "HolyFlames", 1200);
            owner.AddBuff(BuffID.Heartreach, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.RainbowCrystalExplosion, Projectile.velocity
                , -1, true);
        }
        /// <summary>星云柱：魔能与星云烈焰。</summary>
        private void ApplyNebula<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            owner.AddBuff(BuffID.MagicPower, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.NebulaBlaze1, Projectile.velocity);
        }
        /// <summary>星尘柱：召唤增益与星尘细胞射击。</summary>
        private void ApplyStardust<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            owner.AddBuff(BuffID.Summoning, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.StardustCellMinionShot, Projectile.velocity, 1);
        }
        /// <summary>日耀柱：泰坦增益与日耀鞭爆炸。</summary>
        private void ApplySolar<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            owner.AddBuff(BuffID.Titan, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.SolarWhipSwordExplosion, Projectile.velocity
                , 0, false, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
        }
        /// <summary>星旋柱：弹药储备与星旋火箭。</summary>
        private void ApplyVortex<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            owner.AddBuff(BuffID.AmmoReservation, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.VortexBeaterRocket, Projectile.velocity
                , -1, true);
        }
        /// <summary>其余环境：甲壳破碎、树妖祝福与泰拉光束。</summary>
        private void ApplyDefault<T>(T target, Entity hitEntity, Player owner, bool pvp)
        {
            TryAddVersionedBuff(target, "ArmorCrunch", 1200);
            owner.AddBuff(BuffID.DryadsWard, 600);
            SpawnMeleeSecondary(hitEntity, ProjectileID.TerrarianBeam, Projectile.velocity, 1);
        }
        /// <summary>
        /// 星陨感染追加星陨感染减益、重力正常化增益和星陨星弹。
        /// 星陨效果与主群系效果并行。
        /// </summary>
        private void ApplyAstral<T>(T target, Entity hitEntity, Player owner)
        {
            bool applied;
            if (ModLoader.HasMod("CalamityMod"))
                applied = TryAddExternalBuff(target, "CalamityMod", "AstralInfectionDebuff", 1200)
                    && CalamityCompatUtil.TryAddBuff(owner, "CalamityMod", "GravityNormalizerBuff", 600);
            else if (ModLoader.HasMod("CalRD"))
                applied = TryAddExternalBuff(target, "CalRD", "AstralInfectionDebuff", 1200)
                    && CalamityCompatUtil.TryAddBuff(owner, "CalRD", "GravityNormalizerBuff", 600);
            else if (ModLoader.HasMod("CalamityModClassicPreTrailer"))
                applied = TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                        , "AstralInfectionDebuff", 1200)
                    && CalamityCompatUtil.TryAddBuff(owner, "CalamityModClassicPreTrailer"
                        , "GravityNormalizerBuff", 600);
            else
                applied = false;
            if (!applied || !TryGetFirstModProjectile("AstralStar", out ModProjectile astralStar)
                || Projectile.owner != Main.myPlayer)
                return;
            int projectile = Projectile.NewProjectile(Projectile.GetSource_OnHit(hitEntity)
                , Projectile.Center, Projectile.velocity, astralStar.Type, Projectile.damage
                , Projectile.knockBack, Projectile.owner);
            Main.projectile[projectile].DamageType = DamageClass.Melee;
        }
        /// <summary>
        /// 按当前灾厄版本查找同名减益并施加，经典版的圣火自动映射为 HolyLight。
        /// </summary>
        private static bool TryAddVersionedBuff<T>(T target, string buffName, int duration)
        {
            if (ModLoader.HasMod("CalamityMod")
                && TryAddExternalBuff(target, "CalamityMod", buffName, duration))
                return true;
            if (ModLoader.HasMod("CalRD")
                && TryAddExternalBuff(target, "CalRD", buffName, duration))
                return true;
            if (ModLoader.HasMod("CalamityModClassicPreTrailer"))
            {
                string classicName = buffName == "HolyFlames" ? "HolyLight" : buffName;
                return TryAddExternalBuff(target, "CalamityModClassicPreTrailer", classicName, duration);
            }
            return false;
        }
        /// <summary>把外部灾厄减益施加到 NPC 或玩家目标。</summary>
        private static bool TryAddExternalBuff<T>(T target, string modName, string buffName, int duration)
        {
            return target switch
            {
                NPC npc => CalamityCompatUtil.TryAddBuff(npc, modName, buffName, duration),
                Player player => CalamityCompatUtil.TryAddBuff(player, modName, buffName, duration),
                _ => false,
            };
        }
        /// <summary>向 NPC 或 PvP 玩家施加原版减益。</summary>
        private static void AddVanillaBuff<T>(T target, int buffType, int duration)
        {
            switch (target)
            {
                case NPC npc:
                    npc.AddBuff(buffType, duration);
                    break;
                case Player player:
                    player.AddBuff(buffType, duration);
                    break;
            }
        }
        /// <summary>
        /// 仅在拥有者客户端生成一次近战二次弹幕，防止多人环境重复生成。
        /// penetration 小于零时不改写穿透，否则写入指定次数。
        /// </summary>
        private void SpawnMeleeSecondary(Entity hitEntity, int projectileType, Vector2 velocity
            , int penetration = 0, bool localCooldown = false, float ai0 = 0f, float ai1 = 0f)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            int index = Projectile.NewProjectile(Projectile.GetSource_OnHit(hitEntity)
                , Projectile.Center, velocity, projectileType, Projectile.damage
                , Projectile.knockBack, Projectile.owner, ai0, ai1);
            Projectile secondary = Main.projectile[index];
            secondary.DamageType = DamageClass.Melee;
            if (penetration > 0)
                secondary.penetrate = penetration;
            if (localCooldown)
                secondary.localNPCHitCooldown = -1;
        }
        /// <summary>按现代、CalRD、1.4.2 的固定顺序查找第一个可用的灾厄弹幕。</summary>
        private static bool TryGetFirstModProjectile(string projectileName, out ModProjectile projectile)
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind(projectileName, out projectile))
                return true;
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind(projectileName, out projectile))
                return true;
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind(projectileName, out projectile))
                return true;
            projectile = null;
            return false;
        }
    }
}
