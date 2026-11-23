using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>弑神飞镖：近战命中触发的挂载发射器或高速追踪子飞镖。</summary>
    internal sealed class GodSlayerDart : ModProjectile
    {
        /// <summary>首帧初始化标记。</summary>
        private bool initialized;
        /// <summary>挂载形态剩余发射间隔。</summary>
        private int fireDelay = 2;
        /// <summary>挂载形态已发射子飞镖数量。</summary>
        private int firedCount;
        /// <summary>每枚挂载飞镖随机旋转的整圈扇区。</summary>
        private int randomStart = Main.rand.Next(9);
        /// <summary>登记十段残影。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        /// <summary>设置 22×46 判定、单次穿透和无视地形。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 140;
        }
        /// <summary>ai[0] 为零时挂载发射，否则按高速追踪形态运行。</summary>
        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                MountedAI();
                if (!initialized)
                {
                    Projectile.timeLeft = 50;
                    initialized = true;
                }
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            FlyingAI();
            if (!initialized)
            {
                Projectile.timeLeft = 360;
                initialized = true;
            }
        }
        /// <summary>挂载本体没有命中判定。</summary>
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] != 0f;
        }
        /// <summary>命中 NPC 时施加 500 帧神谕狱火。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyInferno(target);
        }
        /// <summary>PvP 命中时施加 500 帧神谕狱火。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ApplyInferno(target);
        }
        /// <summary>飞行形态消散时播放音效并喷出紫黑与蝶形尘。</summary>
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 0f)
                return;
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.Butterfly, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
            }
        }
        /// <summary>挂载本体不绘制；飞行形态绘制十段残影和本体。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0f)
                return false;
            CRUtil.DrawAfterimages(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }
        /// <summary>飞行子飞镖额外绘制发光层。</summary>
        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[0] == 0f)
                return;
            Texture2D glow = ModContent.Request<Texture2D>(
                "CalamityRebirth/Content/Projectiles/Melee/GodslayerDartGlow").Value;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White
                , Projectile.rotation, glow.Size() / 2f, Projectile.scale, SpriteEffects.None);
        }
        /// <summary>每两帧从玩家中心向八方向依次甩出一枚子飞镖。</summary>
        private void MountedAI()
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[Projectile.owner];
            fireDelay--;
            if (fireDelay > 0 || firedCount >= 8)
                return;
            float baseAngle = Projectile.velocity.ToRotation();
            float currentAngle = baseAngle - MathHelper.Pi
                + MathHelper.PiOver4 * firedCount
                + MathHelper.ToRadians(22.5f + 45f * randomStart);
            Vector2 direction = currentAngle.ToRotationVector2();
            Projectile.NewProjectile(Projectile.GetSource_FromThis()
                , owner.RotatedRelativePoint(owner.MountedCenter, true), direction * 32f
                , Type, Projectile.damage, 2f, Projectile.owner, 1f, 0f);
            fireDelay = 3;
            firedCount++;
        }
        /// <summary>前四十帧减速蓄力，之后逐渐加速并追踪一万像素内目标。</summary>
        private void FlyingAI()
        {
            if (Math.Abs(Projectile.velocity.X) >= 2f || Math.Abs(Projectile.velocity.Y) >= 2f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = i == 0 ? Vector2.Zero : Projectile.velocity * 0.5f;
                    int dust = Dust.NewDust(Projectile.position + offset, Projectile.width
                        , Projectile.height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 1f);
                    Main.dust[dust].scale *= 1f + Main.rand.Next(5) * 0.1f;
                    Main.dust[dust].velocity *= 0.2f;
                    Main.dust[dust].noGravity = true;
                }
            }
            if (Projectile.timeLeft > 320)
            {
                Projectile.velocity *= 0.9f;
                return;
            }
            Projectile.ai[1] = MathHelper.Clamp(Projectile.ai[1] + 1.6f, 0f, 25f);
            CRUtil.HomeInNPC(Projectile, 10000f, Projectile.ai[1], 15f
                , !Projectile.tileCollide);
        }
        /// <summary>向 NPC 施加当前灾厄版本的神谕狱火。</summary>
        private static void ApplyInferno(NPC target)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                , "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 500);
        }
        /// <summary>向 PvP 目标施加当前灾厄版本的神谕狱火。</summary>
        private static void ApplyInferno(Player target)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                , "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 500);
        }
    }
}
