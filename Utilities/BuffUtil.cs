using CalamityRebirth.Content.ModBuffs.Debuffs;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 通用增益、减益与环境增益工具。
    /// 战斗减益清单覆盖本工程、原版与已加载灾厄版本中的同名效果。
    /// </summary>
    internal static class BuffUtil
    {
        /// <summary>缓存本工程与已加载灾厄版本实际注册的减益类型。</summary>
        private static HashSet<int> debuffs;
        /// <summary>药水病、中毒与疾病类减益。</summary>
        private static HashSet<int> sicknessDebuffs;
        /// <summary>火焰、燃烧与高温类减益。</summary>
        private static HashSet<int> fireDebuffs;
        /// <summary>
        /// 判断指定 Buff 类型是否属于战斗减益清单。
        /// </summary>
        internal static bool Contains(int buffType)
        {
            EnsureInitialized();
            return debuffs.Contains(buffType);
        }
        /// <summary>判断指定 Buff 是否属于病症/中毒类减益。</summary>
        internal static bool IsSicknessDebuff(int buffType)
        {
            EnsureInitialized();
            return sicknessDebuffs.Contains(buffType);
        }
        /// <summary>判断指定 Buff 是否属于火系/燃烧类减益。</summary>
        internal static bool IsFireDebuff(int buffType)
        {
            EnsureInitialized();
            return fireDebuffs.Contains(buffType);
        }
        /// <summary>
        /// 在首次运行时建立清单；此时所有软依赖模组的内容均已注册完成。
        /// </summary>
        private static void EnsureInitialized()
        {
            if (debuffs is not null)
                return;
            debuffs =
            [
                BuffID.Poisoned,
                BuffID.Darkness,
                BuffID.Cursed,
                BuffID.OnFire,
                BuffID.OnFire3,
                BuffID.Bleeding,
                BuffID.Confused,
                BuffID.Slow,
                BuffID.Weak,
                BuffID.Silenced,
                BuffID.BrokenArmor,
                BuffID.CursedInferno,
                BuffID.Frostburn,
                BuffID.Frostburn2,
                BuffID.Chilled,
                BuffID.Frozen,
                BuffID.Burning,
                BuffID.Suffocation,
                BuffID.Ichor,
                BuffID.Venom,
                BuffID.Blackout,
                BuffID.Electrified,
                BuffID.Rabies,
                BuffID.Webbed,
                BuffID.Stoned,
                BuffID.Dazed,
                BuffID.VortexDebuff,
                BuffID.WitheredArmor,
                BuffID.WitheredWeapon,
                BuffID.OgreSpit,
                BuffID.BetsysCurse,
                ModContent.BuffType<BurningIII>(),
                ModContent.BuffType<DaybreakII>(),
                ModContent.BuffType<ElectrifiedII>(),
                ModContent.BuffType<ElementalMixII>()
            ];
            AddCalamityDebuffs("CalamityMod");
            AddCalamityDebuffs("CalamityModClassicPreTrailer");
            AddCalamityDebuffs("CalRD");
            AddClassicOnlyDebuffs("CalamityModClassicPreTrailer");
            sicknessDebuffs =
            [
                BuffID.Poisoned,
                BuffID.Venom
            ];
            AddIfPresent(sicknessDebuffs, "CalamityMod", "SulphuricPoisoning");
            AddIfPresent(sicknessDebuffs, "CalamityMod", "AstralInfectionDebuff");
            AddIfPresent(sicknessDebuffs, "CalamityMod", "Plague");
            AddIfPresent(sicknessDebuffs, "CalamityMod", "AbsorberAffliction");
            AddIfPresent(sicknessDebuffs, "CalamityMod", "WhisperingDeath");
            AddIfPresent(sicknessDebuffs, "CalamityMod", "Irradiated");
            AddIfPresent(sicknessDebuffs, "CalamityModClassicPreTrailer", "Plague");
            AddIfPresent(sicknessDebuffs, "CalamityModClassicPreTrailer", "WhisperingDeath");
            AddIfPresent(sicknessDebuffs, "CalamityModClassicPreTrailer", "Irradiated");
            AddIfPresent(sicknessDebuffs, "CalRD", "Plague");
            AddIfPresent(sicknessDebuffs, "CalRD", "WhisperingDeath");
            AddIfPresent(sicknessDebuffs, "CalRD", "Irradiated");
            fireDebuffs =
            [
                BuffID.OnFire,
                BuffID.OnFire3,
                BuffID.Burning,
                BuffID.CursedInferno,
                BuffID.ShadowFlame
            ];
            AddIfPresent(fireDebuffs, "CalamityMod", "SearingLava");
            AddIfPresent(fireDebuffs, "CalamityMod", "BrimstoneFlames");
            AddIfPresent(fireDebuffs, "CalamityMod", "HolyFlames");
            AddIfPresent(fireDebuffs, "CalamityMod", "GodSlayerInferno");
            AddIfPresent(fireDebuffs, "CalamityMod", "Dragonfire");
            AddIfPresent(fireDebuffs, "CalamityMod", "WeakBrimstoneFlames");
            AddIfPresent(fireDebuffs, "CalamityMod", "BanishingFire");
            AddIfPresent(fireDebuffs, "CalamityModClassicPreTrailer", "BrimstoneFlames");
            AddIfPresent(fireDebuffs, "CalamityModClassicPreTrailer", "HolyLight");
            AddIfPresent(fireDebuffs, "CalamityModClassicPreTrailer", "GodSlayerInferno");
            AddIfPresent(fireDebuffs, "CalRD", "BrimstoneFlames");
            AddIfPresent(fireDebuffs, "CalRD", "HolyLight");
            AddIfPresent(fireDebuffs, "CalRD", "GodSlayerInferno");
        }
        /// <summary>
        /// 收录现代经典两版灾厄共有的减益名称。
        /// </summary>
        private static void AddCalamityDebuffs(string modName)
        {
            AddIfPresent(modName, "Nightwither");
            AddIfPresent(modName, "Voidfrost");
            AddIfPresent(modName, "WindChilled");
            AddIfPresent(modName, "CrushDepth");
            AddIfPresent(modName, "Eutrophication");
            AddIfPresent(modName, "HadopelagicPressure");
            AddIfPresent(modName, "Riptide");
            AddIfPresent(modName, "AstralInfectionDebuff");
            AddIfPresent(modName, "BrainRot");
            AddIfPresent(modName, "BurningBlood");
            AddIfPresent(modName, "Plague");
            AddIfPresent(modName, "SagePoison");
            AddIfPresent(modName, "SulphuricPoisoning");
            AddIfPresent(modName, "WhisperingDeath");
            AddIfPresent(modName, "StaticDischarge");
            AddIfPresent(modName, "VermillionFlux");
            AddIfPresent(modName, "BrimstoneFlames");
            AddIfPresent(modName, "DemonicFlames");
            AddIfPresent(modName, "GodSlayerInferno");
            AddIfPresent(modName, "HolyFlames");
            AddIfPresent(modName, "SearingLava");
            AddIfPresent(modName, "ArmorCrunch");
            AddIfPresent(modName, "ElementalMix");
            AddIfPresent(modName, "HeavyBleeding");
            AddIfPresent(modName, "Irradiated");
            AddIfPresent(modName, "Laceration");
            AddIfPresent(modName, "HolyInferno");
            AddIfPresent(modName, "IcarusFolly");
        }
        /// <summary>
        /// 收录只在 1.4.2.101 中存在的旧名称。
        /// </summary>
        private static void AddClassicOnlyDebuffs(string modName)
        {
            AddIfPresent(modName, "GlacialState");
            AddIfPresent(modName, "HolyLight");
            AddIfPresent(modName, "AbyssalFlames");
            AddIfPresent(modName, "Horror");
            AddIfPresent(modName, "MarkedforDeath");
        }
        /// <summary>
        /// 仅在指定模组实际加载且存在同名减益时加入，保证全部分支可独立加载。
        /// </summary>
        private static void AddIfPresent(string modName, string buffName)
        {
            if (ModContent.TryFind<ModBuff>(modName, buffName, out ModBuff buff))
                debuffs.Add(buff.Type);
        }
        /// <summary>仅在指定模组存在同名减益时加入专用分类集合。</summary>
        private static void AddIfPresent(HashSet<int> collection, string modName, string buffName)
        {
            if (ModContent.TryFind<ModBuff>(modName, buffName, out ModBuff buff))
                collection.Add(buff.Type);
        }
        /// <summary>
        /// 清空跨模组静态缓存，避免热重载后继续使用失效的 Buff ID。
        /// </summary>
        /// <summary>
        /// 按月亮事件、主群系和星陨感染环境提供对应增益。
        /// pumpkinBuff 允许调用方指定南瓜月分支使用的饱食等级。
        /// </summary>
        internal static void ApplyEnvironmentBuffs(Player player, int pumpkinBuff)
        {
            if (Main.bloodMoon)
                player.AddBuff(BuffID.Battle, 600);
            if (Main.snowMoon)
                player.AddBuff(BuffID.RapidHealing, 600);
            if (Main.pumpkinMoon)
                player.AddBuff(pumpkinBuff, 600);
            else if (player.ZoneJungle)
                player.AddBuff(BuffID.Thorns, 600);
            else if (player.ZoneSnow)
                player.AddBuff(BuffID.Warmth, 600);
            else if (player.ZoneBeach)
                player.AddBuff(BuffID.Wet, 600);
            else if (player.ZoneCorrupt)
                player.AddBuff(BuffID.Wrath, 600);
            else if (player.ZoneCrimson)
                player.AddBuff(BuffID.Rage, 600);
            else if (player.ZoneDungeon)
                player.AddBuff(BuffID.Dangersense, 600);
            else if (player.ZoneDesert)
                player.AddBuff(BuffID.Endurance, 600);
            else if (player.ZoneGlowshroom)
                player.AddBuff(BuffID.Spelunker, 600);
            else if (player.ZoneUnderworldHeight)
                player.AddBuff(BuffID.Inferno, 600);
            else if (player.ZoneHallow)
                player.AddBuff(BuffID.Heartreach, 600);
            else if (player.ZoneTowerNebula)
                player.AddBuff(BuffID.MagicPower, 600);
            else if (player.ZoneTowerStardust)
                player.AddBuff(BuffID.Summoning, 600);
            else if (player.ZoneTowerSolar)
                player.AddBuff(BuffID.Titan, 600);
            else if (player.ZoneTowerVortex)
                player.AddBuff(BuffID.AmmoReservation, 600);
            else
                player.AddBuff(BuffID.DryadsWard, 600);
            if (!CalamityCompatUtil.IsInAstralInfection(player))
                return;
            if (ModLoader.HasMod("CalamityMod"))
                CalamityCompatUtil.TryAddBuff(player, "CalamityMod"
                    , "GravityNormalizerBuff", 600);
            else if (ModLoader.HasMod("CalRD"))
                CalamityCompatUtil.TryAddBuff(player, "CalRD"
                    , "GravityNormalizerBuff", 600);
            else if (ModLoader.HasMod("CalamityModClassicPreTrailer"))
                CalamityCompatUtil.TryAddBuff(player
                    , "CalamityModClassicPreTrailer", "GravityNormalizerBuff", 600);
        }
        internal static void Unload()
        {
            debuffs = null;
            sicknessDebuffs = null;
            fireDebuffs = null;
        }
    }
}
