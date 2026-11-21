using CalamityRebirth.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 稀有度与购买价的通用入口。
    /// 同一个方法同时设置稀有度和价格，避免物品重复书写标准价格表。
    /// </summary>
    internal static class RarityUtil
    {
        /// <summary>
        /// 将指定稀有度应用到物品，并按对应档位写入购买价。
        /// </summary>
        /// <param name="item">正在建立默认值的物品。</param>
        /// <param name="rarity">原版或本工程使用的稀有度类型。</param>
        /// <param name="explicitValue">需要覆盖标准价时传入的精确价值；未传入时使用稀有度标准价。</param>
        internal static void Apply(Item item, int rarity, int? explicitValue = null)
        {
            item.rare = rarity;
            item.value = explicitValue ?? GetBuyPrice(rarity);
        }
        /// <summary>
        /// 返回对应稀有度的标准购买价。
        /// 未列入表的颜色返回 0，避免猜测不属于当前价格档的数值。
        /// </summary>
        private static int GetBuyPrice(int rarity)
        {
            if (rarity == ModContent.RarityType<PostMoonLordPurpleRarity>())
                return Item.buyPrice(1, 50, 0, 0);
            if (rarity == ModContent.RarityType<CosmicPurpleRarity>())
                return Item.buyPrice(2, 0, 0, 0);
            if (rarity == ModContent.RarityType<TurquoiseRarity>())
                return Item.buyPrice(1, 50, 0, 0);
            if (rarity == ModContent.RarityType<PureGreenRarity>())
                return Item.buyPrice(1, 75, 0, 0);
            if (rarity == ModContent.RarityType<BurnishedAuricRarity>())
                return Item.buyPrice(2, 40, 0, 0);
            return rarity switch
            {
                ItemRarityID.White => Item.buyPrice(0, 0, 50, 0),
                ItemRarityID.Blue => Item.buyPrice(0, 1, 0, 0),
                ItemRarityID.Green => Item.buyPrice(0, 2, 0, 0),
                ItemRarityID.Orange => Item.buyPrice(0, 5, 0, 0),
                ItemRarityID.LightRed => Item.buyPrice(0, 10, 0, 0),
                ItemRarityID.Pink => Item.buyPrice(0, 20, 0, 0),
                ItemRarityID.LightPurple => Item.buyPrice(0, 35, 0, 0),
                ItemRarityID.Lime => Item.buyPrice(0, 45, 0, 0),
                ItemRarityID.Yellow => Item.buyPrice(0, 60, 0, 0),
                ItemRarityID.Cyan => Item.buyPrice(0, 80, 0, 0),
                ItemRarityID.Red => Item.buyPrice(1, 0, 0, 0),
                ItemRarityID.Purple => Item.buyPrice(1, 20, 0, 0),
                _ => 0
            };
        }
    }
}
