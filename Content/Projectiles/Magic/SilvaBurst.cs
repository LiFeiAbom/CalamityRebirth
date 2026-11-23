using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Magic
{
    /// <summary>始源林海魔法爆裂：96×96 隐形判定框，每个敌怪只结算一次。</summary>
    public sealed class SilvaBurst : ModProjectile
    {
        /// <summary>使用通用隐形贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置魔法伤害、全穿透、局部单次命中和两帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
        }
        /// <summary>生成偏绿强光，并在判定框内喷出两批叶绿粉尘。</summary>
        public override void AI()
        {
            const float brightness = 1.6f;
            Lighting.AddLight(Projectile.Center, 0.27f * brightness
                , 0.82f * brightness, 0.157f * brightness);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.ChlorophyteWeapon, 0f, 0f, 100
                    , new Color(Main.DiscoR, 203, 103), 1.5f);
            }
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.ChlorophyteWeapon, 0f, 0f, 0
                    , new Color(Main.DiscoR, 203, 103), 2.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 3f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.ChlorophyteWeapon, 0f, 0f, 100
                    , new Color(Main.DiscoR, 203, 103), 1.5f);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
