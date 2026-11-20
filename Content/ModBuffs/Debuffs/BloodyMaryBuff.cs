using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>血腥玛丽：混合输出、机动、防御和生命再生代价，并提高酒醉等级。</summary>
    public class BloodyMaryBuff : AlcoholPotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.bloodyMary = true;
    }
}
