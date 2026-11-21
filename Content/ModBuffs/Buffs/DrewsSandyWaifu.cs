using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
using DrewsSandyWaifuProjectile = global::CalamityRebirth.Content.Projectiles.Summon.DrewsSandyWaifu;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
	/// <summary>治疗型沙元素在场时显示的常驻召唤增益。</summary>
	internal sealed class DrewsSandyWaifu : ModBuff
	{
		/// <summary>隐藏时间条并禁止保存。</summary>
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		/// <summary>按治疗型沙元素是否在场点亮标记；缺失时立即移除增益。</summary>
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<
				DrewsSandyWaifuProjectile>()] > 0)
				modPlayer.drewsSandyWaifu = true;
			if (!modPlayer.drewsSandyWaifu)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}
			player.buffTime[buffIndex] = 18000;
		}
	}
}
