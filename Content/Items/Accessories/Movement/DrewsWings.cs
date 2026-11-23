using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
	/// <summary>Drew之翼：专家翅膀，也是炽天使翼靴的必需前置。</summary>
	[AutoloadEquip(EquipType.Wings)]
	internal sealed class DrewsWings : ModItem
	{
		/// <summary>注册 361 帧飞行、水平速度 12 与 3 倍加速度。</summary>
		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(361, 12f, 3f);
		}
		/// <summary>设置 22×20 判定、1 铂金价格、专家标记与饰品类型。</summary>
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = Item.buyPrice(1, 0, 0, 0);
			Item.expert = true;
			Item.accessory = true;
		}
		/// <summary>飞行时生成钻石色尘，并免疫摔落伤害。</summary>
		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (player.controlJump && player.wingTime > 0f
				&& !VanillaExtraJump.CloudInABottle.CanStart(player) && player.jump == 0
				&& player.velocity.Y != 0f && !hideVisual)
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
			player.noFallDmg = true;
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
	}
}
