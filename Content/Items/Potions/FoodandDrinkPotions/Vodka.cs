using CalamityRebirth.Content.ModBuffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>伏特加：八分钟内提高伤害与暴击，降低防御和少量再生。</summary>
    public class Vodka : AlcoholPotionItem
    {
        /// <summary>注册伏特加透明灰色饮用粒子。</summary>
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            SetAlcoholVisuals(new Color(219, 219, 208, 180), new Color(181, 181, 176, 180));
        }
        /// <summary>采用现代浅红稀有度、显式 2 银价格和 9999 堆叠。</summary>
        public override void SetDefaults() => SetAlcoholDefaults(28, 18
            , ModContent.BuffType<VodkaBuff>(), 8 * 60 * 60, ItemRarityID.LightRed, 9999);
        /// <summary>注册现代圣矿配方、经典圣锭配方和瓶装水血珠配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("HallowedOre", out ModItem hallowedOre)
                && modern.TryFind<ModItem>("BloodOrb", out ModItem modernBloodOrb))
            {
                CreateRecipe().AddIngredient(ItemID.Ale).AddIngredient(hallowedOre.Type, 3)
                    .AddTile(TileID.Kegs).Register();
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(modernBloodOrb.Type, 5)
                    .AddIngredient(hallowedOre.Type).AddTile(TileID.AlchemyTable)
                    .Register().DisableDecraft();
            }
            foreach (string modName in new[] { "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                    continue;
                CreateRecipe().AddIngredient(ItemID.Ale).AddIngredient(ItemID.HallowedBar)
                    .AddTile(TileID.Kegs).Register();
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(bloodOrb.Type, 5)
                    .AddIngredient(ItemID.HallowedBar).AddTile(TileID.AlchemyTable)
                    .Register().DisableDecraft();
            }
        }
    }
}
