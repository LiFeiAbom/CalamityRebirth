using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
namespace CalamityRebirth.Content.Particles
{
    /// <summary>
    /// 禅心剑专用轻量粒子基类，仅保存位置、速度、寿命和颜色。
    /// </summary>
    internal abstract class AtaraxiaParticle
    {
        /// <summary>世界坐标位置，粒子系统每帧先叠加 Velocity 再调用 Update。</summary>
        public Vector2 Position;
        /// <summary>当前帧位移速度；具体粒子可在 Update 中施加阻力或重力。</summary>
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
    /// 复刻 DRK_Spark 与 GlowSparkCal 的共同规则：缩放衰减、三次方淡出、减速、可选重力与快速挤压。
    /// </summary>
    internal sealed class AtaraxiaSparkParticle : AtaraxiaParticle
    {
        /// <summary>生成时颜色，淡出插值始终以该颜色为起点。</summary>
        public Color InitialColor;
        /// <summary>整体缩放，每帧按固定比例衰减。</summary>
        public float Scale;
        /// <summary>贴图的横向和纵向压缩系数，用于形成细长火花。</summary>
        public Vector2 Squash;
        /// <summary>低速时是否开始受重力影响并逐渐下坠。</summary>
        public bool AffectedByGravity;
        /// <summary>是否快速拉长并压扁，通常用于 GlowSpark 爆发火花。</summary>
        public bool QuickShrink;
        /// <summary>是否额外绘制一层更窄的白色高光。</summary>
        public bool Glowing;
        /// <summary>快速压缩比例，1 表示原版默认强度。</summary>
        public float ShrinkSpeed;
        /// <summary>归一到 0 到 1 的生命进度，零寿命时按已完成处理。</summary>
        public float LifetimeCompletion => Lifetime > 0 ? Time / (float)Lifetime : 1f;
        /// <summary>
        /// 创建通用星形火花，并根据初始速度确定朝向。
        /// 各类参数由粒子系统工厂方法提供，粒子本身只负责行为。
        /// </summary>
        public AtaraxiaSparkParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale
            , Vector2 squash, bool affectedByGravity = false, bool quickShrink = false, bool glowing = true
            , float shrinkSpeed = 1f)
        {
            Position = position;
            Velocity = velocity;
            Color = InitialColor = color;
            Lifetime = lifetime;
            Scale = scale;
            Squash = squash;
            AffectedByGravity = affectedByGravity;
            QuickShrink = quickShrink;
            Glowing = glowing;
            ShrinkSpeed = shrinkSpeed;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
        /// <summary>
        /// 每帧缩小、减速并按三次方曲线淡出。
        /// 低速且有重力时增加纵向速度并增强横向阻力，模拟火花逐渐失去动量。
        /// </summary>
        public override void Update()
        {
            Scale *= 0.95f;
            Color = Color.Lerp(InitialColor, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D));
            Velocity *= 0.95f;
            if (QuickShrink)
            {
                if (ShrinkSpeed == 1f)
                {
                    Squash.X *= 0.8f;
                    Squash.Y *= 1.2f;
                }
                else
                {
                    Squash.X *= 1f - 0.2f * ShrinkSpeed;
                    Squash.Y *= 1f + 0.2f * ShrinkSpeed;
                }
            }
            if (Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
        /// <summary>绘制主色火花，可选叠加中心白色高光形成发光核心。</summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Vector2 scale = Squash * Scale;
            float fade = (float)Math.Pow(LifetimeCompletion, 3D);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation
                , texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            if (Glowing)
            {
                spriteBatch.Draw(texture, Position - Main.screenPosition, null
                    , Color.Lerp(Color.White, Color.Transparent, fade), Rotation, texture.Size() * 0.5f
                    , scale * new Vector2(0.45f, 1f), SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation
                    , texture.Size() * 0.5f, scale * new Vector2(0.45f, 1f)
                    , SpriteEffects.None, 0f);
            }
        }
    }
    /// <summary>
    /// 禅心剑主弹死亡时使用的 FlameExplosion 扩散脉冲，复刻 CustomPulse 的 PolyOut(4) 缩放和 sin 淡出。
    /// </summary>
    internal sealed class AtaraxiaPulseParticle : AtaraxiaParticle
    {
        /// <summary>脉冲生成时的基础色，颜色最终会按不透明度乘算。</summary>
        private readonly Color baseColor;
        /// <summary>扩张开始时的初始缩放。</summary>
        private readonly float originalScale;
        /// <summary>扩张结束时的目标缩放。</summary>
        private readonly float finalScale;
        /// <summary>贴图的椭圆压缩比例，使环形扩散不完全呈正圆。</summary>
        private readonly Vector2 squish;
        /// <summary>当前透明度，由正弦曲线控制先亮后暗。</summary>
        private float opacity;
        /// <summary>当前绘制缩放。</summary>
        public float Scale;
        /// <summary>归一到 0 到 1 的生命进度。</summary>
        public float LifetimeCompletion => Lifetime > 0 ? Time / (float)Lifetime : 1f;
        /// <summary>创建从初始尺寸扩张到目标尺寸的火焰脉冲。</summary>
        public AtaraxiaPulseParticle(Vector2 position, Color color, Vector2 squish, float rotation
            , float originalScale, float finalScale, int lifetime)
        {
            Position = position;
            baseColor = color;
            this.squish = squish;
            Rotation = rotation;
            this.originalScale = originalScale;
            this.finalScale = finalScale;
            Scale = originalScale;
            Lifetime = lifetime;
        }
        /// <summary>
        /// 使用 PolyOut(4) 曲线加速扩张，并用正弦曲线降低末端透明度；
        /// 每次扩张同时给周围环境添加与颜色一致的动态光。
        /// </summary>
        public override void Update()
        {
            float progress = 1f - (float)Math.Pow(1f - LifetimeCompletion, 4f);
            Scale = MathHelper.Lerp(originalScale, finalScale, progress);
            opacity = (float)Math.Sin(MathHelper.PiOver2 + LifetimeCompletion * MathHelper.PiOver2);
            Color = baseColor * opacity;
            Lighting.AddLight(Position, Color.R / 255f, Color.G / 255f, Color.B / 255f);
        }
        /// <summary>按椭圆比例绘制已经扩张的火焰脉冲纹理。</summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * opacity, Rotation
                , texture.Size() * 0.5f, Scale * squish, SpriteEffects.None, 0f);
        }
    }
    /// <summary>沿速度方向拉伸的龙颚光片，按四次方缓动改变缩放并按正弦曲线淡出。</summary>
    internal sealed class AtaraxiaJawsParticle : AtaraxiaParticle
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
        public AtaraxiaJawsParticle(Vector2 position, Vector2 velocity, Color color, Vector2 squish
            , float rotation, float originalScale, float finalScale, int lifetime)
        {
            Position = position;
            Velocity = velocity;
            initialColor = color;
            Color = color;
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
    /// <summary>带自身速度的定向脉冲环，用于冲刺起步与中段推进。</summary>
    internal sealed class AtaraxiaDirectionalPulseParticle : AtaraxiaParticle
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
        public AtaraxiaDirectionalPulseParticle(Vector2 position, Vector2 velocity, Color color
            , Vector2 squish, float rotation, float originalScale, float finalScale, int lifetime)
        {
            Position = position;
            Velocity = velocity;
            initialColor = color;
            Color = color;
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
        /// <summary>绘制空心定向脉冲环；透明度按参照表现叠加两层。</summary>
        public override void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * opacity, Rotation
                , texture.Size() * 0.5f, scale * squish, SpriteEffects.None, 0f);
        }
    }
}
