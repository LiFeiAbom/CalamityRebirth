using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 灾厄增强版暗影焰：只用于 PvP 玩家目标，敌怪仍使用原版 BuffID.ShadowFlame。
    /// 掉血在 CalamityRebirthPlayer.UpdateBadLifeRegen 中按 lifeRegen -= 30 结算。
    /// </summary>
    internal sealed class ShadowflameII : ModBuff
    {
        /// <summary>注册为可参与 PvP、不存档的负面减益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        /// <summary>标记玩家当前存在暗影焰，由统一掉血钩子结算。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().shadowflameII = true;
        }
        /// <summary>
        /// 对 NPC 施加原版暗影焰。
        /// 敌怪继续使用 Terraria 原生减益结算，也保留现代灾厄通过原版减益数据集提供的兼容强化。
        /// </summary>
        internal static void ApplyToNPC(NPC target, int duration)
        {
            target.AddBuff(BuffID.ShadowFlame, duration);
        }
        /// <summary>
        /// 对玩家施加本工程的灾厄式暗影焰。
        /// 玩家侧由本模组统一接管生命再生损失，避免 PvP 路径继续依赖原版弱化结算。
        /// </summary>
        internal static void ApplyToPlayer(Player target, int duration)
        {
            target.AddBuff(ModContent.BuffType<ShadowflameII>(), duration);
        }
        /// <summary>
        /// 在玩家正常绘制通道生成稀疏的暗影焰尘。
        /// 一部分尘关闭无重力并缩小，模拟从人物身上落下的余烬。
        /// </summary>
        internal static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            if (Main.rand.Next(5) >= 4)
                return;
            int dustIndex = Dust.NewDust(drawInfo.Position - new Vector2(2f), player.width + 4, player.height + 4
                , DustID.Shadowflame, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default, 1.1f);
            Dust dust = Main.dust[dustIndex];
            dust.noGravity = true;
            dust.velocity *= 0.75f;
            dust.velocity.Y -= 3f;
            if (Main.rand.NextBool(4))
            {
                dust.noGravity = false;
                dust.scale *= 0.3f;
            }
        }
    }
}
