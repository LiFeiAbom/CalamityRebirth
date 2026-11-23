using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿防具公共基类，统一尺寸、稀有度和三版本配方入口。
    /// 子类只负责自身防御、价格与职业属性，避免七件重复编写配方分支。
    /// </summary>
    internal abstract class TarragonArmorItem : ModItem
    {
        /// <summary>设置 18×18 判定尺寸、指定防御与购买价，并统一使用青绿色稀有度。</summary>
        protected void SetArmorDefaults(int defense, int goldPrice)
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = defense;
            RarityUtil.Apply(Item, ModContent.RarityType<TurquoiseRarity>()
                , Item.buyPrice(0, goldPrice, 0, 0));
        }
        /// <summary>
        /// 应用所有龙蒿职业头共有的岩浆、液体移动和减益免疫。
        /// </summary>
        protected static void ApplyHeadProtection(Player player)
        {
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Cursed] = true;
            player.buffImmune[BuffID.Chilled] = true;
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 三条配方。
        /// 现代使用永恒花锭和秘银砧；两款经典版使用 UeliaceBar 和远古操纵机。
        /// </summary>
        protected void AddTarragonRecipes(int modernBarCount, int classicBarCount, int geodeCount)
        {
            AddRecipeVersion("CalamityMod", "UelibloomBar"
                , modernBarCount, geodeCount, TileID.MythrilAnvil);
            AddRecipeVersion("CalamityModClassicPreTrailer", "UeliaceBar"
                , classicBarCount, geodeCount, TileID.LunarCraftingStation);
            AddRecipeVersion("CalRD", "UeliaceBar"
                , classicBarCount, geodeCount, TileID.LunarCraftingStation);
        }
        /// <summary>从指定灾厄分支弱依赖查找锭与神圣晶石并注册一条配方。</summary>
        private void AddRecipeVersion(string modName, string barName, int barCount
            , int geodeCount, int tileType)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>(barName, out ModItem bar)
                || !mod.TryFind<ModItem>("DivineGeode", out ModItem geode))
                return;
            CreateRecipe()
                .AddIngredient(bar.Type, barCount)
                .AddIngredient(geode.Type, geodeCount)
                .AddTile(tileType)
                .Register();
        }
    }
}
