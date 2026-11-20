using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 焦热地狱：硫磺火元素宝藏袋中的专家饰品，每十秒向玩家瞄准方向发射四枚硫磺火球。
    /// 现代对应物为 Slagsplitter Pauldron，因此采用其浅红色稀有度与 10 金标准价格。
    /// </summary>
    internal sealed class Gehenna : ModItem
    {
        /// <summary>每轮齐射间隔。</summary>
        public const int BarrageCooldownFrames = 600;
        /// <summary>每轮火球数量。</summary>
        public const int BarrageProjectiles = 4;
        /// <summary>每轮扇形展开角度。</summary>
        public const float BarrageAngleSpread = 120f;
        /// <summary>每枚火球的基础伤害。</summary>
        public const int FireballBaseDamage = 54;
        /// <summary>设置尺寸、专家标记和现代浅红色价格。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.LightRed);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态，周期性齐射由玩家结算。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().gehenna = true;
        }
    }
}
