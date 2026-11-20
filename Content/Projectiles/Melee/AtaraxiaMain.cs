using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Content.Particles;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 禅心剑主弹：5 帧飞弹，命中或寿命结束后生成半径 130 的小爆炸。
    /// </summary>
    internal sealed class AtaraxiaMain : ModProjectile
    {
        /// <summary>主弹贴图横向排列的动画帧数。</summary>
        private const int NumAnimationFrames = 5;
        /// <summary>每帧持续的游戏更新次数。</summary>
        private const int AnimationFrameTime = 9;
        /// <summary>主弹独立存活计时，用于延迟拖尾和火花生成。</summary>
        public int time;
        /// <summary>注册主弹使用五帧动画。</summary>
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = NumAnimationFrames;
        }
        /// <summary>
        /// 配置高速近战主弹：单次穿透、本地 NPC 无敌帧、六倍额外更新且不受水与物块阻挡。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 300;
        }
        /// <summary>
        /// 更新绘制偏移、朝向、照明、拖尾火花、尘埃与动画。
        /// 火花只在飞出短暂延迟并仍位于玩家 1400 像素内时生成，避免远处弹幕浪费粒子预算。
        /// </summary>
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(owner.Center, Projectile.Center);
            DrawOffsetX = -40;
            DrawOriginOffsetY = -3;
            DrawOriginOffsetX = 18;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, 0.45f, 0.1f, 0.1f);
            if (time > 8 && targetDist < 1400f)
            {
                AtaraxiaParticleSystem.SpawnStarSpark(Projectile.Center - Projectile.velocity * 1.5f
                    , -Projectile.velocity * Main.rand.NextFloat(0.3f, 1.5f)
                    , Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat()) * 0.7f, 8, 0.8f);
            }
            if (time > 8 && targetDist < 1400f)
            {
                AtaraxiaParticleSystem.SpawnStarSpark(Projectile.Center + Projectile.velocity * 2f
                    , Projectile.velocity
                    , Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat()) * 0.85f, 2, 1.9f);
            }
            if (time > 8 && Main.rand.NextBool())
            {
                Vector2 dustVelocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 1.2f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, dustVelocity, 0
                    , default, Main.rand.NextFloat(0.7f, 0.9f));
                dust.noGravity = true;
                dust.color = Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat());
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
            time++;
        }
        /// <summary>主弹直接命中 NPC 时附加 3 秒原版暗影焰。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShadowflameII.ApplyToNPC(target, 180);
        }
        /// <summary>主弹直接命中 PvP 玩家时附加同长度暗影焰。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ShadowflameII.ApplyToPlayer(target, 180);
        }
        /// <summary>
        /// 主弹销毁时播放音效，并由发射者客户端生成半径 130 的小爆炸。
        /// 爆炸伤害固定为主弹当前伤害的一半，后续清理与粒子只做视觉表现。
        /// </summary>
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath55, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero
                    , ModContent.ProjectileType<AtaraxiaBoom>(), Projectile.damage / 2, 0f, Projectile.owner, 1f);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = new Vector2(20f, 20f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.2f);
                Color color = Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat());
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity, DustID.FireworksRGB
                    , velocity * Main.rand.NextFloat(0.2f, 1f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.3f, 0.65f);
                dust.color = color;
                dust.noLight = true;
                dust.noLightEmittence = true;
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = new Vector2(15f, 15f).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.2f);
                AtaraxiaParticleSystem.SpawnGlowBurst(Projectile.Center + velocity, velocity
                    , Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat()));
            }
            for (float i = 0f; i < 3f; i++)
            {
                Color color = Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat());
                Vector2 spawn = Projectile.Center + Main.rand.NextVector2Circular(20f, 20f) * (i + 1f);
                AtaraxiaParticleSystem.SpawnPulse(spawn, color, Main.rand.NextFloat(-10f, 10f)
                    , Main.rand.NextFloat(0.12f, 0.18f), Main.rand.Next(13, 16));
            }
        }
        /// <summary>
        /// 使用中心点加 30 像素邻近距离的圆形碰撞，避免高速小贴图从大型 NPC 边缘穿过。
        /// 分别检查四个角而不是仅检查矩形相交，是为了兼顾旋转弹幕的视觉尺寸。
        /// </summary>
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CollisionUtil.CircleIntersectsRectangle(Projectile.Center, targetHitbox, 30f);
        }
    }
}
