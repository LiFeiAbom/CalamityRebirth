using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 利维坦龙涎香：利维坦宝藏袋中的专家饰品。
    /// 提供水下移动、战斗和反伤能力，移动时留下毒海水，并在浸水受击时毒害附近敌人。
    /// </summary>
    internal sealed class LeviathanAmbergris : ModItem
    {
        /// <summary>干燥环境下提供的伤害减免。</summary>
        public const float DryDamageReduction = 0.05f;
        /// <summary>干燥环境下提供的通用伤害。</summary>
        public const float DryDamageBonus = 0.05f;
        /// <summary>浸水时提供的通用伤害。</summary>
        public const float WetDamageBonus = 0.10f;
        /// <summary>浸水时提供的防御。</summary>
        public const int WetDefenseBonus = 20;
        /// <summary>浸水时提供的移动速度。</summary>
        public const float WetMoveSpeedBonus = 0.75f;
        /// <summary>移动时生成的毒海水基础伤害。</summary>
        public const int SeawaterBaseDamage = 150;
        /// <summary>水下受击光环的伤害。</summary>
        public const int AuraDamage = 15;
        /// <summary>设置尺寸、现代柠檬色稀有度、专家标记和饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            RarityUtil.Apply(Item, ItemRarityID.Lime);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().leviathanAmbergris = true;
        }
        /// <summary>顶点融合之脑已包含利维坦龙涎香效果，禁止重复装备。</summary>
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => incomingItem.type != ModContent.ItemType<TheAmalgam>();
    }
}
