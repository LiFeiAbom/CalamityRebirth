using CalamityRebirth.Content.Projectiles.Typeless;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
	/// <summary>
	/// 星云核心：提供通用输出、漂浮星云之星和低概率致命伤保护的综合专家饰品。
	/// </summary>
	internal sealed class NebulousCore : ModItem
	{
		/// <summary>通用伤害提高比例。</summary>
		public const float DamageBonus = 0.20f;
		/// <summary>通用暴击率提高值。</summary>
		public const int CritBonus = 20;
		/// <summary>致命伤保护触发概率的分母，十表示百分之十。</summary>
		public const int ReviveChanceDenominator = 10;
		/// <summary>致命伤保护触发时回复的生命值。</summary>
		public const int ReviveHeal = 100;
		/// <summary>每枚星云之星的通用伤害基础值。</summary>
		public const int StarBaseDamage = 300;
		/// <summary>同一玩家同时存在的星云之星上限。</summary>
		public const int MaxStars = 20;
		/// <summary>星云之星生成判定的概率分母。</summary>
		public const int StarSpawnChanceDenominator = 15;
		/// <summary>星云之星生成时检查的初始碰撞区域边长。</summary>
		private const int StarSpawnHitbox = 24;
		/// <summary>星云之星二次空间检查使用的边长。</summary>
		private const int StarClearanceHitbox = 90;
		/// <summary>星云之星之间的最小生成间距。</summary>
		private const float StarMinimumSeparation = 48f;
		/// <summary>
		/// 设置物品判定尺寸、现代紫色档位价格、专家标记和饰品类型。
		/// </summary>
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 14;
			RarityUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>());
			Item.expert = true;
			Item.accessory = true;
		}
		/// <summary>
		/// 物品掉落在世界中时发出与星云物品一致的紫色微光。
		/// </summary>
		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			float pulse = Main.rand.Next(90, 111) * 0.01f * Main.essScale;
			int lightX = (int)((Item.position.X + Item.width / 2f) / 16f);
			int lightY = (int)((Item.position.Y + Item.height / 2f) / 16f);
			Lighting.AddLight(lightX, lightY, 0.35f * pulse, 0.05f * pulse, 0.35f * pulse);
		}
		/// <summary>
		/// 点亮装备状态，并在可见时尝试生成一枚漂浮的星云之星。
		/// 隐藏外观仍保留全部属性，但停止生成星星，已有星星会在下一帧消散。
		/// </summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			modPlayer.nebulousCore = true;
			modPlayer.nebulousCoreVisible = !hideVisual;
			if (hideVisual || player.whoAmI != Main.myPlayer
				|| !Main.rand.NextBool(StarSpawnChanceDenominator))
				return;
			int starCount = 0;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active && projectile.owner == player.whoAmI
					&& projectile.type == ModContent.ProjectileType<NebulaStar>())
					starCount++;
			}
			// 星云之星越多，再次生成的成功率越低；达到二十枚后完全停止生成。
			if (starCount >= MaxStars
				|| Main.rand.Next(StarSpawnChanceDenominator) < starCount)
				return;
			int damage = (int)player.GetDamage(DamageClass.Generic).ApplyTo(StarBaseDamage);
			for (int attempt = 0; attempt < 50; attempt++)
			{
				// 随着失败次数增加逐步扩大搜索半径，优先在玩家附近寻找有效落点。
				int offset = Main.rand.Next(200 - attempt * 2, 400 + attempt * 2);
				Vector2 center = player.Center;
				center.X += Main.rand.Next(-offset, offset + 1);
				center.Y += Main.rand.Next(-offset, offset + 1);
				if (Collision.SolidCollision(center, StarSpawnHitbox, StarSpawnHitbox)
					|| Collision.WetCollision(center, StarSpawnHitbox, StarSpawnHitbox))
					continue;
				center.X += StarSpawnHitbox / 2;
				center.Y += StarSpawnHitbox / 2;
				if (!Collision.CanHit(new Vector2(player.Center.X, player.position.Y)
					, 1, 1, center, 1, 1)
					&& !Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f)
						, 1, 1, center, 1, 1))
					continue;
				int tileX = (int)center.X / 16;
				int tileY = (int)center.Y / 16;
				if (!WorldGen.InWorld(tileX, tileY, 1))
					continue;
				bool validSpawn = Main.rand.Next(3) == 0
					&& Main.tile[tileX, tileY].WallType > WallID.None;
				if (!validSpawn)
				{
					center.X -= StarClearanceHitbox / 2;
					center.Y -= StarClearanceHitbox / 2;
					if (Collision.SolidCollision(center, StarClearanceHitbox
						, StarClearanceHitbox))
					{
						center.X += StarClearanceHitbox / 2;
						center.Y += StarClearanceHitbox / 2;
						validSpawn = true;
					}
				}
				if (!validSpawn)
					continue;
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile existing = Main.projectile[i];
					if (!existing.active || existing.owner != player.whoAmI
						|| existing.type != ModContent.ProjectileType<NebulaStar>())
						continue;
					if ((center - existing.Center).Length() < StarMinimumSeparation)
					{
						validSpawn = false;
						break;
					}
				}
				if (!validSpawn)
					continue;
				Projectile.NewProjectile(player.GetSource_Accessory(Item), center, Vector2.Zero
					, ModContent.ProjectileType<NebulaStar>(), damage, 3f, player.whoAmI);
				return;
			}
		}
	}
}
