using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Buffs
{
    /// <summary>
    /// 亚利姆之力：由魔君牌兴奋剂提供，具体属性在 CalamityRebirthPlayer 中统一结算。
    /// </summary>
    internal class YharimPower : ModBuff
    {
        /// <summary>
        /// 标记为正面、可参与 PvP 且随角色保存的增益。
        /// 具体属性不直接写在这里，而是由 ModPlayer 在结算阶段统一添加，便于集中维护。
        /// </summary>
        public override void SetStaticDefaults()
        {
            // 正面增益，允许 PvP 传播且随角色存档保存。
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
        }
        /// <summary>每帧设置增益标记，离开增益后由 ModPlayer.ResetEffects 自动清除。</summary>
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().yharimPower = true;
        }
    }
}
