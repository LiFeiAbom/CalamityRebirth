using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
	/// <summary>生命露（古）：继承蜜露（古）并额外覆盖火系减益。</summary>
	internal sealed class LivingDew2 : ModItem
	{
		/// <summary>设置 34×22 判定、48 金价格、黄绿色稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 22;
			Item.value = Item.buyPrice(0, 48, 0, 0);
			Item.rare = ItemRarityID.Lime;
			Item.accessory = true;
		}
		/// <summary>提高 50 最大生命，并继承蜜露（古）的全部标记。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.statLifeMax2 += 50;
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			modPlayer.alwaysHoneyRegen = true;
			modPlayer.honeyTurboRegen = true;
			modPlayer.honeyDewHalveDebuffs = true;
			modPlayer.livingDewHalveDebuffs = true;
		}
		/// <summary>现代版使用藤蔓替代已删除的捕蝇草球茎。</summary>
		public override void AddRecipes()
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
				&& modern.TryFind<ModItem>("LivingShard", out ModItem modernShard)
				&& modern.TryFind<ModItem>("EssenceofSunlight", out ModItem modernEssence))
			{
				CreateRecipe()
					.AddIngredient<HoneyDew2>()
					.AddIngredient(ItemID.Vine, 3)
					.AddIngredient(modernShard.Type, 6)
					.AddIngredient(modernEssence.Type, 5)
					.AddTile(TileID.MythrilAnvil)
					.Register();
			}
			AddClassicRecipe("CalamityModClassicPreTrailer");
			AddClassicRecipe("CalRD");
		}
		/// <summary>经典两版使用原生捕蝇草球茎、生命碎片与焦火精华。</summary>
		private void AddClassicRecipe(string modName)
		{
			if (!ModLoader.TryGetMod(modName, out Mod mod)
				|| !mod.TryFind<ModItem>("TrapperBulb", out ModItem trapperBulb)
				|| !mod.TryFind<ModItem>("LivingShard", out ModItem livingShard)
				|| !mod.TryFind<ModItem>("EssenceofCinder", out ModItem essence))
				return;
			CreateRecipe()
				.AddIngredient<HoneyDew2>()
				.AddIngredient(trapperBulb.Type, 3)
				.AddIngredient(livingShard.Type, 6)
				.AddIngredient(essence.Type, 5)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
