using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>静默剑鞘：提高盗贼潜行上限与恢复速度的前期组件。</summary>
    internal sealed class SilencingSheath : ModItem
    {
        /// <summary>使用现代稀有度：绿色。</summary>
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        /// <summary>装备时置位潜行链状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().silencingSheath = true;
        }
        /// <summary>使用任意邪恶锭、丝绸与二阶 Boss 材料在工匠作坊制作。</summary>
        public override void AddRecipes()
        {
            if (!RecipeGroup.recipeGroupIDs.ContainsKey("AnyEvilBar")
                || !RecipeGroup.recipeGroupIDs.ContainsKey("Boss2Material"))
                return;
            CreateRecipe()
                .AddRecipeGroup("AnyEvilBar", 8)
                .AddIngredient(ItemID.Silk, 10)
                .AddRecipeGroup("Boss2Material", 3)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
