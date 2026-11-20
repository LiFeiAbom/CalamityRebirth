using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Magic
{
    /// <summary>
    /// 暗影雨：腐烂之脑与聚合之脑受击无敌期间生成的隐形雨滴弹幕。
    /// 弹幕本身透明，仅用恶魔粉尘表现下坠轨迹，并沿生成时的初速度直线飞行。
    /// </summary>
    internal sealed class AuraRain : ModProjectile
    {
        /// <summary>复用工程已有的全透明贴图，视觉完全由恶魔粉尘承担。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>
        /// 使用原版友好雨云 AI 模板与 2×2 微小碰撞箱；伤害类型保留魔法以兼容旧版，
        /// 实际基础伤害已在生成前按通用伤害结算。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = ProjAIStyleID.RainCloud;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.scale = 1.1f;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.RainFriendly;
        }
        /// <summary>每帧生成一粒恶魔粉尘，并沿速度反方向回退少量位置以形成雨滴拖尾。</summary>
        public override void AI()
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.Demonite, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f);
            Dust dust = Main.dust[dustIndex];
            dust.velocity = Vector2.Zero;
            dust.position -= Projectile.velocity / 5f;
            dust.noGravity = true;
            dust.scale = 0.8f;
            dust.noLight = true;
        }
    }
}
