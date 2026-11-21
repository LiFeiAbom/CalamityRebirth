using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>塞壬发射的友方水矛，脉动缩放并穿透一个目标。</summary>
	internal sealed class WaterSpearFriendly : ModProjectile
	{
		/// <summary>设置十四像素碰撞、单穿透、三百帧寿命与初始全透明。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.ignoreWater = true;
			Projectile.minion = true;
			Projectile.minionSlots = 0f;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
			Projectile.alpha = 255;
		}
		/// <summary>生成水花、播放音效，并循环执行缩小淡出与放大淡入。</summary>
		public override void AI()
		{
			Projectile.velocity *= 1.01f;
			if (Projectile.ai[1] == 0f)
			{
				for (int i = 0; i < 5; i++)
				{
					int dustIndex = Dust.NewDust(Projectile.position, Projectile.width
						, Projectile.height, DustID.Water, 0f, 0f, 100, default, 2f);
					Main.dust[dustIndex].velocity *= 3f;
				}
				Projectile.ai[1] = 1f;
				SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
			}
			if (Projectile.localAI[0] == 0f)
			{
				Projectile.scale -= 0.02f;
				Projectile.alpha += 30;
				if (Projectile.alpha >= 250)
				{
					Projectile.alpha = 255;
					Projectile.localAI[0] = 1f;
				}
			}
			else
			{
				Projectile.scale += 0.02f;
				Projectile.alpha -= 30;
				if (Projectile.alpha <= 0)
				{
					Projectile.alpha = 0;
					Projectile.localAI[0] = 0f;
				}
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Lighting.AddLight(Projectile.Center, 0f
				, (255 - Projectile.alpha) * 0.05f / 255f
				, (255 - Projectile.alpha) * 0.35f / 255f);
		}
		/// <summary>消失时生成三粒水尘。</summary>
		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 3; i++)
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width
					, Projectile.height, DustID.Water, Projectile.oldVelocity.X * 0.5f
					, Projectile.oldVelocity.Y * 0.5f);
		}
		/// <summary>按速度方向旋转并应用透明度绘制水矛。</summary>
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null
				, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f
				, Projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
		/// <summary>固定浅灰字色，透明度读取弹幕 alpha。</summary>
		public override Color? GetAlpha(Color lightColor)
			=> new Color(200, 200, 200, Projectile.alpha);
	}
}
