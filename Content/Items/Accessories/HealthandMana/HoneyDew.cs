using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>蜜露：提供蜂蜜式再生、毒素免疫、丛林防御与蜂类减伤。</summary>
    internal sealed class HoneyDew : ModItem
    {
        /// <summary>设置 20×20 判定、十五金价格、橙色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        /// <summary>置位蜜露与蜂类减伤状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.honeyDew = true;
            modPlayer.beeResist = true;
        }
        /// <summary>任一分支加载时注册生命之露与蜂蜜材料配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _)
                || ModLoader.TryGetMod("CalamityModClassicPreTrailer", out _))
                CreateStandardRecipe();
            AddCalRDRecipe();
        }
        /// <summary>现代与经典分支使用蜂蜡和牛黄。</summary>
        private void CreateStandardRecipe()
        {
            CreateRecipe().AddIngredient<LivingDew>().AddIngredient(ItemID.BottledHoney, 10)
                .AddIngredient(ItemID.BeeWax, 10).AddIngredient(ItemID.Bezoar)
                .AddTile(TileID.MythrilAnvil).Register();
        }
        /// <summary>CalRD 分支使用捕兽球茎、蝴蝶尘与甲虫汁。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod mod)
                || !mod.TryFind<ModItem>("TrapperBulb", out ModItem bulb)
                || !mod.TryFind<ModItem>("BeetleJuice", out ModItem juice))
                return;
            CreateRecipe().AddIngredient<LivingDew>().AddIngredient(ItemID.BottledHoney, 10)
                .AddIngredient(bulb.Type, 2).AddIngredient(ItemID.ButterflyDust)
                .AddIngredient(juice.Type, 3).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
