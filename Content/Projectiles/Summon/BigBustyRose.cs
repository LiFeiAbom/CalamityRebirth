using CalamityRebirth.Content.Projectiles.Magic;
using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>玫瑰石与元灵之心召唤的硫火元素，周期性发射硫磺火球。</summary>
	internal sealed class BigBustyRose : ModProjectile
	{
		/// <summary>注册四帧动画与仆从目标功能。</summary>
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 4;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
		}
		/// <summary>设置大型仆从碰撞、零召唤栏占用和按进度递减的本地命中冷却。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 78;
			Projectile.height = 126;
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
		/// <summary>维持饰品存活条件、悬停位置、动画、照明与硫磺火球攻击。</summary>
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if ((!modPlayer.roseStone || !modPlayer.roseStoneVisible) && !modPlayer.allWaifus)
			{
				Projectile.active = false;
				return;
			}
			if (player.dead)
				modPlayer.brimstoneWaifu = false;
			if (modPlayer.brimstoneWaifu)
				Projectile.timeLeft = 2;
			if (Projectile.localAI[0] > 0f)
			{
				Projectile.localAI[0] -= 1f;
				for (int i = 0; i < 50; i++)
				{
					int dustIndex = Dust.NewDust(Projectile.position + new Vector2(0f, 16f)
						, Projectile.width, Projectile.height - 16, DustID.LifeDrain);
					Main.dust[dustIndex].velocity *= 2f;
					Main.dust[dustIndex].scale *= 1.15f;
				}
			}
			ElementalMinionUtil.Animate(Projectile, 4, 20);
			float pulse = Main.rand.Next(90, 111) * 0.01f * Main.essScale;
			Lighting.AddLight(Projectile.Center, 1.25f * pulse, 0f, 0.5f * pulse);
			if (Math.Abs(Projectile.velocity.X) > 0.2f)
				Projectile.spriteDirection = -Projectile.direction;
			Vector2 destination = player.Center + new Vector2(-500f, -60f);
			ElementalMinionUtil.Hover(Projectile, player, destination, 5f, 400f);
			bool foundTarget = ElementalMinionUtil.TryFindTarget(Projectile, player, 700f, true
				, out Vector2 target, out _);
			if (Projectile.ai[1] > 0f)
				Projectile.ai[1] += Main.rand.Next(1, 4);
			if (Projectile.ai[1] > 160f)
			{
				Projectile.ai[1] = 0f;
				Projectile.netUpdate = true;
			}
			if (!foundTarget || Projectile.ai[1] != 0f || Projectile.owner != Main.myPlayer)
				return;
			Projectile.ai[1] = 1f;
			Vector2 velocity = target - Projectile.Center;
			velocity.Normalize();
			velocity *= 14f;
			int fireball = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
				, velocity, ModContent.ProjectileType<BrimstoneHellfireballFriendly2>()
				, Projectile.damage, 0f, Projectile.owner);
			if (Main.projectile.IndexInRange(fireball))
				Main.projectile[fireball].timeLeft = 200;
			Projectile.netUpdate = true;
		}
	}
}
