using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Ranged
{
    /// <summary>
    /// 疯狂护符：远程职业前置饰品，提供基础远程属性与蘑菇矿式站定潜行。
    /// 潜行越高，远程伤害、暴击和降低仇恨的幅度越大。
    /// </summary>
    internal sealed class PsychoticAmulet : ModItem
    {
        /// <summary>基础远程伤害加成。</summary>
        public const float BaseRangedDamageBonus = 0.05f;
        /// <summary>基础远程暴击加成。</summary>
        public const float BaseRangedCritBonus = 5f;
        /// <summary>完全潜行时的额外远程伤害。</summary>
        public const float MaximumStealthDamageBonus = 0.2f;
        /// <summary>完全潜行时的额外远程暴击。</summary>
        public const float MaximumStealthCritBonus = 10f;
        /// <summary>完全潜行时降低的仇恨值。</summary>
        public const float MaximumStealthAggroReduction = 750f;
        /// <summary>设置 26×26 判定尺寸、15 金价格、浅紫稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备标记，属性和潜行状态由玩家结算类统一更新。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().psychoticAmulet = true;
        }
    }
}
