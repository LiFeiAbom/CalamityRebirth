using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Construction
{
    /// <summary>太古粉末：地下、洞穴与地狱层提高防御、减伤和挖掘速度。</summary>
    internal sealed class ArchaicPowder : ModItem
    {
        /// <summary>设置 20×20 判定、九金价格、橙色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().archaicPowder = true;
        }
        /// <summary>注册三个分支的化石、骨粉和原版骨材料配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassicRecipe();
            AddCalRDRecipe();
        }
        /// <summary>现代分支使用远古凿子、古代骨尘与骨头。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod modern)
                || !modern.TryFind<ModItem>("AncientBoneDust", out ModItem boneDust))
                return;
            CreateRecipe()
                .AddIngredient<AncientFossil>()
                .AddIngredient(ItemID.AncientChisel)
                .AddIngredient(boneDust.Type, 3)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
        /// <summary>经典分支使用恶魔骨灰、古代骨尘与腐肉。</summary>
        private void AddClassicRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod mod)
                || !mod.TryFind<ModItem>("DemonicBoneAsh", out ModItem boneAsh)
                || !mod.TryFind<ModItem>("AncientBoneDust", out ModItem boneDust))
                return;
            CreateRecipe()
                .AddIngredient<AncientFossil>()
                .AddIngredient(boneAsh.Type)
                .AddIngredient(boneDust.Type, 3)
                .AddIngredient(ItemID.RottenChunk, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
        /// <summary>CalRD 分支使用远古化石、恶魔骨灰、古代骨尘与骨头。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod mod)
                || !mod.TryFind<ModItem>("DemonicBoneAsh", out ModItem boneAsh)
                || !mod.TryFind<ModItem>("AncientBoneDust", out ModItem boneDust))
                return;
            CreateRecipe()
                .AddIngredient<AncientFossil>()
                .AddIngredient(boneAsh.Type)
                .AddIngredient(boneDust.Type, 3)
                .AddIngredient(ItemID.Bone, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
