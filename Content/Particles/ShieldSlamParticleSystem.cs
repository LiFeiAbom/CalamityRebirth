using CalamityRebirth.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Particles
{
    /// <summary>
    /// 盾牌冲刺撞击粒子系统，统一管理扩散圆环与细节爆炸。
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    internal sealed class ShieldSlamParticleSystem : ModSystem
    {
        /// <summary>同时存在的粒子上限。</summary>
        private const int ParticleLimit = 1600;
        /// <summary>当前活动的客户端粒子。</summary>
        private static List<ShieldSlamParticle> particles;
        /// <summary>细节爆炸贴图。</summary>
        private static Asset<Texture2D> explosionTexture;
        /// <summary>硬边空心圆贴图。</summary>
        private static Asset<Texture2D> ringTexture;
        /// <summary>加载贴图并挂接统一绘制批次。</summary>
        public override void Load()
        {
            particles = [];
            explosionTexture = ModContent.Request<Texture2D>("CalamityRebirth/Content/Particles/DetailedExplosion");
            ringTexture = ModContent.Request<Texture2D>("CalamityRebirth/Content/Particles/HollowCircleHardEdge");
            On_Main.DrawInfernoRings += DrawParticles;
        }
        /// <summary>卸载绘制钩子并清空静态资源。</summary>
        public override void Unload()
        {
            On_Main.DrawInfernoRings -= DrawParticles;
            particles = null;
            explosionTexture = null;
            ringTexture = null;
        }
        /// <summary>切换世界时清空旧粒子。</summary>
        public override void OnWorldUnload() => particles?.Clear();
        /// <summary>积分位置、推进寿命并移除过期粒子。</summary>
        public override void PostUpdateEverything()
        {
            if (Main.dedServ || particles == null)
                return;
            foreach (ShieldSlamParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();
            }
            particles.RemoveAll(particle => particle.Time >= particle.Lifetime);
        }
        /// <summary>生成定向扩散圆环。</summary>
        public static void SpawnRing(Vector2 position, Vector2 squish, float rotation
            , float startScale, float finalScale, int lifetime)
        {
            SpawnRing(position, squish, rotation, startScale, finalScale, lifetime, Color.Aqua);
        }
        /// <summary>生成可指定颜色的定向扩散圆环，供其他冲刺效果复用同一套绘制批次。</summary>
        public static void SpawnRing(Vector2 position, Vector2 squish, float rotation
            , float startScale, float finalScale, int lifetime, Color color)
        {
            Spawn(new ShieldSlamParticle(position, color, squish, rotation, startScale
                , finalScale, lifetime, true));
        }
        /// <summary>生成彩色细节爆炸。</summary>
        public static void SpawnExplosion(Vector2 position, Color color, float rotation
            , float finalScale, int lifetime)
        {
            Spawn(new ShieldSlamParticle(position, color, Vector2.One, rotation, 0f
                , finalScale, lifetime, false));
        }
        /// <summary>在当前客户端和容量允许时加入粒子。</summary>
        private static void Spawn(ShieldSlamParticle particle)
        {
            if (Main.gamePaused || Main.dedServ || particles == null || particles.Count >= ParticleLimit)
                return;
            particles.Add(particle);
        }
        /// <summary>执行原版绘制后，以加法混合统一绘制全部撞击粒子。</summary>
        private static void DrawParticles(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            if (Main.dedServ || particles == null || particles.Count == 0)
                return;
            RasterizerState rasterizer = Main.Rasterizer;
            rasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp
                    , DepthStencilState.None, rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (ShieldSlamParticle particle in particles)
                    particle.Draw(Main.spriteBatch, particle.IsRing ? ringTexture.Value : explosionTexture.Value);
                Main.spriteBatch.End();
            }
        }
    }
    /// <summary>
    /// 单个盾牌撞击粒子，按四次幂缓动扩张并沿正弦曲线衰减透明度。
    /// </summary>
    internal sealed class ShieldSlamParticle
    {
        /// <summary>世界坐标位置。</summary>
        public Vector2 Position;
        /// <summary>每帧速度。</summary>
        public Vector2 Velocity;
        /// <summary>总寿命。</summary>
        public readonly int Lifetime;
        /// <summary>当前寿命。</summary>
        public int Time;
        /// <summary>是否使用圆环贴图。</summary>
        public readonly bool IsRing;
        /// <summary>基础颜色。</summary>
        private readonly Color baseColor;
        /// <summary>椭圆压缩比例。</summary>
        private readonly Vector2 squish;
        /// <summary>旋转角度。</summary>
        private readonly float rotation;
        /// <summary>初始缩放。</summary>
        private readonly float startScale;
        /// <summary>最终缩放。</summary>
        private readonly float finalScale;
        /// <summary>当前绘制缩放。</summary>
        private float scale;
        /// <summary>当前透明度。</summary>
        private float opacity;
        /// <summary>归一到零到一的寿命进度。</summary>
        private float Progress => Lifetime > 0 ? MathHelper.Clamp(Time / (float)Lifetime, 0f, 1f) : 1f;
        /// <summary>构造一枚具有固定扩张曲线的粒子。</summary>
        public ShieldSlamParticle(Vector2 position, Color color, Vector2 squish, float rotation
            , float startScale, float finalScale, int lifetime, bool isRing)
        {
            Position = position;
            baseColor = color;
            this.squish = squish;
            this.rotation = rotation;
            this.startScale = startScale;
            this.finalScale = finalScale;
            scale = startScale;
            Velocity = Vector2.Zero;
            Lifetime = lifetime;
            IsRing = isRing;
        }
        /// <summary>使用四次幂扩张曲线更新尺寸、透明度和动态光。</summary>
        public void Update()
        {
            float eased = 1f - MathF.Pow(1f - Progress, 4f);
            scale = MathHelper.Lerp(startScale, finalScale, eased);
            opacity = MathF.Sin(MathHelper.PiOver2 + Progress * MathHelper.PiOver2);
            Color lit = baseColor * opacity;
            Lighting.AddLight(Position, lit.R / 255f, lit.G / 255f, lit.B / 255f);
            Velocity *= 0.95f;
        }
        /// <summary>绘制当前纹理并应用椭圆压缩与旋转。</summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, baseColor * opacity * opacity, rotation
                , texture.Size() * 0.5f, scale * squish, SpriteEffects.None, 0f);
        }
    }
}
