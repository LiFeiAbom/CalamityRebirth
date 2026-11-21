using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Healing
{
	/// <summary>治疗型沙元素生成的隐形治疗珠，接触主人时回复十五点生命。</summary>
	internal sealed class CactusHealOrb : ModProjectile
	{
		/// <summary>使用通用隐形贴图，视觉效果由绿色尘提供。</summary>
		public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
		/// <summary>设置八像素碰撞、单穿透、全透明和一百八十帧寿命。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 180;
		}
		/// <summary>减速追踪主人，接触后仅在主人端治疗并同步一次灵体治疗消息。</summary>
		public override void AI()
		{
			Projectile.velocity.Y *= 0.985f;
			int owner = Projectile.owner;
			if (!Main.player.IndexInRange(owner) || !Main.player[owner].active
				|| Main.player[owner].dead)
			{
				Projectile.Kill();
				return;
			}
			Player player = Main.player[owner];
			Rectangle expandedPlayer = player.Hitbox;
			expandedPlayer.Inflate(50, 50);
			if (Projectile.Hitbox.Intersects(expandedPlayer))
			{
				if (owner == Main.myPlayer)
				{
					const int heal = 15;
					player.HealEffect(heal);
					player.statLife += heal;
					if (player.statLife > player.statLifeMax2)
						player.statLife = player.statLifeMax2;
					NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, owner, heal);
				}
				Projectile.Kill();
				return;
			}
			int dustIndex = Dust.NewDust(Projectile.position, Projectile.width
				, Projectile.height, DustID.Terra, 0f, 0f, 100, new Color(0, 200, 0), 1.5f);
			Main.dust[dustIndex].noGravity = true;
			Main.dust[dustIndex].velocity = -Projectile.velocity * 0.2f;
		}
	}
}
