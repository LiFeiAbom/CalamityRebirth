using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>月后 14 级深蓝色稀有度，用于现代版本中已删除但需要保留原档位的饰品。</summary>
    internal sealed class PostMoonLordBlueRarity : ModRarity
    {
        /// <summary>深蓝色名称颜色。</summary>
        public override Color RarityColor => new(43, 96, 222);
        /// <summary>低偏移回退到青/蓝，高位保持当前稀有度。</summary>
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return offset switch
            {
                -2 => ItemRarityID.Cyan,
                -1 => ItemRarityID.Blue,
                _ => Type
            };
        }
    }
}
