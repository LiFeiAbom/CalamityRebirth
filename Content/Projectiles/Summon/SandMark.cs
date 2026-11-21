using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>沙之箭留下的沙之印记，在六十帧后生成沙暴。</summary>
	internal sealed class SandMark : ModProjectile
	{
		/// <summary>使用通用隐形贴图，实际表现由沙尘构成。</summary>
		public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
		/// <summary>设置十四像素碰撞、无限穿透、一百二十帧寿命和按进度缩短的命中冷却。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 120;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = ElementalMinionUtil.GetHitCooldown();
		}
		/// <summary>吸附到方块中心、生成沙尘，并在第六十帧创建沙暴。</summary>
		public override void AI()
		{
			if (Projectile.soundDelay == 0)
			{
				Projectile.soundDelay = -1;
				SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
			}
			if (Projectile.localAI[0] == 0f)
			{
				Projectile.localAI[0] = 1f;
				Point tile = Projectile.Center.ToTileCoordinates();
				Projectile.Center = new Vector2(tile.X * 16 + 8f, tile.Y * 16 + 8f);
			}
			Projectile.localAI[1] += 1f;
			Projectile.rotation += 0.08f;
			for (int i = 0; i < 2; i++)
			{
				if (!Main.rand.NextBool(5))
					continue;
				float rotation = Projectile.rotation + i * MathHelper.Pi;
				Vector2 velocity = Vector2.UnitY.RotatedBy(rotation) * 2.5f;
				int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, DustID.Sandnado
					, velocity.X, velocity.Y, 225, Color.White, 1f);
				Main.dust[dustIndex].noGravity = true;
				Main.dust[dustIndex].noLight = true;
			}
			if (Projectile.localAI[1] == 60f && Projectile.owner == Main.myPlayer)
			{
				int storm = Projectile.NewProjectile(Projectile.GetSource_FromThis()
					, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandTornado>()
					, Projectile.damage, 2f, Projectile.owner, 1f);
				if (Main.projectile.IndexInRange(storm))
					Main.projectile[storm].minion = true;
			}
			if (Projectile.localAI[1] > 87f)
				Projectile.scale = MathHelper.Clamp(1f - (Projectile.localAI[1] - 87f) / 33f, 0f, 1f);
		}
		/// <summary>印记固定在原地。</summary>
		public override bool? CanDamage()
			=> true;
	}
}
