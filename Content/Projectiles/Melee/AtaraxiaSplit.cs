using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 禅心剑针叶弹：快速减速、淡出，并在 25 帧后消失。
    /// </summary>
    internal sealed class AtaraxiaSplit : ModProjectile
    {
        /// <summary>
        /// 配置针叶弹：不按穿透次数消失，而由 25 帧寿命控制；
        /// 使用静态 NPC 无敌帧，避免多枚针叶弹在同一目标上重复爆发。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.timeLeft = 25;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
        }
        /// <summary>
        /// 每帧减速并以 alpha 方式淡出，照明强度同步下降。
        /// 尘埃生成概率随 alpha 提高而降低，使弹体接近消失时拖尾自然变稀。
        /// </summary>
        public override void AI()
        {
            DrawOffsetX = -5;
            DrawOriginOffsetY = -1;
            DrawOriginOffsetX = 0;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 0.93f;
            Projectile.alpha += 5;
            float lightFactor = (255f - Projectile.alpha) / 255f;
            Lighting.AddLight(Projectile.Center, 0.3f * lightFactor, 0.05f * lightFactor, 0.2f * lightFactor);
            if (Main.rand.Next(256) <= Projectile.alpha - 60)
                return;
            int index = Dust.NewDust(Projectile.Center, 1, 1, DustID.UndergroundHallowedEnemies);
            Dust dust = Main.dust[index];
            dust.position = Projectile.Center - Projectile.velocity * 0.7f;
            dust.noGravity = true;
            dust.velocity *= 0.3f;
            dust.velocity += Projectile.velocity * 0.4f;
            dust.scale = Main.rand.NextFloat(0.5f, 1f);
            dust.alpha = Main.rand.Next(80, 200);
        }
    }
}
