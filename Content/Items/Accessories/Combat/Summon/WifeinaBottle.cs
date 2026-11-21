using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SandyWaifuBuff = CalamityRebirth.Content.ModBuffs.Buffs.SandyWaifu;
using SandyWaifuProjectile = CalamityRebirth.Content.Projectiles.Summon.SandyWaifu;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
	/// <summary>瓶装沙元素：召唤沙元素并使用沙之箭与沙暴攻击。</summary>
	internal sealed class WifeinaBottle : ModItem
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
		/// <summary>点亮瓶装沙元素状态；本地客户端维持沙元素和对应召唤增益。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().wifeinaBottle = true;
			if (player.whoAmI != Main.myPlayer)
				return;
			if (player.FindBuffIndex(ModContent.BuffType<SandyWaifuBuff>()) == -1)
				player.AddBuff(ModContent.BuffType<SandyWaifuBuff>(), 3600, true);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SandyWaifuProjectile>()] >= 1)
				return;
			int damage = (int)(45f * player.GetDamage(DamageClass.Summon).Multiplicative);
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, -Vector2.UnitY
				, ModContent.ProjectileType<SandyWaifuProjectile>(), damage, 2f, player.whoAmI);
		}
		/// <summary>禁止与元灵之心同时装备。</summary>
		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem
			, Player player)
			=> incomingItem.type != ModContent.ItemType<HeartoftheElements>();
	}
}
