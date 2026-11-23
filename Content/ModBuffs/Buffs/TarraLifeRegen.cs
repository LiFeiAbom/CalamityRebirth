using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>
    /// 龙蒿生命再生：近战龙蒿套装受击时概率获得的临时正面增益。
    /// 增益本体只置位状态，实际再生在玩家结算类中增加。
    /// </summary>
    internal sealed class TarraLifeRegen : ModBuff
    {
        /// <summary>注册为正面、可保存 PvP、不随存档保存的临时增益。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
        /// <summary>每帧置位再生标记。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().tarraLifeRegen = true;
        }
    }
}
