using CalamityRebirth.NPCs;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>始源林海迟滞：短时间大幅降低敌怪生命再生并显示高额持续伤害。</summary>
    internal sealed class SilvaHysteresis : ModBuff
    {
        /// <summary>NPC 每帧生命再生损失；每两帧约对应每秒 900 HP。</summary>
        public const int NpcLifeRegenLoss = 900;
        /// <summary>NPC 侧持续伤害数字保底值。</summary>
        public const int MinimumDisplayedDamage = 400;
        /// <summary>注册为可参与 PvP、不存档且受专家延长时间影响的减益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        /// <summary>置位 NPC 全局标记，由持续伤害结算入口读取。</summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CalamityRebirthGlobalNPC>().silvaHysteresis = true;
        }
        /// <summary>置位玩家标记，供玩家持续伤害结算入口读取。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().silvaHysteresis = true;
        }
    }
}
