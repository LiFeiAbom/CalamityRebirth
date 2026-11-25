using CalamityRebirth.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Particles
{
    /// <summary>魔影灼燃的紫色火簇，依附 NPC 底部并按剩余减益时间持续生成。</summary>
    internal sealed class DemonshadeBurnParticleSet
    {
        /// <summary>紫色亮焰。</summary>
        public static readonly Color BrightColor = new(205, 112, 255);
        /// <summary>深紫暗焰。</summary>
        public static readonly Color DarkColor = new(70, 18, 112);
        /// <summary>单颗火焰寿命。</summary>
        private const int ParticleLifetime = 50;
        /// <summary>当前粒子。</summary>
        private readonly List<DemonshadeBurnFlame> particles = [];
        /// <summary>火簇横向生成宽度。</summary>
        private readonly float compactness;
        /// <summary>火簇纵向强度。</summary>
        private readonly float power;
        /// <summary>火簇已经播放的帧数。</summary>
        public int LocalTimer { get; private set; }
        /// <summary>火簇计划结束的帧数，可随减益续时同步延长。</summary>
        public int SetLifetime { get; set; }
        /// <summary>火簇是否已经达到寿命上限。</summary>
        public bool Finished => SetLifetime > 0 && LocalTimer >= SetLifetime;
        /// <summary>按 NPC 宽度和高度建立火簇。</summary>
        public DemonshadeBurnParticleSet(int lifetime, float compactness, float power)
        {
            SetLifetime = lifetime;
            this.compactness = Math.Max(10f, compactness);
            this.power = MathHelper.Clamp(power, 0.35f, 2.75f);
        }
        /// <summary>生成新火焰、推进全部粒子并移除过期项。</summary>
        public void Update()
        {
            bool closeToDeath = LocalTimer >= SetLifetime - ParticleLifetime && SetLifetime > 0;
            if (!closeToDeath)
            {
                Vector2 offset = Main.rand.NextVector2Circular(1f, 1f) * compactness;
                particles.Add(new DemonshadeBurnFlame(offset, power));
            }
            foreach (DemonshadeBurnFlame particle in particles)
                particle.Update();
            particles.RemoveAll(particle => particle.Time >= ParticleLifetime);
            LocalTimer++;
        }
        /// <summary>在 NPC 底部绘制全部紫色火焰。</summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 basePosition)
        {
            if (Main.dedServ)
                return;
            Texture2D texture = ModContent.Request<Texture2D>(
                "CalamityRebirth/Assets/Particles/DemonshadeBurnFire").Value;
            foreach (DemonshadeBurnFlame particle in particles)
                particle.Draw(spriteBatch, texture, basePosition);
        }
        /// <summary>单颗紫色火焰的上升、扩张和淡出状态。</summary>
        private sealed class DemonshadeBurnFlame
        {
            /// <summary>相对 NPC 底部的当前位置。</summary>
            private Vector2 relativePosition;
            /// <summary>当前帧号。</summary>
            public int Time { get; private set; }
            /// <summary>上升强度。</summary>
            private readonly float power;
            /// <summary>当前缩放。</summary>
            private float scale = 0.06f;
            /// <summary>贴图的三帧变体。</summary>
            private readonly int variant = Main.rand.Next(3);
            /// <summary>建立一颗从小范围随机偏移开始上升的火焰。</summary>
            public DemonshadeBurnFlame(Vector2 relativePosition, float power)
            {
                this.relativePosition = relativePosition;
                this.power = power;
            }
            /// <summary>推进上升、扩张和颜色插值。</summary>
            public void Update()
            {
                Time++;
                scale += power * 0.01f;
                relativePosition.Y -= power * 3f;
            }
            /// <summary>按火焰贴图的三帧结构绘制。</summary>
            public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 basePosition)
            {
                float progress = MathHelper.Clamp(Time / (float)ParticleLifetime, 0f, 1f);
                Color color = Color.Lerp(BrightColor, DarkColor, progress);
                float flash = Utils.GetLerpValue(0.1f, 0.25f, progress, true)
                    * Utils.GetLerpValue(0.4f, 0.25f, progress, true) * 0.65f;
                color = Color.Lerp(color, Color.White, flash);
                float fade = Utils.GetLerpValue(0f, 0.15f, progress, true)
                    * Utils.GetLerpValue(1f, 0.8f, progress, true) * 0.6f;
                color *= fade;
                color.A = 50;
                Rectangle frame = texture.Frame(1, 3, 0, variant);
                Vector2 drawPosition = basePosition - Main.screenPosition + relativePosition;
                spriteBatch.Draw(texture, drawPosition, frame, color, 0f
                    , frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
        }
    }
    /// <summary>魔影灼燃的黑芯紫焰火花，统一在客户端世界层绘制。</summary>
    [Autoload(Side = ModSide.Client)]
    internal sealed class DemonshadeBurnSparkSystem : ModSystem
    {
        /// <summary>同时存在的火花上限。</summary>
        private const int SparkLimit = 600;
        /// <summary>当前火花集合。</summary>
        private static List<DemonshadeBurnSpark> sparks;
        /// <summary>紫色外辉贴图。</summary>
        private static Texture2D glowTexture;
        /// <summary>黑色内芯贴图。</summary>
        private static Texture2D coreTexture;
        /// <summary>加载贴图并挂接世界绘制批次。</summary>
        public override void Load()
        {
            sparks = [];
            glowTexture = ModContent.Request<Texture2D>(
                "CalamityRebirth/Assets/Particles/GlowSpark").Value;
            coreTexture = ModContent.Request<Texture2D>(
                "CalamityRebirth/Assets/Particles/DemonshadeBurnGlowSpark").Value;
            On_Main.DrawInfernoRings += DrawSparks;
        }
        /// <summary>卸载绘制钩子与静态资源。</summary>
        public override void Unload()
        {
            On_Main.DrawInfernoRings -= DrawSparks;
            sparks = null;
            glowTexture = null;
            coreTexture = null;
        }
        /// <summary>切换世界时清除旧火花。</summary>
        public override void OnWorldUnload() => sparks?.Clear();
        /// <summary>逐帧推进并移除过期火花。</summary>
        public override void PostUpdateEverything()
        {
            if (Main.dedServ || sparks == null)
                return;
            foreach (DemonshadeBurnSpark spark in sparks)
                spark.Update();
            sparks.RemoveAll(spark => spark.Finished);
        }
        /// <summary>在指定世界坐标生成一颗黑芯紫焰火花。</summary>
        public static void Spawn(Vector2 position, Vector2 velocity, Color color
            , int lifetime, float scale)
        {
            if (Main.dedServ || sparks == null || sparks.Count >= SparkLimit)
                return;
            sparks.Add(new DemonshadeBurnSpark(position, velocity, color, lifetime, scale));
        }
        /// <summary>在游戏世界绘制全部火花。</summary>
        private static void DrawSparks(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            if (Main.dedServ || sparks == null || sparks.Count == 0)
                return;
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend
                    , SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone
                    , null, Main.GameViewMatrix.TransformationMatrix);
                foreach (DemonshadeBurnSpark spark in sparks)
                    spark.Draw(Main.spriteBatch, glowTexture, coreTexture);
                Main.spriteBatch.End();
            }
        }
        /// <summary>单颗黑色内芯、紫色外辉的上飘火花。</summary>
        private sealed class DemonshadeBurnSpark
        {
            /// <summary>世界坐标。</summary>
            private Vector2 position;
            /// <summary>当前速度。</summary>
            private Vector2 velocity;
            /// <summary>初始颜色。</summary>
            private readonly Color initialColor;
            /// <summary>剩余寿命。</summary>
            private int time;
            /// <summary>总寿命。</summary>
            private readonly int lifetime;
            /// <summary>当前缩放。</summary>
            private float scale;
            /// <summary>纵向拉伸倍率。</summary>
            private float yLength = 1f;
            /// <summary>横向收缩倍率。</summary>
            private float xLength = 0.6f;
            /// <summary>当前旋转。</summary>
            private float rotation;
            /// <summary>是否已经结束。</summary>
            public bool Finished => time >= lifetime;
            /// <summary>建立一颗由指定速度驱动的火花。</summary>
            public DemonshadeBurnSpark(Vector2 position, Vector2 velocity, Color color
                , int lifetime, float scale)
            {
                this.position = position;
                this.velocity = velocity;
                this.lifetime = lifetime;
                this.scale = scale * 0.357f;
                initialColor = color;
            }
            /// <summary>推进缩放、颜色、速度、拉伸与旋转。</summary>
            public void Update()
            {
                time++;
                scale *= 0.9f;
                velocity *= 0.95f;
                yLength *= 1.25f;
                xLength *= 0.7f;
                position += velocity;
                rotation = velocity.ToRotation() + MathHelper.PiOver2;
            }
            /// <summary>先绘制紫色外辉，再用近黑紫色绘制细长内芯。</summary>
            public void Draw(SpriteBatch spriteBatch, Texture2D glow, Texture2D core)
            {
                float progress = MathHelper.Clamp(time / (float)lifetime, 0f, 1f);
                Color color = Color.Lerp(initialColor, Color.Transparent
                    , MathF.Pow(progress, 3f));
                Vector2 drawScale = new Vector2(xLength, yLength) * scale;
                Vector2 drawPosition = position - Main.screenPosition;
                spriteBatch.Draw(glow, drawPosition, null, color with { A = 0 }
                    , rotation, glow.Size() * 0.5f, drawScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(core, drawPosition, null, new Color(18, 0, 30)
                    , rotation, core.Size() * 0.5f, drawScale * new Vector2(0.75f, 0.9f)
                    , SpriteEffects.None, 0f);
            }
        }
    }
}
