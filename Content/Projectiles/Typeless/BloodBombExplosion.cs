using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>血液炸弹的范围爆发，使用大判定框和短寿命。</summary>
    internal sealed class BloodBombExplosion : ModProjectile
    {
        /// <summary>设置 250×250 判定、无限穿透与 60 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }
        /// <summary>首帧播放爆炸音，并按存活帧数逐渐减少向外喷洒的血尘。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) / 255f, 0f, 0f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            float dustCount = 25f;
            if (Projectile.ai[0] > 180f)
                dustCount -= (Projectile.ai[0] - 180f) / 2f;
            if (dustCount <= 0f)
            {
                Projectile.Kill();
                return;
            }
            dustCount *= 0.7f;
            Projectile.ai[0] += 4f;
            for (int i = 0; i < dustCount; i++)
            {
                float vx = Main.rand.Next(-30, 31);
                float vy = Main.rand.Next(-30, 31);
                float speed = Main.rand.Next(9, 27);
                float length = vx * vx + vy * vy;
                if (length > 0f)
                {
                    length = System.MathF.Sqrt(length);
                    vx = vx * speed / length;
                    vy = vy * speed / length;
                }
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.Blood, 0f, 0f, 100, default, 1.8f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = Projectile.Center
                    + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                Main.dust[dust].velocity = new Vector2(vx, vy);
            }
        }
    }
}
