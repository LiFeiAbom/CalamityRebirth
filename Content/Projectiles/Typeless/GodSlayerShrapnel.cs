using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>弑神破片：前五帧平飞，随后受重力下落并持续留下紫色尘埃。</summary>
    internal sealed class GodSlayerShrapnel : ModProjectile
    {
        /// <summary>设置 6×12 判定、单次穿透和 90 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
        }
        /// <summary>前五帧保持平飞，之后添加重力并逐步减缓水平速度。</summary>
        public override void AI()
        {
            Projectile.ai[0] = Math.Min(5f, Projectile.ai[0] + 1f);
            if (Projectile.ai[0] >= 5f)
            {
                if (Projectile.velocity.Y >= 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Math.Abs(Projectile.velocity.X) < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = Math.Min(16f, Projectile.velocity.Y + 0.2f);
            }
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.ShadowbeamStaff, 0f, 0f, 100, default, 0.5f);
            Main.dust[dust].position += new Vector2(-2f, 2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity.Y -= 2f;
            if (!Main.rand.NextBool(2))
                return;
            int extra = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.ShadowbeamStaff, 0f, 0f, 100, default, 0.8f);
            Main.dust[extra].position += new Vector2(-2f, 2f);
            Main.dust[extra].noGravity = true;
            Main.dust[extra].velocity *= 0.1f;
        }
        /// <summary>接触地形后继续下落，由寿命或命中耗尽结束。</summary>
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
