using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
namespace CalamityRebirth.Content.Particles
{
    /// <summary>
    /// 元素混合II使用的全息方条粒子。
    /// 沿用现代灾厄原始 <c>TechyHoloysquareParticle</c> 的六种短方条帧。
    /// 绘制方式、尺寸、反位时间、更新速度和三色色差均与原版元素混合一致。
    /// </summary>
    internal sealed class TechyHoloysquareParticle
    {
        /// <summary>现代灾厄 TechyHolosquare 图集中的六种尺寸帧。</summary>
        private static readonly Rectangle[] Frames =
        [
            new(8, 0, 6, 6),
            new(6, 8, 10, 6),
            new(4, 16, 14, 8),
            new(2, 26, 18, 10),
            new(2, 38, 18, 8),
            new(6, 48, 12, 12)
        ];
        /// <summary>粒子当前位置，由粒子系统统一积分。</summary>
        public Vector2 Position;
        /// <summary>粒子当前速度，更新时会逐步减速。</summary>
        public Vector2 Velocity;
        /// <summary>粒子当前旋转角度，始终对齐运动方向。</summary>
        public float Rotation;
        /// <summary>粒子当前缩放。</summary>
        public float Scale;
        /// <summary>当前寿命帧数。</summary>
        public int Time;
        /// <summary>粒子总寿命。</summary>
        public int Lifetime;
        /// <summary>当前帧的透明度，按原版生命周期公式更新。</summary>
        private float opacity;
        /// <summary>主体元素色。</summary>
        private readonly Color color;
        /// <summary>当前使用的全息方条帧。</summary>
        private readonly Rectangle frame;
        /// <summary>0 到 1 的寿命进度。</summary>
        private float LifetimeCompletion => Lifetime > 0 ? MathHelper.Clamp(Time / (float)Lifetime, 0f, 1f) : 1f;
        /// <summary>按原版透明度、随机初始旋转和随机帧创建一枚全息方条。</summary>
        public TechyHoloysquareParticle(Vector2 position, Vector2 velocity, float scale, Color color, int lifetime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            this.color = color;
            Scale = scale;
            Lifetime = lifetime;
            frame = Frames[Main.rand.Next(Frames.Length)];
            opacity = 1f;
        }
        /// <summary>按原版公式开平方透明、添加灯光、减速、收缩，并让方条朝向运动方向。</summary>
        public void Update()
        {
            opacity = MathF.Sqrt(LifetimeCompletion);
            Lighting.AddLight(Position, color.ToVector3() * opacity);
            Rotation = Velocity.ToRotation();
            Velocity *= 0.875f;
            Scale *= 0.96f;
        }
        /// <summary>
        /// 绘制与现代原版相同的红绿蓝三层色差，不追加白色内芯或双层残影。
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Vector2 screenPosition = Position - Main.screenPosition;
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 aberrationDirection = Vector2.UnitX.RotatedBy(Rotation);
            // 与现代原版相同，分别用红、绿、蓝通道绘制三个紧邻偏移的方条。
            for (int i = -1; i <= 1; i++)
            {
                Color colorMask = i switch
                {
                    -1 => new Color(255, 0, 0),
                    0 => new Color(0, 255, 0),
                    _ => new Color(0, 0, 255)
                };
                Vector2 offset = aberrationDirection.RotatedBy(MathHelper.PiOver2) * i * 1.5f;
                Color drawColor = MultiplyRgb(color, colorMask) * opacity;
                spriteBatch.Draw(texture, screenPosition + offset, frame, drawColor, Rotation
                    , origin, Scale / 2f, SpriteEffects.None, 0f);
            }
        }
        /// <summary>只混合颜色的 RGB 通道，保留方条自身的透明度。</summary>
        private static Color MultiplyRgb(Color source, Color multiplier)
        {
            return new Color(
                source.R * multiplier.R / 255,
                source.G * multiplier.G / 255,
                source.B * multiplier.B / 255,
                source.A);
        }
    }
}
