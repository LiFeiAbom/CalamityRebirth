using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DrewsSandyWaifuBuff = CalamityRebirth.Content.ModBuffs.Buffs.DrewsSandyWaifu;
using DrewsSandyWaifuProjectile = CalamityRebirth.Content.Projectiles.Summon.DrewsSandyWaifu;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
	/// <summary>稀有瓶装沙元素：召唤持续治疗玩家的 Drew 版沙元素。</summary>
	internal sealed class WifeinaBottlewithBoobs : ModItem
	{
		/// <summary>设置二十乘二十六判定、专家标记与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.value = Item.buyPrice(0, 15, 0, 0);
			Item.expert = true;
			Item.accessory = true;
		}
		/// <summary>点亮稀有瓶装沙元素状态；本地客户端维持治疗沙元素和对应增益。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().wifeinaBottlewithBoobs = true;
			if (player.whoAmI != Main.myPlayer)
				return;
			if (player.FindBuffIndex(ModContent.BuffType<DrewsSandyWaifuBuff>()) == -1)
				player.AddBuff(ModContent.BuffType<DrewsSandyWaifuBuff>(), 3600, true);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<DrewsSandyWaifuProjectile>()] >= 1)
				return;
			int damage = (int)(45f * player.GetDamage(DamageClass.Summon).Multiplicative);
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, -Vector2.UnitY
				, ModContent.ProjectileType<DrewsSandyWaifuProjectile>(), damage, 2f, player.whoAmI);
		}
		/// <summary>禁止与元灵之心同时装备。</summary>
		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem
			, Player player)
			=> incomingItem.type != ModContent.ItemType<HeartoftheElements>();
	}
}
