using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Rogue
{
    /// <summary>
    /// 晶光头骨潜行命中时生成的电火花。
    /// 使用普通伤害类型，避免火花继续触发盗贼命中链或其他潜行效果。
    /// </summary>
    internal sealed class CrystalSkullSpark : ModProjectile
    {
        /// <summary>使用不可见弹幕贴图，视觉完全由蓝色电弧尘构成。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>
        /// 配置短寿命重力火花：穿透多个目标，使用静态 NPC 无敌帧控制连续命中。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
        }
        /// <summary>
        /// 模拟原版电工手套火花的抛物线运动：先保留水平速度，随后受重力影响下坠。
        /// 每帧生成蓝紫电弧尘，并限制最大下落速度避免直接穿过屏幕。
        /// </summary>
        public override void AI()
        {
            Projectile.ai[0] = MathHelper.Min(Projectile.ai[0] + 1f, 5f);
            if (Projectile.ai[0] >= 5f)
            {
                Projectile.velocity.X *= 0.97f;
                Projectile.velocity.Y += 0.2f;
            }
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric
                , Main.rand.NextVector2Circular(1.5f, 1.5f) - Vector2.UnitY
                , 100, new Color(Main.rand.Next(20, 100), 204, 250), Main.rand.NextFloat(0.8f, 1.2f));
            dust.noGravity = true;
            Lighting.AddLight(Projectile.Center, 0.08f, 0.28f, 0.55f);
        }
        /// <summary>火花不会因撞到物块而消失。</summary>
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
