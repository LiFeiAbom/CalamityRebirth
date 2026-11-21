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
	/// <summary>极乐翼靴：可切换翅膀与鞋履模式的第三级移动饰品。</summary>
	[AutoloadEquip(EquipType.Wings)]
	internal sealed class ElysianTracers : ModItem
	{
		/// <summary>翅膀槽位缓存。</summary>
		private static int wingSlot;
		/// <summary>物品实例上的翅膀模式开关。</summary>
		private bool toggleEnabled
		{
			get => Item.wingSlot != -1;
			set => Item.wingSlot = value ? wingSlot : -1;
		}
		/// <summary>注册 200 帧飞行、水平速度 10.5 与 2.75 倍加速度。</summary>
		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(200, 10.5f, 2.75f);
			wingSlot = Item.wingSlot;
		}
		/// <summary>设置 48×36 判定、2 铂金价格、月后蓝稀有度与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 48;
			Item.height = 36;
			RarityUtil.Apply(Item, ModContent.RarityType<PostMoonLordBlueRarity>()
				, Item.buyPrice(2, 0, 0, 0));
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
					, player.position.Y + player.height / 2f - 15f), 30, 30
					, Main.rand.NextBool() ? 206 : 173, 0f, 0f, 100, default, 2.4f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= 0.3f;
				if (Main.rand.NextBool(10))
					Main.dust[dust].fadeIn = 2f;
				Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
			}
			player.accRunSpeed = 9.25f;
			player.moveSpeed += 0.2f;
			player.iceSkate = true;
			player.waterWalk = true;
			player.fireWalk = true;
			player.lavaImmune = true;
			player.buffImmune[BuffID.OnFire] = true;
			player.noFallDmg = true;
			player.socialShadowRocketBoots = true;
			if (!toggleEnabled)
			{
				player.rocketBoots = player.vanityRocketBoots = 1;
				player.GetModPlayer<CalamityRebirthPlayer>().angelTreads = true;
			}
		}
		/// <summary>设置垂直飞行速度。</summary>
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling
			, ref float ascentWhenRising, ref float maxCanAscendMultiplier
			, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.95f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1.1f;
			maxAscentMultiplier = 3.15f;
			constantAscend = 0.135f;
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
				$"Mods.CalamityRebirth.Items.ElysianTracers.{(toggleEnabled ? "DisplayName" : "TreadsName")}"));
		}
		/// <summary>注册三版本配方。</summary>
		public override void AddRecipes()
		{
			AddModernRecipe();
			AddClassic142Recipe();
			AddCalRDRecipe();
		}
		/// <summary>现代版使用极乐之翼、宇宙锭与登仙精华。</summary>
		private void AddModernRecipe()
		{
			if (!ModLoader.TryGetMod("CalamityMod", out Mod mod)
				|| !mod.TryFind<ModItem>("ElysianWings", out ModItem wings)
				|| !mod.TryFind<ModItem>("CosmiliteBar", out ModItem bar)
				|| !mod.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
				|| !mod.TryFind<ModTile>("CosmicAnvil", out ModTile anvil))
				return;
			CreateRecipe().AddIngredient<InfinityBoots>().AddIngredient(wings.Type)
				.AddIngredient(bar.Type, 5).AddIngredient(essence.Type, 4)
				.AddTile(anvil.Type).Register();
		}
		/// <summary>1.4.2.101 使用原生极乐之翼和等价登仙材料。</summary>
		private void AddClassic142Recipe()
		{
			if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod mod)
				|| !mod.TryFind<ModItem>("ElysianWings", out ModItem wings)
				|| !mod.TryFind<ModItem>("CosmiliteBar", out ModItem bar)
				|| !mod.TryFind<ModTile>("DraedonsForge", out ModTile forge))
				return;
			Recipe recipe = CreateRecipe().AddIngredient<InfinityBoots>()
				.AddIngredient(wings.Type).AddIngredient(bar.Type, 5);
			if (!CRUtil.TryAddClassicAscendantSpiritEssence(recipe, 4))
				return;
			recipe.AddTile(forge.Type).Register();
		}
		/// <summary>CalRD 使用原生极乐之翼与登仙精华。</summary>
		private void AddCalRDRecipe()
		{
			if (!ModLoader.TryGetMod("CalRD", out Mod mod)
				|| !mod.TryFind<ModItem>("ElysianWings", out ModItem wings)
				|| !mod.TryFind<ModItem>("CosmiliteBar", out ModItem bar)
				|| !mod.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
				|| !mod.TryFind<ModTile>("DraedonsForge", out ModTile forge))
				return;
			CreateRecipe().AddIngredient<InfinityBoots>().AddIngredient(wings.Type)
				.AddIngredient(bar.Type, 5).AddIngredient(essence.Type, 4)
				.AddTile(forge.Type).Register();
		}
	}
}
