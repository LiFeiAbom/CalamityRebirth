using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>魔影套弑神者冲刺的十五秒独立冷却。</summary>
    internal sealed class DemonshadeDashCooldown : ModBuff
    {
        /// <summary>注册为不可由护士移除且不随存档保存的临时冷却。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
