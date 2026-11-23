using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.GodSlayer
{
    /// <summary>弑神者角盔：召唤向头部，提供仆从、鞭子面板和机械蠕虫套装效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class GodSlayerHornedHelm : GodSlayerArmorItem
    {
        /// <summary>设置 12 防御和 75 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(12, 75);
        }
        /// <summary>三件套要求胸甲与护腿均为弑神者部件。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GodSlayerChestplate>()
                && legs.type == ModContent.ItemType<GodSlayerLeggings>();
        }
        /// <summary>套装期间显示角色残影。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        /// <summary>置位通用与召唤套装标记，并增加 65% 召唤伤害。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerSummon = true;
            player.GetDamage(DamageClass.Summon) += 0.65f;
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay, 100);
        }
        /// <summary>增加 3 个仆从栏以及召唤伤害、鞭子范围和鞭子攻速。</summary>
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 3;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.whipRangeMultiplier += 0.12f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
        }
        /// <summary>注册三版本头部配方。</summary>
        public override void AddRecipes()
        {
            AddGodSlayerRecipes(10, 2, 14, 8, 8, 1);
        }
    }
}
