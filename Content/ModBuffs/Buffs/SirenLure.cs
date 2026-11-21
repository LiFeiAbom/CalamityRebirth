using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
using SirenLureProjectile = global::CalamityRebirth.Content.Projectiles.Summon.SirenLure;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
	/// <summary>塞壬在场时显示的常驻召唤增益。</summary>
	internal sealed class SirenLure : ModBuff
	{
		/// <summary>隐藏时间条并禁止保存。</summary>
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		/// <summary>按塞壬是否在场点亮标记；缺失时立即移除增益。</summary>
		public override void Update(Player player, ref int buffIndex)
		{
			CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
			if (player.ownedProjectileCounts[ModContent.ProjectileType<
				SirenLureProjectile>()] > 0)
				modPlayer.sirenLureWaifu = true;
			if (!modPlayer.sirenLureWaifu)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
				return;
			}
			player.buffTime[buffIndex] = 18000;
		}
	}
}
