using CalamityRebirth.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>
    /// 宇宙冲刺爆炸：在玩家中心生成三帧不可见判定，造成范围冲撞伤害。
    /// </summary>
    internal sealed class CosmicDashExplosion : ModProjectile
    {
        /// <summary>使用工程透明占位贴图；全部视觉由死亡粉尘完成。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>弹幕主人，用于取得护盾染色。</summary>
        private Player Owner => Main.player[Projectile.owner];
        /// <summary>设置 140×140 判定、无限穿透、三帧寿命与四帧本地命中冷却。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }
        /// <summary>播放冲击音效，并喷出冷焰与双色电尘。</summary>
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CalamityRebirthSounds.DevourerSegmentBreak with { PitchVariance = 0.3f }, Projectile.position);
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f, PitchVariance = 0.3f }, Projectile.position);
            for (int i = 0; i < 35; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GiantCursedSkullBolt
                    , new Vector2(4.5f, 4.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.9f)
                    , 0, default, Main.rand.NextFloat(1.5f, 2.8f));
                dust.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                dust.noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Vector2 velocity = new Vector2(6f, 6f).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.2f);
                Dust first = Dust.NewDustPerfect(Projectile.Center + velocity * 2f, DustID.WitherLightning
                    , velocity, 0, default, 1f);
                first.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                Dust second = Dust.NewDustPerfect(Projectile.Center + velocity * 2f, DustID.Electric
                    , velocity, 0, default, 1f);
                second.shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
            }
        }
    }
}
