using CalamityRebirth.NPCs;
using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 触电II：晶光头骨闪电附加的强化触电减益。
    /// NPC 承受更高持续伤害、减防与易伤；玩家承受较低的持续伤害和减防。
    /// </summary>
    internal sealed class ElectrifiedII : ModBuff
    {
        /// <summary>默认持续时间，300 帧为 5 秒。</summary>
        public const int Duration = 300;
        /// <summary>Terraria 的 lifeRegen 以半血点计算，250 对应每秒 125 HP。</summary>
        public const int NpcLifeRegenLoss = 250;
        /// <summary>NPC 受到的固定防御降低值。</summary>
        public const float NpcDefenseLoss = 5f;
        /// <summary>NPC 承伤倍率，1.05 表示额外承受 5% 伤害。</summary>
        public const float NpcDamageTakenMultiplier = 1.05f;
        /// <summary>玩家每秒承受 30 HP，对应 lifeRegen 损失 60。</summary>
        public const int PlayerLifeRegenLoss = 60;
        /// <summary>玩家受到的固定防御降低值。</summary>
        public const int PlayerDefenseLoss = 15;
        /// <summary>复用原版触电图标，避免额外维护一份重复的美术资源。</summary>
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Electrified;
        /// <summary>注册为参与 PvP、不存档的负面减益，并允许专家模式延长持续时间。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        /// <summary>NPC 每帧写入全局标记，供掉血、减防与易伤结算读取。</summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CalamityRebirthGlobalNPC>().electrifiedII = true;
        }
        /// <summary>玩家侧由 ModPlayer 统一结算掉血和减防，此处负责点亮状态标记。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().electrifiedII = true;
        }
        /// <summary>
        /// 生成原版触电风格的电弧尘。
        /// 尘从目标包围盒随机位置跳出，少量保留重力，形成短促放电的层次。
        /// </summary>
        public static void SpawnElectricVisuals(Entity target)
        {
            if (Main.rand.Next(3) != 0)
                return;
            Vector2 spawn = target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height));
            Vector2 velocity = target.velocity * 0.15f + Main.rand.NextVector2Circular(3.5f, 3.5f);
            Dust dust = Dust.NewDustPerfect(spawn, DustID.Electric, velocity, 100, Color.Transparent
                , Main.rand.NextFloat(0.8f, 1.25f));
            dust.noGravity = Main.rand.NextBool(4);
            dust.noLight = false;
            Lighting.AddLight(target.Center, 0.12f, 0.35f, 0.65f);
        }
    }
}
