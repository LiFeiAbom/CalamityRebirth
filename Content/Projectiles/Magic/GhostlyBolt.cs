using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Magic
{
    /// <summary>血炎法师的隐形幽灵魔弹，使用诅咒骷髅电束尘表现运动。</summary>
    internal sealed class GhostlyBolt : ModProjectile
    {
        /// <summary>使用工程共用的隐形占位贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置 6×6 判定、无限穿透、6 帧局部免疫与 15 秒寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.timeLeft = 900;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        /// <summary>第 6 帧播放入场音并喷发一圈尘，之后持续生成三颗拖尾尘。</summary>
        public override void AI()
        {
            if (Projectile.localAI[0] == 6f)
            {
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                for (int i = 0; i < 40; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                        , DustID.GiantCursedSkullBolt, 0f, 0f, 100, default, 1f);
                    Main.dust[dust].velocity *= 3f;
                    Main.dust[dust].velocity += Projectile.velocity * 0.75f;
                    Main.dust[dust].scale *= 1.2f;
                    Main.dust[dust].noGravity = true;
                }
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] <= 6f)
                return;
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.GiantCursedSkullBolt, Projectile.velocity.X * 0.2f
                    , Projectile.velocity.Y * 0.2f, 100, default, 1f);
                Main.dust[dust].velocity *= 0.6f;
                Main.dust[dust].scale *= 1.4f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
