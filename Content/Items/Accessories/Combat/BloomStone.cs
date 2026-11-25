using CalamityRebirth.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
	/// <summary>
	/// 花绽石：提供少量通用输出，并周期性诅咒周围敌怪、在玩家脚下生长植物。
	/// </summary>
	internal sealed class BloomStone : ModItem
	{
		/// <summary>注册七帧竖直灵魂动画。</summary>
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		/// <summary>设置二十像素判定、青柠稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 15, 0, 0);
			Item.rare = ItemRarityID.Lime;
			Item.accessory = true;
		}
		/// <summary>点亮花绽石标记；光环和植物生长由玩家结算类统一处理。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().bloomStone = true;
		}
	}
}
