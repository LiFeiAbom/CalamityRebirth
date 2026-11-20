using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>钙质药水：十分钟内完全免疫摔落伤害。</summary>
    public class CalciumPotion : AgentiaPotionItem
    {
        /// <summary>采用现代蓝色稀有度和显式 2 银价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<Calcium>()
            , 36000, ItemRarityID.Blue, Item.sellPrice(silver: 2));
        /// <summary>注册三版本 AncientBoneDust 配方和血珠兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (ModLoader.TryGetMod(modName, out Mod mod)
                    && mod.TryFind<ModItem>("AncientBoneDust", out ModItem boneDust))
                {
                    CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(boneDust.Type)
                        .AddTile(TileID.Bottles).Register();
                }
            }
            AddBloodOrbRecipe(10);
        }
    }
}
