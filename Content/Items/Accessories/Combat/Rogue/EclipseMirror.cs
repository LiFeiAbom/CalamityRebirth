using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>蚀日魔镜：镜片潜行链的最终件。</summary>
    internal sealed class EclipseMirror : ModItem
    {
        /// <summary>用户指定使用弑神者紫色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 38;
            Item.value = Item.buyPrice(1, 40, 0, 0);
            Item.rare = ModContent.RarityType<CosmicPurpleRarity>();
            Item.accessory = true;
        }
        /// <summary>装备时置位最终潜行链状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().eclipseMirror = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 的最终配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代：深渊魔镜 + 暗物质剑鞘 + 暗日碎片×20 @ 星宇砧。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("DarksunFragment", out ModItem darksun)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient<AbyssalMirror>()
                .AddIngredient<DarkMatterSheath>()
                .AddIngredient(darksun.Type, 20)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>1.4.2.101：深渊魔镜 + 暗物质剑鞘 + 暗日碎片×20 @ 嘉登熔炉。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("DarksunFragment", out ModItem darksun)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<AbyssalMirror>()
                .AddIngredient<DarkMatterSheath>()
                .AddIngredient(darksun.Type, 20)
                .AddTile(draedonsForge.Type)
                .Register();
        }
        /// <summary>CalRD：额外使用暗日碎片×10与化神魂精，在嘉登熔炉制作。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("DarksunFragment", out ModItem darksun)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendant)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<AbyssalMirror>()
                .AddIngredient<DarkMatterSheath>()
                .AddIngredient(darksun.Type, 10)
                .AddIngredient(ascendant.Type)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
