using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>甲壳爆发：受击后短时间获得大幅移动速度加成。</summary>
    internal sealed class ShellBoost : ModBuff
    {
        /// <summary>注册为不存档、可 PvP 传播且不受专家时长延长的正面增益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
        /// <summary>每帧置位甲壳爆发状态。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().shellBoost = true;
        }
    }
}
