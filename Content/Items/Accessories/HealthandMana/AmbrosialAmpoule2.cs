using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
	/// <summary>甘露安瓿（古）：完整古老蜂蜜链顶点，提供最大生命与全部蜂蜜增益。</summary>
	internal sealed class AmbrosialAmpoule2 : ModItem
	{
		/// <summary>设置 20×20 判定、80 金价格、青色稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 80, 0, 0);
			Item.rare = ItemRarityID.Cyan;
			Item.accessory = true;
		}
		/// <summary>提高 70 最大生命，继承整条蜂蜜链，并在未装备上位件时发出暖黄光。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statLifeMax2 += 70;
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			modPlayer.aAmpoule = true;
			modPlayer.alwaysHoneyRegen = true;
			modPlayer.honeyTurboRegen = true;
			modPlayer.honeyDewHalveDebuffs = true;
			modPlayer.livingDewHalveDebuffs = true;
			if (!(modPlayer.rOoze || modPlayer.purity) && !hideVisual)
				Lighting.AddLight(player.Center, new Vector3(1.2f, 1.2f, 0.72f));
		}
		/// <summary>注册现代生命合金与经典生命锭两种配方。</summary>
		public override void AddRecipes()
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
				&& modern.TryFind<ModItem>("LifeAlloy", out ModItem lifeAlloy))
			{
				CreateRecipe()
					.AddIngredient<LivingDew2>()
					.AddIngredient<RadiantOoze2>()
					.AddIngredient(lifeAlloy.Type, 3)
					.AddTile(TileID.LunarCraftingStation)
					.Register();
			}
			AddClassicRecipe("CalamityModClassicPreTrailer");
			AddClassicRecipe("CalRD");
		}
		/// <summary>经典版使用原生生命锭。</summary>
		private void AddClassicRecipe(string modName)
		{
			if (!ModLoader.TryGetMod(modName, out Mod mod)
				|| !mod.TryFind<ModItem>("BarofLife", out ModItem barofLife))
				return;
			CreateRecipe()
				.AddIngredient<LivingDew2>()
				.AddIngredient<RadiantOoze2>()
				.AddIngredient(barofLife.Type, 3)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}
