using CalamityRebirth.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 燃烧III：由战士徽章EX与烈火手套EX的近战命中附加。
    /// 每秒造成 100 点生命流失、降低 10 点防御，并使目标受到的伤害增加 5%。
    /// </summary>
    internal class BurningIII : ModBuff
    {
        /// <summary>Terraria 的 lifeRegen 以半血点计算，200 对应每秒 100 HP。</summary>
        public const int BurnLifeRegen = 200;
        /// <summary>命中结算时扣除的固定防御值。</summary>
        public const float DefenseLoss = 10f;
        /// <summary>最终承伤倍率，1.05 表示额外承受 5% 伤害。</summary>
        public const float DamageTakenMultiplier = 1.05f;
        /// <summary>默认持续时间，300 帧为 5 秒。</summary>
        public const int Duration = 300;
        /// <summary>
        /// 注册为参与 PvP、不随存档保存的负面减益，并允许专家模式下自动延长时长。
        /// </summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        /// <summary>NPC 每帧写入全局标记，供掉血、减防和粒子系统读取。</summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CalamityRebirthGlobalNPC>().burningIII = true;
        }
        /// <summary>玩家由 ModPlayer 统一结算掉血与减防，此处仅负责客户端可视化。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            SpawnBurnVisuals(player);
        }
        /// <summary>
        /// 在目标包围盒内以约 75% 概率生成橙色火焰尘，并为周围环境提供暖色照明。
        /// 小概率让尘受重力影响，形成火星下落层次。
        /// </summary>
        public static void SpawnBurnVisuals(Entity target)
        {
            if (Main.rand.Next(4) >= 3)
                return;
            Dust dust = Dust.NewDustDirect(
                new Vector2(target.position.X - 2f, target.position.Y - 2f),
                target.width + 4,
                target.height + 4,
                DustID.OrangeTorch,
                target.velocity.X * 0.4f,
                target.velocity.Y * 0.4f,
                100,
                default,
                3.5f);
            dust.noGravity = true;
            dust.velocity *= 2.8f;
            dust.velocity.Y -= 0.5f;
            if (Main.rand.Next(4) == 0)
            {
                dust.noGravity = false;
                dust.scale *= 0.5f;
            }
            Lighting.AddLight(target.Center, 1f, 0.3f, 0.1f);
        }
    }
}
