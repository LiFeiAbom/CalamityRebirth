using CalamityRebirth.Content.Items.Accessories.Movement;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>阴阳吸星石：综合移动、生命、魔力、荆棘、减伤、浸水与受击回复。</summary>
    internal sealed class TheAbsorber : ModItem
    {
        /// <summary>设置 20×24 判定、六点防御、六十金价格、红色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.defense = 6;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().theAbsorber = true;
        }
        /// <summary>注册三个版本的双路线配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipes();
            AddClassicRecipes("CalamityModClassicPreTrailer");
            AddClassicRecipes("CalRD");
        }
        /// <summary>现代分支使用深海细胞、流明石与菌菇。</summary>
        private void AddModernRecipes()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod modern)
                || !modern.TryFind<ModItem>("DepthCells", out ModItem cells)
                || !modern.TryFind<ModItem>("Lumenyl", out ModItem lumenyl)
                || !modern.TryFind<ModItem>("PlantyMush", out ModItem mush))
                return;
            CreateAbsorberRecipe<CrawCarapace>(cells.Type, lumenyl.Type, mush.Type);
            CreateAbsorberRecipe<GiantShell>(cells.Type, lumenyl.Type, mush.Type);
        }
        /// <summary>经典分支使用深海细胞、流明矿与暗影菌。</summary>
        private void AddClassicRecipes(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("DepthCells", out ModItem cells)
                || !mod.TryFind<ModItem>("Lumenite", out ModItem lumenite)
                || !mod.TryFind<ModItem>("Tenebris", out ModItem tenebris))
                return;
            CreateAbsorberRecipe<CrawCarapace>(cells.Type, lumenite.Type, tenebris.Type);
            CreateAbsorberRecipe<GiantShell>(cells.Type, lumenite.Type, tenebris.Type);
        }
        /// <summary>生成甲壳或巨壳二选一的九材料配方。</summary>
        private void CreateAbsorberRecipe<TCarapace>(int cellsType, int lumenType, int mushroomType)
            where TCarapace : ModItem
        {
            CreateRecipe()
                .AddIngredient<GrandGelatin>()
                .AddIngredient<SeaShell>()
                .AddIngredient<TCarapace>()
                .AddIngredient<FungalCarapace>()
                .AddIngredient<GiantTortoiseShell>()
                .AddIngredient<AmidiasSpark>()
                .AddIngredient(cellsType, 15)
                .AddIngredient(lumenType, 15)
                .AddIngredient(mushroomType, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
