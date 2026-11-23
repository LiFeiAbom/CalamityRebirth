using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>血炎射击套装主动释放的追踪灵魂，远离主人时折返并搜索附近敌人。</summary>
    internal sealed class BloodflareSoul : ModProjectile
    {
        /// <summary>设置四帧动画。</summary>
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        /// <summary>设置 30×30 判定、远程伤害、单次命中与 15 秒寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 1.2f;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
        }
        /// <summary>更新动画、尘尾和索敌；主人死亡后快速消散。</summary>
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame = (Projectile.frame + 1) % 4;
                Projectile.frameCounter = 0;
            }
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.RedTorch, 0f, 0f, 0, default, 1.3f);
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 30);
                return;
            }
            float turnRate = 30f * Projectile.ai[1];
            float homeSpeed = 6f * Projectile.ai[1];
            if (Projectile.Distance(owner.Center) > 600f)
            {
                Vector2 direction = Projectile.DirectionTo(owner.Center).SafeNormalize(Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * (turnRate - 1f) + direction * homeSpeed) / turnRate;
                return;
            }
            Vector2 targetCenter = Projectile.Center;
            float closest = 400f;
            bool found = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile)
                    || !Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1))
                    continue;
                float distance = Math.Abs(Projectile.Center.X - npc.Center.X)
                    + Math.Abs(Projectile.Center.Y - npc.Center.Y);
                if (distance >= closest)
                    continue;
                closest = distance;
                targetCenter = npc.Center;
                found = true;
            }
            if (found)
            {
                Vector2 desired = (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX) * 11f;
                Projectile.velocity = (Projectile.velocity * 20f + desired) / 21f;
            }
        }
        /// <summary>末段渐隐。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte brightness = (byte)(Projectile.timeLeft * 3);
                return new Color(brightness, brightness, brightness
                    , (byte)(100f * brightness / 255f));
            }
            return new Color(255, 255, 255, 100);
        }
        /// <summary>消失时生成血焰尘环并结算一次范围伤害。</summary>
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.position);
            Vector2 center = Projectile.Center;
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.Center = center;
            const int dustCount = 36;
            Vector2 ringBase = Projectile.velocity.SafeNormalize(Vector2.UnitY)
                * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 position = ringBase.RotatedBy(
                    (i - (dustCount / 2 - 1)) * MathHelper.TwoPi / dustCount) + Projectile.Center;
                Vector2 offset = position - Projectile.Center;
                int dust = Dust.NewDust(position + offset, 0, 0, DustID.RedTorch
                    , offset.X * 1.5f, offset.Y * 1.5f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].velocity = offset;
            }
            Projectile.Damage();
        }
    }
}
