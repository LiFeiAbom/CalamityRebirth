using CalamityRebirth.Players;
using Terraria;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>护甲粉碎：点亮伤害、暴击与附加 ArmorCrunch 的命中效果。</summary>
    public class ArmorShattering : PositivePotionBuff
    {
        /// <summary>点亮本帧标记。</summary>
        protected override void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer) => modPlayer.armorShattering = true;
    }
}
