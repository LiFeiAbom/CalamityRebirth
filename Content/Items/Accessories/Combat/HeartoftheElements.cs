using CalamityRebirth.Content.Projectiles.Summon;
using CalamityRebirth.Content.Items.Accessories.Combat.Magic;
using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
	/// <summary>
	/// 元灵之心：四枚元素石与五件元素召唤饰品的综合顶点。
	/// 可见时召唤五种元素仆从；隐藏时保留较高基础属性并关闭召唤物。
	/// </summary>
	internal sealed class HeartoftheElements : ModItem
	{
		/// <summary>未击败月亮领主时的单个仆从基础伤害。</summary>
		public const int EarlyElementalDamage = 100;
		/// <summary>击败月亮领主后的单个仆从基础伤害。</summary>
		public const int MoonLordElementalDamage = 300;
		/// <summary>击败神明吞噬者后的仆从伤害倍率。</summary>
		public const float DevourerDamageMultiplier = 2.5f;
		/// <summary>注册八帧竖直灵魂动画。</summary>
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		/// <summary>设置二十像素判定、九点防御、现代红色稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.defense = 9;
			RarityUtil.Apply(Item, ItemRarityID.Red, Item.buyPrice(0, 80, 0, 0));
			Item.accessory = true;
		}
		/// <summary>
		/// 点亮合集状态；可见时补挂专用增益并维持五种元素仆从各一只。
		/// 隐藏时只保留属性并让已有仆从在下一帧自行消失。
		/// </summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			modPlayer.heartoftheElements = true;
			modPlayer.heartoftheElementsHideVisual = hideVisual;
			modPlayer.allWaifus = !hideVisual;
			if (hideVisual || player.whoAmI != Main.myPlayer || player.dead)
				return;
			TrimExtraMinions(player, ModContent.ProjectileType<BigBustyRose>());
			TrimExtraMinions(player, ModContent.ProjectileType<SirenLure>());
			TrimExtraMinions(player, ModContent.ProjectileType<DrewsSandyWaifu>());
			TrimExtraMinions(player, ModContent.ProjectileType<SandyWaifu>());
			TrimExtraMinions(player, ModContent.ProjectileType<CloudyWaifu>());
			if (player.FindBuffIndex(ModContent.BuffType<Content.ModBuffs.Buffs.HotE>()) == -1)
				player.AddBuff(ModContent.BuffType<Content.ModBuffs.Buffs.HotE>(), 3600, true);
			int baseDamage = NPC.downedMoonlord ? MoonLordElementalDamage : EarlyElementalDamage;
			float progression = CalamityCompatUtil.IsDevourerOfGodsDowned()
				? DevourerDamageMultiplier : 1f;
			int damage = (int)(baseDamage * progression
				* player.GetTotalDamage(DamageClass.Summon).Multiplicative);
			SpawnElemental(player, ModContent.ProjectileType<BigBustyRose>(), damage);
			SpawnElemental(player, ModContent.ProjectileType<SirenLure>(), damage);
			SpawnElemental(player, ModContent.ProjectileType<DrewsSandyWaifu>(), damage);
			SpawnElemental(player, ModContent.ProjectileType<SandyWaifu>(), damage);
			SpawnElemental(player, ModContent.ProjectileType<CloudyWaifu>(), damage);
		}
		/// <summary>元灵之心与五件可重复叠加的元素召唤饰品禁止同时装备。</summary>
		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem
			, Player player)
		{
			return incomingItem.type != ModContent.ItemType<RoseStone>()
				&& incomingItem.type != ModContent.ItemType<EyeoftheStorm>()
				&& incomingItem.type != ModContent.ItemType<LureofEnthrallment>()
				&& incomingItem.type != ModContent.ItemType<WifeinaBottle>()
				&& incomingItem.type != ModContent.ItemType<WifeinaBottlewithBoobs>();
		}
		/// <summary>使用四枚元素石与五件元素召唤饰品在远古操纵机合成。</summary>
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<WifeinaBottle>()
				.AddIngredient<WifeinaBottlewithBoobs>()
				.AddIngredient<LureofEnthrallment>()
				.AddIngredient<EyeoftheStorm>()
				.AddIngredient<RoseStone>()
				.AddIngredient<AeroStone>()
				.AddIngredient<ChaosStone>()
				.AddIngredient<CryoStone>()
				.AddIngredient<BloomStone>()
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
		/// <summary>在本地客户端补齐指定元素仆从，并写入用于召唤伤害追踪的原生基础伤害。</summary>
		private void SpawnElemental(Player player, int projectileType, int damage)
		{
			if (player.ownedProjectileCounts[projectileType] >= 1)
				return;
			int index = Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center
				, -Vector2.UnitY, projectileType, damage, 2f, player.whoAmI);
			if (!Main.projectile.IndexInRange(index))
				return;
			Projectile projectile = Main.projectile[index];
			projectile.originalDamage = damage;
		}
		/// <summary>异常帧序造成重复仆从时，仅保留最早存在的一只。</summary>
		private static void TrimExtraMinions(Player player, int projectileType)
		{
			if (player.ownedProjectileCounts[projectileType] <= 1)
				return;
			int alive = 0;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (!projectile.active || projectile.owner != player.whoAmI
					|| projectile.type != projectileType)
					continue;
				alive++;
				if (alive > 1)
					projectile.Kill();
			}
		}
	}
}
