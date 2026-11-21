using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>风暴之眼召唤的云元素，近距离时高速冲锋撞击敌怪。</summary>
	internal sealed class CloudyWaifu : ModProjectile
	{
		/// <summary>注册八帧动画和仆从目标功能。</summary>
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 8;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
		}
		/// <summary>设置碰撞、零召唤栏、常驻寿命和本地命中冷却。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 58;
			Projectile.height = 116;
			Projectile.netImportant = true;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.ignoreWater = true;
			Projectile.minion = true;
			Projectile.minionSlots = 0f;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 18000;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = ElementalMinionUtil.GetHitCooldown();
		}
		/// <summary>维持云元素悬停、索敌、冲锋状态与撞击后回位。</summary>
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (!modPlayer.eyeoftheStorm && !modPlayer.allWaifus)
			{
				Projectile.active = false;
				return;
			}
			if (player.dead)
				modPlayer.cloudWaifu = false;
			if (modPlayer.cloudWaifu)
				Projectile.timeLeft = 2;
			if (Projectile.localAI[0] > 0f)
			{
				Projectile.localAI[0] -= 1f;
				for (int i = 0; i < 50; i++)
				{
					int dustIndex = Dust.NewDust(Projectile.position + new Vector2(0f, 16f)
						, Projectile.width, Projectile.height - 16, DustID.Cloud);
					Main.dust[dustIndex].velocity *= 2f;
					Main.dust[dustIndex].scale *= 1.15f;
				}
			}
			if (Math.Abs(Projectile.velocity.X) > 0.2f)
				Projectile.spriteDirection = -Projectile.direction;
			float pulse = Main.rand.Next(90, 111) * 0.01f * Main.essScale;
			Lighting.AddLight(Projectile.Center, 0.25f * pulse, 0.55f * pulse, 0.75f * pulse);
			if (Projectile.ai[0] == 2f)
			{
				Projectile.extraUpdates = 2;
				Projectile.ai[1] += 1f;
				ElementalMinionUtil.Animate(Projectile, 8, 48);
				if (Projectile.ai[1] > 30f)
				{
					Projectile.ai[0] = 0f;
					Projectile.ai[1] = 1f;
					Projectile.extraUpdates = 0;
					Projectile.numUpdates = 0;
					Projectile.netUpdate = true;
				}
				return;
			}
			bool foundTarget = ElementalMinionUtil.TryFindTarget(Projectile, player, 500f, true
				, out Vector2 target, out float targetDistance);
			if (foundTarget && targetDistance > 100f)
			{
				Vector2 direction = target - Projectile.Center;
				direction.Normalize();
				Projectile.velocity = (Projectile.velocity * 40f + direction * 8f) / 41f;
			}
			else if (foundTarget)
			{
				Vector2 direction = target - Projectile.Center;
				direction.Normalize();
				Projectile.velocity = (Projectile.velocity * 40f - direction * 4f) / 41f;
			}
			else
			{
				ElementalMinionUtil.Hover(Projectile, player
					, player.Center + new Vector2(500f, -60f), 6f, 400f);
			}
			ElementalMinionUtil.Animate(Projectile, 8, 16);
			if (Projectile.ai[1] > 0f)
				Projectile.ai[1] += Main.rand.Next(1, 4);
			if (Projectile.ai[1] > 40f)
				Projectile.ai[1] = 0f;
			if (!foundTarget || Projectile.ai[1] != 0f || targetDistance >= 500f
				|| Projectile.owner != Main.myPlayer)
				return;
			Projectile.ai[1] = 1f;
			Projectile.ai[0] = 2f;
			Vector2 charge = target - Projectile.Center;
			charge.Normalize();
			Projectile.velocity = charge * 8f;
			Projectile.netUpdate = true;
		}
	}
}
