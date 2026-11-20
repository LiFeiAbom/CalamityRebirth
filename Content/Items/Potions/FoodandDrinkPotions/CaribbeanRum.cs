using CalamityRebirth.Content.ModBuffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>加勒比朗姆酒：八分钟内提高再生和移速，降低当前防御并漂浮。</summary>
    public class CaribbeanRum : AlcoholPotionItem
    {
        /// <summary>注册加勒比朗姆酒棕红饮用粒子。</summary>
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            SetAlcoholVisuals(new Color(105, 29, 14), new Color(128, 39, 22), new Color(138, 28, 7));
        }
        /// <summary>采用现代粉色稀有度、显式 2 银价格和 9999 堆叠。</summary>
        public override void SetDefaults() => SetAlcoholDefaults(28, 18
            , ModContent.BuffType<CaribbeanRumBuff>(), 8 * 60 * 60, ItemRarityID.Pink, 9999);
        /// <summary>注册三版本 LivingShard 配方和瓶装水血珠配方。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("LivingShard", out ModItem livingShard))
                    continue;
                CreateRecipe().AddIngredient(ItemID.Ale).AddIngredient(livingShard.Type)
                    .AddTile(TileID.Kegs).Register();
                if (mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(bloodOrb.Type, 5)
                        .AddIngredient(livingShard.Type).AddTile(TileID.AlchemyTable)
                        .Register().DisableDecraft();
                }
            }
        }
    }
}
