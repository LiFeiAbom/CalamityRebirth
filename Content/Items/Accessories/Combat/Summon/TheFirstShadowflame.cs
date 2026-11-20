using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
    /// <summary>
    /// 始源暗影炎：召唤职业饰品。
    /// 直接增加 1 个仆从栏，并让实际仆从与哨兵命中施加暗影焰。
    /// </summary>
    internal sealed class TheFirstShadowflame : ModItem
    {
        /// <summary>仆从命中施加的原版暗影焰持续时间。</summary>
        public const int ShadowflameDuration = 300;
        /// <summary>使用 26×26 判定尺寸、15 金价值与粉色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }
        /// <summary>
        /// 直接增加一个真正的仆从栏，不经过现代版 shadowMinions 的组合式间接结算。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions++;
            player.GetModPlayer<CalamityRebirthPlayer>().theFirstShadowflame = true;
        }
    }
}
