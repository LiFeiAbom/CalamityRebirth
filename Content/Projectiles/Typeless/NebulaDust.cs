using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
	/// <summary>
	/// 星云尘埃：星云之星命中后生成的短寿命隐身伤害区域。
	/// </summary>
	internal sealed class NebulaDust : ModProjectile
	{
		/// <summary>
		/// 设置三十二像素碰撞尺寸、无限穿透和三帧本地 NPC 命中冷却。
		/// </summary>
		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 3600;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 3;
		}
		/// <summary>
		/// 按水平与垂直速度旋转并减速，前六十帧淡入，之后逐渐淡出并销毁。
		/// </summary>
		public override void AI()
		{
			Projectile.rotation += Projectile.velocity.X * 0.02f;
			if (Projectile.velocity.X < 0f)
				Projectile.rotation -= Math.Abs(Projectile.velocity.Y) * 0.02f;
			else
				Projectile.rotation += Math.Abs(Projectile.velocity.Y) * 0.02f;
			Projectile.velocity *= 0.98f;
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] >= 60f)
			{
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
			if (Projectile.alpha > 80)
			{
				Projectile.alpha -= 30;
				if (Projectile.alpha < 80)
					Projectile.alpha = 80;
			}
		}
	}
}
