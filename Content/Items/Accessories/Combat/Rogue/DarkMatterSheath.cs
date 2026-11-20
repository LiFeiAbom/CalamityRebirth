using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>暗物质剑鞘：静默剑鞘与毁灭徽章汇合后的高阶潜行组件。</summary>
    internal sealed class DarkMatterSheath : ModItem
    {
        /// <summary>使用现代稀有度：红色。</summary>
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 62;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }
        /// <summary>装备时置位潜行链状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().darkMatterSheath = true;
        }
        /// <summary>分别注册现代/经典的融构块配方与 CalRD 的融构锭配方。</summary>
        public override void AddRecipes()
        {
            AddMeldBlobRecipe("CalamityMod");
            AddMeldBlobRecipe("CalamityModClassicPreTrailer");
            AddCalRDRecipe();
        }
        /// <summary>使用静默剑鞘、毁灭徽章与融构块×14，在远古操纵机制作。</summary>
        private void AddMeldBlobRecipe(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod calamity)
                || !calamity.TryFind<ModItem>("MeldBlob", out ModItem meldBlob))
                return;
            CreateRecipe()
                .AddIngredient<SilencingSheath>()
                .AddIngredient<RuinMedallion>()
                .AddIngredient(meldBlob.Type, 14)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>CalRD 使用融构锭×5替代融构块。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("MeldiateBar", out ModItem meldiateBar))
                return;
            CreateRecipe()
                .AddIngredient<SilencingSheath>()
                .AddIngredient<RuinMedallion>()
                .AddIngredient(meldiateBar.Type, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
