using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>莫斯科骡子：提高伤害、暴击和击退，并降低生命再生。</summary>
    public class MoscowMuleBuff : AlcoholPotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.moscowMule = true;
    }
}
