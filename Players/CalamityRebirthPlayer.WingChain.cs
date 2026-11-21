using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Players
{
	/// <summary>天使之靴与炽天使翼靴共享的玩家状态、飞行时间与受击保护。</summary>
	public partial class CalamityRebirthPlayer : ModPlayer
	{
		/// <summary>本帧是否获得天使之靴的飞行时间加成。</summary>
		public bool angelTreads;
		/// <summary>本帧是否装备炽天使翼靴。</summary>
		public bool seraphTracers;
		/// <summary>清空翼靴链的逐帧状态。</summary>
		public void ResetWingChainEffects()
		{
			angelTreads = false;
			seraphTracers = false;
		}
		/// <summary>结算天使之靴飞行时间与炽天使翼靴的灾厄兼容标记。</summary>
		public void ApplyWingChainEffects()
		{
			if (angelTreads)
				Player.wingTimeMax = (int)(Player.wingTimeMax * 1.1f);
			if (seraphTracers)
				CalamityCompatUtil.MirrorSeraphTracersToCalamity(Player);
		}
		/// <summary>
		/// 炽天使翼靴在单次受伤超过二百点时追加三十帧无敌。
		/// 只由本地玩家写入，随后通过玩家状态同步到其他端。
		/// </summary>
		public void ApplyWingChainPostHurt(Player.HurtInfo info)
		{
			if (seraphTracers && Player.whoAmI == Main.myPlayer && info.Damage > 200)
				Player.immuneTime += 30;
		}
	}
}
