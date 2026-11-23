using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>始源林海复苏：标记 10 秒免死窗口，实际免死由玩家死亡前结算。</summary>
    internal sealed class SilvaRevival : ModBuff
    {
        /// <summary>注册为可参与 PvP、不随存档保存且不受专家延长时间影响的正面增益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
    }
}
