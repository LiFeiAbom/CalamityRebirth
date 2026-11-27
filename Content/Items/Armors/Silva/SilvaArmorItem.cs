using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Content.Items.Accessories.Immunity;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Silva
{
    /// <summary>
    /// 始源林海防具公共基类，统一尺寸、稀有度、价格和三版本配方。
    /// </summary>
    internal abstract class SilvaArmorItem : ModItem
    {
        /// <summary>设置 18×18 判定尺寸、防御、金币价格和现代 Cosmic Purple 稀有度。</summary>
        protected void SetArmorDefaults(int defense, int goldPrice)
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = defense;
            RarityUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>()
                , Item.buyPrice(0, goldPrice, 0, 0));
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 三版本配方。
        /// 各版本的材料、数量和 LeadCore 要求均独立传入，避免跨版本套用错误配方。
        /// </summary>
        protected void AddSilvaRecipes(int mushroomCount, int modernFeatherCount
            , int modernEssenceCount, int darksunCount, int classicFeatherCount
            , int goldBarCount, int tenebrisCount, int nightmareFuelCount
            , int endothermicEnergyCount, int calRDEssenceCount, bool modernLeadCore
            , bool classic142LeadCore, bool calRDLeadCore)
        {
            AddModernRecipe(mushroomCount, modernFeatherCount, modernEssenceCount, modernLeadCore);
            AddClassic142Recipe(darksunCount, classicFeatherCount, goldBarCount, tenebrisCount
                , nightmareFuelCount, endothermicEnergyCount, classic142LeadCore);
            AddCalRDRecipe(darksunCount, classicFeatherCount, goldBarCount, tenebrisCount
                , calRDEssenceCount, calRDLeadCore);
        }
        /// <summary>注册现代灾厄的植物混融块、耀斑羽与至尊精神配方。</summary>
        private void AddModernRecipe(int mushroomCount, int featherCount, int essenceCount, bool addLeadCore)
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("PlantyMush", out ModItem plantyMush)
                || !calamity.TryFind<ModItem>("EffulgentFeather", out ModItem feather)
                || !calamity.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(plantyMush.Type, mushroomCount);
            recipe.AddIngredient(feather.Type, featherCount);
            recipe.AddIngredient(essence.Type, essenceCount);
            if (addLeadCore)
                recipe.AddIngredient<LeadCore>();
            recipe.AddTile(cosmicAnvil.Type);
            recipe.Register();
        }
        /// <summary>注册经典 1.4.2.101 的暗日碎片、梦魇燃料与吸热能量配方。</summary>
        private void AddClassic142Recipe(int darksunCount, int featherCount, int cosmiliteBarCount
            , int tenebrisCount, int nightmareFuelCount, int endothermicEnergyCount, bool addLeadCore)
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("DarksunFragment", out ModItem darksun)
                || !classic.TryFind<ModItem>("EffulgentFeather", out ModItem feather)
                || !classic.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !classic.TryFind<ModItem>("Tenebris", out ModItem tenebris)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(darksun.Type, darksunCount);
            recipe.AddIngredient(feather.Type, featherCount);
            recipe.AddIngredient(cosmiliteBar.Type, cosmiliteBarCount);
            recipe.AddIngredient(tenebris.Type, tenebrisCount);
            recipe.AddIngredient(nightmareFuel.Type, nightmareFuelCount);
            recipe.AddIngredient(endothermicEnergy.Type, endothermicEnergyCount);
            if (addLeadCore)
                recipe.AddIngredient<LeadCore>();
            recipe.AddTile(forge.Type);
            recipe.Register();
        }
        /// <summary>注册 CalRD 的暗日碎片、至尊精神与嘉登熔炉配方。</summary>
        private void AddCalRDRecipe(int darksunCount, int featherCount, int goldBarCount
            , int tenebrisCount, int essenceCount, bool addLeadCore)
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("DarksunFragment", out ModItem darksun)
                || !calRD.TryFind<ModItem>("EffulgentFeather", out ModItem feather)
                || !calRD.TryFind<ModItem>("Tenebris", out ModItem tenebris)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile forge)
                || !RecipeGroup.recipeGroupIDs.ContainsKey("AnyGoldBar"))
                return;
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(darksun.Type, darksunCount);
            recipe.AddIngredient(feather.Type, featherCount);
            recipe.AddRecipeGroup("AnyGoldBar", goldBarCount);
            recipe.AddIngredient(tenebris.Type, tenebrisCount);
            recipe.AddIngredient(essence.Type, essenceCount);
            if (addLeadCore)
                recipe.AddIngredient<LeadCore>();
            recipe.AddTile(forge.Type);
            recipe.Register();
        }
    }
}
