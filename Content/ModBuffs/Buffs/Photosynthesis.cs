using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>光合作用：静止时强化再生，并提高红心治疗效果。</summary>
    public class Photosynthesis : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.photosynthesis = true;
    }
}
