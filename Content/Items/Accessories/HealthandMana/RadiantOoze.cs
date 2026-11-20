using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>辉光软泥：夜间提供暖光和生命再生。</summary>
    internal sealed class RadiantOoze : ModItem
    {
        /// <summary>设置 20×20 判定、六金价格、浅红色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().radiantOoze = true;
        }
        /// <summary>注册三个分支的邪恶凝胶与纯净凝胶配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("BlightedGel", out ModItem blighted)
                && modern.TryFind<ModItem>("PurifiedGel", out ModItem purified))
                CreateRecipe().AddIngredient(blighted.Type, 45).AddIngredient(purified.Type, 15)
                    .AddTile(TileID.Anvils).Register();
            AddClassicRecipe("CalamityModClassicPreTrailer");
            AddClassicRecipe("CalRD");
        }
        /// <summary>经典分支使用混浊泥浆与纯净凝胶。</summary>
        private void AddClassicRecipe(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("MurkySludge", out ModItem sludge)
                || !mod.TryFind<ModItem>("PurifiedGel", out ModItem purified))
                return;
            CreateRecipe().AddIngredient(sludge.Type, 5).AddIngredient(purified.Type, 15)
                .AddTile(TileID.Anvils).Register();
        }
    }
}
