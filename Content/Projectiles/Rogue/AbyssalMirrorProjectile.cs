using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Rogue
{
    /// <summary>深渊魔镜闪避时释放的流明流体弹幕。</summary>
    internal sealed class AbyssalMirrorProjectile : ModProjectile
    {
        /// <summary>贴图包含三帧纵向动画。</summary>
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }
        /// <summary>流体可穿墙、无限穿透，并在 50 帧后消失。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 50;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Generic;
        }
        /// <summary>最后 25 帧逐步淡出。</summary>
        public override void AI()
        {
            if (Projectile.timeLeft < 25)
                Projectile.alpha += 10;
        }
        /// <summary>命中附加当前版本对应的深海压力类减益。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "Eutrophication", 120);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "CrushDepth", 120);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "CrushDepth", 120);
        }
    }
}
