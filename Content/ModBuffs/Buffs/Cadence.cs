using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>韵律：提供商店折扣、生命磁铁、仇恨/爱心状态、再生和最大生命加成。</summary>
    public class Cadence : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.cadence = true;
    }
}
