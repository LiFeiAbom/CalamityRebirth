using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>魔影套使用的专属橙红动态稀有度，对应现代 Hot Pink 开发者档的自定义色。</summary>
    public class DemonshadeRarity : ModRarity
    {
        /// <summary>每四秒在亮橙与深橙红之间完成一次平滑往返。</summary>
        public override Color RarityColor
        {
            get
            {
                double timeMultiplier = MathHelper.TwoPi / 4.0;
                float blend = (float)((Math.Sin(timeMultiplier * Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
                return Color.Lerp(new Color(255, 132, 22), new Color(221, 85, 7), blend);
            }
        }
    }
}
