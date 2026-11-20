using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>粉碎药水：提高伤害和暴击，并让命中附加 ArmorCrunch。</summary>
    public class ShatteringPotion : AgentiaPotionItem
    {
        /// <summary>设置五分钟增益、30 堆叠和 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<ArmorShattering>()
            , 18000, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0), 30);
        /// <summary>注册由崩裂药水升级的配方和三版本血珠兜底。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                CreateRecipe(2).AddIngredient<CrumblingPotion>(2).AddIngredient(ItemID.BottledWater)
                    .AddIngredient(ItemID.BeetleHusk).AddTile(TileID.AlchemyTable).Register();
            }
            foreach (string modName in new[] { "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out _))
                    continue;
                CreateRecipe().AddIngredient<CrumblingPotion>().AddIngredient(ItemID.BottledWater)
                    .AddIngredient(ItemID.BeetleHusk).AddTile(TileID.AlchemyTable).Register();
            }
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                    continue;
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(bloodOrb.Type, 30)
                    .AddIngredient(ItemID.BeetleHusk).AddTile(TileID.AlchemyTable).Register();
            }
        }
    }
}
