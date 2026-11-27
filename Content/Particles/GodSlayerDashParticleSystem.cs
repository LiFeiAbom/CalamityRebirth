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
    /// 弑神者冲刺粒子系统：管理双色火花、龙颚光片和定向脉冲环。
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    internal sealed class GodSlayerDashParticleSystem : ModSystem
    {
        /// <summary>粒子硬上限，防止极端战斗在同一帧积累过多绘制对象。</summary>
        private const int ParticleLimit = 5000;
        /// <summary>当前活动粒子列表，仅客户端存在。</summary>
        private static List<GodSlayerDashParticle> particles;
        /// <summary>冲刺火花使用的星形纹理。</summary>
        private static Asset<Texture2D> sparkTexture;
        /// <summary>弑神者冲刺使用的龙颚光片纹理。</summary>
        private static Asset<Texture2D> jawsTexture;
        /// <summary>定向脉冲环使用的空心圆纹理。</summary>
        private static Asset<Texture2D> directionalRingTexture;
        /// <summary>初始化纹理，并在世界绘制结束后注册统一粒子绘制。</summary>
        public override void Load()
        {
            particles = [];
            sparkTexture = ModContent.Request<Texture2D>("CalamityRebirth/Assets/Particles/StarProj");
            jawsTexture = ModContent.Request<Texture2D>("CalamityRebirth/Assets/Particles/Jaws");
            directionalRingTexture = ModContent.Request<Texture2D>(
                "CalamityRebirth/Content/Particles/HollowCircleHardEdge");
            On_Main.DrawInfernoRings += DrawParticlesAfterEverything;
        }
        /// <summary>注销绘制钩子并释放静态资源引用。</summary>
        public override void Unload()
        {
            On_Main.DrawInfernoRings -= DrawParticlesAfterEverything;
            particles = null;
            sparkTexture = null;
            jawsTexture = null;
            directionalRingTexture = null;
        }
        /// <summary>切换世界时清除旧世界粒子，避免坐标和战斗对象跨场景残留。</summary>
        public override void OnWorldUnload() => particles?.Clear();
        /// <summary>
        /// 每帧先积分位置、累计寿命并执行粒子自身更新，最后一次性移除过期对象。
        /// </summary>
        public override void PostUpdateEverything()
        {
            if (Main.dedServ || particles == null)
                return;
            foreach (GodSlayerDashParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();
            }
            particles.RemoveAll(particle => particle.Expired);
        }
        /// <summary>生成弑神者冲刺使用的双色长条火花。</summary>
        public static void SpawnGodSlayerDashSpark(Vector2 position, Vector2 velocity, Color color
            , int lifetime, float scale)
        {
            if (CanSpawn())
                particles.Add(new GodSlayerDashSparkParticle(position, velocity, color, lifetime, scale));
        }
        /// <summary>生成带自行速度的龙颚光片；颜色、压扁比例、朝向和首尾缩放由调用方决定。</summary>
        public static void SpawnJaws(Vector2 position, Vector2 velocity, Color color, Vector2 squish
            , float rotation, float originalScale, float finalScale, int lifetime)
        {
            if (CanSpawn())
                particles.Add(new GodSlayerDashJawsParticle(position, velocity, color, squish, rotation
                    , originalScale, finalScale, lifetime));
        }
        /// <summary>生成带速度、朝向和椭圆压扁的定向脉冲环。</summary>
        public static void SpawnDirectionalPulse(Vector2 position, Vector2 velocity, Color color
            , Vector2 squish, float rotation, float originalScale, float finalScale, int lifetime)
        {
            if (CanSpawn())
                particles.Add(new GodSlayerDashPulseParticle(position, velocity, color, squish
                    , rotation, originalScale, finalScale, lifetime));
        }
        /// <summary>判断当前客户端是否允许再创建粒子。</summary>
        private static bool CanSpawn()
        {
            return !Main.gamePaused && !Main.dedServ && particles != null
                && particles.Count < ParticleLimit;
        }
        /// <summary>
        /// 先执行原版 AfterEverything 绘制，再接管批次以加法混合统一绘制所有冲刺粒子。
        /// 开启屏幕裁剪并使用 LinearClamp，确保脉冲扩张和屏幕边缘表现平滑。
        /// </summary>
        private static void DrawParticlesAfterEverything(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            if (Main.dedServ || particles == null || particles.Count == 0)
                return;
            RasterizerState scissorRasterizer = Main.Rasterizer;
            scissorRasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0
                , Main.screenWidth, Main.screenHeight);
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive
                    , SamplerState.LinearClamp, DepthStencilState.None, scissorRasterizer, null
                    , Main.GameViewMatrix.TransformationMatrix);
                foreach (GodSlayerDashParticle particle in particles)
                {
                    // 粒子类型决定纹理，避免每个对象额外保存纹理引用。
                    Texture2D texture = particle switch
                    {
                        GodSlayerDashPulseParticle => directionalRingTexture.Value,
                        GodSlayerDashJawsParticle => jawsTexture.Value,
                        _ => sparkTexture.Value
                    };
                    particle.Draw(Main.spriteBatch, texture);
                }
                Main.spriteBatch.End();
            }
        }
    }
}
