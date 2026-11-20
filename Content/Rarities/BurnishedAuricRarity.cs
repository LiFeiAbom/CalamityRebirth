using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;
namespace CalamityRebirth.Content.Rarities
{
    /// <summary>
    /// 灼辉金源稀有度：复刻灾厄的金色描边、暗红内核、逐字扫光和短时青蓝闪金。
    /// 绘制直接复用 tooltip 当前批次，避免切换全屏渲染目标导致中文资源包下黑屏。
    /// </summary>
    internal class BurnishedAuricRarity : ModRarity
    {
        /// <summary>稀有度基础色，乘二后用于名称和部分界面描边。</summary>
        public override Color RarityColor => TextClr * 2f;
        /// <summary>物品名称外层使用的低亮金色辉光。</summary>
        public static readonly Color BloomClr = new(48, 33, 4, 0);
        /// <summary>灼辉金源文字主色，对应灾厄原版的核心金色。</summary>
        public static readonly Color TextClr = new(157, 110, 11, 255);
        /// <summary>上一次触发青色闪烁的游戏帧号。</summary>
        private static float lastFlashTime;
        /// <summary>当前是否处于短暂的青色闪烁窗口。</summary>
        private static bool isFlashing;
        /// <summary>
        /// 替换普通文本片段的专用绘制器。
        /// 所有绘制都在现有 UI 批次中完成，不创建渲染目标，也不结束或重启外部 SpriteBatch。
        /// </summary>
        /// <param name="text">已经由 ChatManager 解析后的纯文本片段。</param>
        public sealed class CustomTextSnippet(string text) : TextSnippet
        {
            /// <summary>
            /// 将一段名称文字绘制为八方向描边、暗红内核和逐字扫光。
            /// 纯测量调用只返回布局尺寸，避免不可见的 tooltip 产生额外绘制工作。
            /// </summary>
            /// <param name="justCheckingString">true 表示仅计算尺寸，不产生绘制调用。</param>
            /// <param name="size">返回该片段占用的布局尺寸。</param>
            /// <param name="spriteBatch">tooltip 使用的 SpriteBatch。</param>
            /// <param name="position">片段在屏幕上的左上角位置。</param>
            /// <param name="color">父级 tooltip 传入的基础颜色。</param>
            /// <param name="scale">tooltip 文本缩放。</param>
            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch
                , Vector2 position = new(), Color color = new(), float scale = 1f)
            {
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                size = new Vector2(GetStringLength(font), font.MeasureString(" ").Y * scale);
                if (color == default || color == Main.MouseTextColorReal)
                    color = TextClr;
                if (justCheckingString || (color.R == 0 && color.G == 0 && color.B == 0))
                    return true;
                Color borderColor = color * 2f;
                Color coreColor = new(77, 0, 33);
                Color shineColor = new(254, 231, 117);
                if (isFlashing)
                {
                    shineColor = new Color(90, 207, 255);
                    position += Main.rand.NextVector2Circular(8f, 4.8f);
                }
                Vector2 fontScale = new(scale);
                // 八方向轻微偏移形成连续外描边，之后用暗酒红覆盖文字内核。
                for (float rotation = 0f; rotation < MathHelper.TwoPi; rotation += MathHelper.TwoPi * 0.125f)
                {
                    Vector2 offset = new Vector2(2f, 0f).RotatedBy(rotation);
                    ChatManager.DrawColorCodedString(spriteBatch, font, text, position + offset
                        , borderColor, 0f, Vector2.Zero, fontScale);
                }
                ChatManager.DrawColorCodedString(spriteBatch, font, text, position
                    , coreColor, 0f, Vector2.Zero, fontScale);
                // 逐字符推进基准 X 坐标，避免每帧反复测量整段累计字符串。
                float characterX = position.X;
                foreach (char character in text)
                {
                    float wave = (MathF.Sin(characterX * 0.02f
                        + Main.GlobalTimeWrappedHourly * -1.5f) + 1f) * 0.5f;
                    Color shine = shineColor * MathF.Pow(wave, 120f);
                    if (shine.A > 0)
                    {
                        ChatManager.DrawColorCodedString(spriteBatch, font, character.ToString()
                            , new Vector2(characterX, position.Y), shine, 0f, Vector2.Zero, fontScale);
                    }
                    characterX += font.MeasureString(character.ToString()).X * scale;
                }
                return true;
            }
            /// <summary>
            /// 返回文本在当前字体和缩放下的实际像素宽度，供 ChatManager 正常换行和排版。
            /// </summary>
            public override float GetStringLength(DynamicSpriteFont font) => font.MeasureString(text).X * Scale;
        }
        /// <summary>
        /// 绘制完整灼辉金源 tooltip 名称：解析颜色标签、替换普通片段，并应用低频青色闪烁。
        /// </summary>
        /// <param name="item">当前正在显示名称的物品。</param>
        /// <param name="spriteBatch">tooltip 的 SpriteBatch。</param>
        /// <param name="text">原始名称文本，可能包含聊天颜色标签。</param>
        /// <param name="x">绘制起点 X。</param>
        /// <param name="y">绘制起点 Y。</param>
        /// <param name="textColor">普通字色。</param>
        /// <param name="lightColor">辉光颜色。</param>
        /// <param name="baseScale">tooltip 基础缩放。</param>
        /// <param name="font">名称字体。</param>
        public static void Draw(Item item, SpriteBatch spriteBatch, string text, int x, int y, Color textColor
            , Color lightColor, Vector2 baseScale, DynamicSpriteFont font)
        {
            const float flashChance = 0.005f;
            const float flashDuration = 0.2f;
            if (Main.GameUpdateCount - lastFlashTime > flashDuration * 60f)
            {
                // 每帧只有约 0.5% 概率进入短暂闪色；持续期内保持状态，避免随机闪烁过于破碎。
                if (Main.rand.NextFloat() < flashChance)
                {
                    isFlashing = true;
                    lastFlashTime = Main.GameUpdateCount;
                }
                else
                {
                    isFlashing = false;
                }
            }
            TextSnippet[] snippets = ChatManager.ParseMessage(text, textColor).ToArray();
            for (int i = 0; i < snippets.Length; i++)
            {
                if (snippets[i].GetType() == typeof(TextSnippet))
                    snippets[i] = new CustomTextSnippet(snippets[i].Text);
            }
            if (item.expert)
                textColor = Main.DiscoColor;
            if (isFlashing)
            {
                textColor = new Color(0, 183, 241, 50);
                lightColor = new Color(14, 255, 255, 50);
            }
            textColor.A = 255;
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, new Vector2(x, y), textColor, 0f
                , Vector2.Zero, baseScale, out _, -1, true);
        }
        /// <summary>
        /// 从 DrawableTooltipLine 直接绘制名称行，统一采用鼠标提示字体和行坐标。
        /// </summary>
        public static void Draw(Item item, DrawableTooltipLine line)
        {
            Draw(item, Main.spriteBatch, line.Text, line.X, line.Y, TextClr, BloomClr, line.BaseScale
                , FontAssets.MouseText.Value);
        }
    }
}
