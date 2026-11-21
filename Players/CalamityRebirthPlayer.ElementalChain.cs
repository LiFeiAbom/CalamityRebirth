using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Players
{
	/// <summary>四枚元素石、五件元素召唤饰品与元灵之心的玩家状态和属性结算。</summary>
	public partial class CalamityRebirthPlayer : ModPlayer
	{
		/// <summary>本帧是否装备气之石。</summary>
		public bool aeroStone;
		/// <summary>本帧是否装备寒晶石。</summary>
		public bool cryoStone;
		/// <summary>本帧是否装备混沌石。</summary>
		public bool chaosStone;
		/// <summary>本帧是否装备花绽石。</summary>
		public bool bloomStone;
		/// <summary>本帧是否装备玫瑰石。</summary>
		public bool roseStone;
		/// <summary>玫瑰石是否显示外观；隐藏时关闭粉色照明和硫火元素。</summary>
		public bool roseStoneVisible;
		/// <summary>本帧是否装备元灵之心。</summary>
		public bool heartoftheElements;
		/// <summary>元灵之心是否隐藏外观；隐藏档使用略高的基础属性且不召唤元素。</summary>
		public bool heartoftheElementsHideVisual;
		/// <summary>元灵之心是否允许全部五种元素仆从存活。</summary>
		public bool allWaifus;
		/// <summary>本帧是否装备风暴之眼。</summary>
		public bool eyeoftheStorm;
		/// <summary>本帧是否装备魅惑之饵。</summary>
		public bool lureofEnthrallment;
		/// <summary>本帧是否装备瓶装沙元素。</summary>
		public bool wifeinaBottle;
		/// <summary>本帧是否装备稀有瓶装沙元素。</summary>
		public bool wifeinaBottlewithBoobs;
		/// <summary>硫火元素在场标记，由召唤增益维护。</summary>
		public bool brimstoneWaifu;
		/// <summary>云元素在场标记，由召唤增益维护。</summary>
		public bool cloudWaifu;
		/// <summary>塞壬在场标记，由召唤增益维护。</summary>
		public bool sirenLureWaifu;
		/// <summary>沙元素在场标记，由召唤增益维护。</summary>
		public bool sandyWaifu;
		/// <summary>治疗型沙元素在场标记，由召唤增益维护。</summary>
		public bool drewsSandyWaifu;
		/// <summary>清空元素链的全部逐帧装备与召唤状态。</summary>
		public void ResetElementalChainEffects()
		{
			aeroStone = false;
			cryoStone = false;
			chaosStone = false;
			bloomStone = false;
			roseStone = false;
			roseStoneVisible = false;
			heartoftheElements = false;
			heartoftheElementsHideVisual = false;
			allWaifus = false;
			eyeoftheStorm = false;
			lureofEnthrallment = false;
			wifeinaBottle = false;
			wifeinaBottlewithBoobs = false;
			brimstoneWaifu = false;
			cloudWaifu = false;
			sirenLureWaifu = false;
			sandyWaifu = false;
			drewsSandyWaifu = false;
		}
		/// <summary>
		/// 结算元素石、玫瑰石与元灵之心的逐帧面板，并处理照明、诅咒光环和植物生长。
		/// </summary>
		public void ApplyElementalChainEffects()
		{
			if (roseStone)
			{
				if (roseStoneVisible)
					Lighting.AddLight(Player.Center, 0.6f, 0f, 0.25f);
				Player.lifeRegen += 2;
				Player.statLifeMax2 += 20;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (aeroStone)
			{
				Lighting.AddLight(Player.Center, 0f, 0.425f, 0.425f);
				Player.moveSpeed += 0.1f;
				Player.jumpSpeedBoost += 2f;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (cryoStone)
			{
				Lighting.AddLight(Player.Center, 0f, 0.25f, 0.6f);
				Player.endurance += 0.05f;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (chaosStone)
			{
				Lighting.AddLight(Player.Center, 0.85f, 0f, 0f);
				Player.statManaMax2 += 50;
				Player.manaCost *= 0.95f;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (bloomStone)
			{
				Lighting.AddLight(Player.Center, 0.25f, 0.4f, 0.2f);
				Player.GetDamage(DamageClass.Generic) += 0.02f;
				Player.GetCritChance(DamageClass.Generic) += 2;
				ApplyElementalCurseAura();
			}
			if (heartoftheElements)
			{
				ApplyHeartBaseStats();
				ApplyElementalCurseAura();
			}
			if ((bloomStone || heartoftheElements) && Player.whoAmI == Main.myPlayer)
				GrowElementalPlants();
		}
		/// <summary>应用元灵之心两档共用的再生、魔力效率与通用暴击。</summary>
		private void ApplyHeartBaseStats()
		{
			Player.lifeRegen += 4;
			Player.manaRegenBonus += 2;
			Player.manaCost *= 0.95f;
			Player.GetCritChance(DamageClass.Generic) += 10;
			if (!heartoftheElementsHideVisual)
			{
				Player.statLifeMax2 += 20;
				Player.statManaMax2 += 50;
				Player.moveSpeed += 0.1f;
				Player.jumpSpeedBoost += 2f;
				Player.endurance += 0.05f;
				Player.GetDamage(DamageClass.Generic) += 0.1f;
			}
			else
			{
				Player.statLifeMax2 += 25;
				Player.statManaMax2 += 60;
				Player.moveSpeed += 0.12f;
				Player.jumpSpeedBoost += 2.2f;
				Player.endurance += 0.06f;
				Player.GetDamage(DamageClass.Generic) += 0.12f;
			}
		}
		/// <summary>
		/// 每帧按十分之一概率诅咒一百五十像素内敌怪，并造成十点通用伤害。
		/// 只在拥有者客户端结算，避免多人环境重复施加与伤害。
		/// </summary>
		private void ApplyElementalCurseAura()
		{
			if (Player.whoAmI != Main.myPlayer || !Main.rand.NextBool(10))
				return;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage
					|| npc.buffImmune[BuffID.BetsysCurse]
					|| Vector2.DistanceSquared(Player.Center, npc.Center) > 150f * 150f)
					continue;
				npc.AddBuff(BuffID.BetsysCurse, 120);
				npc.SimpleStrikeNPC(10, 0, false, 0f, DamageClass.Generic);
			}
		}
		/// <summary>
		/// 玩家站稳且未抓钩时，在下方的泥土、草地、神圣草地或丛林草地上生成随机植物。
		/// 只在本地玩家端修改方块，并在多人环境发送单格方块更新。
		/// </summary>
		private void GrowElementalPlants()
		{
			if (Player.velocity.Y != 0f || Player.grappling[0] != -1 || Player.dead)
				return;
			int x = (int)Player.Center.X / 16;
			int y = (int)(Player.position.Y + Player.height - 1f) / 16;
			if (!WorldGen.InWorld(x, y, 2) || Main.tile[x, y].HasTile
				|| Main.tile[x, y].LiquidAmount != 0 || !WorldGen.SolidTile(x, y + 1))
				return;
			Tile below = Main.tile[x, y + 1];
			bool changed = false;
			if (below.TileType == TileID.Dirt)
			{
				if (Main.rand.NextBool(1000))
				{
					SetElementalPlant(x, y, TileID.DyePlants
						, (short)(34 * Main.rand.Next(1, 13)));
					changed = true;
				}
			}
			else if (below.TileType == TileID.Grass)
			{
				if (Main.rand.NextBool(2))
					SetElementalPlant(x, y, TileID.Plants
						, (short)(18 * Main.rand.Next(6, 11)));
				else
					SetElementalPlant(x, y, TileID.Plants2
						, (short)(18 * Main.rand.Next(6, 21)));
				changed = true;
			}
			else if (below.TileType == TileID.HallowedGrass)
			{
				if (Main.rand.NextBool(2))
					SetElementalPlant(x, y, TileID.HallowedPlants
						, (short)(18 * Main.rand.Next(4, 7)));
				else
					SetElementalPlant(x, y, TileID.HallowedPlants2
						, (short)(18 * Main.rand.Next(2, 8)));
				changed = true;
			}
			else if (below.TileType == TileID.JungleGrass)
			{
				SetElementalPlant(x, y, TileID.JunglePlants2
					, (short)(18 * Main.rand.Next(9, 17)));
				changed = true;
			}
			if (changed && Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
		}
		/// <summary>写入一格无坡度的植物，并清除可能继承的锤击状态。</summary>
		private static void SetElementalPlant(int x, int y, ushort tileType, short frameX)
		{
			Tile tile = Main.tile[x, y];
			tile.HasTile = true;
			tile.TileType = tileType;
			tile.TileFrameX = frameX;
			tile.TileFrameY = 0;
			tile.Get<TileWallWireStateData>().Slope = 0;
			tile.Get<TileWallWireStateData>().IsHalfBlock = false;
		}
	}
}
