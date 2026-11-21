using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
using CloudyWaifuProjectile = global::CalamityRebirth.Content.Projectiles.Summon.CloudyWaifu;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
	/// <summary>云元素在场时显示的常驻召唤增益。</summary>
	internal sealed class CloudyWaifu : ModBuff
	{
		/// <summary>隐藏时间条并禁止保存。</summary>
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		/// <summary>按云元素是否在场点亮标记；缺失时立即移除增益。</summary>
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<
				CloudyWaifuProjectile>()] > 0)
				modPlayer.cloudWaifu = true;
			if (!modPlayer.cloudWaifu)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}
			player.buffTime[buffIndex] = 18000;
		}
	}
}
