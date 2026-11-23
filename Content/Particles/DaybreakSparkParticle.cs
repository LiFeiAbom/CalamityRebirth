using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
namespace CalamityRebirth.Content.Particles
{
    /// <summary>
    /// 破晓II 的火花粒子：完整复刻灾厄现代版 MiracleBlight 使用的 GlowSparkParticle。
    /// 仅将原版四色随机配色替换为燃烧橙红色阶。
    /// </summary>
    internal sealed class DaybreakSparkParticle
    {
        // 当前颜色会随寿命淡出；InitialColor 保留生成时颜色供 Lerp 使用。
        public Color Color;
        public Color InitialColor;
        // 原版 MiracleBlight 的火花固定存活 4 帧，Scale 固定从 0.008 开始。
        public int Lifetime;
        public Vector2 Position;
        public float Rotation;
        public float Scale;
        // 横向压扁、纵向拉长，形成 GlowSpark 的细长发光形态。
        public Vector2 Squash;
        public int Time;
        /// <summary>归一到 0 到 1 的寿命进度，用于三次方淡出。</summary>
        public float LifetimeCompletion => Lifetime > 0 ? Time / (float)Lifetime : 1f;
        /// <summary>
        /// 按灾厄 GlowSparkParticle 的默认参数创建粒子：
        /// 初始缩放 0.008、寿命 4 帧、窄长比例，并根据速度自动朝向。
        /// </summary>
        public DaybreakSparkParticle(Vector2 position, Vector2 velocity, Color color)
        {
            Position = position;
            Velocity = velocity;
            Scale = 0.008f;
            Lifetime = 4;
            Color = InitialColor = color;
            Squash = new Vector2(0.6f, 1.2f);
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
        /// <summary>每帧速度，粒子系统在调用 Update 前先积分到 Position。</summary>
        public Vector2 Velocity;
        // 缩小、按寿命三次方淡出、减速并朝向速度方向。
        public void Update()
        {
            Scale *= 0.95f;
            Color = Color.Lerp(InitialColor, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D));
            Velocity *= 0.95f;
            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
        // 第一层画彩色主体，第二层画更细的白色高光；绘制批次由 DaybreakParticleSystem 设为加法混合。
        /// <summary>绘制彩色主体与窄白色高光，两层叠加形成高亮火花核心。</summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Vector2 scale = Squash * Scale;
            float fade = (float)Math.Pow(LifetimeCompletion, 3D);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation
                , texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.Lerp(Color.White, Color.Transparent, fade)
                , Rotation, texture.Size() * 0.5f, scale * new Vector2(0.45f, 1f), SpriteEffects.None, 0f);
        }
    }
}
