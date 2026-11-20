using CalamityRebirth.Content.ModBuffs.Debuffs;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 宝石系饰品使用的有害减益清单。
    /// 清单包含五种本工程战斗减益、原版战斗减益与已加载灾厄版本中的同名战斗减益；
    /// 长时酒类减益和药水病等非该链目标不会被计入，避免持续酒醉长期触发宝石强化。
    /// </summary>
    internal static class JewelDebuffUtil
    {
        /// <summary>缓存本工程与已加载灾厄版本实际注册的减益类型。</summary>
        private static HashSet<int> debuffs;
        /// <summary>
        /// 判断指定 buff 类型是否属于宝石系要统计的负面效果。
        /// </summary>
        internal static bool Contains(int buffType)
        {
            EnsureInitialized();
            return debuffs.Contains(buffType);
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
                ModContent.BuffType<ElementalMixII>(),
                ModContent.BuffType<ShadowflameII>()
            ];
            AddCalamityDebuffs("CalamityMod");
            AddCalamityDebuffs("CalamityModClassicPreTrailer");
            AddCalamityDebuffs("CalRD");
            AddClassicOnlyDebuffs("CalamityModClassicPreTrailer");
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
        /// <summary>
        /// 清空跨模组静态缓存，避免热重载后继续使用失效的 Buff ID。
        /// </summary>
        internal static void Unload()
        {
            debuffs = null;
        }
    }
}
