using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 月后 15 级使用的紫色稀有度。
    /// 使用 2.0.3.9 月后 15 级名称颜色，并由统一价格表映射为 1 铂金 50 金。
    /// </summary>
    internal class PostMoonLordPurpleRarity : ModRarity
    {
        /// <summary>月后 15 级使用的紫色名称颜色。</summary>
        public override Color RarityColor => new(108, 45, 199);
        /// <summary>
        /// 将重铸偏移映射到已有稀有度；向上保持月后紫色，避免降回普通红色显示。
        /// </summary>
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return offset switch
            {
                -2 => ItemRarityID.Red,
                -1 => ItemRarityID.Purple,
                _ => Type
            };
        }
    }
}
