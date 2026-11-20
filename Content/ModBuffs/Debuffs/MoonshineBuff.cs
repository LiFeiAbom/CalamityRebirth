using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>摩闪酒：提高防御与减伤，同时降低少量生命再生。</summary>
    public class MoonshineBuff : AlcoholPotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.moonshine = true;
    }
}
