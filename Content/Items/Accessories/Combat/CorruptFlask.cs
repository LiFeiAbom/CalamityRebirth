using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>腐化之瓶：身处腐化环境时提高防御与减伤。</summary>
    internal sealed class CorruptFlask : ModItem
    {
        /// <summary>设置 20×20 判定、三金价格、绿色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().corruptFlask = true;
        }
        /// <summary>注册现代与经典分支的邪恶粉、腐肉配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
                CreateRecipe().AddIngredient(ItemID.VilePowder, 3).AddIngredient(ItemID.RottenChunk, 10)
                    .AddTile(TileID.Anvils).Register();
            AddClassicRecipe("CalamityModClassicPreTrailer");
            if (ModLoader.TryGetMod("CalRD", out _))
                CreateRecipe().AddIngredient(ItemID.VilePowder, 15).AddIngredient(ItemID.RottenChunk, 10)
                    .AddTile(TileID.Anvils).Register();
        }
        /// <summary>经典分支使用腐化精粹与腐肉。</summary>
        private void AddClassicRecipe(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("FetidEssence", out ModItem essence))
                return;
            CreateRecipe().AddIngredient(essence.Type, 3).AddIngredient(ItemID.RottenChunk, 10)
                .AddTile(TileID.Anvils).Register();
        }
    }
}
