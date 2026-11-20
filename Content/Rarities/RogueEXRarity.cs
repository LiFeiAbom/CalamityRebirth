using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>盗贼EX 稀有度：皇天套装的暗红→暗紫→暗蓝→灰紫高光。</summary>
    internal class RogueEXRarity : ModRarity
    {
        /// <summary>皇天盔甲色系的暗红、暗紫、暗蓝和灰紫高光循环色带。</summary>
        private static readonly Color[] GradientColors =
        {
            new Color(20, 6, 10),
            new Color(60, 42, 56),
            new Color(33, 54, 92),
            new Color(97, 82, 88),
        };
        /// <summary>按全局时间读取盗贼系 EX 稀有度的当前渐变色。</summary>
        private static Color GradientColor => EXRarityGradient.GetColor(GradientColors);
        /// <summary>返回当前帧的盗贼 EX 稀有度颜色。</summary>
        public override Color RarityColor => GradientColor;
    }
}
