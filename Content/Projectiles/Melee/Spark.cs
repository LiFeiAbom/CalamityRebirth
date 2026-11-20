using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>火花：受重力下坠并持续洒落光尘的近战弹幕。</summary>
    internal sealed class Spark : ModProjectile
    {
        /// <summary>使用透明占位贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置 6×12 判定、无限穿透、近战伤害与十帧本地命中冷却。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Melee;
        }
        /// <summary>前五帧保持初速，之后施加重力、摩擦并生成光尘。</summary>
        public override void AI()
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
                Projectile.velocity.X *= -0.1f;
            if (Projectile.velocity.X != Projectile.velocity.X)
                Projectile.velocity.X *= -0.5f;
            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 1f)
                Projectile.velocity.Y *= -0.5f;
            Projectile.ai[0] = MathHelper.Min(Projectile.ai[0] + 1f, 5f);
            if (Projectile.ai[0] >= 5f)
            {
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (Math.Abs(Projectile.velocity.X) < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.2f, 16f);
            }
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            SpawnSparkDust();
            if (Projectile.velocity.Y > 0.15f && Projectile.velocity.Y < 0.25f)
                Projectile.velocity.X *= 0.8f;
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
        }
        /// <summary>每帧生成主光尘，并有半概率追加一颗较大的辅助尘。</summary>
        private void SpawnSparkDust()
        {
            Dust first = Dust.NewDustPerfect(Projectile.position, DustID.UnusedWhiteBluePurple
                , Vector2.Zero, 100, default, 1f);
            first.position += new Vector2(-2f, 2f);
            first.scale += Main.rand.Next(50) * 0.01f;
            first.noGravity = true;
            first.velocity.Y -= 2f;
            if (!Main.rand.NextBool(2))
                return;
            Dust second = Dust.NewDustPerfect(Projectile.position, DustID.UnusedWhiteBluePurple
                , Vector2.Zero, 100, default, 1f);
            second.position += new Vector2(-2f, 2f);
            second.scale += 0.3f + Main.rand.Next(50) * 0.01f;
            second.noGravity = true;
            second.velocity *= 0.1f;
        }
        /// <summary>保持无限穿透，不因撞墙销毁。</summary>
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
