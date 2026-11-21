using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>塞壬发射的友方霜雾，短暂追踪最近玩家并脉动透明。</summary>
	internal sealed class FrostMistFriendly : ModProjectile
	{
		/// <summary>注册六帧动画。</summary>
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 6;
		}
		/// <summary>设置二十像素碰撞、单穿透、三百帧寿命和初始全透明。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.ignoreWater = true;
			Projectile.minion = true;
			Projectile.minionSlots = 0f;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
			Projectile.alpha = 255;
		}
		/// <summary>维护动画、音效、脉动透明和第三十至一百一十帧的轻微追踪。</summary>
		public override void AI()
		{
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 4)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame > 3)
				Projectile.frame = 0;
			if (Projectile.ai[1] == 0f)
			{
				for (int i = 0; i < 5; i++)
				{
					int dustIndex = Dust.NewDust(Projectile.position, Projectile.width
						, Projectile.height, DustID.BlueFlare, 0f, 0f, 100, default, 2f);
					Main.dust[dustIndex].velocity *= 3f;
				}
				Projectile.ai[1] = 1f;
				SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
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
			Projectile.ai[1] += 1f;
			if (Projectile.ai[1] > 30f && Projectile.ai[1] < 110f)
			{
				int closest = (int)Player.FindClosest(Projectile.Center, 1, 1);
				float speed = Projectile.velocity.Length();
				Vector2 desired = Main.player[closest].Center - Projectile.Center;
				desired.Normalize();
				desired *= speed;
				Projectile.velocity = (Projectile.velocity * 24f + desired) / 25f;
				Projectile.velocity.Normalize();
				Projectile.velocity *= speed;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Lighting.AddLight(Projectile.Center, 0f
				, (255 - Projectile.alpha) * 0.35f / 255f
				, (255 - Projectile.alpha) * 0.35f / 255f);
		}
		/// <summary>按当前动画帧裁切并绘制霜雾。</summary>
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			int frameHeight = texture.Height / Main.projFrames[Type];
			Rectangle frame = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition
				+ Vector2.UnitY * Projectile.gfxOffY, frame, Projectile.GetAlpha(lightColor)
				, Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f)
				, Projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
		/// <summary>固定浅灰字色，透明度读取弹幕 alpha。</summary>
		public override Color? GetAlpha(Color lightColor)
			=> new Color(200, 200, 200, Projectile.alpha);
	}
}
