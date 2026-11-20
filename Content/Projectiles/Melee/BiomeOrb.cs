using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 环境之刃发射的环境球。
    /// 颜色、粉尘和命中减益会随发射者在命中时的所在环境变化。
    /// </summary>
    internal class BiomeOrb : ModProjectile
    {
        /// <summary>当前帧用于本体与残影的环境颜色。</summary>
        private Color environmentColor = Color.Green;
        /// <summary>当前帧环境对应的粉尘类型，默认值三代表草地尘。</summary>
        private int dustType = 3;
        /// <summary>
        /// 当前环境。世界同时满足多个群系标记时按固定优先级选出唯一结果，
        /// 星陨感染另行覆盖视觉并追加对应命中效果。
        /// </summary>
        protected enum Environment
        {
            Forest,
            Jungle,
            Snow,
            Beach,
            Corruption,
            Crimson,
            Dungeon,
            Desert,
            Glowshroom,
            Underworld,
            Sky,
            Hallow,
            Nebula,
            Stardust,
            Solar,
            Vortex,
        }
        /// <summary>当前层级保留的历史位置数量。</summary>
        protected virtual int TrailLength => 2;
        /// <summary>当前层级生成时的基础存活时间。</summary>
        protected virtual int InitialLifeTime => 300;
        /// <summary>当前层级自动追踪半径；零表示不追踪。</summary>
        protected virtual float TrackingDistance => 0f;
        /// <summary>当前层级拖尾粉尘的基准尺寸。</summary>
        protected virtual float DustScale => 0.9f;
        /// <summary>是否把四柱群系纳入颜色与粉尘判定。</summary>
        protected virtual bool IncludesLunarTowers => false;
        /// <summary>NPC 目标在灾厄环境下的基础减益时长。</summary>
        protected virtual int NpcDebuffDuration => 90;
        /// <summary>玩家目标在 PvP 中的基础减益时长。</summary>
        protected virtual int PvpDebuffDuration => 360;
        /// <summary>经典灾厄专属减益的固定时长。</summary>
        protected virtual int ClassicModDebuffDuration => 180;
        /// <summary>登记两格历史位置，用于当前层级的短拖尾。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        /// <summary>
        /// 环境球使用 20×20 碰撞箱、原版光束 AI、近战伤害、单次穿透和 300 帧寿命。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            AIType = ProjectileID.LightBeam;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = InitialLifeTime;
        }
        /// <summary>逐帧刷新环境颜色和粉尘，并生成一粒与弹速轻微联动的无重力拖尾尘。</summary>
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Environment environment = ResolveEnvironment(owner, IncludesLunarTowers);
            RefreshVisuals(environment);
            if (CalamityCompatUtil.IsInAstralInfection(owner))
            {
                environmentColor = new Color(255, 127, 80);
                if (TryFindAstralDust(out ModDust astralDust))
                    dustType = astralDust.Type;
            }
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , dustType, 0f, 0f, 100, default, DustScale);
            Dust dust = Main.dust[dustIndex];
            dust.noGravity = true;
            dust.velocity *= 0.5f;
            dust.velocity += Projectile.velocity * 0.1f;
            if (TrackingDistance > 0f)
                CRUtil.HomeInNPC(Projectile, TrackingDistance, 24f, 20f);
        }
        /// <summary>
        /// 按当前灾厄版本为 NPC 附加环境减益。
        /// 现代与 1.4.2 使用现有元素组合，CalRD 使用其原生环境列表。
        /// </summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            ApplyEnvironmentalDebuff(target, owner, false);
            ApplyOwnerOnHitBuff(owner);
        }
        /// <summary>
        /// PvP 命中采用更长的控制时间，避免 1.5 秒环境减益在玩家目标上几乎无效。
        /// 各灾厄专属减益仍按对应版本支持的名称弱依赖施加。
        /// </summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player owner = Main.player[Projectile.owner];
            ApplyEnvironmentalDebuff(target, owner, true);
            ApplyOwnerOnHitBuff(owner);
        }
        /// <summary>沿旧速度反向铺设 27 组双尘尾迹，形成环境色爆散效果。</summary>
        public override void OnKill(int timeLeft)
        {
            for (int i = 4; i < 31; i++)
            {
                float backstepX = Projectile.oldVelocity.X * (30f / i);
                float backstepY = Projectile.oldVelocity.Y * (30f / i);
                Vector2 spawn = new(Projectile.oldPosition.X - backstepX, Projectile.oldPosition.Y - backstepY);
                int largeIndex = Dust.NewDust(spawn, 8, 8, dustType, Projectile.oldVelocity.X
                    , Projectile.oldVelocity.Y, 100, default, 1.8f);
                Dust large = Main.dust[largeIndex];
                large.noGravity = true;
                large.velocity *= 0.5f;
                int smallIndex = Dust.NewDust(spawn, 8, 8, dustType, Projectile.oldVelocity.X
                    , Projectile.oldVelocity.Y, 100, default, 1.4f);
                Dust small = Main.dust[smallIndex];
                small.velocity *= 0.05f;
            }
        }
        /// <summary>出生后的前五帧暂不绘制，避免光束生成瞬间出现贴图跳变。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > InitialLifeTime - 5)
                return false;
            CRUtil.DrawAfterimages(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);
            return false;
        }
        /// <summary>返回逐帧环境色，透明度沿用当前弹幕状态。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            environmentColor.A = (byte)MathHelper.Clamp(Projectile.alpha, 0, 255);
            return environmentColor;
        }
        /// <summary>
        /// 按固定顺序解析玩家的主环境。顺序会决定同时满足多个区域时的优先级。
        /// </summary>
        private static Environment ResolveEnvironment(Player player, bool includesLunarTowers)
        {
            if (player.ZoneJungle)
                return Environment.Jungle;
            if (player.ZoneSnow)
                return Environment.Snow;
            if (player.ZoneBeach)
                return Environment.Beach;
            if (player.ZoneCorrupt)
                return Environment.Corruption;
            if (player.ZoneCrimson)
                return Environment.Crimson;
            if (player.ZoneDungeon)
                return Environment.Dungeon;
            if (player.ZoneDesert)
                return Environment.Desert;
            if (player.ZoneGlowshroom)
                return Environment.Glowshroom;
            if (player.ZoneUnderworldHeight)
                return Environment.Underworld;
            if (player.ZoneSkyHeight)
                return Environment.Sky;
            if (player.ZoneHallow)
                return Environment.Hallow;
            if (includesLunarTowers && player.ZoneTowerNebula)
                return Environment.Nebula;
            if (includesLunarTowers && player.ZoneTowerStardust)
                return Environment.Stardust;
            if (includesLunarTowers && player.ZoneTowerSolar)
                return Environment.Solar;
            if (includesLunarTowers && player.ZoneTowerVortex)
                return Environment.Vortex;
            return Environment.Forest;
        }
        /// <summary>把主环境映射为对应的粉尘类型和颜色。</summary>
        private void RefreshVisuals(Environment environment)
        {
            switch (environment)
            {
                case Environment.Jungle:
                    dustType = 39;
                    environmentColor = new Color(128, 255, 128);
                    break;
                case Environment.Snow:
                    dustType = 51;
                    environmentColor = new Color(128, 255, 255);
                    break;
                case Environment.Beach:
                    dustType = 33;
                    environmentColor = new Color(0, 0, 128);
                    break;
                case Environment.Corruption:
                    dustType = 14;
                    environmentColor = new Color(128, 64, 255);
                    break;
                case Environment.Crimson:
                    dustType = 5;
                    environmentColor = new Color(128, 0, 0);
                    break;
                case Environment.Dungeon:
                    dustType = 29;
                    environmentColor = new Color(64, 0, 128);
                    break;
                case Environment.Desert:
                    dustType = 32;
                    environmentColor = new Color(255, 255, 128);
                    break;
                case Environment.Glowshroom:
                    dustType = 56;
                    environmentColor = new Color(0, 255, 255);
                    break;
                case Environment.Underworld:
                    dustType = 6;
                    environmentColor = new Color(255, 128, 0);
                    break;
                case Environment.Sky:
                    dustType = 213;
                    environmentColor = Color.White;
                    break;
                case Environment.Hallow:
                    dustType = 57;
                    environmentColor = new Color(255, 255, 0);
                    break;
                case Environment.Nebula:
                    dustType = 242;
                    environmentColor = new Color(255, 0, 255);
                    break;
                case Environment.Stardust:
                    dustType = 206;
                    environmentColor = new Color(0, 255, 255);
                    break;
                case Environment.Solar:
                    dustType = 244;
                    environmentColor = new Color(255, 128, 0);
                    break;
                case Environment.Vortex:
                    dustType = 107;
                    environmentColor = new Color(0, 255, 0);
                    break;
                default:
                    dustType = 3;
                    environmentColor = new Color(0, 128, 0);
                    break;
            }
        }
        /// <summary>
        /// 查找任一已加载灾厄版本提供的星陨橙色粉尘。
        /// 缺失该视觉资源时保留原环境粉尘，不影响命中减益。
        /// </summary>
        private static bool TryFindAstralDust(out ModDust astralDust)
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind("AstralOrange", out astralDust))
                return true;
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind("AstralOrange", out astralDust))
                return true;
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind("AstralOrange", out astralDust))
                return true;
            astralDust = null;
            return false;
        }
        /// <summary>
        /// 按当前加载的灾厄版本施加环境减益。
        /// NPC 目标优先采用对应版本的原始持续时间；PvP 最少保持六秒，现代时之悲维持缩短规则。
        /// </summary>
        private void ApplyEnvironmentalDebuff<T>(T target, Player owner, bool pvp)
        {
            Environment environment = ResolveEnvironment(owner, IncludesLunarTowers);
            if (ModLoader.HasMod("CalamityMod"))
            {
                ApplyModernDebuff(target, environment, pvp ? PvpDebuffDuration : NpcDebuffDuration);
            }
            else if (ModLoader.HasMod("CalRD"))
            {
                if (CalamityCompatUtil.IsInAstralInfection(owner))
                {
                    TryAddExternalBuff(target, "CalRD", "AstralInfectionDebuff"
                        , pvp ? PvpDebuffDuration : NpcDebuffDuration);
                    return;
                }
                ApplyCalRDDebuff(target, environment, pvp ? PvpDebuffDuration : NpcDebuffDuration);
            }
            else if (ModLoader.HasMod("CalamityModClassicPreTrailer"))
            {
                ApplyClassic142Debuff(target, environment
                    , pvp ? PvpDebuffDuration : ClassicModDebuffDuration, ClassicModDebuffDuration);
            }
            else
            {
                ApplyVanillaFallback(target, environment);
            }
            if (ModLoader.HasMod("CalamityMod") && CalamityCompatUtil.IsInAstralInfection(owner))
                TryAddExternalBuff(target, "CalamityMod", "AstralInfectionDebuff"
                    , pvp ? PvpDebuffDuration : NpcDebuffDuration);
            if (ModLoader.HasMod("CalamityModClassicPreTrailer")
                && CalamityCompatUtil.IsInAstralInfection(owner))
                TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                    , "AstralInfectionDebuff", pvp ? PvpDebuffDuration : ClassicModDebuffDuration);
        }
        /// <summary>
        /// 现代灾厄环境减益：丛林同时施加毒液和瘟疫，发光蘑菇地使用三分之一的时之悲时长。
        /// </summary>
        private static void ApplyModernDebuff<T>(T target, Environment environment, int duration)
        {
            switch (environment)
            {
                case Environment.Jungle:
                    AddVanillaBuff(target, BuffID.Venom, duration);
                    TryAddExternalBuff(target, "CalamityMod", "Plague", duration);
                    break;
                case Environment.Snow:
                case Environment.Dungeon:
                    AddVanillaBuff(target, BuffID.Frostburn, duration);
                    break;
                case Environment.Beach:
                    TryAddExternalBuff(target, "CalamityMod", "CrushDepth", duration);
                    break;
                case Environment.Desert:
                case Environment.Hallow:
                    TryAddExternalBuff(target, "CalamityMod", "HolyFlames", duration);
                    break;
                case Environment.Glowshroom:
                    TryAddExternalBuff(target, "CalamityMod", "TemporalSadness", duration / 3);
                    break;
                case Environment.Underworld:
                    TryAddExternalBuff(target, "CalamityMod", "BrimstoneFlames", duration);
                    break;
                default:
                    TryAddExternalBuff(target, "CalamityMod", "ArmorCrunch", duration);
                    break;
            }
        }
        /// <summary>
        /// 经典 1.4.2.101 环境减益：毒液加瘟疫、
        /// 霜火、圣光和甲壳破碎映射。
        /// </summary>
        private static void ApplyClassic142Debuff<T>(T target, Environment environment
            , int vanillaDuration, int modDuration)
        {
            switch (environment)
            {
                case Environment.Jungle:
                    AddVanillaBuff(target, BuffID.Venom, vanillaDuration);
                    TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                        , "Plague", modDuration);
                    break;
                case Environment.Snow:
                case Environment.Dungeon:
                    AddVanillaBuff(target, BuffID.Frostburn, vanillaDuration);
                    break;
                case Environment.Beach:
                    TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                        , "CrushDepth", modDuration);
                    break;
                case Environment.Desert:
                case Environment.Hallow:
                    TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                        , "HolyLight", modDuration);
                    break;
                case Environment.Glowshroom:
                    TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                        , "TemporalSadness", modDuration);
                    break;
                case Environment.Underworld:
                    TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                        , "BrimstoneFlames", modDuration);
                    break;
                default:
                    TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                        , "ArmorCrunch", modDuration);
                    break;
            }
        }
        /// <summary>
        /// CalRD 1.4.5.7 环境减益：雪原使用其原生的极寒状态，
        /// 丛林只保留毒液，其余环境使用该版本的圣火、深海重压与元素减益名称。
        /// </summary>
        private static void ApplyCalRDDebuff<T>(T target, Environment environment, int duration)
        {
            switch (environment)
            {
                case Environment.Jungle:
                    AddVanillaBuff(target, BuffID.Venom, duration);
                    break;
                case Environment.Snow:
                    if (!TryAddExternalBuff(target, "CalRD", "GlacialState", duration / 2))
                        AddVanillaBuff(target, BuffID.Frostburn, duration);
                    break;
                case Environment.Beach:
                    TryAddExternalBuff(target, "CalRD", "CrushDepth", duration);
                    break;
                case Environment.Dungeon:
                    AddVanillaBuff(target, BuffID.Frostburn, duration);
                    break;
                case Environment.Desert:
                case Environment.Hallow:
                    TryAddExternalBuff(target, "CalRD", "HolyFlames", duration);
                    break;
                case Environment.Glowshroom:
                    TryAddExternalBuff(target, "CalRD", "TemporalSadness", duration);
                    break;
                case Environment.Underworld:
                    TryAddExternalBuff(target, "CalRD", "BrimstoneFlames", duration);
                    break;
                default:
                    TryAddExternalBuff(target, "CalRD", "ArmorCrunch", duration);
                    break;
            }
        }
        /// <summary>供欧米茄环境球在命中时追加血月、霜月或南瓜月增益。</summary>
        protected virtual void ApplyOwnerOnHitBuff(Player owner)
        {
        }
        /// <summary>无灾厄时保留原版可见的环境减益，使武器仍具备基础功能。</summary>
        private static void ApplyVanillaFallback<T>(T target, Environment environment)
        {
            int duration = 180;
            switch (environment)
            {
                case Environment.Jungle:
                case Environment.Beach:
                    AddVanillaBuff(target, BuffID.Venom, duration);
                    break;
                case Environment.Snow:
                case Environment.Dungeon:
                    AddVanillaBuff(target, BuffID.Frostburn, duration);
                    break;
                case Environment.Desert:
                case Environment.Hallow:
                case Environment.Underworld:
                    AddVanillaBuff(target, BuffID.OnFire, duration);
                    break;
                case Environment.Glowshroom:
                    AddVanillaBuff(target, BuffID.Slow, duration);
                    break;
                default:
                    AddVanillaBuff(target, BuffID.BrokenArmor, duration);
                    break;
            }
        }
        /// <summary>
        /// 按目标类型调用 Terraria 的原版 AddBuff 接口。
        /// 泛型分支同时支持 NPC 与 PvP 玩家，避免为相同环境表维护两份重复代码。
        /// </summary>
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
        /// 按 NPC 或 PvP 玩家目标类型转交外部灾厄减益施加。
        /// 该泛型入口只存在于当前弹幕内部，避免通用兼容类为环境球的泛型流程增加重复重载。
        /// </summary>
        private static bool TryAddExternalBuff<T>(T target, string modName, string buffName, int duration)
        {
            return target switch
            {
                NPC npc => CalamityCompatUtil.TryAddBuff(npc, modName, buffName, duration),
                Player player => CalamityCompatUtil.TryAddBuff(player, modName, buffName, duration),
                _ => false,
            };
        }
    }
}
