using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 现代灾厄 PureGreen / 第 13 档稀有度。
    /// 低于该档时按紫色和 Turquoise 回退；更高档在现代版中均为独立稀有度，因此保持自身。
    /// </summary>
    public class PureGreenRarity : ModRarity
    {
        /// <summary>现代灾厄使用的纯绿色基础字色。</summary>
        public override Color RarityColor => new(0, 255, 0);
        /// <summary>根据重铸偏移回退到已有低档稀有度。</summary>
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return offset switch
            {
                -2 => ItemRarityID.Purple,
                -1 => ModContent.RarityType<TurquoiseRarity>(),
                _ => Type
            };
        }
    }
}
