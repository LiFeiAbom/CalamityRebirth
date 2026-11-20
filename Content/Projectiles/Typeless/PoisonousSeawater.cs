using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>有毒海水：利维坦龙涎香与融合之脑移动时留下的短寿命毒水判定。</summary>
    internal sealed class PoisonousSeawater : ModProjectile
    {
        /// <summary>使用透明贴图，以水泡和毒泡尘表现。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置 30×30 判定、六帧寿命和无限穿透。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6;
        }
        /// <summary>每帧生成两粒淡蓝水泡尘或毒泡尘。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.15f, 0.4f);
            int dustID = Main.rand.NextBool(4) ? 89 : 33;
            for (int i = 0; i < 2; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , dustID, 0f, 0f, 100, default, 1f);
                if (dustID == 89)
                    Main.dust[dustIndex].scale *= 0.35f;
                Main.dust[dustIndex].velocity = Vector2.Zero;
            }
        }
        /// <summary>命中 NPC 时施加中毒与剧毒各 120 帧。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(BuffID.Poisoned, 120);
        }
        /// <summary>PvP 命中施加相同减益。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(BuffID.Poisoned, 120);
        }
    }
}
