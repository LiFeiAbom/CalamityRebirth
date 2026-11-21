using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>瓶装沙元素召唤的沙元素，追踪敌怪并发射沙之箭。</summary>
	internal sealed class SandyWaifu : ModProjectile
	{
		/// <summary>注册十二帧动画和仆从目标功能。</summary>
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 12;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
		}
		/// <summary>设置碰撞、零召唤栏、常驻寿命和按进度缩短的命中冷却。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 42;
			Projectile.height = 98;
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
		/// <summary>维持沙元素悬停、双段动画、目标追踪和沙之箭攻击。</summary>
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (!modPlayer.wifeinaBottle && !modPlayer.allWaifus)
			{
				Projectile.active = false;
				return;
			}
			if (player.dead)
				modPlayer.sandyWaifu = false;
			if (modPlayer.sandyWaifu)
				Projectile.timeLeft = 2;
			if (Projectile.localAI[0] > 0f)
			{
				Projectile.localAI[0] -= 1f;
				for (int i = 0; i < 50; i++)
				{
					int dustIndex = Dust.NewDust(Projectile.position + new Vector2(0f, 16f)
						, Projectile.width, Projectile.height - 16, DustID.Sand);
					Main.dust[dustIndex].velocity *= 2f;
					Main.dust[dustIndex].scale *= 1.15f;
				}
			}
			if (Math.Abs(Projectile.velocity.X) > 0.2f)
				Projectile.spriteDirection = -Projectile.direction;
			float pulse = Main.rand.Next(90, 111) * 0.01f * Main.essScale;
			Lighting.AddLight(Projectile.Center, 0.7f * pulse, 0.6f * pulse, 0f);
			bool foundTarget = ElementalMinionUtil.TryFindTarget(Projectile, player, 700f, true
				, out Vector2 target, out _);
			if (foundTarget)
			{
				float distance = Vector2.Distance(Projectile.Center, target);
				if (distance > 200f)
				{
					Vector2 direction = target - Projectile.Center;
					direction.Normalize();
					Projectile.velocity = (Projectile.velocity * 40f + direction * 8f) / 41f;
				}
				else
				{
					Vector2 direction = target - Projectile.Center;
					direction.Normalize();
					Projectile.velocity = (Projectile.velocity * 40f - direction * 4f) / 41f;
				}
				if (Projectile.frame < 6)
					Projectile.frame = 6;
				Projectile.frameCounter++;
				if (Projectile.frameCounter > 16)
				{
					Projectile.frame++;
					Projectile.frameCounter = 0;
				}
				if (Projectile.frame > 11)
					Projectile.frame = 6;
			}
			else
			{
				ElementalMinionUtil.Hover(Projectile, player
					, player.Center + new Vector2(250f, -60f), 6f, 800f);
				ElementalMinionUtil.Animate(Projectile, 6, 16);
			}
			if (Projectile.ai[1] > 0f)
				Projectile.ai[1] += Main.rand.Next(1, 4);
			if (Projectile.ai[1] > 220f)
			{
				Projectile.ai[1] = 0f;
				Projectile.netUpdate = true;
			}
			if (!foundTarget || Projectile.ai[1] != 0f || Projectile.owner != Main.myPlayer)
				return;
			Projectile.ai[1] = 1f;
			Vector2 velocity = target - Projectile.Center;
			velocity.Normalize();
			velocity *= 11f;
			int bolt = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
				, velocity, ModContent.ProjectileType<SandBolt>(), Projectile.damage, 0f
				, Projectile.owner);
			if (Main.projectile.IndexInRange(bolt))
				Main.projectile[bolt].timeLeft = 300;
			Projectile.netUpdate = true;
		}
	}
}
