using CalamityRebirth.Content.ModBuffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>摩闪酒：八分钟内提高防御和减伤，降低少量再生。</summary>
    public class Moonshine : AlcoholPotionItem
    {
        /// <summary>注册摩闪酒灰白饮用粒子。</summary>
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            SetAlcoholVisuals(new Color(237, 237, 218, 128), new Color(227, 219, 191, 128)
                , new Color(209, 204, 194, 128));
        }
        /// <summary>采用现代 Lime 稀有度、显式 2 银价格和 9999 堆叠。</summary>
        public override void SetDefaults() => SetAlcoholDefaults(28, 18
            , ModContent.BuffType<MoonshineBuff>(), 8 * 60 * 60, ItemRarityID.Lime, 9999);
        /// <summary>注册 ScoriaOre → CruptixBar → ChaoticOre 的双配方。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod))
                    continue;
                ModItem material = null;
                if (mod.TryFind<ModItem>("ScoriaOre", out ModItem scoria))
                    material = scoria;
                else if (mod.TryFind<ModItem>("CruptixBar", out ModItem cruptix))
                    material = cruptix;
                else if (mod.TryFind<ModItem>("ChaoticOre", out ModItem chaotic))
                    material = chaotic;
                if (material == null)
                    continue;
                CreateRecipe().AddIngredient(ItemID.Ale).AddIngredient(material.Type, 3)
                    .AddTile(TileID.Kegs).Register();
                if (mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(bloodOrb.Type, 5)
                        .AddIngredient(material.Type).AddTile(TileID.AlchemyTable)
                        .Register().DisableDecraft();
                }
            }
        }
    }
}
