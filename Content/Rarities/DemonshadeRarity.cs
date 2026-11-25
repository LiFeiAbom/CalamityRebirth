using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>魔影套与同档后期物品使用的品红稀有度。</summary>
    public class DemonshadeRarity : ModRarity
    {
        /// <summary>使用月后 16 档的纯品红名称颜色。</summary>
        public override Color RarityColor => new(255, 0, 255);
        /// <summary>
        /// 低两档回退红色，低一档回退紫色；正向重铸保持当前后期档位。
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
