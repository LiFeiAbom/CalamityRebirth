using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>钙质：点亮摔落伤害免疫标记。</summary>
    public class Calcium : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.calcium = true;
    }
}
