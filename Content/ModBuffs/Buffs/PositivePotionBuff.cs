using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>
    /// Agentia 正面药水增益的公共注册逻辑。
    /// 子类只需要把对应玩家标记置位，具体数值由 CalamityRebirthPlayer 统一结算。
    /// </summary>
    public abstract class PositivePotionBuff : ModBuff
    {
        /// <summary>注册为正面、可 PvP、可存档且不被专家模式延长。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
        /// <summary>每帧调用子类实现以点亮玩家状态。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            ApplyEffect(player, player.GetModPlayer<CalamityRebirthPlayer>());
        }
        /// <summary>点亮当前药水对应的玩家标记。</summary>
        protected abstract void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer);
    }
}
