using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>光合作用药水：静止时快速回血，白天效果更强，并提高红心治疗量。</summary>
    public class PhotosynthesisPotion : AgentiaPotionItem
    {
        /// <summary>采用现代浅红稀有度与显式 2 银价格，持续五分钟。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<Photosynthesis>()
            , 18000, ItemRarityID.LightRed, Item.sellPrice(silver: 2));
        /// <summary>注册现代阳光下精华配方、经典植物材料配方和血珠兜底。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("EssenceofSunlight", out ModItem sunlight))
            {
                CreateRecipe().AddIngredient(ItemID.ShinePotion).AddIngredient(ItemID.RegenerationPotion)
                    .AddIngredient(sunlight.Type).AddTile(TileID.AlchemyTable).Register();
            }
            foreach (string modName in new[] { "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("BeetleJuice", out ModItem beetleJuice)
                    || !mod.TryFind<ModItem>("TrapperBulb", out ModItem trapper)
                    || !mod.TryFind<ModItem>("EssenceofCinder", out ModItem cinder))
                    continue;
                ModItem plant = mod.TryFind<ModItem>("ManeaterBulb", out ModItem maneater)
                    ? maneater
                    : mod.TryFind<ModItem>("PlantyMush", out ModItem plantyMush) ? plantyMush : null;
                if (plant == null)
                    continue;
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(beetleJuice.Type, 2)
                    .AddIngredient(plant.Type).AddIngredient(trapper.Type).AddIngredient(cinder.Type)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(40);
        }
    }
}
