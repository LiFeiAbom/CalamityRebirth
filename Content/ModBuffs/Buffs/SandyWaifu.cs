using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
using SandyWaifuProjectile = global::CalamityRebirth.Content.Projectiles.Summon.SandyWaifu;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
	/// <summary>沙元素在场时显示的常驻召唤增益。</summary>
	internal sealed class SandyWaifu : ModBuff
	{
		/// <summary>隐藏时间条并禁止保存。</summary>
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		/// <summary>按沙元素是否在场点亮标记；缺失时立即移除增益。</summary>
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<
				SandyWaifuProjectile>()] > 0)
				modPlayer.sandyWaifu = true;
			if (!modPlayer.sandyWaifu)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}
			player.buffTime[buffIndex] = 18000;
		}
	}
}
