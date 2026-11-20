using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 禅心剑侧弹：命中后向六个方向分裂出针叶弹。
    /// </summary>
    internal sealed class AtaraxiaSide : ModProjectile
    {
        /// <summary>侧弹贴图横向排列的动画帧数。</summary>
        private const int NumAnimationFrames = 5;
        /// <summary>每帧持续的游戏更新次数。</summary>
        private const int AnimationFrameTime = 9;
        /// <summary>侧弹独立存活计时，用于延迟生成拖尾尘埃。</summary>
        public int time;
        /// <summary>注册侧弹使用五帧动画。</summary>
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = NumAnimationFrames;
        }
        /// <summary>配置轻型侧弹：单次穿透、六倍额外更新且不受水面和物块阻挡。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 300;
        }
        /// <summary>
        /// 根据 ai[1] 选择左右偏移，使两枚侧弹形成外张轨迹。
        /// 动画与照明流程与主弹一致，但拖尾改用古代光尘以降低画面重复感。
        /// </summary>
        public override void AI()
        {
            DrawOffsetX = -28;
            DrawOriginOffsetY = -2;
            DrawOriginOffsetX = 12;
            float rotationOffset = Projectile.ai[1] == 2f ? 0.2f : -0.2f;
            Projectile.rotation = Projectile.velocity.RotatedBy(rotationOffset).ToRotation();
            Lighting.AddLight(Projectile.Center, 0.3f, 0.1f, 0.45f);
            if (time > 8 && Main.rand.NextBool())
            {
                Vector2 dustVelocity = Projectile.velocity.RotatedBy(rotationOffset);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.AncientLight
                    , -dustVelocity * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(0.4f, 0.6f));
                dust.noGravity = true;
                dust.color = Color.Lerp(Color.DarkOrchid, Color.IndianRed, Main.rand.NextFloat());
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
            time++;
        }
        /// <summary>侧弹直接命中 NPC 时附加 3 秒暗影焰。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShadowflameII.ApplyToNPC(target, 180);
        }
        /// <summary>侧弹直接命中 PvP 玩家时附加同长度暗影焰。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ShadowflameII.ApplyToPlayer(target, 180);
        }
        /// <summary>
        /// 侧弹销毁时向六个等角方向分裂出针叶弹。
        /// 每枚针叶弹只继承侧弹当前伤害的 2%，并通过随机起始角打散严格对称的形状。
        /// </summary>
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
            const int numSplits = 6;
            int splitID = ModContent.ProjectileType<AtaraxiaSplit>();
            int damage = (int)(Projectile.damage * 0.02f);
            float angleVariance = MathHelper.TwoPi / numSplits;
            Vector2 velocity = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.TwoPi);
            for (int i = 0; i < numSplits; i++)
            {
                velocity = velocity.RotatedBy(angleVariance);
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity
                        , splitID, damage, 1.5f, Main.myPlayer);
                }
            }
        }
        /// <summary>使用中心点加 15 像素邻近距离的碰撞检测，匹配较小侧弹的视觉尺寸。</summary>
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CollisionUtil.CircleIntersectsRectangle(Projectile.Center, targetHitbox, 15f);
        }
    }
}
