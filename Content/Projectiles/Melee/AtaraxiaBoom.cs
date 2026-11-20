using CalamityRebirth.Content.Particles;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 禅心剑爆炸区：ai[0] 为 0 时是半径 200 的近战大爆炸，为 1 时是半径 130 的主弹小爆炸。
    /// </summary>
    internal sealed class AtaraxiaBoom : ModProjectile
    {
        /// <summary>使用工程内不可见弹幕贴图，爆炸表现完全由粒子与光照完成。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>
        /// 配置只存活一帧的隐形伤害区。
        /// ai[0] 携带爆炸等级：0 为近战大爆炸，1 为主弹死亡小爆炸。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        /// <summary>爆炸区存活期间提供短暂紫红光照。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.1f, 0.45f);
        }
        /// <summary>
        /// 仅近战大爆炸生成花瓣状尘埃环和向上抛射火花。
        /// 小爆炸保持较轻量，避免主弹每次死亡都产生过密画面。
        /// </summary>
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] != 0f)
                return;
            int petalCount = 6;
            float thetaDelta = new Vector2(3f, 3f).RotatedByRandom(100f).ToRotation();
            for (float theta = 0f; theta < MathHelper.TwoPi; theta += 0.05f)
            {
                Vector2 velocity = theta.ToRotationVector2() * (0.5f
                    + (float)(Math.Sin(thetaDelta + theta * petalCount) + 1f) * 10f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, velocity);
                dust.noGravity = true;
                dust.scale = 1.15f;
                dust.color = Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat());
            }
            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = new Vector2(15f, 15f).RotatedByRandom(100) * Main.rand.NextFloat(0.6f, 1.2f);
                AtaraxiaParticleSystem.SpawnStarSpark(Projectile.Center + velocity, velocity
                    , Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat())
                    , 50, Main.rand.NextFloat(0.7f, 0.95f), true);
            }
        }
        /// <summary>NPC 命中前应用多目标递减。</summary>
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ApplyFalloff();
        }
        /// <summary>PvP 玩家命中前应用与 NPC 相同的多目标递减。</summary>
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            ApplyFalloff();
        }
        /// <summary>
        /// 第一个目标保持全额伤害，之后每多命中一个目标就将当前伤害乘以 0.80。
        /// 最终下限为 1，防止高目标数量下伤害归零。
        /// </summary>
        private void ApplyFalloff()
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.80f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        /// <summary>
        /// 根据 ai[0] 选择 130 或 200 像素半径，并用中心点加四个角距离做圆形碰撞检测。
        /// </summary>
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float radius = Projectile.ai[0] == 1f ? 130f : 200f;
            return CollisionUtil.CircleIntersectsRectangle(Projectile.Center, targetHitbox, radius);
        }
    }
}
