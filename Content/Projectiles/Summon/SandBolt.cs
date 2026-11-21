using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
	/// <summary>沙元素发射的隐形沙之箭，消失时生成沙之印记。</summary>
	internal sealed class SandBolt : ModProjectile
	{
		/// <summary>使用通用隐形贴图，轨迹由沙尘表现。</summary>
		public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
		/// <summary>设置十像素碰撞、单穿透、三百帧寿命与召唤伤害。</summary>
		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
			Projectile.light = 1f;
		}
		/// <summary>沿飞行路径生成沙尘尾迹。</summary>
		public override void AI()
		{
			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width
				, Projectile.height, DustID.Sand, Projectile.velocity.X * 0.1f
				, Projectile.velocity.Y * 0.1f);
		}
		/// <summary>只在主人端生成沙之印记。</summary>
		public override void OnKill(int timeLeft)
		{
			if (Projectile.owner != Main.myPlayer)
				return;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
				, Vector2.Zero, ModContent.ProjectileType<SandMark>(), Projectile.damage
				, Projectile.knockBack, Projectile.owner);
		}
	}
}
