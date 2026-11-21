using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
	/// <summary>天界翼靴：可切换翅膀与鞋履模式的第二级移动饰品。</summary>
	[AutoloadEquip(EquipType.Wings, EquipType.Shoes)]
	internal sealed class InfinityBoots : ModItem
	{
		/// <summary>翅膀槽位缓存。</summary>
		private static int wingSlot;
		/// <summary>物品实例上的翅膀模式开关，随存档与网络同步。</summary>
		private bool toggleEnabled
		{
			get => Item.wingSlot != -1;
			set => Item.wingSlot = value ? wingSlot : -1;
		}
		/// <summary>注册 160 帧飞行、水平速度 9 与 2.6 倍加速度。</summary>
		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(160, 9f, 2.6f);
			wingSlot = Item.wingSlot;
		}
		/// <summary>设置 48×36 判定、1 铂金价格、红色稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 48;
			Item.height = 36;
			Item.value = Item.buyPrice(1, 0, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.accessory = true;
		}
		/// <summary>处理飞行粉尘、移动属性和鞋履模式下的飞行时间加成。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.controlJump && player.wingTime > 0f && player.jump == 0
				&& player.velocity.Y != 0f && !hideVisual && toggleEnabled)
			{
				int offset = player.direction == 1 ? -40 : 4;
				int dust = Dust.NewDust(new Vector2(player.position.X + player.width / 2f + offset
					, player.position.Y + player.height / 2f - 15f), 30, 30, DustID.TerraBlade
					, 0f, 0f, 100, default, 2.4f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 0.3f;
				if (Main.rand.NextBool(10))
					Main.dust[dust].fadeIn = 2f;
				Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
			}
			player.accRunSpeed = 8.5f;
			player.moveSpeed += 0.16f;
			player.iceSkate = true;
			player.waterWalk = true;
			player.fireWalk = true;
			player.lavaImmune = true;
			player.buffImmune[BuffID.OnFire] = true;
			player.noFallDmg = true;
			if (!toggleEnabled)
			{
				player.rocketBoots = player.vanityRocketBoots = 4;
				player.GetModPlayer<CalamityRebirthPlayer>().angelTreads = true;
			}
		}
		/// <summary>设置垂直飞行速度。</summary>
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling
			, ref float ascentWhenRising, ref float maxCanAscendMultiplier
			, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.75f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 2.5f;
			constantAscend = 0.125f;
		}
		/// <summary>鞋履模式移除“可飞行”提示。</summary>
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (!toggleEnabled)
				tooltips.RemoveAll(line => line.Name == "Tooltip0");
		}
		/// <summary>Shift+右键切换飞行模式。</summary>
		public override bool CanRightClick() => Main.keyState.PressingShift();
		/// <summary>切换模式并触发物品网络同步。</summary>
		public override void RightClick(Player player)
		{
			toggleEnabled = !toggleEnabled;
			Item.NetStateChanged();
		}
		/// <summary>切换操作不消耗物品。</summary>
		public override bool ConsumeItem(Player player) => false;
		/// <summary>保存翅膀模式。</summary>
		public override void SaveData(TagCompound tag) => tag.Add("toggleEffect", toggleEnabled);
		/// <summary>读取翅膀模式。</summary>
		public override void LoadData(TagCompound tag) => toggleEnabled = tag.GetBool("toggleEffect");
		/// <summary>发送翅膀模式。</summary>
		public override void NetSend(BinaryWriter writer) => writer.Write(toggleEnabled);
		/// <summary>接收翅膀模式。</summary>
		public override void NetReceive(BinaryReader reader) => toggleEnabled = reader.ReadBoolean();
		/// <summary>根据模式切换物品显示名。</summary>
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position
			, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Item.SetNameOverride(Language.GetTextValue(
				$"Mods.CalamityRebirth.Items.InfinityBoots.{(toggleEnabled ? "DisplayName" : "TreadsName")}"));
		}
		/// <summary>注册三版本配方。</summary>
		public override void AddRecipes()
		{
			AddModernRecipe();
			AddClassicRecipe("CalamityModClassicPreTrailer");
			AddClassicRecipe("CalRD");
		}
		/// <summary>现代版使用生命合金与灾厄核心。</summary>
		private void AddModernRecipe()
		{
			if (!ModLoader.TryGetMod("CalamityMod", out Mod mod)
				|| !mod.TryFind<ModItem>("CoreofCalamity", out ModItem core)
				|| !mod.TryFind<ModItem>("LifeAlloy", out ModItem lifeAlloy))
				return;
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<AngelTreads>();
			AddAnyWings(recipe);
			recipe.AddIngredient(core.Type, 3);
			recipe.AddIngredient(lifeAlloy.Type, 5);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
		/// <summary>经典版使用生命锭与灾厄核心。</summary>
		private void AddClassicRecipe(string modName)
		{
			if (!ModLoader.TryGetMod(modName, out Mod mod)
				|| !mod.TryFind<ModItem>("CoreofCalamity", out ModItem core)
				|| !mod.TryFind<ModItem>("BarofLife", out ModItem bar))
				return;
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient<AngelTreads>();
			AddAnyWings(recipe);
			recipe.AddIngredient(core.Type, 3);
			recipe.AddIngredient(bar.Type, 5);
			recipe.AddIngredient(ItemID.LunarBar, 5);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
		/// <summary>优先加入现代或经典灾厄提供的任意翅膀配方组。</summary>
		private static void AddAnyWings(Recipe recipe)
		{
			if (RecipeGroup.recipeGroupIDs.ContainsKey("AnyWings"))
				recipe.AddRecipeGroup("AnyWings");
			else if (RecipeGroup.recipeGroupIDs.ContainsKey("WingsGroup"))
				recipe.AddRecipeGroup("WingsGroup");
		}
	}
}
