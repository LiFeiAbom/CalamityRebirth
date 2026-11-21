using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Projectiles.Summon;
using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
	/// <summary>玫瑰石：提供生命、再生和通用输出，并召唤一只硫火元素。</summary>
	internal sealed class RoseStone : ModItem
	{
		/// <summary>设置二十像素判定、粉红稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 15, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.accessory = true;
		}
		/// <summary>点亮玫瑰石状态；可见时维持硫火元素及召唤增益。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			modPlayer.roseStone = true;
			modPlayer.roseStoneVisible = !hideVisual;
			if (hideVisual || player.whoAmI != Main.myPlayer)
				return;
			if (player.FindBuffIndex(ModContent.BuffType<BrimstoneWaifu>()) == -1)
				player.AddBuff(ModContent.BuffType<BrimstoneWaifu>(), 3600, true);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<BigBustyRose>()] >= 1)
				return;
			int damage = (int)(45f * player.GetDamage(DamageClass.Summon).Multiplicative);
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, -Vector2.UnitY
				, ModContent.ProjectileType<BigBustyRose>(), damage, 2f, player.whoAmI);
		}
		/// <summary>禁止与包含全部元素仆从的元灵之心同时装备。</summary>
		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem
			, Player player)
			=> incomingItem.type != ModContent.ItemType<HeartoftheElements>();
	}
}
