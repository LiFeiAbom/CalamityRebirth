using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 战士系 EX 饰品专属稀有度，采用灾厄日耀橙红的四段循环渐变。
    /// </summary>
    internal class WarriorEXRarity : ModRarity
    {
        /// <summary>日耀橙红系的暗红、亮红、橙色和浅金色循环色带。</summary>
        private static readonly Color[] GradientColors =
        {
            new Color(200, 44, 18),
            new Color(245, 30, 18),
            new Color(255, 96, 16),
            new Color(255, 160, 50),
        };
        /// <summary>实时读取全局渐变颜色，使物品名称和稀有度描边持续流动。</summary>
        private static Color GradientColor => EXRarityGradient.GetColor(GradientColors);
        /// <summary>返回当前帧的战士 EX 稀有度颜色。</summary>
        public override Color RarityColor => GradientColor;
    }
}
