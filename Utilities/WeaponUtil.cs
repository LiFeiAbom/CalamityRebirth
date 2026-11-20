using Terraria;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 修正大型近战武器的挥砍位置，使剑身在三段挥砍中的位置更贴合视觉中心。
    /// </summary>
    internal static partial class CRUtil
    {
        /// <summary>
        /// 按挥砍前、中、后三个阶段水平与垂直微调武器位置。
        /// 原始剑身中心偏上，因此常规阶段向上移动；中后段再向内收，避免宽贴图遮住角色。
        /// </summary>
        /// <param name="player">正在执行近战挥砍的玩家。</param>
        public static void BetterSwing(Player player)
        {
            float xOffset = 6f;
            float yOffset = -10f;
            if (player.itemAnimation < player.itemAnimationMax * 0.333f)
                yOffset = 4f;
            else if (player.itemAnimation >= player.itemAnimationMax * 0.666f)
                xOffset = -4f;
            player.itemLocation.X = player.Center.X + xOffset * player.direction;
            player.itemLocation.Y = player.MountedCenter.Y + yOffset;
            if (player.gravDir < 0)
                // 重力翻转时以玩家中心为镜像轴，重新计算武器 Y 坐标。
                player.itemLocation.Y = player.Center.Y + (player.position.Y - player.itemLocation.Y);
        }
    }
}
