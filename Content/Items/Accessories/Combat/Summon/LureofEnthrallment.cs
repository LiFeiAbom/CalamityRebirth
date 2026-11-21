using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SirenLureBuff = CalamityRebirth.Content.ModBuffs.Buffs.SirenLure;
using SirenLureProjectile = CalamityRebirth.Content.Projectiles.Summon.SirenLure;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
	/// <summary>魅惑之饵：召唤固定在玩家上方的塞壬，并交替发射水矛、霜雾与歌声。</summary>
	internal sealed class LureofEnthrallment : ModItem
	{
		/// <summary>设置五十六像素判定、黄绿稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 56;
			Item.height = 56;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.Lime;
			Item.accessory = true;
		}
		/// <summary>点亮魅惑之饵状态；本地客户端维持塞壬和对应召唤增益。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().lureofEnthrallment = true;
			if (player.whoAmI != Main.myPlayer)
				return;
			if (player.FindBuffIndex(ModContent.BuffType<SirenLureBuff>()) == -1)
				player.AddBuff(ModContent.BuffType<SirenLureBuff>(), 3600, true);
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SirenLureProjectile>()] >= 1)
				return;
			int damage = (int)(65f * player.GetDamage(DamageClass.Summon).Multiplicative);
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, -Vector2.UnitY
				, ModContent.ProjectileType<SirenLureProjectile>(), damage, 2f, player.whoAmI);
		}
		/// <summary>禁止与元灵之心同时装备。</summary>
		public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem
			, Player player)
			=> incomingItem.type != ModContent.ItemType<HeartoftheElements>();
	}
}
