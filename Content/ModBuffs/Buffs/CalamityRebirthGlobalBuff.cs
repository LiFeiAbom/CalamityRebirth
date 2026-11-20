using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>
    /// 原版增益侧的经典灾厄削弱回调入口。
    /// 实际补偿数据集中在 <see cref="VanillaNerfRevertSystem"/>，避免配置和版本判断散落在各个减益类型中。
    /// </summary>
    internal sealed class CalamityRebirthGlobalBuff : GlobalBuff
    {
        /// <summary>
        /// 每帧对持有的原版增益执行回调；未启用或未加载经典灾厄时由统一入口直接跳过。
        /// </summary>
        public override void Update(int type, Player player, ref int buffIndex)
        {
            VanillaNerfRevertSystem.RestoreBuffStats(type, player);
        }
    }
}
