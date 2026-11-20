using CalamityRebirth.Content.ModBuffs.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>渊洋秘药：提供三分钟水下无限呼吸，包括深渊环境。</summary>
    public class AbyssalElixir : AgentiaPotionItem
    {
        /// <summary>设置三分钟增益、橙色稀有度与 2 金价格。</summary>
        public override void SetDefaults() => SetAgentiaDefaults(ModContent.BuffType<AbyssalWish>()
            , 10800, ItemRarityID.Orange, Item.buyPrice(0, 2, 0, 0));
        /// <summary>注册现代沉海旗鱼配方、经典珊瑚贝壳配方和血珠兜底。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("SunkenSailfish", out ModItem sunkenSailfish))
            {
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(sunkenSailfish.Type)
                    .AddIngredient(ItemID.Coral).AddTile(TileID.AlchemyTable).Register();
            }
            foreach (string modName in new[] { "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out _))
                    continue;
                CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(ItemID.Coral)
                    .AddIngredient(ItemID.Seashell).AddTile(TileID.AlchemyTable).Register();
            }
            AddBloodOrbRecipe(40);
        }
    }
}
