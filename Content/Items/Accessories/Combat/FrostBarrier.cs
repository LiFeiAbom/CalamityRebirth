using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 寒霜屏障：提供少量防御，并在受到伤害时冻结附近敌怪。
    /// 本体只负责标记装备状态，免疫与受击冻结由玩家结算类统一处理。
    /// </summary>
    internal sealed class FrostBarrier : ModItem
    {
        /// <summary>固定提供 4 点防御。</summary>
        public const int DefenseBonus = 4;
        /// <summary>设置 20×24 判定尺寸、4 防御、9 金价格、橙色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.defense = DefenseBonus;
            Item.value = Item.buyPrice(0, 9, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备标记，供玩家结算类授予寒冷免疫和受击冻结。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().frostBarrier = true;
        }
    }
}
