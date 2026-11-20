using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Rogue
{
    /// <summary>纳米技术飞行途中留下的纳米刀刃。</summary>
    internal sealed class NanotechBlade : ModProjectile
    {
        /// <summary>生成后 30 帧才开始追踪并允许命中。</summary>
        private const float HomeDelay = 30f;
        /// <summary>60 帧后开始淡出。</summary>
        private const float FadeDelay = 60f;
        /// <summary>刀刃可穿墙、可穿水并以盗贼伤害结算。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = CRUtil.GetRogueDamageClass();
        }
        /// <summary>等待 30 帧后才允许命中可追踪目标。</summary>
        public override bool? CanHitNPC(NPC target) =>
            Projectile.ai[1] >= HomeDelay && target.CanBeChasedBy(Projectile);
        /// <summary>更新旋转、淡出，并在等待结束后追踪最近敌人。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.075f, 0.4f, 0.15f));
            Projectile.rotation += Projectile.velocity.X * 0.2f;
            Projectile.rotation += Projectile.velocity.X > 0f ? 0.08f : -0.08f;
            Projectile.ai[1]++;
            if (Projectile.ai[1] > FadeDelay)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            if (Projectile.ai[1] >= HomeDelay)
                HomeInOnClosestNPC();
        }
        /// <summary>在 800 像素内以曼哈顿距离寻找最近敌人并转向。</summary>
        private void HomeInOnClosestNPC()
        {
            Vector2 targetCenter = Projectile.Center;
            float closest = 800f;
            bool found = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile))
                    continue;
                float distance = Math.Abs(Projectile.Center.X - npc.Center.X)
                    + Math.Abs(Projectile.Center.Y - npc.Center.Y);
                if (distance >= closest)
                    continue;
                closest = distance;
                targetCenter = npc.Center;
                found = true;
            }
            if (!found)
                return;
            Vector2 toTarget = targetCenter - Projectile.Center;
            if (toTarget.LengthSquared() < 0.001f)
                return;
            toTarget = Vector2.Normalize(toTarget) * 12f;
            Projectile.velocity = (Projectile.velocity * 20f + toTarget) / 21f;
        }
        /// <summary>死亡时抛出两枚绿色纳米尘。</summary>
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 outward = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + outward * 10f, DustID.TerraBlade
                    , outward * Main.rand.NextFloat(4.5f, 9.1f));
                dust.color = Color.Lerp(Main.hslToRgb(Main.rand.NextFloat(0.4f, 0.6f)
                    , 0.9f, 0.5f), Color.White, 0.3f);
                dust.noGravity = true;
                dust.scale = 0.7f;
            }
        }
    }
}
