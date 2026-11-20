using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>深渊魔镜：提供高阶潜行恢复、仇恨降低与一次闪避。</summary>
    internal sealed class AbyssalMirror : ModItem
    {
        /// <summary>使用现代稀有度：青柠色。</summary>
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }
        /// <summary>装备时置位潜行链状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().abyssalMirror = true;
        }
        /// <summary>分别注册现代、经典 1.4.2.101 与 CalRD 的深渊材料配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代：幻影魔镜 + 墨炸弹 + 深渊细胞×20 + 流明素×15 @ 秘银砧。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("InkBomb", out ModItem inkBomb)
                || !calamity.TryFind<ModItem>("DepthCells", out ModItem depthCells)
                || !calamity.TryFind<ModItem>("Lumenyl", out ModItem lumenyl))
                return;
            CreateRecipe()
                .AddIngredient<MirageMirror>()
                .AddIngredient(inkBomb.Type)
                .AddIngredient(depthCells.Type, 20)
                .AddIngredient(lumenyl.Type, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>1.4.2.101 使用幽魂锭、海棱晶、深渊细胞与流明石。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("SeaPrism", out ModItem seaPrism)
                || !classic.TryFind<ModItem>("DepthCells", out ModItem depthCells)
                || !classic.TryFind<ModItem>("Lumenite", out ModItem lumenite))
                return;
            CreateRecipe()
                .AddIngredient<MirageMirror>()
                .AddIngredient(ItemID.SpectreBar, 8)
                .AddIngredient(seaPrism.Type, 10)
                .AddIngredient(depthCells.Type, 5)
                .AddIngredient(lumenite.Type, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>CalRD 使用墨炸弹、幽魂锭、深渊细胞与流明石。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("InkBomb", out ModItem inkBomb)
                || !calRD.TryFind<ModItem>("DepthCells", out ModItem depthCells)
                || !calRD.TryFind<ModItem>("Lumenite", out ModItem lumenite))
                return;
            CreateRecipe()
                .AddIngredient<MirageMirror>()
                .AddIngredient(inkBomb.Type)
                .AddIngredient(ItemID.SpectreBar, 8)
                .AddIngredient(depthCells.Type, 5)
                .AddIngredient(lumenite.Type, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
