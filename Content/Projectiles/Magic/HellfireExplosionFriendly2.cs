using CalamityRebirth.Content.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Magic
{
    /// <summary>友方硫磺火爆炸：由周期火球消亡时生成，以红色尘扩散并施加硫磺火。</summary>
    internal sealed class HellfireExplosionFriendly2 : ModProjectile
    {
        /// <summary>使用工程透明贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置 130×130 区域、魔法伤害与 300 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }
        /// <summary>播放一次爆炸音效，并按逐帧递增的 ai[0] 向中心四周喷出红色尘。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.75f, 0.05f, 0.05f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            float dustCount = 25f;
            if (Projectile.ai[0] > 180f)
                dustCount -= (Projectile.ai[0] - 180f) / 2f;
            if (dustCount <= 0f)
            {
                Projectile.Kill();
                return;
            }
            dustCount *= 0.7f;
            Projectile.ai[0] += 4f;
            for (int i = 0; i < (int)dustCount; i++)
            {
                Vector2 direction = new(Main.rand.Next(-15, 16), Main.rand.Next(-15, 16));
                direction = direction.SafeNormalize(Vector2.UnitY) * Main.rand.Next(5, 12);
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.LifeDrain, 0f, 0f, 100, default, 0.75f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
                dust.position = Projectile.Center + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                dust.velocity = direction;
            }
        }
        /// <summary>爆炸命中施加 1000 帧硫磺火。</summary>
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
    }
}
