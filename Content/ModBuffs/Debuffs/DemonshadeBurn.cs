using CalamityRebirth.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>魔影灼燃：以紫色巫咒火焰对敌怪与 PvP 玩家施加高额持续伤害。</summary>
    internal sealed class DemonshadeBurn : ModBuff
    {
        /// <summary>单次命中统一施加三百帧，即五秒。</summary>
        public const int Duration = 300;
        /// <summary>每秒生命损失。</summary>
        public const int DamagePerSecond = 2500;
        /// <summary>注册为可传播、不存档且受专家延长规则影响的临时减益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        /// <summary>把当前 NPC 标记为正在承受魔影灼燃。</summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CalamityRebirthGlobalNPC>().demonshadeBurn = true;
        }
    }
}
