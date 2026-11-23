using CalamityRebirth.Players;
using CalamityRebirth.Enums;
using CalamityRebirth.Content.Items.Potions.PermanentPowerUps;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth
{
	/// <summary>
	/// 模组主入口，负责保存本模组实例。
	/// 客户端视觉系统均在各自内容类型中按需初始化，避免在主加载阶段阻塞资源。
	/// </summary>
	public class CalamityRebirth : Mod
	{
		/// <summary>
		/// 当前模组实例的快捷引用，供全工程请求资源或跨系统访问模组服务。
		/// 卸载时必须清空，避免热重载后残留旧实例。
		/// </summary>
		public static CalamityRebirth Instance;
		/// <summary>
		/// 模组加载入口，建立全局实例供全工程访问。
		/// </summary>
		public override void Load()
		{
			Instance = this;
		}
		/// <summary>
		/// 在全部模组内容注册完成后应用现代灾厄翅膀静态属性回调。
		/// 此时翅膀属性表已固定，重新写入的飞行时间和悬浮参数不会被后续注册覆盖。
		/// </summary>
		public override void PostSetupContent()
		{
			VanillaNerfRevertSystem.ApplyWingStatOverrides();
		}
		/// <summary>
		/// 模组卸载入口，清除模组实例，保证后续热重载安全。
		/// </summary>
		public override void Unload()
		{
			Instance = null;
			BuffUtil.Unload();
			CalamityCompatUtil.ClearBuffTypeCache();
			CalamityCompatUtil.ClearPlayerBoolBindings();
			CalamityCompatUtil.ClearAstralBindings();
			CalamityRebirthPlayer.ResetSilvaStaticCache();
			CalamityRebirthPlayer.ResetTarragonStaticCache();
			CRUtil.UnloadRogueBindings();
		}
		/// <summary>
		/// 处理天界洋葱永久解锁状态的多人同步。
		/// 服务器以连接身份 whoAmI 为准，不信任客户端包内自报的玩家索引。
		/// </summary>
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte messageType = reader.ReadByte();
			switch (messageType)
			{
				case CalamityRebirthPlayer.MsgPermanentUnlock:
					HandlePermanentUnlockPacket(reader, whoAmI);
					break;
				case CalamityRebirthPlayer.MsgRequestHeal:
					HandleHealRequest(reader, whoAmI);
					break;
				case CalamityRebirthPlayer.MsgShieldSlamDash:
					HandleShieldSlamDashPacket(reader, whoAmI);
					break;
				case CalamityRebirthPlayer.MsgShieldSlamDashHit:
					HandleShieldSlamDashHitPacket(reader, whoAmI);
					break;
				case CalamityRebirthPlayer.MsgSilvaRevivalState:
					CalamityRebirthPlayer.HandleSilvaRevivalState(reader, whoAmI);
					break;
				case CalamityRebirthPlayer.MsgGodSlayerDash:
					CalamityRebirthPlayer.HandleGodSlayerDashPacket(reader, whoAmI);
					break;
				case CalamityRebirthPlayer.MsgGodSlayerDashHit:
					CalamityRebirthPlayer.HandleGodSlayerDashHitPacket(reader, whoAmI);
					break;
			}
		}
		/// <summary>
		/// 转发盾牌冲刺的开始或结束状态。
		/// 服务端以连接玩家索引覆盖包内索引，再广播给其他客户端生成远端拖尾。
		/// </summary>
		private void HandleShieldSlamDashPacket(BinaryReader reader, int whoAmI)
		{
			int reportedOwner = reader.ReadByte();
			byte dashType = reader.ReadByte();
			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = GetPacket();
				packet.Write(CalamityRebirthPlayer.MsgShieldSlamDash);
				packet.Write((byte)whoAmI);
				packet.Write(dashType);
				packet.Send(-1, whoAmI);
				return;
			}
			if (reportedOwner < 0 || reportedOwner >= Main.player.Length || !Main.player[reportedOwner].active)
				return;
			Main.player[reportedOwner].GetModPlayer<CalamityRebirthPlayer>()
				.ReceiveShieldSlamDash((ShieldSlamDash)dashType);
		}
		/// <summary>
		/// 转发盾牌冲刺的撞击表现。
		/// 伤害和减益由本地权威命中结算，网络包只负责让其他客户端生成粒子。
		/// </summary>
		private void HandleShieldSlamDashHitPacket(BinaryReader reader, int whoAmI)
		{
			int reportedOwner = reader.ReadByte();
			byte dashType = reader.ReadByte();
			int npcIndex = reader.ReadInt16();
			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = GetPacket();
				packet.Write(CalamityRebirthPlayer.MsgShieldSlamDashHit);
				packet.Write((byte)whoAmI);
				packet.Write(dashType);
				packet.Write((short)npcIndex);
				packet.Send(-1, whoAmI);
				return;
			}
			if (reportedOwner < 0 || reportedOwner >= Main.player.Length || !Main.player[reportedOwner].active)
				return;
			Main.player[reportedOwner].GetModPlayer<CalamityRebirthPlayer>()
				.ReceiveShieldSlamDashHit((ShieldSlamDash)dashType, npcIndex);
		}
		/// <summary>
		/// 处理天界洋葱解锁握手。
		/// 服务器只接受正向解锁，并要求服务器侧已经解锁或能从玩家背包实际扣除一枚天界洋葱。
		/// </summary>
		private void HandlePermanentUnlockPacket(BinaryReader reader, int whoAmI)
		{
			int reportedPlayer = reader.ReadInt32();
			bool unlocked = reader.ReadBoolean();
			if (Main.netMode == NetmodeID.Server)
			{
				if (!unlocked || !Main.expertMode
					|| whoAmI < 0 || whoAmI >= Main.player.Length || !Main.player[whoAmI].active)
					return;
				Player owner = Main.player[whoAmI];
				CalamityRebirthPlayer serverPlayer = Main.player[whoAmI].GetModPlayer<CalamityRebirthPlayer>();
				if (!serverPlayer.extraAccessoryML)
				{
					int onionType = ModContent.ItemType<CelestialOnion>();
					bool consumed = false;
					for (int i = 0; i < owner.inventory.Length; i++)
					{
						if (owner.inventory[i].type != onionType || owner.inventory[i].stack <= 0)
							continue;
						owner.inventory[i].stack--;
						if (owner.inventory[i].stack <= 0)
							owner.inventory[i].TurnToAir();
						consumed = true;
						break;
					}
					if (!consumed)
						return;
				}
				serverPlayer.extraAccessoryML = unlocked;
				ModPacket packet = GetPacket();
				packet.Write(CalamityRebirthPlayer.MsgPermanentUnlock);
				packet.Write(whoAmI);
				packet.Write(unlocked);
				packet.Send(-1, -1);
				return;
			}
			if (!unlocked)
				return;
			if (reportedPlayer < 0 || reportedPlayer >= Main.player.Length || !Main.player[reportedPlayer].active)
				return;
			CalamityRebirthPlayer clientCopy = Main.player[reportedPlayer].GetModPlayer<CalamityRebirthPlayer>();
			clientCopy.extraAccessoryML = unlocked;
		}
		/// <summary>
		/// 服务端接收治疗请求并调用玩家侧统一验证函数。
		/// 治疗来源与伤害数值都由服务器重新校验并限制，客户端无法直接修改生命。
		/// </summary>
		private static void HandleHealRequest(BinaryReader reader, int whoAmI)
		{
			byte reason = reader.ReadByte();
			int damageSource = reader.ReadInt32();
			if (Main.netMode != NetmodeID.Server
				|| whoAmI < 0 || whoAmI >= Main.player.Length || !Main.player[whoAmI].active)
				return;
			CalamityRebirthPlayer.ApplyServerHealRequest(Main.player[whoAmI], reason, damageSource);
		}
	}
}
