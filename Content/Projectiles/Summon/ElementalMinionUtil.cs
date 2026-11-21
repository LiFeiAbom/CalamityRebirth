using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>五类元素仆从共用的命中冷却、索敌、悬停和动画工具。</summary>
	internal static class ElementalMinionUtil
	{
		/// <summary>按月亮领主、神明吞噬者和犽戎进度缩短仆从本地命中冷却。</summary>
		internal static int GetHitCooldown()
		{
			return 20 - (NPC.downedGolemBoss ? 5 : 0) - (NPC.downedMoonlord ? 5 : 0)
				- (CalamityCompatUtil.IsDevourerOfGodsDowned() ? 4 : 0)
				- (CalamityCompatUtil.IsYharonDowned() ? 3 : 0);
		}
		/// <summary>
		/// 优先读取玩家右键指定的仆从目标，否则搜索范围内最近的可追踪敌怪。
		/// requireLineOfSight 为 true 时只接受无方块遮挡的目标。
		/// </summary>
		internal static bool TryFindTarget(Projectile projectile, Player player, float range
			, bool requireLineOfSight, out Vector2 targetCenter, out float targetDistance)
		{
			targetCenter = Vector2.Zero;
			targetDistance = range;
			bool found = false;
			if (player.HasMinionAttackTargetNPC)
			{
				NPC selected = Main.npc[player.MinionAttackTargetNPC];
				if (selected.CanBeChasedBy(projectile)
					&& (!requireLineOfSight || Collision.CanHitLine(projectile.position
						, projectile.width, projectile.height, selected.position
						, selected.width, selected.height)))
				{
					float distance = Vector2.Distance(selected.Center, projectile.Center);
					if (distance < range)
					{
						targetCenter = selected.Center;
						targetDistance = distance;
						return true;
					}
				}
			}
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.CanBeChasedBy(projectile))
					continue;
				if (requireLineOfSight && !Collision.CanHitLine(projectile.position
					, projectile.width, projectile.height, npc.position, npc.width, npc.height))
					continue;
				float distance = Vector2.Distance(npc.Center, projectile.Center);
				if (distance >= targetDistance)
					continue;
				targetDistance = distance;
				targetCenter = npc.Center;
				found = true;
			}
			return found;
		}
		/// <summary>用四十比一的平滑权重把仆从速度推向指定悬停位置。</summary>
		internal static void Hover(Projectile projectile, Player player, Vector2 destination
			, float baseSpeed, float recallDistance)
		{
			Vector2 offset = destination - projectile.Center;
			float distance = offset.Length();
			if (distance > 2000f)
			{
				projectile.Center = player.Center;
				projectile.netUpdate = true;
				return;
			}
			float speed = baseSpeed;
			if (distance > recallDistance)
				speed = MathHelper.Max(speed, 12f);
			if (distance < 70f)
			{
				if (projectile.velocity == Vector2.Zero)
					projectile.velocity = new Vector2(-0.18f, -0.08f);
				return;
			}
			offset.Normalize();
			offset *= speed;
			projectile.velocity = (projectile.velocity * 40f + offset) / 41f;
		}
		/// <summary>按固定节拍循环指定数量的动画帧。</summary>
		internal static void Animate(Projectile projectile, int frameCount, int ticksPerFrame)
		{
			projectile.frameCounter++;
			if (projectile.frameCounter > ticksPerFrame)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame >= frameCount)
				projectile.frame = 0;
		}
	}
}
