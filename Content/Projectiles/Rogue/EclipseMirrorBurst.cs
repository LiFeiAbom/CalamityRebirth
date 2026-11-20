using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Rogue
{
    /// <summary>蚀日魔镜闪避时释放的暗日强光。</summary>
    internal sealed class EclipseMirrorBurst : ModProjectile
    {
        /// <summary>4 帧动画计数。</summary>
        private int frameCounter;
        /// <summary>当前动画横向帧。</summary>
        private int frameX;
        /// <summary>当前动画纵向帧。</summary>
        private int frameY;
        /// <summary>强光拥有巨大判定框和 5 帧局部无敌冷却。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 752;
            Projectile.height = 752;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.DamageType = DamageClass.Generic;
        }
        /// <summary>按 2×2 图集顺序播放四帧，播放完毕立即销毁。</summary>
        public override void AI()
        {
            frameCounter++;
            if (frameCounter > 3)
            {
                frameCounter = 0;
                frameY++;
                if (frameY > 1)
                {
                    frameX++;
                    frameY = 0;
                }
            }
            if (frameX > 0 && frameY > 0)
                Projectile.Kill();
        }
        /// <summary>从 2×2 图集中绘制当前帧。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition
                , new Rectangle(frameX * 752, frameY * 752, 752, 752), Color.White
                , Projectile.rotation, Projectile.Size / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
