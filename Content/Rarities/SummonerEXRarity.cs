using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 召唤EX 稀有度（SummonerEXRarity）——本工程「召唤系 EX 饰品」的专属档位，
    /// 配色为深蓝紫 → 蓝 → 青 → 浅金。
    /// </summary>
    internal class SummonerEXRarity : ModRarity
    {
        /// <summary>星尘盔甲色系的深蓝紫、蓝、青色和浅金色循环色带。</summary>
        private static readonly Color[] GradientColors =
        {
            new Color(43, 40, 80),
            new Color(5, 120, 204),
            new Color(39, 255, 255),
            new Color(250, 216, 124),
        };
        /// <summary>按全局时间读取召唤系 EX 稀有度的当前渐变色。</summary>
        private static Color GradientColor => EXRarityGradient.GetColor(GradientColors);
        /// <summary>返回当前帧的召唤 EX 稀有度颜色。</summary>
        public override Color RarityColor => GradientColor;
    }
}
