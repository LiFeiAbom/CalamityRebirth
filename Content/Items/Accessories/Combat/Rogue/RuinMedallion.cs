using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>毁灭徽章：把欺诈硬币推进为半价潜行打击饰品。</summary>
    internal sealed class RuinMedallion : ModItem
    {
        /// <summary>使用现代稀有度：粉色。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }
        /// <summary>装备时置位潜行链状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().ruinMedallion = true;
        }
        /// <summary>按版本使用浩劫精华或混乱精华，与不洁核心×4在秘银砧制作。</summary>
        public override void AddRecipes()
        {
            AddVersionRecipe("CalamityMod", "EssenceofHavoc");
            AddVersionRecipe("CalamityModClassicPreTrailer", "EssenceofChaos");
            AddVersionRecipe("CalRD", "EssenceofChaos");
        }
        /// <summary>从指定灾厄版本查找核心材料并注册等价配方。</summary>
        private void AddVersionRecipe(string modName, string essenceName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod calamity)
                || !calamity.TryFind<ModItem>("UnholyCore", out ModItem unholyCore)
                || !calamity.TryFind<ModItem>(essenceName, out ModItem essence))
                return;
            CreateRecipe()
                .AddIngredient<CoinofDeceit>()
                .AddIngredient(unholyCore.Type, 4)
                .AddIngredient(essence.Type, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
