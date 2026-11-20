using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>渎神之怒药水：三分钟内提高暴击和移动速度。</summary>
    public class ProfanedRagePotion : AgentiaPotionItem
    {
        /// <summary>设置三分钟增益与 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<ProfanedRage>()
            , 10800, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>注册暴怒药水 + UnholyEssence + 银河奇点，以及血珠兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("UnholyEssence", out ModItem unholy))
                    continue;
                int singularityType = mod.TryFind<ModItem>("GalacticaSingularity", out ModItem singularity)
                    ? singularity.Type
                    : ModContent.ItemType<GalacticaSingularity>();
                CreateRecipe().AddIngredient(ItemID.RagePotion).AddIngredient(unholy.Type)
                    .AddIngredient(singularityType)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(40);
        }
    }
}
