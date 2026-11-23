using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿面具：法师向头部，提供法力与蓝耗减免，并触发叶暴风和法弹治疗。
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class TarragonMask : TarragonArmorItem
    {
        /// <summary>设置 14 防御与 50 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(14, 50);
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
        /// <summary>置位通用与法师套装标记并写入快捷键提示。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraMage = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(TarragonKeybinds.KeyDisplay);
        }
        /// <summary>增加魔法伤害、暴击、最大法力并降低蓝耗，同时应用头部防护。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.10f;
            player.GetCritChance(DamageClass.Magic) += 10f;
            player.endurance += 0.05f;
            player.statManaMax2 += 100;
            player.manaCost *= 0.85f;
            ApplyHeadProtection(player);
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddTarragonRecipes(12, 7, 6);
        }
    }
}
