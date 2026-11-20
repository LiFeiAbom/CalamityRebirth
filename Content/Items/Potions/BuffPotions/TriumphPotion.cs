using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>胜利药水：两分钟内按敌怪剩余生命比例降低接触伤害。</summary>
    public class TriumphPotion : AgentiaPotionItem
    {
        /// <summary>设置两分钟增益和 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<Triumph>()
            , 7200, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>注册现代 PearlShard、经典胜利碎片配方和血珠兜底。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("PearlShard", out ModItem pearlShard))
            {
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(pearlShard.Type, 3)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            foreach (string modName in new[] { "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("StormlionMandible", out ModItem mandible)
                    || !mod.TryFind<ModItem>("VictoryShard", out ModItem victoryShard))
                    continue;
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(mandible.Type)
                    .AddIngredient(victoryShard.Type, 3).AddTile(TileID.Bottles).Register();
            }
            AddBloodOrbRecipe(30);
        }
    }
}
