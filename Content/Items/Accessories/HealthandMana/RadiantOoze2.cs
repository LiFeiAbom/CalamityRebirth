using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
	/// <summary>光辉软泥（古）：按已损失生命提供动态再生。</summary>
	internal sealed class RadiantOoze2 : ModItem
	{
		/// <summary>设置 20×20 判定、12 金价格、浅红稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 12, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}
		/// <summary>点亮动态再生标记，并在未装备上位件且可见时发出暖黄光。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			modPlayer.rOoze = true;
			if (!(modPlayer.aAmpoule || modPlayer.purity) && !hideVisual)
				Lighting.AddLight(player.Center, new Vector3(1f, 1f, 0.6f));
		}
		/// <summary>注册现代污化凝胶配方以及两个经典分支的纯凝胶配方。</summary>
		public override void AddRecipes()
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
				&& modern.TryFind<ModItem>("BlightedGel", out ModItem blightedGel)
				&& modern.TryFind<ModItem>("PurifiedGel", out ModItem modernPurifiedGel))
			{
				CreateRecipe()
					.AddIngredient(blightedGel.Type, 45)
					.AddIngredient(modernPurifiedGel.Type, 15)
					.AddTile(TileID.Anvils)
					.Register();
			}
			AddClassicRecipe("CalamityModClassicPreTrailer");
			AddClassicRecipe("CalRD");
		}
		/// <summary>经典版使用六十份原生纯凝胶。</summary>
		private void AddClassicRecipe(string modName)
		{
			if (!ModLoader.TryGetMod(modName, out Mod mod)
				|| !mod.TryFind<ModItem>("PurifiedGel", out ModItem purifiedGel))
				return;
			CreateRecipe()
				.AddIngredient(purifiedGel.Type, 60)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
