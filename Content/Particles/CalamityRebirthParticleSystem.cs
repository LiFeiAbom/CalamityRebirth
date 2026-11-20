using CalamityRebirth.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Particles
{
    /// <summary>
    /// 本工程元素混合II的客户端粒子系统。
    /// 在世界绘制结束后以一次加法批次还原原版的红绿蓝色差方条。
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    internal sealed class CalamityRebirthParticleSystem : ModSystem
    {
        /// <summary>粒子数量上限，避免元素之握同时命中大量目标时无限增加绘制负担。</summary>
        private const int ParticleLimit = 4200;
        /// <summary>当前活动的元素混合II粒子，仅客户端存在。</summary>
        private static List<TechyHoloysquareParticle> particles;
        /// <summary>从现代灾厄复制而来的六帧全息方条图集。</summary>
        private static Asset<Texture2D> holoTexture;
        /// <summary>加载纹理并挂接世界绘制结束后的粒子批次。</summary>
        public override void Load()
        {
            particles = [];
            holoTexture = ModContent.Request<Texture2D>("CalamityRebirth/Content/Particles/ElementalMixIIHolo");
            On_Main.DrawInfernoRings += DrawParticlesAfterEverything;
        }
        /// <summary>卸载钩子、粒子和纹理引用，保证热重载安全。</summary>
        public override void Unload()
        {
            On_Main.DrawInfernoRings -= DrawParticlesAfterEverything;
            particles = null;
            holoTexture = null;
        }
        /// <summary>切换世界时清空旧粒子，避免坐标和元素颜色残留。</summary>
        public override void OnWorldUnload() => particles?.Clear();
        /// <summary>每帧统一积分位置、累计寿命、更新粒子并移除过期对象。</summary>
        public override void PostUpdateEverything()
        {
            if (Main.dedServ || particles == null)
                return;
            foreach (TechyHoloysquareParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();
            }
            particles.RemoveAll(particle => particle.Time >= particle.Lifetime);
        }
        /// <summary>在当前客户端和容量允许时生成一枚原版全息方条。</summary>
        public static void SpawnParticle(Vector2 position, Vector2 velocity, Color color, float scale, int lifetime)
        {
            if (Main.gamePaused || Main.dedServ || particles == null || particles.Count >= ParticleLimit)
                return;
            particles.Add(new TechyHoloysquareParticle(position, velocity, scale, color, lifetime));
        }
        /// <summary>
        /// 先执行被拦截的原版绘制，再保存并开启屏幕裁剪，以线性采样和加法混合统一绘制全息粒子。
        /// </summary>
        private static void DrawParticlesAfterEverything(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            if (Main.dedServ || particles == null || particles.Count == 0)
                return;
            RasterizerState scissorRasterizer = Main.Rasterizer;
            scissorRasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp
                    , DepthStencilState.None, scissorRasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (TechyHoloysquareParticle particle in particles)
                    particle.Draw(Main.spriteBatch, holoTexture.Value);
                Main.spriteBatch.End();
            }
        }
    }
}
