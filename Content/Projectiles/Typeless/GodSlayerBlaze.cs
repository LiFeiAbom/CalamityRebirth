using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>弑神者烈焰：法师套装受击时释放的持续范围爆炸。</summary>
    internal sealed class GodSlayerBlaze : ModProjectile
    {
        /// <summary>设置 250×250 判定、无限穿透和五帧局部命中冷却。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        /// <summary>生成紫黑爆发尘，悬停五十八帧后消散。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0.75f);
            int dustCount = (int)MathHelper.Max(0f, 17.5f - Projectile.ai[0] / 4f);
            Projectile.ai[0] += 4f;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 direction = new Vector2(Main.rand.Next(-10, 11)
                    , Main.rand.Next(-10, 11)).SafeNormalize(Vector2.UnitX);
                Vector2 velocity = direction * Main.rand.Next(9, 27);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.ShadowbeamStaff
                    , velocity, 100, default, 1.5f);
                dust.noGravity = true;
            }
            if (Projectile.ai[0] > 232f)
                Projectile.Kill();
        }
        /// <summary>命中 NPC 时施加 500 帧神谕狱火。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                , "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 500);
        }
        /// <summary>PvP 命中时施加同样的神谕狱火。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer"
                , "GodSlayerInferno", 500);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "GodSlayerInferno", 500);
        }
    }
}
