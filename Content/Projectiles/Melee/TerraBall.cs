using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 大地弹：真·远古方舟召唤的绿色追踪弹幕，命中或消散时会产生光尘和星形残片。
    /// </summary>
    internal sealed class TerraBall : ModProjectile
    {
        /// <summary>登记六格历史位置用于绘制拖尾。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        /// <summary>大地弹具有 20×20 碰撞箱、单次穿透和 180 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }
        /// <summary>
        /// 持续生成绿色妖精尘，按随机间隔播放闪烁音效，偶尔产生星形残片，
        /// 并追踪 1600 像素内最近敌人。
        /// </summary>
        public override void AI()
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.GreenFairy, 0f, 0f, 100, default, 0.8f);
            Dust dust = Main.dust[dustIndex];
            dust.noGravity = true;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            if (Main.rand.NextBool(48))
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center
                    , Projectile.velocity * 0.2f, 16, 1f);
                Main.gore[gore].velocity *= 0.66f;
                Main.gore[gore].velocity += Projectile.velocity * 0.3f;
            }
            CRUtil.HomeInNPC(Projectile, 1600f, 30f, 20f, !Projectile.tileCollide);
        }
        /// <summary>消亡时扩大特效范围并分两轮生成扩张粉尘与三枚星形残片。</summary>
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center - Vector2.One * 25f;
            Projectile.width = 50;
            Projectile.height = 50;
            for (int i = 0; i < 5; i++)
            {
                int heavyIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.GreenFairy, 0f, 0f, 100, default, 1.2f);
                Dust heavy = Main.dust[heavyIndex];
                heavy.velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    heavy.scale = 0.5f;
                    heavy.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                int lightIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.GreenFairy, 0f, 0f, 100, default, 1.7f);
                Dust light = Main.dust[lightIndex];
                light.noGravity = true;
                light.velocity *= 5f;
                int secondaryIndex = Dust.NewDust(Projectile.position, Projectile.width
                    , Projectile.height, DustID.GreenFairy, 0f, 0f, 100, default, 1f);
                Dust secondary = Main.dust[secondaryIndex];
                secondary.velocity *= 2f;
            }
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position
                    , new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f)
                    , Main.rand.Next(16, 18), 1f);
            }
        }
        /// <summary>绘制六格历史残影后不再执行默认本体绘制。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            CRUtil.DrawAfterimages(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor);
            return false;
        }
        /// <summary>使用浅灰色着色，透明度随弹幕寿命淡出。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }
    }
}
