using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
    /// <summary>巨型贝壳：降低移动速度，受击后获得短暂高速移动。</summary>
    internal sealed class GiantShell : ModItem
    {
        /// <summary>设置 20×24 判定、六点防御、三金价格、蓝色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.defense = 6;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().giantShell = true;
        }
    }
}
