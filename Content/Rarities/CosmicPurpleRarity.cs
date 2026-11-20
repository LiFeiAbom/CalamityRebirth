using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 现代灾厄 CosmicPurple / 月后暗紫稀有度。
    /// 名称使用暗紫双层字色、水晶背景辉光与确定性星屑；所有绘制都在当前 tooltip 批次完成。
    /// </summary>
    public class CosmicPurpleRarity : ModRarity
    {
        /// <summary>星屑在名称上方展开的最大纵向比例。</summary>
        private const float SparkleMaxY = 4.5f;
        /// <summary>名称背后的冷色低亮辉光。</summary>
        private static readonly Color BloomClr = new(65, 38, 87, 0);
        /// <summary>CosmicPurple 的暗紫文字主色。</summary>
        private static readonly Color TextClr = new(103, 66, 138, 255);
        /// <summary>固定种子让星屑位置在每次排版测量时保持一致。</summary>
        private static readonly UnifiedRandom SparkleRandom = new(1);
        /// <summary>稀有度基础色，乘二后用于名称和界面描边。</summary>
        public override Color RarityColor => TextClr * 2f;
        /// <summary>
        /// 将普通文本片段替换为带旋转暗紫辉光的自定义绘制器。
        /// </summary>
        public sealed class CustomTextSnippet(string text) : TextSnippet
        {
            /// <summary>
            /// 绘制一段 CosmicPurple 名称文本。
            /// 仅在真实绘制时执行阴影和环形辉光，测量阶段只计算排版尺寸。
            /// </summary>
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch
                , Vector2 position = new(), Color color = new(), float scale = 1f)
            {
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                size = new Vector2(GetStringLength(font), font.MeasureString(" ").Y * scale);
                if (color == default || color == Main.MouseTextColorReal)
                    color = TextClr;
                if (justCheckingString || (color.R == 0 && color.G == 0 && color.B == 0))
                    return true;
                float pulsing = 2.5f + MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
                Color glowColor = color with { A = 0 } * 0.5f;
                // 八个方向的小幅偏移形成旋转暗紫外辉光，不创建额外渲染目标。
                for (float rotation = 0f; rotation < MathHelper.TwoPi; rotation += 0.79f)
                {
                    Vector2 offset = new Vector2(pulsing, 0f).RotatedBy(
                        rotation + Main.GlobalTimeWrappedHourly * 2f % MathHelper.TwoPi);
                    ChatManager.DrawColorCodedString(spriteBatch, font, text, position + offset
                        , glowColor, 0f, Vector2.Zero, new Vector2(scale));
                }
                ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, position, color * 2f
                    , 0f, Vector2.Zero, new Vector2(scale));
                ChatManager.DrawColorCodedString(spriteBatch, font, text, position, Color.Black
                    , 0f, Vector2.Zero, new Vector2(scale));
                return true;
            }
            /// <summary>返回当前片段在 tooltip 字体下的实际排版宽度。</summary>
            public override float GetStringLength(DynamicSpriteFont font) => font.MeasureString(text).X * Scale;
        }
        /// <summary>
        /// 绘制完整 CosmicPurple tooltip 名称，包括水晶背光、暗紫描边文字和固定种子星屑。
        /// </summary>
        public static void Draw(Item item, SpriteBatch spriteBatch, string text, int x, int y
            , Color textColor, Color lightColor, float rotation, Vector2 origin, Vector2 baseScale
            , float time, bool renderTextSparkles, DynamicSpriteFont font)
        {
            Texture2D crystalTextGlow = ModContent.Request<Texture2D>(
                "CalamityRebirth/Content/Rarities/CrystalTextGlow").Value;
            Texture2D sparkle = ModContent.Request<Texture2D>(
                "CalamityRebirth/Content/Rarities/CrystalTextSparkle").Value;
            Vector2 fontSize = ChatManager.GetStringSize(font, text, new Vector2(1f));
            Vector2 center = fontSize / 2f;
            if (item.expert)
                textColor = Main.DiscoColor;
            TextSnippet[] snippets = ChatManager.ParseMessage(text, textColor).ToArray();
            for (int i = 0; i < snippets.Length; i++)
            {
                if (snippets[i].GetType() == typeof(TextSnippet))
                    snippets[i] = new CustomTextSnippet(snippets[i].Text);
            }
            Vector2 glowPosition = new(x + center.X, y + center.Y / 1.5f);
            spriteBatch.Draw(crystalTextGlow, glowPosition, null, lightColor
                , rotation + MathHelper.PiOver2, new Vector2(6f, 33f)
                , new Vector2(1.6f, fontSize.X / crystalTextGlow.Height * 1.2f)
                , SpriteEffects.None, 0f);
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, new Vector2(x, y), textColor
                , 0f, Vector2.Zero, baseScale, out _, -1, true);
            if (!renderTextSparkles)
                return;
            SparkleRandom.SetSeed(1);
            int sparkleCount = SparkleRandom.Next((int)fontSize.X / 7, (int)fontSize.X / 5) + 1;
            Color sparkleColor = lightColor;
            sparkleColor.A = 0;
            Vector2 sparkleOrigin = new(15f, 15f);
            for (int i = 0; i < sparkleCount; i++)
            {
                Vector2 offset = new(SparkleRandom.NextFloat(fontSize.X)
                    , SparkleRandom.NextFloat(fontSize.Y * 0.6f) + 1f);
                float lifeTime = Main.GlobalTimeWrappedHourly * 4f + SparkleRandom.NextFloat(MathHelper.TwoPi);
                lifeTime %= MathHelper.TwoPi;
                float sinValue = MathF.Sin(lifeTime);
                Color white = new Color(200 + lightColor.R / 20, 200 + lightColor.G / 20
                    , 200 + lightColor.B / 20, 255) * sinValue;
                float rotationSpeed = Main.rand.NextFloat(0.8f, 1.5f);
                float sparkleRotation = time * rotationSpeed;
                Vector2 sparklePosition = new Vector2(x, y - lifeTime * SparkleMaxY + 3f) + offset;
                spriteBatch.Draw(sparkle, sparklePosition, null, white, sparkleRotation
                    , sparkleOrigin, lifeTime / MathHelper.TwoPi * 0.3f, SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkle, sparklePosition - Vector2.UnitY, null, white * 0.5f
                    , sparkleRotation, sparkleOrigin, lifeTime / MathHelper.TwoPi, SpriteEffects.None, 0f);
                float verticalScale = MathF.Sin(lifeTime / MathHelper.PiOver2) + 1f;
                float baseSparkleScale = lifeTime / MathHelper.TwoPi;
                verticalScale *= 0.2f;
                baseSparkleScale *= 0.15f;
                Vector2 secondPosition = sparklePosition - Vector2.UnitY;
                spriteBatch.Draw(sparkle, secondPosition, null, sparkleColor * sinValue, sparkleRotation
                    , sparkleOrigin, new Vector2(baseSparkleScale) * 1.5f, SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkle, secondPosition, null, sparkleColor * sinValue, sparkleRotation
                    , sparkleOrigin, new Vector2(verticalScale, baseSparkleScale), SpriteEffects.None, 0f);
                spriteBatch.Draw(sparkle, secondPosition, null, sparkleColor * sinValue, sparkleRotation
                    , sparkleOrigin, new Vector2(baseSparkleScale, verticalScale), SpriteEffects.None, 0f);
            }
        }
        /// <summary>
        /// 使用工具提示当前批次和默认字体绘制 CosmicPurple 名称行。
        /// </summary>
        public static void Draw(Item item, DrawableTooltipLine line)
        {
            Draw(item, Main.spriteBatch, line.Text, line.X, line.Y, TextClr
                , BloomClr, line.Rotation, line.Origin, line.BaseScale, Main.GlobalTimeWrappedHourly
                , true, FontAssets.MouseText.Value);
        }
    }
}
