using CalamityRebirth.Content.Particles;
using CalamityRebirth.NPCs;
using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 元素混合II：元素之握近战命中附加的强化元素持续伤害。
    /// NPC 承受每秒 700 HP 流失、50 点减防和 15% 易伤；玩家承受每秒 50 HP 流失、
    /// 50 点减防和 15% 易伤。
    /// 视觉完全采用现代灾厄 ElementalMix：四色全息方条、原版尺寸与生成频率。
    /// 同时保留 Dust 66/247 的原始速度、染色、透明度和感光。
    /// </summary>
    internal sealed class ElementalMixII : ModBuff
    {
        /// <summary>NPC 每秒流失 700 HP，Terraria 的 lifeRegen 每 2 点对应每秒 1 HP。</summary>
        public const int NpcLifeRegenLoss = 1400;
        /// <summary>NPC 与玩家受击结算时额外扣除的固定防御。</summary>
        public const float DefenseLoss = 50f;
        /// <summary>NPC 与玩家最终承伤倍率，1.15 表示额外承受 15% 伤害。</summary>
        public const float DamageTakenMultiplier = 1.15f;
        /// <summary>玩家每秒流失 50 HP，对应 lifeRegen 损失 100。</summary>
        public const int PlayerLifeRegenLoss = 100;
        /// <summary>元素之握统一使用的 120 帧命中持续时间。</summary>
        public const int DefaultDuration = 120;
        /// <summary>现代 ElementalMix 的四色元素调色板。</summary>
        private static readonly Color[] ElementalColors =
        [
            new Color(0, 191, 255),
            new Color(0, 250, 154),
            new Color(255, 140, 0),
            new Color(238, 130, 238)
        ];
        /// <summary>注册为参与 PvP、不存档且受专家模式延长规则影响的负面减益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        /// <summary>NPC 每帧点亮全局标记，供掉血、减防、易伤和绘制读取。</summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CalamityRebirthGlobalNPC>().elementalMixII = true;
        }
        /// <summary>玩家每帧点亮状态标记；持续伤害、减防与易伤均由 ModPlayer 统一结算。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().elementalMixII = true;
        }
        /// <summary>
        /// 玩家侧按原版每帧生成一枚向上全息方条和一枚 Dust 66/247。
        /// 生成范围、速度、尺寸、寿命、Alpha 与速度补偿均与现代 ElementalMix 一致。
        /// </summary>
        public static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Color effectColor = RandomElementColor();
            Vector2 spawn = player.Center + new Vector2(Main.rand.NextFloat(-10f, 10f)
                , Main.rand.NextFloat(-20f, 20f));
            Vector2 speed = new(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2.5f, -8.3f));
            CalamityRebirthParticleSystem.SpawnParticle(spawn, speed, effectColor
                , Main.rand.NextFloat(1.2f, 1.8f), Main.rand.Next(8, 14));
            int dustType = Main.rand.NextBool() ? 66 : 247;
            Dust dust = Dust.NewDustPerfect(spawn, dustType);
            dust.scale = dustType == 66 ? 1.4f : 1.2f;
            dust.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(4f, 8f))
                - player.velocity / 2f;
            dust.noGravity = true;
            dust.alpha = Main.rand.Next(90, 150);
            dust.color = effectColor;
        }
        /// <summary>
        /// NPC 侧按原版概率生成全息方条与 Dust 66/247，并另有五分之一概率生成高亮晶尘。
        /// 两段粒子均使用原版生成范围、速度、尺寸、寿命、Alpha 与目标速度补偿。
        /// </summary>
        public static void DrawEffects(NPC npc)
        {
            if (Main.rand.NextBool())
            {
                Color effectColor = RandomElementColor();
                Vector2 spawn = npc.Center + new Vector2(
                    Main.rand.NextFloat(-npc.width / 2f, npc.width / 2f),
                    Main.rand.NextFloat(-npc.height / 2f, npc.height / 2f));
                Vector2 speed = new(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2.5f, -8.3f));
                CalamityRebirthParticleSystem.SpawnParticle(spawn, speed, effectColor
                    , Main.rand.NextFloat(1.2f, 3.1f), Main.rand.Next(8, 14));
                int dustType = Main.rand.NextBool() ? 66 : 247;
                Dust dust = Dust.NewDustPerfect(spawn, dustType);
                dust.scale = dustType == 66 ? 1.4f : 1.2f;
                dust.velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(2f, 4f))
                    - npc.velocity / 2f;
                dust.noGravity = true;
                dust.alpha = Main.rand.Next(90, 150);
                dust.color = effectColor;
            }
            if (Main.rand.NextBool(5))
            {
                Color effectColor = RandomElementColor();
                Vector2 spawn = npc.Center + new Vector2(
                    Main.rand.NextFloat(-npc.width / 2f, npc.width / 2f),
                    Main.rand.NextFloat(-npc.height / 2f, npc.height / 2f));
                int dustType = Main.rand.NextBool() ? 66 : 247;
                Dust dust = Dust.NewDustPerfect(spawn, dustType);
                dust.scale = dustType == 66 ? 0.9f : 1.4f;
                dust.velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(2f, 4f));
                dust.noGravity = true;
                dust.alpha = Main.rand.Next(35, 90);
                dust.color = effectColor;
            }
        }
        /// <summary>从四色元素调色板中随机选择主体色。</summary>
        internal static Color RandomElementColor() => ElementalColors[Main.rand.Next(ElementalColors.Length)];
    }
}
