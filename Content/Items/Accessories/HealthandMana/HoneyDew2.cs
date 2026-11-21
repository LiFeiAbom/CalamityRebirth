using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
	/// <summary>蜜露（古）：古老蜂蜜链的起始件，提供常驻蜂蜜再生与病症抗性。</summary>
	internal sealed class HoneyDew2 : ModItem
	{
		/// <summary>设置 20×20 判定、2 金价格、绿色稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}
		/// <summary>提高 30 最大生命，并点亮蜂蜜再生、站桩再生与病症减半标记。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statLifeMax2 += 30;
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			modPlayer.alwaysHoneyRegen = true;
			modPlayer.honeyTurboRegen = true;
			modPlayer.honeyDewHalveDebuffs = true;
		}
		/// <summary>
		/// 经典两版使用原生污浊糊；现代版已删除该材料，因此直接展开为丛林孢子。
		/// </summary>
		public override void AddRecipes()
		{
			if (ModLoader.TryGetMod("CalamityMod", out _))
			{
				CreateRecipe()
					.AddIngredient(ItemID.BottledHoney, 10)
					.AddIngredient(ItemID.BeeWax, 3)
					.AddIngredient(ItemID.JungleSpores, 3)
					.AddTile(TileID.Anvils)
					.Register();
			}
			AddNativeMurkyPasteRecipe("CalamityModClassicPreTrailer");
			AddNativeMurkyPasteRecipe("CalRD");
		}
		/// <summary>使用指定经典分支原生的污浊糊注册标准配方。</summary>
		private void AddNativeMurkyPasteRecipe(string modName)
		{
			if (!ModLoader.TryGetMod(modName, out Mod mod)
				|| !mod.TryFind<ModItem>("MurkyPaste", out ModItem murkyPaste))
				return;
			CreateRecipe()
				.AddIngredient(ItemID.BottledHoney, 10)
				.AddIngredient(ItemID.BeeWax, 3)
				.AddIngredient(murkyPaste.Type, 3)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
