using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Items.Accessories.Combat;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Players
{
    /// <summary>灾劫之尖啸与侵蚀增益的玩家属性结算。</summary>
    public partial class CalamityRebirthPlayer : ModPlayer
    {
        /// <summary>本帧是否装备灾劫之尖啸。</summary>
        public bool affliction;
        /// <summary>本帧是否受到侵蚀增益。</summary>
        public bool afflicted;
        /// <summary>清空本帧装备与增益状态。</summary>
        public void ResetAfflictionEffects()
        {
            affliction = false;
            afflicted = false;
        }
        /// <summary>
        /// 结算灾劫之尖啸与侵蚀共用的属性，并每十帧刷新一次同队玩家的侵蚀增益。
        /// </summary>
        public void ApplyAfflictionEffects()
        {
            if (!affliction && !afflicted)
                return;
            Player.noKnockback = true;
            Player.GetDamage(DamageClass.Generic) += Affliction.DamageBonus;
            Player.statDefense += Affliction.DefenseBonus;
            Player.endurance += Affliction.DamageReduction;
            Player.statLifeMax2 += (int)(Player.statLifeMax2
                * Affliction.MaxLifeBonus);
            Player.lifeRegen += Affliction.LifeRegenBonus;
            if (!affliction || Player.miscCounter % Affliction.TeamAuraInterval != 0)
                return;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player teammate = Main.player[i];
                if (!teammate.active || teammate.whoAmI == Player.whoAmI
                    || Player.team == 0 || teammate.team != Player.team)
                    continue;
                teammate.AddBuff(ModContent.BuffType<Afflicted>()
                    , Affliction.TeamBuffDuration, true);
            }
        }
    }
}
