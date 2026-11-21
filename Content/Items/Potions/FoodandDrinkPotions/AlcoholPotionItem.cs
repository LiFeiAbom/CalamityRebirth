using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>
    /// 酒类药水公共面板：统一 17 帧饮用动作、消耗品标记与现代价格/稀有度入口。
    /// </summary>
    public abstract class AlcoholPotionItem : ModItem
    {
        /// <summary>统一设置现代研究解锁数量。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }
        /// <summary>注册酒类饮用粒子和食物标记。</summary>
        protected void SetAlcoholVisuals(params Color[] colors)
        {
            ItemID.Sets.DrinkParticleColors[Type] = colors;
            ItemID.Sets.IsFood[Type] = true;
        }
        /// <summary>按指定尺寸、堆叠和持续时间设置酒类面板。</summary>
        /// <param name="width">物品宽度字段。</param>
        /// <param name="height">物品高度字段。</param>
        /// <param name="buffType">酒类 ModBuff 类型。</param>
        /// <param name="buffTime">持续时间帧数。</param>
        /// <param name="rarity">现代 2.2.2 可确认的稀有度。</param>
        /// <param name="maxStack">最大堆叠。</param>
        protected void SetAlcoholDefaults(int width, int height, int buffType, int buffTime
            , int rarity, int maxStack)
        {
            Item.width = width;
            Item.height = height;
            Item.useTurn = true;
            Item.maxStack = maxStack;
            RarityUtil.Apply(Item, rarity, Item.sellPrice(silver: 2));
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = buffType;
            Item.buffTime = buffTime;
        }
    }
}
