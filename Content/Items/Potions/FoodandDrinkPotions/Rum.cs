using CalamityRebirth.Content.ModBuffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>朗姆酒：八分钟内提高再生和移速，固定降低 8 点防御。</summary>
    public class Rum : AlcoholPotionItem
    {
        /// <summary>注册朗姆酒金黄色饮用粒子。</summary>
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            SetAlcoholVisuals(new Color(237, 165, 9), new Color(247, 219, 54), new Color(255, 195, 31));
        }
        /// <summary>采用现代浅红稀有度、显式 2 银价格和 9999 堆叠。</summary>
        public override void SetDefaults() => SetAlcoholDefaults(28, 18
            , ModContent.BuffType<RumBuff>(), 8 * 60 * 60, ItemRarityID.LightRed, 9999);
        /// <summary>注册瓶装水、飞翔之魂、羽毛和蜘蛛牙的酒桶配方。</summary>
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(ItemID.SoulofFlight)
                .AddIngredient(ItemID.Feather).AddIngredient(ItemID.SpiderFang)
                .AddTile(TileID.Kegs).Register();
        }
    }
}
