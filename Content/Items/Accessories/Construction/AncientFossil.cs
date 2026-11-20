using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Construction
{
    /// <summary>远古化石：地下与洞穴层提高挖掘速度。</summary>
    internal sealed class AncientFossil : ModItem
    {
        /// <summary>设置 26×26 判定、三金价格、蓝色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().ancientFossil = true;
        }
        /// <summary>注册现代与经典分支的一百块泥沙配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
                CreateRecipe().AddRecipeGroup("AnySiltBlock", 100).AddTile(TileID.Furnaces).Register();
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out _))
                CreateRecipe().AddRecipeGroup("SiltGroup", 100).AddTile(TileID.Furnaces).Register();
            if (ModLoader.TryGetMod("CalRD", out _))
                CreateRecipe().AddRecipeGroup("SiltGroup", 100).AddTile(TileID.Furnaces).Register();
        }
    }
}
