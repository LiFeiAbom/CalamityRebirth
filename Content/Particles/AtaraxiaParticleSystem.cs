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
    /// 禅心剑粒子系统：分别管理星形火花、GlowSparkCal 爆发火花和 FlameExplosion 脉冲。
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    internal sealed class AtaraxiaParticleSystem : ModSystem
    {
        /// <summary>粒子硬上限，防止极端战斗在同一帧积累过多绘制对象。</summary>
        private const int ParticleLimit = 5000;
        /// <summary>当前活动粒子列表，仅客户端存在。</summary>
        private static List<AtaraxiaParticle> particles;
        /// <summary>禅心剑常规星形火花纹理。</summary>
        private static Asset<Texture2D> starTexture;
        /// <summary>快速压缩爆发使用的高亮火花纹理。</summary>
        private static Asset<Texture2D> glowSparkTexture;
        /// <summary>主弹死亡脉冲使用的火焰爆炸纹理。</summary>
        private static Asset<Texture2D> flameTexture;
        /// <summary>初始化纹理，并在世界绘制结束后注册统一粒子绘制。</summary>
        public override void Load()
        {
            particles = [];
            starTexture = ModContent.Request<Texture2D>("CalamityRebirth/Assets/Particles/StarProj");
            glowSparkTexture = ModContent.Request<Texture2D>("CalamityRebirth/Content/Particles/GlowSparkCal");
            flameTexture = ModContent.Request<Texture2D>("CalamityRebirth/Content/Particles/FlameExplosion");
            On_Main.DrawInfernoRings += DrawParticlesAfterEverything;
        }
        /// <summary>注销绘制钩子并释放静态资源引用。</summary>
        public override void Unload()
        {
            On_Main.DrawInfernoRings -= DrawParticlesAfterEverything;
            particles = null;
            starTexture = null;
            glowSparkTexture = null;
            flameTexture = null;
        }
        /// <summary>切换世界时清除旧世界粒子，避免坐标和战斗对象跨场景残留。</summary>
        public override void OnWorldUnload() => particles?.Clear();
        /// <summary>
        /// 每帧先积分位置、累计寿命并执行粒子自身更新，最后一次性移除过期对象。
        /// 统一逆序遍历并非必要，因为 RemoveAll 在遍历完成后才执行。
        /// </summary>
        public override void PostUpdateEverything()
        {
            if (Main.dedServ || particles == null)
                return;
            foreach (AtaraxiaParticle particle in particles)
            {
                particle.Position += particle.Velocity;
                particle.Time++;
                particle.Update();
            }
            particles.RemoveAll(particle => particle.Expired);
        }
        /// <summary>生成常规星形拖尾火花；重力开关用于主弹死亡后的下坠效果。</summary>
        public static void SpawnStarSpark(Vector2 position, Vector2 velocity, Color color, int lifetime
            , float scale, bool affectedByGravity = false)
        {
            SpawnSpark(position, velocity, color, lifetime, scale, affectedByGravity, false, 1f);
        }
        /// <summary>生成短寿命 GlowSpark 爆发火花，固定使用高亮压扁纹理。</summary>
        public static void SpawnGlowBurst(Vector2 position, Vector2 velocity, Color color)
        {
            SpawnSpark(position, velocity, color, 11, Main.rand.NextFloat(0.015f, 0.025f), false, true, 1f);
        }
        /// <summary>
        /// 统一创建火花粒子，并在达到上限或游戏暂停时安全跳过。
        /// 常规火光采用窄长比例，爆发火光采用更宽的快速压缩比例。
        /// </summary>
        private static void SpawnSpark(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale
            , bool affectedByGravity, bool quickShrink, float shrinkSpeed)
        {
            if (CanSpawn())
            {
                particles.Add(new AtaraxiaSparkParticle(position, velocity, color, lifetime, scale
                    , quickShrink ? new Vector2(2.2f, 0.9f) : new Vector2(0.5f, 1.6f)
                    , affectedByGravity, quickShrink, true, shrinkSpeed));
            }
        }
        /// <summary>生成从极小尺寸扩张到指定尺寸的火焰脉冲。</summary>
        public static void SpawnPulse(Vector2 position, Color color, float rotation, float finalScale, int lifetime)
        {
            if (CanSpawn())
                particles.Add(new AtaraxiaPulseParticle(position, color * 0.6f, Vector2.One, rotation, 0.07f, finalScale, lifetime));
        }
        /// <summary>判断当前客户端是否允许再创建粒子。</summary>
        private static bool CanSpawn()
        {
            return !Main.gamePaused && !Main.dedServ && particles != null && particles.Count < ParticleLimit;
        }
        /// <summary>
        /// 先执行原版 AfterEverything 绘制，再接管批次以加法混合统一绘制所有粒子。
        /// 开启屏幕裁剪并把采样设为 LinearClamp，确保脉冲扩张和屏幕边缘表现平滑。
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
                foreach (AtaraxiaParticle particle in particles)
                {
                    // 粒子类型及 QuickShrink 标记共同决定最终纹理，避免每个对象额外保存纹理引用。
                    Texture2D texture = particle is AtaraxiaPulseParticle ? flameTexture.Value
                        : particle is AtaraxiaSparkParticle spark && spark.QuickShrink ? glowSparkTexture.Value
                        : starTexture.Value;
                    particle.Draw(Main.spriteBatch, texture);
                }
                Main.spriteBatch.End();
            }
        }
    }
}
