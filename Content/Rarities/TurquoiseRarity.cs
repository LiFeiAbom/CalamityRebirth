using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 现代灾厄 Turquoise / 第 12 档稀有度。
    /// 映射用于饰品重铸：低两档回退红色，低一档回退紫色；更高档在本工程直接保持不变。
    /// </summary>
    public class TurquoiseRarity : ModRarity
    {
        /// <summary>现代灾厄使用的青绿色基础字色。</summary>
        public override Color RarityColor => new(0, 255, 200);
        /// <summary>
        /// 将重铸偏移映射到本工程现有稀有度。
        /// 现代版向上会进入 PureGreen；本工程已有该档，因此正向重铸严格复用。
        /// </summary>
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return offset switch
            {
                -2 => ItemRarityID.Red,
                -1 => ItemRarityID.Purple,
                1 or 2 => ModContent.RarityType<PureGreenRarity>(),
                _ => Type
            };
        }
    }
}
