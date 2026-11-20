using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>加勒比朗姆酒：提高再生与移速，降低当前防御并提供漂浮。</summary>
    public class CaribbeanRumBuff : AlcoholPotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.caribbeanRum = true;
    }
}
