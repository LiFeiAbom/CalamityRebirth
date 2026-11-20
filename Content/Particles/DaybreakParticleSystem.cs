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
    /// 破晓II 专用粒子系统：只承载从灾厄 MiracleBlight GlowSparkParticle 还原后的火花，
    /// 在 AfterEverything 阶段以加法混合绘制，不再依赖通用 DRK 粒子框架。
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    internal sealed class DaybreakParticleSystem : ModSystem
    {
        // 上限只用于防止异常刷屏；破晓II正常生成量远低于该值。
        private const int ParticleLimit = 5000;
        /// <summary>当前活动的破晓II火花列表，仅客户端存在。</summary>
        private static List<DaybreakSparkParticle> particles;
        /// <summary>破晓II专用的 GlowSpark 纹理资源。</summary>
        private static Asset<Texture2D> sparkTexture;
        /// <summary>加载纹理并注册世界绘制结束后的粒子渲染。</summary>
        public override void Load()
        {
            particles = [];
            sparkTexture = ModContent.Request<Texture2D>("CalamityRebirth/Assets/Particles/GlowSpark");
            On_Main.DrawInfernoRings += DrawParticlesAfterEverything;
        }
        /// <summary>卸载绘制钩子和静态纹理引用。</summary>
        public override void Unload()
        {
            On_Main.DrawInfernoRings -= DrawParticlesAfterEverything;
            particles = null;
            sparkTexture = null;
        }
        /// <summary>清空旧世界粒子，防止场景切换后继续更新。</summary>
        public override void OnWorldUnload() => particles?.Clear();
        public override void PostUpdateEverything()
        {
            if (Main.dedServ || particles == null)
                return;
            // 与原版 GeneralParticleHandler 相同：先叠加速度、累计寿命，再执行粒子自身更新。
            foreach (DaybreakSparkParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();
            }
            particles.RemoveAll(particle => particle.Time >= particle.Lifetime);
        }
        /// <summary>在客户端和粒子容量允许时创建一枚破晓II火花。</summary>
        public static void SpawnParticle(Vector2 position, Vector2 velocity, Color color)
        {
            // Dust 和粒子的生成都只发生在客户端，服务器只负责减益状态同步。
            if (Main.gamePaused || Main.dedServ || particles == null || particles.Count >= ParticleLimit)
                return;
            particles.Add(new DaybreakSparkParticle(position, velocity, color));
        }
        /// <summary>在世界与减益绘制结束后，以加法混合统一绘制所有破晓II火花。</summary>
        private static void DrawParticlesAfterEverything(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            // 原版粒子在 AfterEverything 层绑定：先完成世界绘制，再单独以加法混合画粒子。
            orig(self);
            if (Main.dedServ || particles == null || particles.Count == 0)
                return;
            // 保留原版的 scissor、LinearClamp、GameViewMatrix 与加法混合参数。
            RasterizerState scissorRasterizer = Main.Rasterizer;
            scissorRasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp
                    , DepthStencilState.None, scissorRasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (DaybreakSparkParticle particle in particles)
                    particle.Draw(Main.spriteBatch, sparkTexture.Value);
                Main.spriteBatch.End();
            }
        }
    }
}
