using CalamityRebirth.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Magic
{
	/// <summary>
	/// 混沌石：提供最大魔力、魔力消耗和通用输出加成的法师元素石。
	/// </summary>
	internal sealed class ChaosStone : ModItem
	{
		/// <summary>注册四帧竖直灵魂动画。</summary>
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		/// <summary>设置二十像素判定、粉红稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 15, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.accessory = true;
		}
		/// <summary>点亮混沌石标记；属性与红色照明由玩家结算类统一应用。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().chaosStone = true;
		}
	}
}
