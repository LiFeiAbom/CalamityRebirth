using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
namespace CalamityRebirth.Content.Particles
{
    /// <summary>
    /// 弑神者冲刺使用的轻量粒子基类，仅保存位置、速度、寿命、颜色和旋转。
    /// </summary>
    internal abstract class GodSlayerDashParticle
    {
        /// <summary>世界坐标位置，粒子系统每帧先叠加 Velocity 再调用 Update。</summary>
        public Vector2 Position;
        /// <summary>当前帧位移速度，具体粒子可在 Update 中施加阻力。</summary>
        public Vector2 Velocity;
        /// <summary>当前绘制颜色，通常随生命进度淡出。</summary>
        public Color Color;
        /// <summary>已经存活的帧数。</summary>
        public int Time;
        /// <summary>粒子总寿命，Time 达到该值后由系统移除。</summary>
        public int Lifetime;
        /// <summary>贴图绕自身中心的绘制旋转角。</summary>
        public float Rotation;
        /// <summary>是否已经超过寿命，供粒子系统批量清理。</summary>
        public bool Expired => Time >= Lifetime;
        /// <summary>更新单个粒子的运动、颜色和尺寸。</summary>
        public abstract void Update();
        /// <summary>使用粒子系统选择的纹理绘制当前粒子。</summary>
        public abstract void Draw(SpriteBatch spriteBatch, Texture2D texture);
    }
    /// <summary>
    /// 沿冲刺速度方向拉伸的双色火花，缩放逐帧衰减并沿三次方曲线淡出。
    /// </summary>
    internal sealed class GodSlayerDashSparkParticle : GodSlayerDashParticle
    {
        /// <summary>生成时颜色，淡出插值始终以该颜色为起点。</summary>
        private readonly Color initialColor;
        /// <summary>贴图的横向和纵向压缩系数，用于形成细长火花。</summary>
        private readonly Vector2 squash;
        /// <summary>整体缩放，每帧按固定比例衰减。</summary>
        private float scale;
        /// <summary>归一到 0 到 1 的生命进度，零寿命时按已完成处理。</summary>
        private float LifetimeCompletion => Lifetime > 0 ? Time / (float)Lifetime : 1f;
        /// <summary>创建沿速度方向拉伸的冲刺火花。</summary>
        public GodSlayerDashSparkParticle(Vector2 position, Vector2 velocity, Color color
            , int lifetime, float scale)
        {
            Position = position;
            Velocity = velocity;
            Color = initialColor = color;
            Lifetime = lifetime;
            this.scale = scale;
            squash = new Vector2(0.5f, 1.6f);
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
        /// <summary>
        /// 每帧缩小、减速并按三次方曲线淡出，同时让粒子持续朝向当前速度。
        /// </summary>
        public override void Update()
        {
            scale *= 0.95f;
            Color = Color.Lerp(initialColor, Color.Transparent
                , MathF.Pow(LifetimeCompletion, 3f));
            Velocity *= 0.95f;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
        /// <summary>绘制主火花，并叠加一层同色窄条以形成冲刺拖尾亮度。</summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Vector2 drawScale = squash * scale;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation
                , texture.Size() * 0.5f, drawScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation
                , texture.Size() * 0.5f, drawScale * new Vector2(0.45f, 1f)
                , SpriteEffects.None, 0f);
        }
    }
    /// <summary>沿速度方向拉伸的龙颚光片，按四次方缓动改变缩放并按正弦曲线淡出。</summary>
    internal sealed class GodSlayerDashJawsParticle : GodSlayerDashParticle
    {
        /// <summary>粒子生成时的保留颜色。</summary>
        private readonly Color initialColor;
        /// <summary>起始缩放。</summary>
        private readonly float originalScale;
        /// <summary>结束缩放。</summary>
        private readonly float finalScale;
        /// <summary>横向与纵向压扁比例。</summary>
        private readonly Vector2 squish;
        /// <summary>当前绘制缩放。</summary>
        private float scale;
        /// <summary>构造龙颚光片并设置位置、速度、颜色、朝向和生命周期。</summary>
        public GodSlayerDashJawsParticle(Vector2 position, Vector2 velocity, Color color
            , Vector2 squish, float rotation, float originalScale, float finalScale, int lifetime)
        {
            Position = position;
            Velocity = velocity;
            Color = initialColor = color;
            this.squish = squish;
            Rotation = rotation;
            this.originalScale = originalScale;
            this.finalScale = finalScale;
            scale = originalScale;
            Lifetime = lifetime;
        }
        /// <summary>逐帧放大、衰减速度并按正弦曲线降低亮度。</summary>
        public override void Update()
        {
            float progress = Lifetime > 0 ? MathHelper.Clamp(Time / (float)Lifetime, 0f, 1f) : 1f;
            float eased = 1f - MathF.Pow(1f - progress, 4f);
            scale = MathHelper.Lerp(originalScale, finalScale, eased);
            float opacity = MathF.Sin(MathHelper.PiOver2 + progress * MathHelper.PiOver2);
            Color = initialColor * opacity;
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
            Velocity *= 0.95f;
        }
        /// <summary>按朝向和椭圆压扁比例绘制龙颚光片。</summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation
                , texture.Size() * 0.5f, scale * squish, SpriteEffects.None, 0f);
        }
    }
    /// <summary>带自身速度的定向脉冲环，用于弑神者冲刺起步与中段推进。</summary>
    internal sealed class GodSlayerDashPulseParticle : GodSlayerDashParticle
    {
        /// <summary>粒子生成时的保留颜色。</summary>
        private readonly Color initialColor;
        /// <summary>起始缩放。</summary>
        private readonly float originalScale;
        /// <summary>结束缩放。</summary>
        private readonly float finalScale;
        /// <summary>横向与纵向压扁比例。</summary>
        private readonly Vector2 squish;
        /// <summary>当前绘制缩放。</summary>
        private float scale;
        /// <summary>当前透明度。</summary>
        private float opacity;
        /// <summary>构造带速度的定向脉冲环。</summary>
        public GodSlayerDashPulseParticle(Vector2 position, Vector2 velocity, Color color
            , Vector2 squish, float rotation, float originalScale, float finalScale, int lifetime)
        {
            Position = position;
            Velocity = velocity;
            Color = initialColor = color;
            this.squish = squish;
            Rotation = rotation;
            this.originalScale = originalScale;
            this.finalScale = finalScale;
            scale = originalScale;
            Lifetime = lifetime;
        }
        /// <summary>按四次方缓动改变缩放，并按正弦曲线淡出和衰减速度。</summary>
        public override void Update()
        {
            float progress = Lifetime > 0 ? MathHelper.Clamp(Time / (float)Lifetime, 0f, 1f) : 1f;
            float eased = 1f - MathF.Pow(1f - progress, 4f);
            scale = MathHelper.Lerp(originalScale, finalScale, eased);
            opacity = MathF.Sin(MathHelper.PiOver2 + progress * MathHelper.PiOver2);
            Color = initialColor * opacity;
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
            Velocity *= 0.95f;
        }
        /// <summary>按缩放、朝向和椭圆压扁比例绘制空心脉冲环。</summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * opacity
                , Rotation, texture.Size() * 0.5f, scale * squish, SpriteEffects.None, 0f);
        }
    }
}
