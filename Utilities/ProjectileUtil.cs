using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 通用弹幕辅助方法，供环境之刃及后续方舟链弹幕共用。
    /// </summary>
    internal static partial class CRUtil
    {
        /// <summary>
        /// 按历史位置绘制半透明残影，并在最后绘制当前弹幕本体。
        /// 历史位置数量由 ProjectileID.Sets.TrailCacheLength 控制。
        /// </summary>
        /// <param name="projectile">需要绘制的弹幕。</param>
        /// <param name="mode">残影模式；本系列使用零号模式。</param>
        /// <param name="lightColor">场景光照颜色；实际颜色由弹幕 GetAlpha 覆盖。</param>
        /// <param name="increment">历史位置的取样步长，至少为一。</param>
        public static void DrawAfterimages(Projectile projectile, int mode, Color lightColor
            , int increment = 1)
        {
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 origin = frame.Size() / 2f;
            Vector2 centerOffset = projectile.Size / 2f;
            SpriteEffects effects = projectile.spriteDirection == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            Color alphaColor = projectile.GetAlpha(lightColor);
            bool validMode = true;
            switch (mode)
            {
                case 0:
                    for (int i = 0; i < trailLength; i++)
                    {
                        Vector2 drawPosition = projectile.oldPos[i] + centerOffset - Main.screenPosition
                            + new Vector2(0f, projectile.gfxOffY);
                        Color trailColor = alphaColor * ((trailLength - i) / (float)trailLength);
                        Main.spriteBatch.Draw(texture, drawPosition, frame, trailColor
                            , projectile.rotation, origin, projectile.scale, effects, 0f);
                    }
                    break;
                case 1:
                    int step = System.Math.Max(1, increment);
                    Color drawColor = alphaColor;
                    float colorDivisor = trailLength * 1.5f;
                    for (int i = 0; i < trailLength; i += step)
                    {
                        Vector2 drawPosition = projectile.oldPos[i] + centerOffset - Main.screenPosition
                            + new Vector2(0f, projectile.gfxOffY);
                        if (i > 0)
                            drawColor *= (trailLength - i) / colorDivisor;
                        Main.spriteBatch.Draw(texture, drawPosition, frame, drawColor
                            , projectile.rotation, origin, projectile.scale, effects, 0f);
                    }
                    break;
                case 2:
                    for (int i = 0; i < trailLength; i++)
                    {
                        Vector2 drawPosition = projectile.oldPos[i] + centerOffset - Main.screenPosition
                            + new Vector2(0f, projectile.gfxOffY);
                        Color trailColor = alphaColor * ((trailLength - i) / (float)trailLength);
                        SpriteEffects trailEffects = projectile.oldSpriteDirection[i] == -1
                            ? SpriteEffects.FlipHorizontally
                            : SpriteEffects.None;
                        Main.spriteBatch.Draw(texture, drawPosition, frame, trailColor
                            , projectile.oldRot[i], origin, projectile.scale, trailEffects, 0f);
                    }
                    break;
                default:
                    validMode = false;
                    break;
            }
            if (trailLength <= 0 || !validMode)
            {
                Vector2 bodyPosition = projectile.Center - Main.screenPosition
                    + new Vector2(0f, projectile.gfxOffY);
                Main.spriteBatch.Draw(texture, bodyPosition, frame, alphaColor
                    , projectile.rotation, origin, projectile.scale, effects, 0f);
            }
        }
        /// <summary>
        /// 让友方弹幕寻找指定半径内最近的合法敌人，并按惯性平滑转向。
        /// 追踪只计算速度方向，不直接修改弹幕伤害或速度上限。
        /// </summary>
        /// <param name="projectile">需要追踪的友方弹幕。</param>
        /// <param name="distance">最大搜索距离。</param>
        /// <param name="speed">期望追踪速度。</param>
        /// <param name="inertia">旧速度保留权重，数值越高转向越柔和。</param>
        /// <param name="ignoreTiles">是否忽略视线上的物块阻挡。</param>
        public static void HomeInNPC(Projectile projectile, float distance, float speed, float inertia
            , bool ignoreTiles = true)
        {
            NPC target = null;
            float bestDistance = distance;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(projectile))
                    continue;
                float currentDistance = Vector2.Distance(projectile.Center, npc.Center);
                if (currentDistance >= bestDistance)
                    continue;
                if (!ignoreTiles && !Collision.CanHitLine(projectile.Center, 1, 1, npc.Center, 1, 1))
                    continue;
                bestDistance = currentDistance;
                target = npc;
            }
            if (target is null)
                return;
            Vector2 desiredDirection = (target.Center - projectile.Center).SafeNormalize(Vector2.UnitY);
            projectile.velocity = (projectile.velocity * inertia + desiredDirection * speed) / (inertia + 1f);
        }
        /// <summary>
        /// 生成方舟武器“从天而降”弹幕的随机位置。
        /// 横向位置在玩家与鼠标瞄准方向之间随机散布，纵向固定在玩家上方六百像素，并按序号继续抬高。
        /// </summary>
        /// <param name="player">正在使用武器的玩家。</param>
        /// <param name="index">当前弹幕组序号，用于逐层抬高出生点。</param>
        /// <param name="horizontalJitter">是否追加正负二百像素的水平随机散布。</param>
        internal static Vector2 GetFallingProjectileSpawn(Player player, int index
            , bool horizontalJitter = true)
        {
            Vector2 spawn = new(
                player.position.X + player.width * 0.5f
                    + Main.rand.Next(201) * -player.direction
                    + (Main.MouseWorld.X - player.position.X),
                player.MountedCenter.Y - 600f);
            spawn.X = (spawn.X + player.Center.X) / 2f;
            if (horizontalJitter)
                spawn.X += Main.rand.Next(-200, 201);
            spawn.Y -= 100f * index;
            return spawn;
        }
    }
}
