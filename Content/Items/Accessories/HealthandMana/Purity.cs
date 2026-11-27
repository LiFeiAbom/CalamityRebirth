using CalamityRebirth.Players;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>
    /// 无暇粹魂晶：宝石系的顶件，也是甘露安瓿古链的终点。
    /// 提供最大生命、击退免疫、按缺失生命缩放的生命再生、减益时长减半、
    /// 持续伤害抵消以及按减益数量累计的动态防御。
    /// </summary>
    public class Purity : ModItem
    {
        /// <summary>
        /// 注册七帧纵向灵魂动画，每五帧切换一次；贴图尺寸为 50×350。
        /// </summary>
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 7));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        /// <summary>
        /// 设置 18×44、十二点防御、1 铂金 50 金价格和现代 Cosmic Purple 稀有度。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 44;
            Item.defense = 12;
            RarityUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>()
                , Item.buyPrice(1, 50, 0, 0));
            Item.accessory = true;
        }
        /// <summary>
        /// 应用固定的 100 最大生命、击退免疫与甘露安瓿古链标记。
        /// 可见且未隐藏时附加冷紫色光照；再生和防御继续由玩家类统一结算。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 100;
            player.noKnockback = true;
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.purity = true;
            modPlayer.alwaysHoneyRegen = true;
            modPlayer.honeyDewHalveDebuffs = true;
            modPlayer.livingDewHalveDebuffs = true;
            if (!hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1.32f, 1.32f, 1.82f));
        }
        /// <summary>
        /// 注册三版本配方。三个分支均消耗感染宝石与版本对应的甘露安瓿，
        /// 再加五份金源锭和四份化神魂精；经典 1.4.2.101 使用等价材料换算。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                && calamity.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                && calamity.TryFind<ModItem>("AscendantSpiritEssence", out ModItem spiritEssence)
                && calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                Recipe recipe = CreateRecipe();
                recipe.AddIngredient<AmbrosialAmpoule2>();
                recipe.AddIngredient<InfectedJewel>();
                recipe.AddIngredient(auricBar.Type, 5);
                recipe.AddIngredient(spiritEssence.Type, 4);
                recipe.AddTile(cosmicAnvil.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile classicForge))
            {
                Recipe recipeClassic = CreateRecipe();
                recipeClassic.AddIngredient<AmbrosialAmpoule2>();
                recipeClassic.AddIngredient<InfectedJewel>();
                if (!CRUtil.TryAddClassicAuricBar(recipeClassic, 5)
                    || !CRUtil.TryAddClassicAscendantSpiritEssence(recipeClassic, 4))
                    return;
                recipeClassic.AddTile(classicForge.Type);
                recipeClassic.Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("AuricBar", out ModItem calRDAuricBar)
                && calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem calRDSpiritEssence)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDForge))
            {
                Recipe recipeCalRD = CreateRecipe();
                recipeCalRD.AddIngredient<AmbrosialAmpoule2>();
                recipeCalRD.AddIngredient<InfectedJewel>();
                recipeCalRD.AddIngredient(calRDAuricBar.Type, 5);
                recipeCalRD.AddIngredient(calRDSpiritEssence.Type, 4);
                recipeCalRD.AddTile(calRDForge.Type);
                recipeCalRD.Register();
            }
        }
    }
}
