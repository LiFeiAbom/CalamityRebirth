using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>硫磺爆裂：阿巴顿弹幕暴击命中时生成的原位圆形爆炸。</summary>
    internal sealed class AbaddonCrit : ModProjectile
    {
        /// <summary>爆炸判定半径。</summary>
        private const float ExplosionRadius = 300f;
        /// <summary>命中附加的硫磺火持续时间。</summary>
        private const int BrimstoneFrames = 360;
        /// <summary>缺少专用粉尘时使用的原版占位尘。</summary>
        private const int FallbackDust = 218;
        /// <summary>外部硫磺火尘类型缓存。</summary>
        private static int brimstoneFlameDust = -1;
        /// <summary>使用透明占位贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>弹幕主人，用于取得面部饰品染色。</summary>
        private Player Owner => Main.player[Projectile.owner];
        /// <summary>设置 300×300 判定、两帧寿命与单次命中间隔。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        /// <summary>每帧喷出三十一颗硫磺火尘。</summary>
        public override void AI()
        {
            int flameDust = GetBrimstoneFlameDust();
            for (int i = 0; i < 31; i++)
            {
                int dustType = Main.rand.NextBool(4) || flameDust == 0 ? FallbackDust : flameDust;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType
                    , new Vector2(5f, 5f).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1.1f, 2.2f)
                    , 0, default, Main.rand.NextFloat(2.8f, 3.4f));
                dust.shader = GameShaders.Armor.GetSecondaryShader(Owner.cFace, Owner);
                dust.noGravity = true;
            }
        }
        /// <summary>命中 NPC 时施加三百六十帧硫磺火并播放撞击音。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyBrimstone(target);
        }
        /// <summary>PvP 命中时执行与 NPC 相同的硫磺火结算。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ApplyBrimstone(target);
        }
        /// <summary>使用圆形半径与目标矩形最近点进行命中判定。</summary>
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle center = new((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1);
            if (center.Intersects(targetHitbox))
                return true;
            float closest = Vector2.Distance(Projectile.Center, targetHitbox.TopLeft());
            closest = Math.Min(closest, Vector2.Distance(Projectile.Center, targetHitbox.TopRight()));
            closest = Math.Min(closest, Vector2.Distance(Projectile.Center, targetHitbox.BottomLeft()));
            closest = Math.Min(closest, Vector2.Distance(Projectile.Center, targetHitbox.BottomRight()));
            return closest <= ExplosionRadius;
        }
        /// <summary>硫磺爆裂不破坏物块。</summary>
        public override bool? CanCutTiles() => false;
        /// <summary>给 NPC 施加所有已加载分支的硫磺火。</summary>
        private void ApplyBrimstone(NPC target)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "BrimstoneFlames", BrimstoneFrames);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "BrimstoneFlames", BrimstoneFrames);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "BrimstoneFlames", BrimstoneFrames);
            SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.5f, PitchVariance = 0.4f }, Projectile.Center);
        }
        /// <summary>给 PvP 玩家施加所有已加载分支的硫磺火。</summary>
        private void ApplyBrimstone(Player target)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "BrimstoneFlames", BrimstoneFrames);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "BrimstoneFlames", BrimstoneFrames);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "BrimstoneFlames", BrimstoneFrames);
            SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.5f, PitchVariance = 0.4f }, Projectile.Center);
        }
        /// <summary>懒加载并缓存外部硫磺火粉尘类型。</summary>
        private static int GetBrimstoneFlameDust()
        {
            if (brimstoneFlameDust < 0)
                brimstoneFlameDust = ModContent.TryFind("CalamityMod", "BrimstoneFlame", out ModDust dust)
                    ? dust.Type : 0;
            return brimstoneFlameDust;
        }
    }
}
