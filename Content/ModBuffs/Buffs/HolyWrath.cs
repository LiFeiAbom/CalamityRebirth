using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>神圣之怒：提供伤害、移速并在命中时附加灾厄圣焰。</summary>
    public class HolyWrath : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.holyWrath = true;
    }
}
