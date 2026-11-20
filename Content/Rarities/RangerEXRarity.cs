using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 游侠EX 稀有度（RangerEXRarity）——本工程「远程系 EX 饰品」的专属档位，与战士EX 档（<see cref="WarriorEXRarity"/>）
    /// 一一对应，只是配色从日曜橙红换成**星旋盔甲的绿 → 青**。目前由游侠徽章EX 使用。
    /// <para>
    /// 取色规则（用户 2026-10-17 指定「稀有度颜色和星旋盔甲的色系对应」）：
    /// 深绿 `(24,93,66)` 是甲片暗部、绿 `(47,184,131)` 是主体甲片色、
    /// 亮绿 `(34,221,151)` 是甲片高光、青 `(67,255,255)` 是胸口能量光。
    /// </para>
    /// <para>
    /// 写法与 <see cref="WarriorEXRarity"/> 同源：每帧读
    /// <see cref="Main.GlobalTimeWrappedHourly"/> 算进度，在四色之间循环插值（口径同灾厄的 MulticolorLerp）。
    /// </para>
    /// </summary>
    internal class RangerEXRarity : ModRarity
    {
        /// <summary>渐变色谱（按顺序循环）：深绿 → 绿 → 亮绿 → 青。</summary>
        private static readonly Color[] GradientColors =
        {
            new Color(24, 93, 66),
            new Color(47, 184, 131),
            new Color(34, 221, 151),
            new Color(67, 255, 255),
        };
        /// <summary>当前渐变色：按时间取进度，在色谱上循环插值</summary>
        private static Color GradientColor => EXRarityGradient.GetColor(GradientColors);
        /// <summary>游侠EX 档位配色：星旋绿青渐变（每帧被读取，所以物品名与稀有度描边会持续流动）</summary>
        public override Color RarityColor => GradientColor;
        // 说明：与战士EX 档一样不覆盖 GetPrefixedRarity——本工程目前没有与游侠EX 同级的兄弟档可挂。
    }
}
