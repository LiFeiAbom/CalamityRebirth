using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
	/// <summary>元灵之心激活且在无时间条状态下显示的合集召唤增益。</summary>
	internal sealed class HotE : ModBuff
	{
		/// <summary>隐藏时间条并禁止保存；存在与否完全由饰品状态决定。</summary>
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		/// <summary>元灵之心关闭时移除增益，否则持续刷新剩余时间。</summary>
		public override void Update(Player player, ref int buffIndex)
		{
			if (!player.GetModPlayer<CalamityRebirthPlayer>().allWaifus)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}
			player.buffTime[buffIndex] = 18000;
		}
	}
}
