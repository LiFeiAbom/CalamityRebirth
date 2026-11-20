using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>翱翔药水：四分钟内提高翅膀飞行时间和水平飞行能力。</summary>
    public class SoaringPotion : AgentiaPotionItem
    {
        /// <summary>采用现代浅红稀有度与显式 2 银价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<Soaring>()
            , 14400, ItemRarityID.LightRed, Item.sellPrice(silver: 2));
        /// <summary>注册阳光/余烬精华飞行材料配方和血珠兜底。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod))
                    continue;
                ModItem essence = null;
                if (mod.TryFind<ModItem>("EssenceofSunlight", out ModItem sunlight))
                    essence = sunlight;
                else if (mod.TryFind<ModItem>("EssenceofCinder", out ModItem cinder))
                    essence = cinder;
                if (essence == null)
                    continue;
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(ItemID.Feather)
                    .AddIngredient(ItemID.SoulofFlight).AddIngredient(essence.Type)
                    .AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(30);
        }
    }
}
