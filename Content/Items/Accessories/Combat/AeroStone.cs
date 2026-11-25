using CalamityRebirth.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
	/// <summary>
	/// 气之石：提供移动、跳跃和通用输出加成的元素石。
	/// </summary>
	internal sealed class AeroStone : ModItem
	{
		/// <summary>注册八帧竖直灵魂动画。</summary>
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		/// <summary>设置二十像素判定、浅红稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 15, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}
		/// <summary>点亮气之石标记；属性与照明由玩家结算类统一应用。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().aeroStone = true;
		}
	}
}
