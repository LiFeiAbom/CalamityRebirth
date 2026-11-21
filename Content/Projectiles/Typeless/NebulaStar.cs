using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
	/// <summary>
	/// 星云之星：围绕玩家漂浮、追踪附近敌怪并在命中后散射星云尘埃的星云弹幕。
	/// </summary>
	internal sealed class NebulaStar : ModProjectile
	{
		/// <summary>
		/// 设置三十四像素碰撞尺寸、单穿透、无液体和方块碰撞，并登记一分钟基础寿命。
		/// </summary>
		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 34;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.alpha = 255;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 3600;
		}
		/// <summary>
		/// 维护星云之星的发光、缩放、S 形漂移、远距离加速淡出和六百像素追踪。
		/// 饰品卸下或隐藏时立即停止该弹幕。
		/// </summary>
		public override void AI()
		{
			CalamityRebirthPlayer modPlayer = Main.player[Projectile.owner]
				.GetModPlayer<CalamityRebirthPlayer>();
			if (!modPlayer.nebulousCore || !modPlayer.nebulousCoreVisible)
			{
				Projectile.active = false;
				return;
			}
			float visibility = 1f - Projectile.alpha / 255f;
			visibility *= Projectile.scale;
			Lighting.AddLight(Projectile.Center, 0.25f * visibility, 0.025f * visibility
				, 0.275f * visibility);
			Projectile.localAI[0] += 1f;
			// 每九十帧反转一次缩放方向，使星体大小缓慢来回呼吸。
			if (Projectile.localAI[0] >= 90f)
				Projectile.localAI[0] *= -1f;
			Projectile.scale += Projectile.localAI[0] >= 0f ? 0.003f : -0.003f;
			Projectile.rotation += 0.0025f * Projectile.scale;
			float horizontalDrift = 1f;
			float verticalDrift = 1f;
			// 弹幕身份决定漂移方向，使同批星云之星形成互不重复的六种曲线。
			switch (Projectile.identity % 6)
			{
				case 0:
					verticalDrift *= -1f;
					break;
				case 1:
					horizontalDrift *= -1f;
					break;
				case 2:
					verticalDrift *= -1f;
					horizontalDrift *= -1f;
					break;
				case 3:
					verticalDrift = 0f;
					break;
				case 4:
					horizontalDrift = 0f;
					break;
			}
			Projectile.localAI[1] += 1f;
			if (Projectile.localAI[1] > 60f)
				Projectile.localAI[1] = -180f;
			if (Projectile.localAI[1] >= -60f)
			{
				Projectile.velocity.X += 0.002f * verticalDrift;
				Projectile.velocity.Y += 0.002f * horizontalDrift;
			}
			else
			{
				Projectile.velocity.X -= 0.002f * verticalDrift;
				Projectile.velocity.Y -= 0.002f * horizontalDrift;
			}
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] > 5400f)
			{
				// 存活过久的星星失去伤害并逐渐透明，随后由主人端销毁。
				Projectile.damage = 0;
				Projectile.ai[1] = 1f;
				if (Projectile.alpha < 255)
				{
					Projectile.alpha += 5;
					if (Projectile.alpha > 255)
						Projectile.alpha = 255;
				}
				else if (Projectile.owner == Main.myPlayer)
					Projectile.Kill();
				return;
			}
			// 离玩家越远时寿命消耗越快，限制星星无限漂移到战场外。
			float distanceFactor = (Projectile.Center - Main.player[Projectile.owner].Center)
				.Length() / 100f;
			if (distanceFactor > 4f)
				distanceFactor *= 1.1f;
			if (distanceFactor > 5f)
				distanceFactor *= 1.2f;
			if (distanceFactor > 6f)
				distanceFactor *= 1.3f;
			if (distanceFactor > 7f)
				distanceFactor *= 1.4f;
			if (distanceFactor > 8f)
				distanceFactor *= 1.5f;
			if (distanceFactor > 9f)
				distanceFactor *= 1.6f;
			if (distanceFactor > 10f)
				distanceFactor *= 1.7f;
			Projectile.ai[0] += distanceFactor;
			if (Projectile.alpha > 50)
			{
				Projectile.alpha -= 10;
				if (Projectile.alpha < 50)
					Projectile.alpha = 50;
			}
			bool foundTarget = false;
			Vector2 targetCenter = Vector2.Zero;
			float nearestDistance = 600f;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.CanBeChasedBy(Projectile))
					continue;
				float distance = Math.Abs(Projectile.Center.X - npc.Center.X)
					+ Math.Abs(Projectile.Center.Y - npc.Center.Y);
				if (distance >= nearestDistance)
					continue;
				nearestDistance = distance;
				targetCenter = npc.Center;
				foundTarget = true;
			}
			if (foundTarget)
			{
				Vector2 direction = targetCenter - Projectile.Center;
				direction.Normalize();
				direction *= 0.75f;
				Projectile.velocity = (Projectile.velocity * 10f + direction) / 11f;
				return;
			}
			if (Projectile.velocity.Length() > 0.2f)
				Projectile.velocity *= 0.98f;
		}
		/// <summary>
		/// 命中 NPC 时由主人端散射一枚星云尘埃；正在淡出的星星不再产生尘埃。
		/// </summary>
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnNebulaDust();
		}
		/// <summary>PvP 命中沿用与 NPC 相同的星云尘埃散射路径。</summary>
		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			SpawnNebulaDust();
		}
		/// <summary>
		/// 在主人端以随机方向生成一枚星云尘埃，并继承当前星星伤害。
		/// </summary>
		private void SpawnNebulaDust()
		{
			if (Projectile.owner != Main.myPlayer || Projectile.ai[1] != 0f)
				return;
			Vector2 velocity = new(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
			velocity.Normalize();
			velocity *= 0.3f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
				, velocity, ModContent.ProjectileType<NebulaDust>(), Projectile.damage, 0f
				, Projectile.owner);
		}
		/// <summary>
		/// 在主贴图上叠加同步旋转与缩放的发光层，强化星云之星的紫白光晕。
		/// </summary>
		public override void PostDraw(Color lightColor)
		{
			Texture2D glow = ModContent.Request<Texture2D>(
				"CalamityRebirth/Content/Projectiles/Typeless/NebulaStarGlow").Value;
			Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null
				, Color.White, Projectile.rotation, new Vector2(17f), Projectile.scale
				, SpriteEffects.None, 0f);
		}
	}
}
