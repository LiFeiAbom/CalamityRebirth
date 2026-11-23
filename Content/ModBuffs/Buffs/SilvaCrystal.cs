using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>始源林海水晶：维护叶棱晶在场状态，使召唤物持续存在且不占普通仆从栏。</summary>
    internal sealed class SilvaCrystal : ModBuff
    {
        /// <summary>注册为不显示剩余时间且不随存档保存的召唤增益。</summary>
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        /// <summary>叶棱晶仍存在时续满增益；否则清除增益和玩家侧在场标记。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Content.Projectiles.Summon.SilvaCrystal>()] > 0)
                modPlayer.hasSilvaCrystal = true;
            if (!modPlayer.hasSilvaCrystal)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            player.buffTime[buffIndex] = 18000;
        }
    }
}
