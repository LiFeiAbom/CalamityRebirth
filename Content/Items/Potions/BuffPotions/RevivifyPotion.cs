using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>新生药水：五分钟内受到伤害会按伤害的一小部分治疗。</summary>
    public class RevivifyPotion : AgentiaPotionItem
    {
        /// <summary>设置五分钟增益与 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<Revivify>()
            , 18000, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>注册现代圣水配方、经典终局材料配方和血珠兜底。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("StarblightSoot", out ModItem soot)
                && modern.TryFind<ModItem>("EssenceofSunlight", out ModItem sunlight))
            {
                CreateRecipe().AddIngredient(ItemID.HolyWater).AddIngredient(soot.Type, 4)
                    .AddIngredient(ItemID.CrystalShard).AddIngredient(sunlight.Type)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            foreach (string modName in new[] { "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("Stardust", out ModItem stardust)
                    || !mod.TryFind<ModItem>("EssenceofCinder", out ModItem cinder))
                    continue;
                CreateRecipe().AddIngredient(ItemID.HolyWater, 5).AddIngredient(stardust.Type, 20)
                    .AddIngredient(ItemID.CrystalShard, 5).AddIngredient(cinder.Type, 3)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(50);
        }
    }
}
