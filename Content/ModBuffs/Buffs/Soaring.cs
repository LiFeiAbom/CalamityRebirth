using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>翱翔：提高翅膀最大飞行时间。</summary>
    public class Soaring : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.soaring = true;
    }
}
