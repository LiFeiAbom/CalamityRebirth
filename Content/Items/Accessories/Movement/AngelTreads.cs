using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
	/// <summary>天使之靴：炽天使翼靴链的第一级移动饰品。</summary>
	[AutoloadEquip(EquipType.Shoes)]
	internal sealed class AngelTreads : ModItem
	{
		/// <summary>设置 36×34 判定、35 金价格、浅紫稀有度与鞋类饰品。</summary>
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 34;
			Item.value = Item.buyPrice(0, 35, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.accessory = true;
		}
		/// <summary>设置跑速、移动速度、火箭靴、冰水岩浆行走和岩浆防护。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<CalamityRebirthPlayer>().angelTreads = true;
			player.accRunSpeed = 7.5f;
			player.rocketBoots = player.vanityRocketBoots = 3;
			player.moveSpeed += 0.12f;
			player.iceSkate = true;
			player.waterWalk = true;
			player.fireWalk = true;
			player.lavaMax += 420;
			player.lavaRose = true;
			player.buffImmune[BuffID.OnFire] = true;
		}
		/// <summary>时装栏保持三级火箭靴视觉效果。</summary>
		public override void UpdateVanity(Player player)
		{
			player.vanityRocketBoots = 3;
		}
		/// <summary>注册现代、1.4.2.101 与 CalRD 三个配方分支。</summary>
		public override void AddRecipes()
		{
			AddModernRecipe();
			AddClassicRecipe("CalamityModClassicPreTrailer");
			AddClassicRecipe("CalRD");
		}
		/// <summary>现代版使用鸟妖指环、日光精华、泰拉闪耀靴与三魂。</summary>
		private void AddModernRecipe()
		{
			if (!ModLoader.TryGetMod("CalamityMod", out Mod mod)
				|| !mod.TryFind<ModItem>("HarpyRing", out ModItem harpyRing)
				|| !mod.TryFind<ModItem>("EssenceofSunlight", out ModItem essence))
				return;
			CreateRecipe()
				.AddIngredient(ItemID.TerrasparkBoots)
				.AddIngredient(harpyRing.Type)
				.AddIngredient(essence.Type, 5)
				.AddIngredient(ItemID.SoulofMight)
				.AddIngredient(ItemID.SoulofSight)
				.AddIngredient(ItemID.SoulofFright)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
		/// <summary>经典两版使用霜火靴、熔岩涉水靴、鸟妖指环、焦火精华与天空合金。</summary>
		private void AddClassicRecipe(string modName)
		{
			if (!ModLoader.TryGetMod(modName, out Mod mod)
				|| !mod.TryFind<ModItem>("HarpyRing", out ModItem harpyRing)
				|| !mod.TryFind<ModItem>("EssenceofCinder", out ModItem essence)
				|| !mod.TryFind<ModItem>("AerialiteBar", out ModItem bar))
				return;
			CreateRecipe()
				.AddIngredient(ItemID.FrostsparkBoots)
				.AddIngredient(ItemID.LavaWaders)
				.AddIngredient(harpyRing.Type)
				.AddIngredient(essence.Type, 5)
				.AddIngredient(bar.Type, 20)
				.AddIngredient(ItemID.SoulofMight)
				.AddIngredient(ItemID.SoulofSight)
				.AddIngredient(ItemID.SoulofFright)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
