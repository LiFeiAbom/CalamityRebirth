using Microsoft.Xna.Framework;
using Terraria;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 五套 EX 稀有度共用的循环渐变计算，口径与灾厄 MulticolorLerp 一致。
    /// </summary>
    internal static class EXRarityGradient
    {
        /// <summary>每秒推进的完整色带比例，数值越低颜色流动越缓慢。</summary>
        private const float CycleSpeed = 0.35f;
        /// <summary>
        /// 根据全局游戏时间在颜色数组间循环插值。
        /// 计算方式刻意匹配灾厄 MulticolorLerp，使五套职业 EX 稀有度保持相同节奏。
        /// </summary>
        /// <param name="colors">按循环顺序排列的色带。</param>
        /// <returns>当前帧对应的插值颜色。</returns>
        public static Color GetColor(Color[] colors)
        {
            float progress = Main.GlobalTimeWrappedHourly * CycleSpeed % 1f;
            float scaled = progress * colors.Length;
            int index = (int)scaled % colors.Length;
            int next = (index + 1) % colors.Length;
            return Color.Lerp(colors[index], colors[next], scaled - (int)scaled);
        }
    }
}
