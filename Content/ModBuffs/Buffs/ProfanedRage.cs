using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>渎神之怒：提供通用暴击和移动速度加成。</summary>
    public class ProfanedRage : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.profanedRage = true;
    }
}
