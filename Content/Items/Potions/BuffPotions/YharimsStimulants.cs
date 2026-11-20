using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>
    /// 魔君牌兴奋剂：饮用后获得亚利姆之力，提升战斗与生存属性。
    /// </summary>
    internal class YharimsStimulants : ModItem
    {
        /// <summary>统一设置现代研究解锁数量。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }
        /// <summary>
        /// 药水类消耗品：使用后获得 30 分钟（108000 帧）亚利姆之力。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 28;                              // 贴图宽
            Item.height = 18;                             // 贴图高
            Item.useTurn = true;                          // 使用时允许转向
            Item.maxStack = 999;                          // 最大堆叠
            Item.rare = ItemRarityID.Orange;              // 橙色稀有度
            Item.useAnimation = 17;                       // 动画时长 17 帧
            Item.useTime = 17;                            // 使用间隔 17 帧
            Item.useStyle = ItemUseStyleID.EatFood;       // 进食动作
            Item.UseSound = SoundID.Item3;                // 饮用音效
            Item.consumable = true;                       // 一次性消耗品
            Item.buffType = ModContent.BuffType<YharimPower>();
            Item.buffTime = 108000;                       // 30 分钟
            Item.value = Item.sellPrice(silver: 40);      // 售价 40 银币
        }
        /// <summary>
        /// 按当前加载的灾厄版本，分别注册六药水配方与血珠配方。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                // 现代灾厄：使用本模组的六药水配方，并追加血珠的等价直饮配方。
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient(ItemID.EndurancePotion);
                recipe.AddIngredient(ItemID.IronskinPotion);
                recipe.AddIngredient(ItemID.SwiftnessPotion);
                recipe.AddIngredient(ItemID.ArcheryPotion);
                recipe.AddIngredient(ItemID.MagicPowerPotion);
                recipe.AddIngredient(ItemID.TitanPotion);
                recipe.AddTile(TileID.AlchemyTable);
                recipe.Register();
                if (calamity.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    recipe = CreateRecipe();
                    recipe.AddIngredient(bloodOrb.Type, 50);
                    recipe.AddIngredient(ItemID.BottledWater);
                    recipe.AddTile(TileID.AlchemyTable);
                    recipe.Register();
                }
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic))
            {
                // 经典 1.4.2.101 的 BloodOrb 属于另一个内部模组，需要在该版本中重新查找。
                Recipe recipeClassic = CreateRecipe();
                recipeClassic.AddIngredient(ItemID.EndurancePotion);
                recipeClassic.AddIngredient(ItemID.IronskinPotion);
                recipeClassic.AddIngredient(ItemID.SwiftnessPotion);
                recipeClassic.AddIngredient(ItemID.ArcheryPotion);
                recipeClassic.AddIngredient(ItemID.MagicPowerPotion);
                recipeClassic.AddIngredient(ItemID.TitanPotion);
                recipeClassic.AddTile(TileID.AlchemyTable);
                recipeClassic.Register();
                if (classic.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    recipeClassic = CreateRecipe();
                    recipeClassic.AddIngredient(bloodOrb.Type, 50);
                    recipeClassic.AddIngredient(ItemID.BottledWater);
                    recipeClassic.AddTile(TileID.AlchemyTable);
                    recipeClassic.Register();
                }
            }
            if (ModLoader.TryGetMod("CalRD", out Mod classic1457))
            {
                // CalRD 1.4.5.7 同样提供 BloodOrb，但 ModItem 类型与另外两版完全隔离。
                Recipe recipeClassic1457 = CreateRecipe();
                recipeClassic1457.AddIngredient(ItemID.EndurancePotion);
                recipeClassic1457.AddIngredient(ItemID.IronskinPotion);
                recipeClassic1457.AddIngredient(ItemID.SwiftnessPotion);
                recipeClassic1457.AddIngredient(ItemID.ArcheryPotion);
                recipeClassic1457.AddIngredient(ItemID.MagicPowerPotion);
                recipeClassic1457.AddIngredient(ItemID.TitanPotion);
                recipeClassic1457.AddTile(TileID.AlchemyTable);
                recipeClassic1457.Register();
                if (classic1457.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    recipeClassic1457 = CreateRecipe();
                    recipeClassic1457.AddIngredient(bloodOrb.Type, 50);
                    recipeClassic1457.AddIngredient(ItemID.BottledWater);
                    recipeClassic1457.AddTile(TileID.AlchemyTable);
                    recipeClassic1457.Register();
                }
            }
        }
    }
}
