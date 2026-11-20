using CalamityRebirth.Content.ModBuffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Rogue
{
    /// <summary>
    /// 晶光头骨召唤的隐形闪电。
    /// 命中后施加触电II，并以原版电弧尘绘制从天而降的折线闪电。
    /// </summary>
    internal sealed class CrystalSkullLightning : ModProjectile
    {
        /// <summary>使用不可见弹幕，避免额外维护一张闪电贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>关闭屏幕裁剪剔除，确保跨屏闪电仍会生成粒子。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }
        /// <summary>配置一帧命中、一帧视觉收尾的盗贼闪电。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        /// <summary>
        /// ai[0] 保存目标 NPC 索引，ai[1] 控制只生成一次主闪电视觉。
        /// 电弧尘从目标上方约 600 像素处一路抖动落下，形成原版触电风格的高亮路径。
        /// </summary>
        public override void AI()
        {
            NPC target = Projectile.ai[0] >= 0f && Projectile.ai[0] < Main.maxNPCs
                ? Main.npc[(int)Projectile.ai[0]]
                : null;
            if (Projectile.ai[1] == 0f)
            {
                Vector2 end = target?.Center ?? Projectile.Center;
                Vector2 start = end - Vector2.UnitY * 600f;
                for (int i = 0; i <= 54; i++)
                {
                    float progress = i / 54f;
                    Vector2 position = Vector2.Lerp(start, end, progress);
                    position.X += (float)System.Math.Sin(progress * 34f + Projectile.identity) * 10f
                        + Main.rand.NextFloat(-5f, 5f);
                    Dust dust = Dust.NewDustPerfect(position, DustID.Electric, Vector2.UnitY * 2f
                        , 100, new Color(Main.rand.Next(20, 100), 220, 255), Main.rand.NextFloat(0.9f, 1.5f));
                    dust.noGravity = true;
                }
                Projectile.ai[1] = 1f;
            }
            Lighting.AddLight(Projectile.Center, 0.35f, 0.75f, 1f);
        }
        /// <summary>闪电命中 NPC 时施加触电II。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElectrifiedII>(), ElectrifiedII.Duration);
        }
        /// <summary>PvP 命中时对玩家施加同样的触电II。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ElectrifiedII>(), ElectrifiedII.Duration);
        }
        /// <summary>消失时在落点生成短暂的紫色电弧尘爆发。</summary>
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric
                    , Main.rand.NextVector2Circular(7f, 7f), 100, new Color(180, 220, 255)
                    , Main.rand.NextFloat(1f, 1.6f));
                dust.noGravity = true;
            }
        }
    }
}
