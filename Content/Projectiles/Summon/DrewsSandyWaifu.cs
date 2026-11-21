using CalamityRebirth.Content.Projectiles.Healing;
using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>稀有瓶装沙元素召唤的治疗型沙元素，周期性生成治疗珠。</summary>
	internal sealed class DrewsSandyWaifu : ModProjectile
	{
		/// <summary>注册五帧宠物动画与仆从牺牲功能。</summary>
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 5;
			Main.projPet[Type] = true;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
		}
		/// <summary>设置碰撞、零召唤栏、常驻寿命和本地命中冷却。</summary>
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
		/// <summary>靠近玩家悬停，并在延迟与冷却结束后生成治疗珠。</summary>
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (!modPlayer.wifeinaBottlewithBoobs && !modPlayer.allWaifus)
			{
				Projectile.active = false;
				return;
			}
			if (player.dead)
				modPlayer.drewsSandyWaifu = false;
			if (modPlayer.drewsSandyWaifu)
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
			ElementalMinionUtil.Animate(Projectile, 5, 16);
			if (Math.Abs(Projectile.velocity.X) > 0.2f)
				Projectile.spriteDirection = -Projectile.direction;
			float pulse = Main.rand.Next(90, 111) * 0.01f * Main.essScale;
			Lighting.AddLight(Projectile.Center, 0.7f * pulse, 0.6f * pulse, 0f);
			ElementalMinionUtil.Hover(Projectile, player
				, player.Center + new Vector2(-250f, -60f), 7f, 400f);
			if (Projectile.ai[1] > 0f)
				Projectile.ai[1] += Main.rand.Next(1, 4);
			if (Projectile.ai[1] > 220f)
			{
				Projectile.ai[1] = 0f;
				Projectile.netUpdate = true;
			}
			if (Projectile.localAI[1] < 120f)
				Projectile.localAI[1] += 1f;
			if (Projectile.ai[1] != 0f || Projectile.localAI[1] < 120f
				|| Projectile.owner != Main.myPlayer || player.statLife >= player.statLifeMax2)
				return;
			Projectile.ai[1] = 1f;
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			for (int i = 0; i < 36; i++)
			{
				Vector2 direction = (MathHelper.TwoPi * i / 36f).ToRotationVector2();
				int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.Terra
					, direction.X * 4f, direction.Y * 4f, 100, new Color(0, 200, 0), 1f);
				Main.dust[dustIndex].noGravity = true;
				Main.dust[dustIndex].noLight = true;
			}
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
				, -Vector2.UnitY * 6f, ModContent.ProjectileType<CactusHealOrb>(), 0, 0f
				, Projectile.owner);
		}
	}
}
