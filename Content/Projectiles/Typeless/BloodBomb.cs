using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>血炎射手的血液炸弹：飞行时撒血尘，消失时生成范围爆炸。</summary>
    internal sealed class BloodBomb : ModProjectile
    {
        /// <summary>设置 20×20 判定、单次命中与 10 秒寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
        }
        /// <summary>提供红光、首帧音效和每帧三颗血糖尘尾。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.4f / 255f, 0f, 0f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.Blood, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].velocity += Projectile.velocity * 0.1f;
            }
        }
        /// <summary>消失时只在主人端生成爆炸弹幕。</summary>
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                , Vector2.Zero, ModContent.ProjectileType<BloodBombExplosion>()
                , Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
