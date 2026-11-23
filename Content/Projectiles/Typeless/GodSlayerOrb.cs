using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>弑神光球：自动追踪 600 像素内目标并施加神谕狱火。</summary>
    internal sealed class GodSlayerOrb : ModProjectile
    {
        /// <summary>设置 4×4 判定、200 帧寿命和额外一次更新。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.extraUpdates = 1;
        }
        /// <summary>生成紫色拖尾并选取视线内最近敌人进行 12 像素/帧追踪。</summary>
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity = Vector2.Zero;
            float nearest = 600f;
            Vector2 target = Projectile.Center;
            bool found = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile)
                    || !Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1))
                    continue;
                float distance = Math.Abs(Projectile.Center.X - npc.Center.X)
                    + Math.Abs(Projectile.Center.Y - npc.Center.Y);
                if (distance >= nearest)
                    continue;
                nearest = distance;
                target = npc.Center;
                found = true;
            }
            if (!found)
                return;
            Vector2 desired = (target - Projectile.Center).SafeNormalize(Vector2.UnitY) * 12f;
            Projectile.velocity = (Projectile.velocity * 20f + desired) / 21f;
        }
        /// <summary>命中 NPC 时施加 200 帧神谕狱火。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyInferno(target);
        }
        /// <summary>PvP 命中时施加同样的神谕狱火。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ApplyInferno(target);
        }
        /// <summary>按已加载灾厄版本弱依赖查找神谕狱火。</summary>
        private static void ApplyInferno(NPC target)
        {
            if (CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 200)
                || CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                    , "GodSlayerInferno", 200)
                || CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 200))
                return;
        }
        /// <summary>按已加载灾厄版本弱依赖查找神谕狱火并施加给玩家。</summary>
        private static void ApplyInferno(Player target)
        {
            if (CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 200)
                || CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                    , "GodSlayerInferno", 200)
                || CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 200))
                return;
        }
    }
}
