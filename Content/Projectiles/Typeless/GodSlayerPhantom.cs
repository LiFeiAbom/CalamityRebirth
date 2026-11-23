using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>弑神幻影：召唤套装命中时生成的追踪飞行弹幕。</summary>
    internal sealed class GodSlayerPhantom : ModProjectile
    {
        /// <summary>登记三帧循环动画。</summary>
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        /// <summary>设置 20×20 判定、无限穿透、十帧局部命中和 300 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        /// <summary>离主人过远时回追，否则追踪四百像素内最近敌人。</summary>
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame = (Projectile.frame + 1) % 3;
                Projectile.frameCounter = 0;
            }
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.ShadowbeamStaff, 0f, 0f, 0, default, 1.3f);
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);
            if (!Main.player.IndexInRange(Projectile.owner)
                || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 30);
                return;
            }
            Player owner = Main.player[Projectile.owner];
            float inertia = 40f * Math.Clamp(Projectile.ai[1], 0.5f, 1.5f);
            if (Vector2.Distance(Projectile.Center, owner.Center) > 600f)
            {
                Vector2 desired = (owner.Center - Projectile.Center).SafeNormalize(Vector2.UnitY)
                    * (8f * Math.Clamp(Projectile.ai[1], 0.5f, 1.5f));
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + desired) / inertia;
                return;
            }
            CRUtil.HomeInNPC(Projectile, 400f, 15f, 20f, false);
        }
        /// <summary>按三帧贴图绘制幻影。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / 3;
            Rectangle frame = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition
                + new Vector2(0f, Projectile.gfxOffY), frame, Projectile.GetAlpha(lightColor)
                , Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
        /// <summary>最后八十五帧逐渐淡出。</summary>
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
        /// <summary>命中 NPC 时施加 600 帧神谕狱火。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyInferno(target);
        }
        /// <summary>PvP 命中时施加 600 帧神谕狱火。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ApplyInferno(target);
        }
        /// <summary>消散时形成一次 40×40 范围伤害。</summary>
        public override void OnKill(int timeLeft)
        {
            Vector2 center = Projectile.Center;
            for (int i = 0; i < 36; i++)
            {
                Vector2 velocity = (Vector2.UnitY * 18f).RotatedBy(i * MathHelper.TwoPi / 36f);
                int dust = Dust.NewDust(center + velocity, 0, 0, DustID.ShadowbeamStaff
                    , velocity.X * 1.5f, velocity.Y * 1.5f, 100, default, 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
            }
            Projectile.Center = center;
            Projectile.width = Projectile.height = 40;
            Projectile.Center = center;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
        }
        /// <summary>按三版本弱依赖施加神谕狱火。</summary>
        private static void ApplyInferno(NPC target)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 600);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                , "GodSlayerInferno", 600);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 600);
        }
        /// <summary>按三版本弱依赖施加神谕狱火给玩家。</summary>
        private static void ApplyInferno(Player target)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 600);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                , "GodSlayerInferno", 600);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 600);
        }
    }
}
