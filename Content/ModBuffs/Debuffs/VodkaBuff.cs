using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>伏特加：提高伤害与暴击，并固定降低防御和少量再生。</summary>
    public class VodkaBuff : AlcoholPotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.vodka = true;
    }
}
