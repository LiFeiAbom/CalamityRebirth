using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>塞壬发射的友方歌声，缓慢减速、脉动缩放并发出蓝光。</summary>
	internal sealed class SirenSongFriendly : ModProjectile
	{
		/// <summary>设置二十六像素碰撞、单穿透、六百帧寿命与召唤伤害。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
			Projectile.minionSlots = 0f;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
		}
		/// <summary>减速、循环缩放在零点七五到一点二五之间，并在首次更新播放音效。</summary>
		public override void AI()
		{
			Projectile.velocity *= 0.985f;
			if (Projectile.localAI[0] == 0f)
			{
				Projectile.scale += 0.02f;
				if (Projectile.scale >= 1.25f)
					Projectile.localAI[0] = 1f;
			}
			else
			{
				Projectile.scale -= 0.02f;
				if (Projectile.scale <= 0.75f)
					Projectile.localAI[0] = 0f;
			}
			if (Projectile.ai[1] == 0f)
			{
				Projectile.ai[1] = 1f;
				SoundEngine.PlaySound(SoundID.Item26, Projectile.position);
			}
			Lighting.AddLight(Projectile.Center, 0f, 0f, 1.2f);
		}
		/// <summary>使用非预乘白色实现半透明叠加绘制。</summary>
		public override Color? GetAlpha(Color lightColor)
			=> new Color(255, 255, 255, 0);
	}
}
