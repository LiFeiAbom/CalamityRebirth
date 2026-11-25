using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
    /// <summary>
    /// 斯塔提斯的祝福：召唤职业饰品。
    /// 提供召唤伤害、召唤击退与仆从栏，并让召唤攻击施加时间之殇。
    /// </summary>
    internal sealed class StatisBlessing : ModItem
    {
        /// <summary>使用 28×32 判定尺寸、45 金价值与黄色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }
        /// <summary>装备时只置位状态；召唤面板由 ModPlayer 按系列最高档统一结算。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().statisBlessing = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 的祝福配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代使用日光精华×5、圣水×30 与三件原版召唤饰品，在秘银砧制作。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("EssenceofSunlight", out ModItem essenceOfSunlight))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.PapyrusScarab)
                .AddIngredient(ItemID.PygmyNecklace)
                .AddIngredient(ItemID.SummonerEmblem)
                .AddIngredient(essenceOfSunlight.Type, 5)
                .AddIngredient(ItemID.HolyWater, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>经典 1.4.2.101 使用原生日耀核心×5、圣水×30，在秘银砧制作。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CoreofCinder", out ModItem coreOfCinder))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.PapyrusScarab)
                .AddIngredient(ItemID.PygmyNecklace)
                .AddIngredient(ItemID.SummonerEmblem)
                .AddIngredient(coreOfCinder.Type, 5)
                .AddIngredient(ItemID.HolyWater, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>CalRD 1.4.5.7 使用原生日耀核心×5、圣水×30，在秘银砧制作。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("CoreofCinder", out ModItem coreOfCinder))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.PapyrusScarab)
                .AddIngredient(ItemID.PygmyNecklace)
                .AddIngredient(ItemID.SummonerEmblem)
                .AddIngredient(coreOfCinder.Type, 5)
                .AddIngredient(ItemID.HolyWater, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
