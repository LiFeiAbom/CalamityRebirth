using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>
    /// 感染宝石：王冠宝石与无暇粹魂晶之间的中位饰品。
    /// 保留基础再生、带减益时强化再生以及按减益数量增长的动态防御。
    /// </summary>
    public class InfectedJewel : ModItem
    {
        /// <summary>
        /// 设置物品尺寸、六点防御、黄绿稀有度与 48 金价格。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.defense = 6;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.Lime
                , Item.buyPrice(0, 48, 0, 0));
            Item.accessory = true;
        }
        /// <summary>
        /// 点亮本帧的感染宝石状态；再生和动态防御由玩家类统一结算。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().infectedJewel = true;
        }
        /// <summary>
        /// 注册王冠宝石到感染宝石的三版本升级配方。
        /// 三个分支统一使用 10 份阶段材料与 25 份星尘材料，并保留秘银砧。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                && calamity.TryFind<ModItem>("AureusCell", out ModItem aureusCell)
                && calamity.TryFind<ModItem>("StarblightSoot", out ModItem starblightSoot))
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient<CrownJewel>();
                recipe.AddIngredient(aureusCell.Type, 10);
                recipe.AddIngredient(starblightSoot.Type, 25);
                recipe.AddTile(TileID.MythrilAnvil);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("AstralJelly", out ModItem classicJelly)
                && classic.TryFind<ModItem>("Stardust", out ModItem classicStardust))
            {
                Recipe recipeClassic = CreateRecipe();
                recipeClassic.AddIngredient<CrownJewel>();
                recipeClassic.AddIngredient(classicJelly.Type, 10);
                recipeClassic.AddIngredient(classicStardust.Type, 25);
                recipeClassic.AddTile(TileID.MythrilAnvil);
                recipeClassic.Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("AstralJelly", out ModItem calRDJelly)
                && calRD.TryFind<ModItem>("Stardust", out ModItem calRDStardust))
            {
                Recipe recipeCalRD = CreateRecipe();
                recipeCalRD.AddIngredient<CrownJewel>();
                recipeCalRD.AddIngredient(calRDJelly.Type, 10);
                recipeCalRD.AddIngredient(calRDStardust.Type, 25);
                recipeCalRD.AddTile(TileID.MythrilAnvil);
                recipeCalRD.Register();
            }
        }
    }
}
