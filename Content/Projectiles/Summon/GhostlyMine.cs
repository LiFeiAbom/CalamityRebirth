using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
    /// <summary>血炎召唤套装的环绕幽灵地雷，围绕主人缓慢旋转并在命中时爆发。</summary>
    internal sealed class GhostlyMine : ModProjectile
    {
        /// <summary>首次 AI 初始化闩锁。</summary>
        private bool initialized;
        /// <summary>入场尘是否尚未生成。</summary>
        private bool spawnDust = true;
        /// <summary>设置 30×30 判定、不占地形碰撞和 15 秒寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
        }
        /// <summary>围绕主人半径 550 像素旋转，并生成入场地牢魂尘。</summary>
        public override void AI()
        {
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.ai[1] = Projectile.ai[0];
                initialized = true;
            }
            Player owner = Main.player[Projectile.owner];
            double radians = Projectile.ai[1] * (Math.PI / 180D);
            const float distance = 550f;
            Projectile.Center = owner.Center + new Vector2(
                (float)(Math.Cos(radians) * distance),
                (float)(Math.Sin(radians) * distance));
            Projectile.ai[1]++;
            if (!spawnDust)
                return;
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.DungeonSpirit, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
            }
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.DungeonSpirit, 0f, 0f, 100, default, 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.DungeonSpirit, 0f, 0f, 100, default, 2f);
            }
            spawnDust = false;
        }
        /// <summary>末段渐隐并保持半透明白色绘制。</summary>
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
        /// <summary>命中后扩大判定框并生成灵魂尘爆发。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.Center = target.Center;
            Projectile.width = 150;
            Projectile.height = 150;
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.DungeonSpirit, 0f, 0f, 100, default, 1.2f);
            }
            for (int i = 0; i < 60; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.DungeonSpirit, 0f, 0f, 100, default, 1.7f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.DungeonSpirit, 0f, 0f, 100, default, 1f);
            }
        }
    }
}
