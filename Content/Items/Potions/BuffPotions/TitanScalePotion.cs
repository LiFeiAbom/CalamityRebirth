using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>泰坦之鳞药水：提高击退抗性、防御与伤害减免。</summary>
    public class TitanScalePotion : AgentiaPotionItem
    {
        /// <summary>设置五分钟增益和 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<TitanScale>()
            , 18000, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>注册泰坦药水 + 甲虫壳和三版本血珠兜底。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _)
                || ModLoader.TryGetMod("CalamityModClassicPreTrailer", out _)
                || ModLoader.TryGetMod("CalRD", out _))
            {
                CreateRecipe().AddIngredient(ItemID.TitanPotion).AddIngredient(ItemID.BeetleHusk)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                    continue;
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(bloodOrb.Type, 10)
                    .AddIngredient(ItemID.BeetleHusk).AddTile(TileID.AlchemyTable).Register();
            }
        }
    }
}
