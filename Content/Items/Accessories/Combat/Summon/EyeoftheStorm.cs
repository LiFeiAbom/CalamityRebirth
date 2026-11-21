using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CloudyWaifuBuff = CalamityRebirth.Content.ModBuffs.Buffs.CloudyWaifu;
using CloudyWaifuProjectile = CalamityRebirth.Content.Projectiles.Summon.CloudyWaifu;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
	/// <summary>风暴之眼：召唤云元素并使用冲锋攻击。</summary>
	internal sealed class EyeoftheStorm : ModItem
	{
		/// <summary>设置二十乘二十六判定、粉红稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.value = Item.buyPrice(0, 15, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.accessory = true;
		}
		/// <summary>点亮风暴之眼状态；本地客户端维持云元素和对应召唤增益。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().eyeoftheStorm = true;
			if (player.whoAmI != Main.myPlayer)
				return;
			if (player.FindBuffIndex(ModContent.BuffType<CloudyWaifuBuff>()) == -1)
				player.AddBuff(ModContent.BuffType<CloudyWaifuBuff>(), 3600, true);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<CloudyWaifuProjectile>()] >= 1)
				return;
			int damage = (int)(45f * player.GetDamage(DamageClass.Summon).Multiplicative);
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, -Vector2.UnitY
				, ModContent.ProjectileType<CloudyWaifuProjectile>(), damage, 2f, player.whoAmI);
		}
		/// <summary>禁止与元灵之心同时装备。</summary>
		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem
			, Player player)
			=> incomingItem.type != ModContent.ItemType<HeartoftheElements>();
	}
}
