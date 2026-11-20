using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>
    /// 侵蚀：由灾劫之尖啸传播给同队玩家的正面增益。
    /// </summary>
    internal sealed class Afflicted : ModBuff
    {
        /// <summary>注册为不存档、可 PvP 传播且不受专家时长延长的正面增益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
        /// <summary>每帧置位侵蚀状态，属性由玩家统一结算。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().afflicted = true;
        }
    }
}
