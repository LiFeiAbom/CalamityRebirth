using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>渊洋之愿：把呼吸值维持在最大呼吸值之上，实现在深渊内持续呼吸。</summary>
    public class AbyssalWish : PositivePotionBuff
    {
        /// <summary>把当前呼吸值抬高至上限 +91。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer)
        {
            player.breath = player.breathMax + 91;
        }
    }
}
