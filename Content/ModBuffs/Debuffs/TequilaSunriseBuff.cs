using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 龙舌兰日出增益：由同名药水提供，具体数值统一在 CalamityRebirthPlayer 中结算。
    /// 该 buff 标记为持续型酒类减益，同时允许 PvP 传播并随角色保存。
    /// </summary>
    public class TequilaSunriseBuff : ModBuff
    {
        /// <summary>设置持久保存、PvP 生效且护士不能移除的酒类状态。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        /// <summary>点亮本帧的药水状态，并把本地酒醉等级提高一级。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tequilaSunrise = true;
            modPlayer.alcoholPoisonLevel++;
        }
    }
}
