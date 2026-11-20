using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
    /// <summary>
    /// 斯塔提斯的诅咒：由斯塔提斯的祝福与始源暗影炎合成的召唤职业饰品。
    /// 在祝福基础上增加鞭子范围与速度，并让召唤攻击施加时间之殇和暗影焰。
    /// </summary>
    internal sealed class StatisCurse : ModItem
    {
        /// <summary>使用 28×32 判定尺寸、60 金价值与红色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }
        /// <summary>装备时只置位状态；面板、鞭子加成与命中附加由 ModPlayer 统一处理。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().statisCurse = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 的诅咒配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassicRecipe("CalamityModClassicPreTrailer");
            AddClassicRecipe("CalRD");
        }
        /// <summary>现代使用祝福、始源暗影炎与星尘碎片×6，在远古操纵机制作。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out _))
                return;
            CreateRecipe()
                .AddIngredient<StatisBlessing>()
                .AddIngredient<TheFirstShadowflame>()
                .AddIngredient(ItemID.FragmentStardust, 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>两个经典版本使用祝福、始源暗影炎与星尘碎片×10，在远古操纵机制作。</summary>
        private void AddClassicRecipe(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out _))
                return;
            CreateRecipe()
                .AddIngredient<StatisBlessing>()
                .AddIngredient<TheFirstShadowflame>()
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
