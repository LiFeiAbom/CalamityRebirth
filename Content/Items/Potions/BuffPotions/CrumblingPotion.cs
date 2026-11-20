using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>崩裂药水：提高暴击率，命中后附加 ArmorCrunch。</summary>
    public class CrumblingPotion : AgentiaPotionItem
    {
        /// <summary>设置五分钟增益与 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<ArmorCrumbling>()
            , 18000, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>注册五瓶一组的终局材料配方和血珠兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("AncientBoneDust", out ModItem boneDust))
                    continue;
                ModItem essence = null;
                if (mod.TryFind<ModItem>("EssenceofSunlight", out ModItem sunlight))
                    essence = sunlight;
                else if (mod.TryFind<ModItem>("EssenceofCinder", out ModItem cinder))
                    essence = cinder;
                if (essence == null)
                    continue;
                CreateRecipe(5).AddIngredient(ItemID.BottledWater, 5).AddIngredient(boneDust.Type)
                    .AddIngredient(ItemID.AncientBattleArmorMaterial).AddIngredient(essence.Type)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(20);
        }
    }
}
