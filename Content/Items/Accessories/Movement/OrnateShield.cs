using CalamityRebirth.Enums;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
    /// <summary>华丽盾：盾牌冲刺链的入门饰品，提供冰霜冲撞与基础生存属性。</summary>
    internal sealed class OrnateShield : ModItem
    {
        /// <summary>设置判定尺寸、十二金价格、粉色稀有度与八点防御。</summary>
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 8;
            Item.accessory = true;
        }
        /// <summary>置位装备状态并注册最低优先级的冰霜冲撞。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.ornateShield = true;
            modPlayer.ProvideShieldSlamDash(ShieldSlamDash.OrnateShield);
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 的矿锭配方。</summary>
        public override void AddRecipes()
        {
            AddRecipe("CalamityMod", "CryonicBar");
            AddRecipe("CalamityModClassicPreTrailer", "VerstaltiteBar");
            AddRecipe("CalRD", "VerstaltiteBar");
        }
        /// <summary>使用指定分支的寒元系矿锭五枚与水晶碎片十枚在秘银砧制作。</summary>
        private void AddRecipe(string modName, string barName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>(barName, out ModItem bar))
                return;
            CreateRecipe()
                .AddIngredient(bar.Type, 5)
                .AddIngredient(ItemID.CrystalShard, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
