using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>弹跳药水：提高跳跃高度、速度和摔落抗性。</summary>
    public class BoundingPotion : AgentiaPotionItem
    {
        /// <summary>采用现代蓝色稀有度与显式 2 银价格，持续三分钟。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<Bounding>()
            , 10800, ItemRarityID.Blue, Item.sellPrice(silver: 2));
        /// <summary>解析现代 PlantyMush 或经典 ManeaterBulb，并注册血珠兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod))
                    continue;
                ModItem plant = null;
                if (mod.TryFind<ModItem>("PlantyMush", out ModItem plantyMush))
                    plant = plantyMush;
                else if (mod.TryFind<ModItem>("ManeaterBulb", out ModItem maneaterBulb))
                    plant = maneaterBulb;
                if (plant != null)
                {
                    CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(ItemID.Frog)
                        .AddIngredient(plant.Type).AddTile(TileID.Bottles).Register();
                }
            }
            AddBloodOrbRecipe(20);
        }
    }
}
