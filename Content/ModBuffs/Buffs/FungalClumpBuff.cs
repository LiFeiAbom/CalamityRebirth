using CalamityRebirth.Content.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>真菌团块召唤增益：仅用于显示真菌团块仆从仍在场。</summary>
    public class FungalClumpBuff : ModBuff
    {
        /// <summary>隐藏时间条且不写入存档。</summary>
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        /// <summary>仆从在场时续期；不在场时移除增益。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FungalClumpMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
                return;
            }
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
