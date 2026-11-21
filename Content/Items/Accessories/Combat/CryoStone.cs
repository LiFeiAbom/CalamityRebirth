using CalamityRebirth.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
	/// <summary>
	/// 寒晶石：提供防御、减伤和通用输出加成的元素石。
	/// </summary>
	internal sealed class CryoStone : ModItem
	{
		/// <summary>注册五帧竖直灵魂动画。</summary>
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 5));
			ItemID.Sets.AnimatesAsSoul[Type] = true;
		}
		/// <summary>设置二十像素判定、六点防御、粉红稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 15, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.defense = 6;
			Item.accessory = true;
		}
		/// <summary>点亮寒晶石标记；属性与蓝色照明由玩家结算类统一应用。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().cryoStone = true;
		}
	}
}
