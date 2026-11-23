using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>
    /// 龙蒿生命能量：远程弹幕消失时概率生成的直线伤害弹幕。
    /// 本体不可见，以绿色尘尾表现运动轨迹。
    /// </summary>
    internal sealed class TarraEnergy : ModProjectile
    {
        /// <summary>使用工程共用的隐形占位贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置 8×8 碰撞箱、单次命中与 120 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
        }
        /// <summary>每帧沿运动反方向生成三颗无重力绿色尘。</summary>
        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 dustPosition = Projectile.position - Projectile.velocity / 10f * i;
                int dust = Dust.NewDust(dustPosition, 1, 1, DustID.TerraBlade);
                Main.dust[dust].position = dustPosition;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
