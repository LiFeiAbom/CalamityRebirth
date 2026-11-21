using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>沙之印记生成的竖直沙暴，前三十帧造成伤害后快速淡出。</summary>
	internal sealed class SandTornado : ModProjectile
	{
		/// <summary>使用通用沙暴贴图。</summary>
		public override string Texture => "CalamityRebirth/Content/Projectiles/TornadoProj";
		/// <summary>设置初始碰撞、穿透数量、寿命和按进度缩短的本地命中冷却。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.tileCollide = false;
			Projectile.minion = true;
			Projectile.penetrate = NPC.downedMoonlord ? -1 : 3;
			Projectile.timeLeft = 1200;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = ElementalMinionUtil.GetHitCooldown();
		}
		/// <summary>扩展为竖直长碰撞体，在前三十帧保留伤害，随后进入淡出尾段。</summary>
		public override void AI()
		{
			if (Projectile.soundDelay == 0)
			{
				Projectile.soundDelay = -1;
				SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
			}
			Projectile.ai[0] += 1f;
			Projectile.localAI[0] += 1f;
			if (Projectile.localAI[0] >= 30f)
			{
				Projectile.damage = 0;
				if (Projectile.ai[0] < 180f)
					Projectile.ai[0] = 180f + Projectile.ai[0] % 60f;
			}
			Point centerTile = Projectile.Center.ToTileCoordinates();
			Collision.ExpandVertically(centerTile.X, centerTile.Y, out int top
				, out int bottom, 15, 15);
			top++;
			bottom--;
			Vector2 topWorld = new Vector2(centerTile.X, top) * 16f + new Vector2(8f);
			Vector2 bottomWorld = new Vector2(centerTile.X, bottom) * 16f + new Vector2(8f);
			Vector2 center = Vector2.Lerp(topWorld, bottomWorld, 0.5f);
			float height = bottomWorld.Y - topWorld.Y;
			Projectile.width = (int)(height * 0.2f * 0.65f);
			Projectile.height = (int)height;
			Projectile.Center = center;
			if (Projectile.ai[0] >= 300f)
				Projectile.Kill();
		}
		/// <summary>沿沙暴高度重复绘制旋转沙柱，并按首尾时间缩放淡出。</summary>
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 origin = texture.Size() / 2f;
			float life = Projectile.ai[0];
			float scale = MathHelper.Clamp(life / 30f, 0f, 1f);
			if (life > 240f)
				scale = MathHelper.Lerp(1f, 0f, (life - 240f) / 60f);
			Point tile = Projectile.Center.ToTileCoordinates();
			Collision.ExpandVertically(tile.X, tile.Y, out int top, out int bottom, 15, 15);
			top++;
			bottom--;
			for (float y = bottom * 16f; y > top * 16f; y -= 5.1f)
			{
				float ratio = (bottom * 16f - y)
					/ MathHelper.Max(1f, (bottom - top) * 16f);
				float alpha = ratio > 0.5f ? (1f - ratio) * 2f : ratio * 2f;
				Color color = new Color(225, 225, 100, 128) * alpha * scale;
				float rotation = -0.0628f * life + ratio * MathHelper.TwoPi * 4f;
				Vector2 position = new Vector2(tile.X * 16f + 8f, y) - Main.screenPosition;
				Main.spriteBatch.Draw(texture, position, null, color, rotation, origin
					, new Vector2(1f + ratio * 0.3f), SpriteEffects.None, 0f);
			}
			return false;
		}
	}
}
