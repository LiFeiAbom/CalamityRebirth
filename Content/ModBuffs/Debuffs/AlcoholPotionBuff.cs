using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 酒类药水增益的公共注册逻辑。
    /// 酒类标记为持久、可 PvP、护士不可移除的 debuff，并累计一级酒醉。
    /// </summary>
    public abstract class AlcoholPotionBuff : ModBuff
    {
        /// <summary>设置酒类共有的持久减益标记。</summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        /// <summary>点亮子类效果并累计一级酒醉。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            ApplyEffect(player, modPlayer);
            modPlayer.alcoholPoisonLevel++;
        }
        /// <summary>点亮当前酒类对应的玩家标记。</summary>
        protected abstract void ApplyEffect(Player player, CalamityRebirthPlayer modPlayer);
    }
}
