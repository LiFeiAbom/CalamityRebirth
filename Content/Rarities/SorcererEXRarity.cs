using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 法师EX 稀有度（SorcererEXRarity）——本工程「魔法系 EX 饰品」的专属档位，与战士EX / 游侠EX 档一一对应，
    /// 配色为靛蓝 → 亮紫 → 品红 → 冰蓝。目前由法师徽章EX 与天界纹章EX 共用。
    /// </summary>
    internal class SorcererEXRarity : ModRarity
    {
        /// <summary>渐变色谱（按顺序循环）：星云靛蓝 → 亮紫 → 品红 → 冰蓝</summary>
        private static readonly Color[] GradientColors =
        {
            new Color(80, 76, 164),
            new Color(132, 37, 228),
            new Color(218, 18, 168),
            new Color(155, 227, 255),
        };
        /// <summary>当前渐变色：按时间取进度，在色谱上循环插值</summary>
        private static Color GradientColor => EXRarityGradient.GetColor(GradientColors);
        /// <summary>法师EX 档位配色：星云紫/品红/冰蓝渐变</summary>
        public override Color RarityColor => GradientColor;
    }
}
