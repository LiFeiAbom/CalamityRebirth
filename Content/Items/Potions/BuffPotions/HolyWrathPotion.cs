using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>神圣之怒药水：三分钟内提高伤害与机动，并让攻击附加圣焰。</summary>
    public class HolyWrathPotion : AgentiaPotionItem
    {
        /// <summary>设置三分钟增益与 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<HolyWrath>()
            , 10800, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>现代使用四色月亮碎片，经典两版使用原生银河奇点，并提供血珠兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("UnholyEssence", out ModItem unholy))
                    continue;
                Recipe recipe = CreateRecipe().AddIngredient(ItemID.WrathPotion).AddIngredient(unholy.Type);
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
