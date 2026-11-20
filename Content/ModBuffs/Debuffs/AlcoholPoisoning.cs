using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 本地酒类系统的酒精中毒提示 buff。
    /// 酒醉等级超过三级时由 UpdateLifeRegen 每帧挂两帧，具体生命再生惩罚也在该处结算。
    /// </summary>
    public class AlcoholPoisoning : ModBuff
    {
        /// <summary>设置负面、不可存档、不显示剩余时间且护士无法移除。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        /// <summary>点亮酒精中毒标记，供后续死亡文案或其他兼容逻辑读取。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().alcoholPoisoning = true;
        }
    }
}
