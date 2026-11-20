using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>生命之露：身处丛林时提高再生、防御与减伤。</summary>
    internal sealed class LivingDew : ModItem
    {
        /// <summary>设置 20×20 判定、十二金价格、黄绿色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().livingDew = true;
        }
        /// <summary>注册现代与两个经典分支的丛林材料配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
                CreateRecipe().AddIngredient(ItemID.BottledHoney, 10).AddIngredient(ItemID.BeeWax, 3)
                    .AddIngredient(ItemID.JungleSpores, 6).AddTile(TileID.Anvils).Register();
            AddClassicRecipe("CalamityModClassicPreTrailer");
            AddCalRDRecipe();
        }
        /// <summary>经典分支使用捕食者球茎、捕兽球茎等丛林材料。</summary>
        private void AddClassicRecipe(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("ManeaterBulb", out ModItem maneater)
                || !mod.TryFind<ModItem>("TrapperBulb", out ModItem trapper)
                || !mod.TryFind<ModItem>("MurkyPaste", out ModItem paste)
                || !mod.TryFind<ModItem>("GypsyPowder", out ModItem powder)
                || !mod.TryFind<ModItem>("BeetleJuice", out ModItem juice))
                return;
            CreateRecipe().AddIngredient(maneater.Type, 2).AddIngredient(trapper.Type, 2)
                .AddIngredient(paste.Type, 5).AddIngredient(powder.Type).AddIngredient(juice.Type, 3)
                .AddTile(TileID.MythrilAnvil).Register();
        }
        /// <summary>CalRD 分支使用藤蔓、混浊粘贴、蜂蜡与牛黄。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod mod)
                || !mod.TryFind<ModItem>("MurkyPaste", out ModItem paste))
                return;
            CreateRecipe().AddIngredient(ItemID.Vine, 2).AddIngredient(paste.Type, 5)
                .AddIngredient(ItemID.BeeWax, 10).AddIngredient(ItemID.Bezoar)
                .AddTile(TileID.Anvils).Register();
        }
    }
}
