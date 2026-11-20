using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>无尽饥渴：点亮大范围掉落物吸附标记。</summary>
    public class CeaselessHunger : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.ceaselessHunger = true;
    }
}
