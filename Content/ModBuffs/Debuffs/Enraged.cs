using CalamityRebirth.NPCs;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>狂怒：同时提高玩家输出与狂怒敌怪的危险性。</summary>
    internal sealed class Enraged : ModBuff
    {
        /// <summary>注册为可 PvP 传播、可存档且不可由护士移除的状态。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        /// <summary>标记玩家处于狂怒输出状态。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().enraged = true;
        }
        /// <summary>标记 NPC 处于狂怒状态，供染色和伤害修正读取。</summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CalamityRebirthGlobalNPC>().enraged = true;
        }
    }
}
