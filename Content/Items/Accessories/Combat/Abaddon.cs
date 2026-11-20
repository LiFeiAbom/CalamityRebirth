using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>阿巴顿：增加通用暴击，并让弹幕暴击命中触发硫磺爆炸。</summary>
    [AutoloadEquip(EquipType.Face)]
    internal sealed class Abaddon : ModItem
    {
        /// <summary>设置判定尺寸、二十四金价格、粉色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }
        /// <summary>置位暴击加成、硫磺火免疫与暴击爆炸标记。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().abaddon = true;
        }
    }
}
