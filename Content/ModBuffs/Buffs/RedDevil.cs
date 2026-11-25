using CalamityRebirth.Content.Projectiles.Summon;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
using RedDevilProjectile = CalamityRebirth.Content.Projectiles.Summon.RedDevil;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>红魔召唤物的维持增益，不占召唤栏。</summary>
    internal sealed class RedDevil : ModBuff
    {
        /// <summary>隐藏计时并禁止随存档保存。</summary>
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        /// <summary>场上存在红魔时持续续时，否则立刻移除自身。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RedDevilProjectile>()] > 0)
                modPlayer.redDevil2 = true;
            if (!modPlayer.redDevil2)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
            player.buffTime[buffIndex] = 18000;
        }
    }
}
