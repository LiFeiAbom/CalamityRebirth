using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 灾劫之尖啸：为佩戴者和同队玩家提供伤害、防御、生命、再生与减伤增益。
    /// </summary>
    internal sealed class Affliction : ModItem
    {
        /// <summary>通用伤害加成。</summary>
        public const float DamageBonus = 0.15f;
        /// <summary>固定防御加成。</summary>
        public const int DefenseBonus = 45;
        /// <summary>减伤加成。</summary>
        public const float DamageReduction = 0.08f;
        /// <summary>最大生命比例加成。</summary>
        public const float MaxLifeBonus = 0.20f;
        /// <summary>每帧生命再生值，每两点约等于每秒一点生命。</summary>
        public const int LifeRegenBonus = 8;
        /// <summary>团队增益刷新间隔。</summary>
        public const int TeamAuraInterval = 10;
        /// <summary>团队增益持续时间。</summary>
        public const int TeamBuffDuration = 20;
        /// <summary>设置判定尺寸、六十金价格、专家标记和现代纯绿稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.expert = true;
            RarityUtil.Apply(
                Item, ModContent.RarityType<PureGreenRarity>(), Item.buyPrice(0, 60, 0, 0));
        }
        /// <summary>置位佩戴状态，具体数值与团队光环由玩家结算类处理。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().affliction = true;
        }
    }
}
