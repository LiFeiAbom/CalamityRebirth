using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>魅惑之饵召唤的塞壬，固定在玩家头顶并发射三种水系弹幕。</summary>
	internal sealed class SirenLure : ModProjectile
	{
		/// <summary>注册六帧动画和仆从目标功能。</summary>
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 6;
			ProjectileID.Sets.MinionSacrificable[Type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;
		}
		/// <summary>设置碰撞、零召唤栏、常驻寿命和悬浮型仆从属性。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 70;
			Projectile.height = 120;
			Projectile.netImportant = true;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.ignoreWater = true;
			Projectile.minion = true;
			Projectile.minionSlots = 0f;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 18000;
		}
		/// <summary>固定到玩家上方，按十二帧间隔选择水矛、霜雾或歌声攻击。</summary>
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (!modPlayer.lureofEnthrallment && !modPlayer.allWaifus)
			{
				Projectile.active = false;
				return;
			}
			if (player.dead)
				modPlayer.sirenLureWaifu = false;
			if (modPlayer.sirenLureWaifu)
				Projectile.timeLeft = 2;
			if (Projectile.localAI[0] > 0f)
			{
				Projectile.localAI[0] -= 1f;
				for (int i = 0; i < 50; i++)
				{
					int dustIndex = Dust.NewDust(Projectile.position + new Vector2(0f, 16f)
						, Projectile.width, Projectile.height - 16, DustID.Water);
					Main.dust[dustIndex].velocity *= 2f;
					Main.dust[dustIndex].scale *= 1.15f;
				}
			}
			Lighting.AddLight(Projectile.Center, 0f, 0.25f, 1.5f);
			ElementalMinionUtil.Animate(Projectile, 6, 12);
			Projectile.Center = player.Center + new Vector2(0f, -180f + player.gfxOffY);
			Projectile.rotation = player.gravDir == -1f ? MathHelper.Pi : 0f;
			if (Projectile.owner != Main.myPlayer)
				return;
			if (Projectile.ai[0] > 0f)
			{
				Projectile.ai[0] -= 1f;
				return;
			}
			if (!ElementalMinionUtil.TryFindTarget(Projectile, player, 1000f, true
				, out Vector2 target, out _))
				return;
			int projectileType = ModContent.ProjectileType<WaterSpearFriendly>();
			if (Main.rand.NextBool(9))
				projectileType = ModContent.ProjectileType<FrostMistFriendly>();
			else if (Main.rand.NextBool(9))
				projectileType = ModContent.ProjectileType<SirenSongFriendly>();
			Vector2 velocity = target - Projectile.Center;
			velocity.Normalize();
			velocity *= Main.rand.Next(12, 20);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitX * 4f
				, velocity, projectileType, Projectile.damage, Projectile.knockBack, Projectile.owner);
			Projectile.ai[0] = 12f;
		}
		/// <summary>使用固定浅灰透明色绘制塞壬。</summary>
		public override Color? GetAlpha(Color lightColor)
			=> new Color(200, 200, 200, 200);
		/// <summary>塞壬本体不造成接触伤害，伤害全部来自发射的弹幕。</summary>
		public override bool? CanDamage()
			=> false;
	}
}
