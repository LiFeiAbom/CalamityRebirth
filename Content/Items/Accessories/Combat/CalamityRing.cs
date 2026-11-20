using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 灾厄之环：灾厄克隆体宝藏袋中的专家饰品。
    /// 常驻提高通用伤害，并在受击无敌期间概率召唤从天而降的站火。
    /// </summary>
    internal sealed class CalamityRing : ModItem
    {
        /// <summary>通用伤害提高比例。</summary>
        public const float DamageBonus = 0.15f;
        /// <summary>无敌期间触发站火的概率分母。</summary>
        public const int StandingFireChanceDenominator = 10;
        /// <summary>站火弹幕的基础伤害。</summary>
        public const int StandingFireBaseDamage = 30;
        /// <summary>设置 24 金价格与柠檬色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().calamityRing = true;
        }
        /// <summary>终结虚空与顶点融合之脑已包含灾厄之环效果，因此禁止重复装备。</summary>
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => incomingItem.type != ModContent.ItemType<CalamityRing>()
            && incomingItem.type != ModContent.ItemType<VoidofExtinction>()
            && incomingItem.type != ModContent.ItemType<TheAmalgam>();
    }
}
