using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Items.Materials;
using CalamityRebirth.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>无尽饥渴药水：十秒内吸附全世界的掉落物。</summary>
    public class CeaselessHungerPotion : AgentiaPotionItem
    {
        /// <summary>采用现代 PureGreen 稀有度和显式 10 银价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<CeaselessHunger>()
            , 600, ModContent.RarityType<PureGreenRarity>(), Item.sellPrice(silver: 10));
        /// <summary>注册四瓶一组的 DarkPlasma + 银河奇点配方，以及血珠 + DarkPlasma 兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("DarkPlasma", out ModItem darkPlasma))
                    continue;
                if (mod.TryFind<ModItem>("GalacticaSingularity", out ModItem externalSingularity))
                {
                    CreateRecipe(4).AddIngredient(ItemID.BottledWater, 4)
                        .AddIngredient(darkPlasma.Type).AddIngredient(externalSingularity.Type)
                        .AddTile(TileID.AlchemyTable).Register();
                }
                else if (ModLoader.TryGetMod("CalamityMod", out _) && modName == "CalamityMod")
                {
                    CreateRecipe(4).AddIngredient(ItemID.BottledWater, 4)
                        .AddIngredient(darkPlasma.Type)
                        .AddIngredient(ModContent.ItemType<GalacticaSingularity>())
                        .AddTile(TileID.AlchemyTable).Register();
                }
                if (mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    CreateRecipe(4).AddIngredient(ItemID.BottledWater, 4)
                        .AddIngredient(bloodOrb.Type, 20).AddIngredient(darkPlasma.Type)
                        .AddTile(TileID.AlchemyTable).Register();
                }
            }
        }
    }
}
