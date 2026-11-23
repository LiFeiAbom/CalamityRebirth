using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
    /// <summary>机械蠕虫身体与尾节共用的父节点跟随逻辑。</summary>
    internal static class MechwormSegmentUtil
    {
        /// <summary>校验玩家与父节点，更新段序、旋转、缩放和位置。</summary>
        internal static void Update(Projectile projectile, ModProjectile modProjectile
            , ref bool initialized, ref int initialMaxMinions, bool killWithoutParent)
        {
            if (!Main.player.IndexInRange(projectile.owner))
            {
                projectile.Kill();
                return;
            }
            Player player = Main.player[projectile.owner];
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (!initialized)
            {
                initialized = true;
                initialMaxMinions = player.maxMinions;
            }
            if (!player.active || player.dead || player.maxMinions < initialMaxMinions)
            {
                modPlayer.mWorm = false;
                projectile.active = false;
                return;
            }
            initialMaxMinions = System.Math.Max(initialMaxMinions, player.maxMinions);
            if (modPlayer.mWorm)
                projectile.timeLeft = 2;
            if ((int)Main.time % 120 == 0)
                projectile.netUpdate = true;
            int parent = (int)projectile.ai[0];
            if (!Main.projectile.IndexInRange(parent) || !Main.projectile[parent].active)
            {
                if (killWithoutParent)
                    projectile.Kill();
                return;
            }
            Projectile parentProjectile = Main.projectile[parent];
            float parentScale = MathHelper.Clamp(parentProjectile.scale, 0.1f, 50f);
            parentProjectile.localAI[0] = projectile.localAI[0] + 1f;
            Vector2 toParent = parentProjectile.Center - projectile.Center;
            if (toParent == Vector2.Zero)
                toParent = -Vector2.UnitY;
            projectile.rotation = toParent.ToRotation() + MathHelper.PiOver2;
            projectile.scale = parentScale;
            projectile.width = projectile.height = (int)(10f * projectile.scale);
            projectile.Center = parentProjectile.Center
                - toParent.SafeNormalize(Vector2.UnitY) * 16f * parentScale;
            projectile.spriteDirection = toParent.X > 0f ? 1 : -1;
            if (projectile.alpha > 0)
                projectile.alpha = System.Math.Max(0, projectile.alpha - 42);
        }
    }
}
