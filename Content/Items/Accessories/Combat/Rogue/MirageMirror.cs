using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>幻影魔镜：镜片潜行链的第一件。</summary>
    internal sealed class MirageMirror : ModItem
    {
        /// <summary>使用现代稀有度：橙色。</summary>
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        /// <summary>装备时置位潜行链状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().mirageMirror = true;
        }
        /// <summary>魔法镜或冰雪镜 + 黑透镜 + 骨头×50 @ 工匠作坊。</summary>
        public override void AddRecipes()
        {
            AddRecipe(ItemID.MagicMirror);
            AddRecipe(ItemID.IceMirror);
        }
        /// <summary>注册指定镜子变体的幻影魔镜配方。</summary>
        private void AddRecipe(int mirrorType)
        {
            CreateRecipe()
                .AddIngredient(mirrorType)
                .AddIngredient(ItemID.BlackLens)
                .AddIngredient(ItemID.Bone, 50)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
