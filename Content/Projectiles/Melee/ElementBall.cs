using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 元素方舟散射的隐形追踪元素球，视觉完全由彩虹粉尘承担。
    /// </summary>
    internal sealed class ElementBall : ModProjectile
    {
        /// <summary>复用工程已有的全透明弹幕资源。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>
        /// 元素球使用 8×8 碰撞箱、单次穿透、两次额外更新、穿墙和 150 帧寿命。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
        }
        /// <summary>每帧生成彩虹色定位尘，并向 1600 像素内的最近敌人平滑追踪。</summary>
        public override void AI()
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.RainbowTorch, Projectile.direction * 2f, 0f, 150
                , new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            Dust dust = Main.dust[dustIndex];
            dust.noGravity = true;
            dust.velocity *= 0f;
            CRUtil.HomeInNPC(Projectile, 1600f, 24f, 20f);
        }
    }
}
