using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>巨型陆龟壳：降低少量移动速度并提供荆棘反伤。</summary>
    internal sealed class GiantTortoiseShell : ModItem
    {
        /// <summary>设置 20×24 判定、八点防御、九金价格、浅红色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.defense = 8;
            Item.value = Item.buyPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().giantTortoiseShell = true;
        }
    }
}
