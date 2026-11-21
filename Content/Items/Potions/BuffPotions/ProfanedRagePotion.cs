using CalamityRebirth.Content.ModBuffs.Buffs;
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
        /// <summary>现代使用四色月亮碎片，经典两版使用原生银河奇点，并提供血珠兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("UnholyEssence", out ModItem unholy))
                    continue;
                Recipe recipe = CreateRecipe().AddIngredient(ItemID.RagePotion).AddIngredient(unholy.Type);
                if (modName == "CalamityMod")
                    recipe.AddIngredient(ItemID.FragmentSolar).AddIngredient(ItemID.FragmentVortex)
                        .AddIngredient(ItemID.FragmentNebula).AddIngredient(ItemID.FragmentStardust);
                else if (mod.TryFind<ModItem>("GalacticaSingularity", out ModItem singularity))
                    recipe.AddIngredient(singularity.Type);
                else
                    continue;
                recipe.AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(40);
        }
    }
}
