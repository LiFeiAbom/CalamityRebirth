using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿角盔：召唤向头部，提高仆从栏、召唤与鞭子面板，并启用生命光环。
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class TarragonHornedHelm : TarragonArmorItem
    {
        /// <summary>设置 7 防御与 50 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(7, 50);
        }
        /// <summary>三件套要求胸甲与护腿均为龙蒿部件。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TarragonBreastplate>()
                && legs.type == ModContent.ItemType<TarragonLeggings>();
        }
        /// <summary>套装期间显示柔和残影与轮廓。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }
        /// <summary>置位通用与召唤套装标记，并立即提供 50% 召唤伤害。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraSummon = true;
            player.GetDamage(DamageClass.Summon) += 0.50f;
            player.setBonus = this.GetLocalization("SetBonus").Format(TarragonKeybinds.KeyDisplay);
        }
        /// <summary>增加仆从栏、召唤伤害、鞭子范围/攻速、减伤与头部元素防护。</summary>
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 3;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.whipRangeMultiplier += 0.12f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
            player.endurance += 0.05f;
            ApplyHeadProtection(player);
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddTarragonRecipes(12, 7, 6);
        }
    }
}
