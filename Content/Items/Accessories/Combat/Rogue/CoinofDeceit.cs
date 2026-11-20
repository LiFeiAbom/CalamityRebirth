using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>欺诈硬币：盗贼镜片潜行链的早期潜行消耗饰品。</summary>
    internal sealed class CoinofDeceit : ModItem
    {
        /// <summary>使用现代稀有度：蓝色。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        /// <summary>装备时置位潜行链状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().coinofDeceit = true;
        }
        /// <summary>使用任意铜锭×12与任意邪恶锭×8在铁砧制作。</summary>
        public override void AddRecipes()
        {
            if (!RecipeGroup.recipeGroupIDs.ContainsKey("AnyCopperBar")
                || !RecipeGroup.recipeGroupIDs.ContainsKey("AnyEvilBar"))
                return;
            CreateRecipe()
                .AddRecipeGroup("AnyCopperBar", 12)
                .AddRecipeGroup("AnyEvilBar", 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
