using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>机械蠕虫：维护噬神机械蠕虫头部在场状态。</summary>
    internal sealed class Mechworm : ModBuff
    {
        /// <summary>注册为不显示时间和不随存档保存的召唤增益。</summary>
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        /// <summary>蠕虫头在场时续满增益，否则自动移除。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Summon.MechwormHead>()] > 0)
                modPlayer.mWorm = true;
            if (!modPlayer.mWorm)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            player.buffTime[buffIndex] = 18000;
        }
    }
}
