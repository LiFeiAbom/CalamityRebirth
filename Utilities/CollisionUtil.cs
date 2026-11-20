using Microsoft.Xna.Framework;
using Terraria;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 本模组使用的确定性碰撞几何工具。
    /// </summary>
    internal static class CollisionUtil
    {
        /// <summary>
        /// 判断圆形与矩形是否相交。
        /// 先将圆心钳制到矩形边界取得最近点，再比较距离，避免长条或大型目标仅因四角超出范围而漏判。
        /// </summary>
        public static bool CircleIntersectsRectangle(Vector2 center, Rectangle rectangle, float radius)
        {
            Vector2 closest = Vector2.Clamp(center, rectangle.TopLeft(), rectangle.BottomRight());
            return Vector2.DistanceSquared(center, closest) <= radius * radius;
        }
    }
}
