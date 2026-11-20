using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>护甲崩裂：点亮暴击加成与附加 ArmorCrunch 的命中效果。</summary>
    public class ArmorCrumbling : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.armorCrumbling = true;
    }
}
