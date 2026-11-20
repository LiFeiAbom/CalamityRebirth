using CalamityRebirth.Content.ModBuffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>血腥玛丽：六分钟高强度进攻与移速增益，同时承受防御、减伤和大量再生惩罚。</summary>
    public class BloodyMary : AlcoholPotionItem
    {
        /// <summary>注册血腥玛丽暗红饮用粒子。</summary>
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            SetAlcoholVisuals(new Color(204, 90, 73), new Color(168, 37, 37), new Color(120, 28, 28));
        }
        /// <summary>采用现代蓝色稀有度、显式 2 银价格和 9999 堆叠。</summary>
        public override void SetDefaults() => SetAlcoholDefaults(20, 32
            , ModContent.BuffType<BloodyMaryBuff>(), 6 * 60 * 60, ItemRarityID.Blue, 9999);
        /// <summary>注册血月召唤物配方，以及三版本瓶装水血珠配方。</summary>
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(ItemID.Ale)
                .AddIngredient(ItemID.BloodMoonStarter).AddTile(TileID.Kegs).Register();
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                    continue;
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(bloodOrb.Type, 50)
                    .AddTile(TileID.Kegs).Register();
            }
        }
    }
}
