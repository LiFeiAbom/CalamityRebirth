using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
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
	/// <summary>炽天使翼靴：翼靴链顶点，兼具最强机动、岩浆免疫和重伤保护。</summary>
	[AutoloadEquip(EquipType.Wings)]
	internal sealed class SeraphTracers : ModItem
	{
		/// <summary>翅膀槽位缓存。</summary>
		private static int wingSlot;
		/// <summary>物品实例上的翅膀模式开关。</summary>
		private bool toggleEnabled
		{
			get => Item.wingSlot != -1;
			set => Item.wingSlot = value ? wingSlot : -1;
		}
		/// <summary>注册 250 帧飞行、水平速度 12 与 3 倍加速度。</summary>
		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(250, 12f, 3f);
			wingSlot = Item.wingSlot;
		}
		/// <summary>设置 54×42 判定、2 铂金 40 金价格、灼辉金源稀有度和饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 54;
			Item.height = 42;
			RarityUtil.Apply(Item, ModContent.RarityType<BurnishedAuricRarity>()
				, Item.buyPrice(2, 40, 0, 0));
			Item.accessory = true;
		}
		/// <summary>处理飞行粉尘、极限移速、完整免疫与鞋履模式。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.controlJump && player.wingTime > 0f && player.jump == 0
				&& player.velocity.Y != 0f && !hideVisual && toggleEnabled)
			{
				int offset = player.direction == 1 ? -40 : 4;
				int dust = Dust.NewDust(new Vector2(player.position.X + player.width / 2f + offset
					, player.position.Y + player.height / 2f - 15f), 30, 30, DustID.GemDiamond
					, 0f, 0f, 100, default, 2.4f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 0.3f;
				if (Main.rand.NextBool(10))
					Main.dust[dust].fadeIn = 2f;
				Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
			}
			player.accRunSpeed = 10f;
			player.moveSpeed += 0.24f;
			player.iceSkate = true;
			player.waterWalk = true;
			player.fireWalk = true;
			player.lavaImmune = true;
			player.buffImmune[BuffID.OnFire] = true;
			player.noFallDmg = true;
			player.GetModPlayer<CalamityRebirthPlayer>().seraphTracers = true;
			if (!toggleEnabled)
			{
				player.rocketBoots = player.vanityRocketBoots = 2;
				player.GetModPlayer<CalamityRebirthPlayer>().angelTreads = true;
			}
		}
		/// <summary>设置垂直飞行速度。</summary>
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling
			, ref float ascentWhenRising, ref float maxCanAscendMultiplier
			, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 1f;
			ascentWhenRising = 0.175f;
			maxCanAscendMultiplier = 1.2f;
			maxAscentMultiplier = 3.25f;
			constantAscend = 0.15f;
		}
		/// <summary>鞋履模式移除“可飞行”提示。</summary>
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (!toggleEnabled)
				tooltips.RemoveAll(line => line.Name == "Tooltip0");
		}
		/// <summary>Shift+右键切换飞行模式。</summary>
		public override bool CanRightClick() => Main.keyState.PressingShift();
		/// <summary>切换模式并同步物品状态。</summary>
		public override void RightClick(Player player)
		{
			toggleEnabled = !toggleEnabled;
			Item.NetStateChanged();
		}
		/// <summary>切换不消耗物品。</summary>
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
				$"Mods.CalamityRebirth.Items.SeraphTracers.{(toggleEnabled ? "DisplayName" : "TreadsName")}"));
		}
		/// <summary>注册三版本配方。</summary>
		public override void AddRecipes()
		{
			AddModernRecipe();
			AddClassic142Recipe();
			AddCalRDRecipe();
		}
		/// <summary>现代版使用极乐翼靴、Drew之翼与金源锭。</summary>
		private void AddModernRecipe()
		{
			if (!ModLoader.TryGetMod("CalamityMod", out Mod mod)
				|| !mod.TryFind<ModItem>("AuricBar", out ModItem auricBar)
				|| !mod.TryFind<ModTile>("CosmicAnvil", out ModTile anvil))
				return;
			CreateRecipe().AddIngredient<ElysianTracers>().AddIngredient<DrewsWings>()
				.AddIngredient(auricBar.Type, 5).AddTile(anvil.Type).Register();
		}
		/// <summary>1.4.2.101 使用暗日碎片与金源矿。</summary>
		private void AddClassic142Recipe()
		{
			if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod mod)
				|| !mod.TryFind<ModItem>("DarksunFragment", out ModItem shard)
				|| !mod.TryFind<ModItem>("AuricOre", out ModItem ore)
				|| !mod.TryFind<ModTile>("DraedonsForge", out ModTile forge))
				return;
			CreateRecipe().AddIngredient<ElysianTracers>().AddIngredient<DrewsWings>()
				.AddIngredient(shard.Type, 5).AddIngredient(ore.Type, 25)
				.AddTile(forge.Type).Register();
		}
		/// <summary>CalRD 使用金源锭与嘉登熔炉。</summary>
		private void AddCalRDRecipe()
		{
			if (!ModLoader.TryGetMod("CalRD", out Mod mod)
				|| !mod.TryFind<ModItem>("AuricBar", out ModItem auricBar)
				|| !mod.TryFind<ModTile>("DraedonsForge", out ModTile forge))
				return;
			CreateRecipe().AddIngredient<ElysianTracers>().AddIngredient<DrewsWings>()
				.AddIngredient(auricBar.Type, 4).AddTile(forge.Type).Register();
		}
	}
}
