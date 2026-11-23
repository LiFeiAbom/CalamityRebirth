using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>弑神破片弹：命中或超时后炸成 4～6 枚次级破片。</summary>
    internal sealed class GodSlayerShrapnelRound : ModProjectile
    {
        /// <summary>设置 8×8 判定、单次穿透、300 帧寿命和额外一次更新。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }
        /// <summary>首帧播放音效并持续留下紫黑尘尾。</summary>
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
                Projectile.localAI[0] = 1f;
            }
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.ShadowbeamStaff, 0f, 0f, 100, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = Main.dust[dust].velocity * 0.5f
                    + Projectile.velocity * 0.1f;
            }
        }
        /// <summary>在主人端炸出四到六枚次级破片，每枚继承 30% 伤害。</summary>
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            int count = Main.rand.Next(4, 7);
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.Next(7, 11);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                    , velocity, ModContent.ProjectileType<GodSlayerShrapnel>()
                    , (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
            }
        }
    }
}
