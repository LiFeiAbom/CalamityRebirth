using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>阿米迪亚斯之火花：受击时向四周释放电火花。</summary>
    internal sealed class AmidiasSpark : ModItem
    {
        /// <summary>设置 26×26 判定、三金价格、蓝色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().amidiasSpark = true;
        }
    }
}
