using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>韵律药水：降低商店价格和仇恨，提高生命类属性。</summary>
    public class CadencePotion : AgentiaPotionItem
    {
        /// <summary>设置五分钟增益，现代版缺失时沿用橙色与 2 金。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<Cadence>()
            , 18000, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>注册五种原版药水的配方和三版本血珠兜底。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _)
                || ModLoader.TryGetMod("CalamityModClassicPreTrailer", out _)
                || ModLoader.TryGetMod("CalRD", out _))
            {
                CreateRecipe().AddIngredient(ItemID.LovePotion).AddIngredient(ItemID.HeartreachPotion)
                    .AddIngredient(ItemID.LifeforcePotion).AddIngredient(ItemID.RegenerationPotion)
                    .AddIngredient(ItemID.CalmingPotion).AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(40);
        }
    }
}
