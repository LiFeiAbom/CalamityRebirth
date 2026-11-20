using CalamityRebirth.Content.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Magic
{
    /// <summary>
    /// 友方硫磺火球：焦热地狱、终结虚空与融合之脑周期齐射使用的隐形火球。
    /// </summary>
    internal sealed class BrimstoneHellfireballFriendly2 : ModProjectile
    {
        /// <summary>隐形贴图；运动轨迹由红色生命汲取粉尘表现。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置 12×12 碰撞箱、魔法伤害和无限穿透。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
        }
        /// <summary>首次更新播放火球音效，持续提供红橙色光照与五粒拖尾尘。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.05f, 0.05f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.LifeDrain, 0f, 0f, 100, default, 0.6f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.velocity = dust.velocity * 0.5f + Projectile.velocity * 0.1f;
            }
        }
        /// <summary>命中 NPC 时施加 1000 帧硫磺火，并把本地免疫窗口压到 7 帧。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 7;
            StandingFire.ApplyBrimstoneFlames(target, 1000);
        }
        /// <summary>PvP 命中也施加 1000 帧硫磺火。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            StandingFire.ApplyBrimstoneFlames(target, 1000);
        }
        /// <summary>消失时由拥有者生成同伤害的隐形硫磺火爆炸。</summary>
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero
                , ModContent.ProjectileType<HellfireExplosionFriendly2>(), Projectile.damage
                , Projectile.knockBack, Projectile.owner);
        }
    }
}
