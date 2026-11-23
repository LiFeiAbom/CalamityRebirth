using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>弑神冷却：阻止再次触发弑神保命，并在冷却期间提供 10% 通用伤害。</summary>
    internal sealed class GodSlayerCooldown : ModBuff
    {
        /// <summary>注册为不可被护士移除、不随存档保存的负面冷却状态。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        /// <summary>每帧置位玩家冷却标记。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().godSlayerCooldown = true;
        }
    }
}
