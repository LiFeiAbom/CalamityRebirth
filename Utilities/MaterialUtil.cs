using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 配方材料兼容分部。经典 1.4.2.101 缺少现代版化神魂精，
    /// 因此这里把一份魂精换算为四种经典终局材料的等价组合。
    /// </summary>
    internal static partial class CRUtil
    {
        /// <summary>每份经典金源锭兑换所需的金源矿数量。</summary>
        internal const int AuricOrePerBar = 5;
        /// <summary>每份经典金源锭兑换所需的龙魂碎片数量。</summary>
        internal const int DragonSoulFragmentPerBar = 1;
        /// <summary>每份化神魂精对应的梦魇燃料数量。</summary>
        internal const int NightmareFuelPerEssence = 5;
        /// <summary>每份化神魂精对应的噬魂胶质数量。</summary>
        internal const int NecroplasmPerEssence = 2;
        /// <summary>每份化神魂精对应的恒温能量数量。</summary>
        internal const int EndothermicEnergyPerEssence = 5;
        /// <summary>每份化神魂精对应的暗日碎片数量。</summary>
        internal const int DarksunFragmentPerEssence = 5;
        /// <summary>
        /// 尝试向配方追加经典版化神魂精等价材料。
        /// 任一前置模组或 ModItem 缺失时返回 false，调用方应放弃注册该配方。
        /// </summary>
        /// <param name="recipe">正在构建且尚未注册的配方。</param>
        /// <param name="essenceCount">需要模拟的现代化神魂精份数。</param>
        /// <returns>所有等价材料均已成功追加时返回 true。</returns>
        internal static bool TryAddClassicAscendantSpiritEssence(Recipe recipe, int essenceCount)
        {
            if (essenceCount <= 0
                || !ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind("Phantoplasm", out ModItem phantoplasm)
                || !classic.TryFind("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind("DarksunFragment", out ModItem darksunFragment))
                return false;
            recipe.AddIngredient(nightmareFuel.Type, NightmareFuelPerEssence * essenceCount);
            recipe.AddIngredient(phantoplasm.Type, NecroplasmPerEssence * essenceCount);
            recipe.AddIngredient(endothermicEnergy.Type, EndothermicEnergyPerEssence * essenceCount);
            recipe.AddIngredient(darksunFragment.Type, DarksunFragmentPerEssence * essenceCount);
            return true;
        }
        /// <summary>
        /// 将现代金源锭换算为 1.4.2.101 的金源矿与龙魂碎片。
        /// 经典版没有 AuricBar 物品，因此每锭固定使用五矿一碎片。
        /// </summary>
        /// <param name="recipe">正在构建且尚未注册的配方。</param>
        /// <param name="barCount">需要模拟的现代金源锭数量。</param>
        /// <returns>所有等价材料均成功加入时返回 true。</returns>
        internal static bool TryAddClassicAuricBar(Recipe recipe, int barCount)
        {
            if (barCount <= 0
                || !ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind("AuricOre", out ModItem auricOre)
                || !classic.TryFind("HellcasterFragment", out ModItem dragonSoulFragment))
                return false;
            recipe.AddIngredient(auricOre.Type, AuricOrePerBar * barCount);
            recipe.AddIngredient(dragonSoulFragment.Type, DragonSoulFragmentPerBar * barCount);
            return true;
        }
    }
}
