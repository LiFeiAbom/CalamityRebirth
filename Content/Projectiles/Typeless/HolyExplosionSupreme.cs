using CalamityRebirth.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>至圣神圣爆炸：极乐之庇护冲撞命中的不可见范围判定。</summary>
    internal sealed class HolyExplosionSupreme : ModProjectile
    {
        /// <summary>使用透明占位贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>弹幕主人，用于取得护盾染色。</summary>
        private Player Owner => Main.player[Projectile.owner];
        /// <summary>设置 90×90 判定、三帧寿命、无限穿透与四帧本地命中冷却。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }
        /// <summary>播放神圣冲击音并喷出金色火焰与日耀尘。</summary>
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CalamityRebirthSounds.ProvidenceHolyBlastImpact, Projectile.Center);
            for (int i = 0; i < 24; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame
                    , new Vector2(4.5f, 4.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.9f)
                    , 0, default, Main.rand.NextFloat(1.5f, 2.8f));
                dust.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                dust.noGravity = true;
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = new Vector2(6f, 6f).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.2f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + velocity * 2f, DustID.SolarFlare
                    , velocity, 0, default, 1f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
            }
        }
    }
}
