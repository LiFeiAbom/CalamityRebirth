using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>
    /// 血契：最大生命翻倍，但任何受击都有四分之一概率承受 2.5 倍最终伤害。
    /// 机制为最大生命翻倍，并附带概率承受高倍最终伤害。
    /// </summary>
    public class BloodPact : ModItem
    {
        /// <summary>受击被暴击的概率。</summary>
        public const float CriticalHitChance = 0.25f;
        /// <summary>血契暴击后的最终伤害倍率。</summary>
        public const float CriticalHitDamageMultiplier = 2.5f;
        /// <summary>设置贴图尺寸、60 金售价、黄色稀有度和专家标记。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            RarityUtil.Apply(Item, ItemRarityID.Yellow);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>点亮本帧的血契状态，具体生命倍率与受击风险在 ModPlayer 中结算。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().bloodPact = true;
        }
    }
}
