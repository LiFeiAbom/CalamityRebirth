using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Silva
{
    /// <summary>始源林海罩帽：魔法向头部，提供魔法面板与法师套装状态。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class SilvaMaskedCap : SilvaArmorItem
    {
        /// <summary>设置 21 防御和 90 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(21, 90);
        }
        /// <summary>三件套要求胸甲与护胫均为始源林海部件。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>()
                && legs.type == ModContent.ItemType<SilvaLeggings>();
        }
        /// <summary>套装期间显示角色残影。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        /// <summary>置位通用套装与法师分支标记，并写入套装说明。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.silvaSet = true;
            modPlayer.silvaMage = true;
            player.setBonus = this.GetLocalization("SetBonus").Value;
        }
        /// <summary>增加魔法伤害和暴击、最大魔力并降低魔力消耗。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.13f;
            player.GetCritChance(DamageClass.Magic) += 13f;
            player.statManaMax2 += 100;
            player.manaCost *= 0.81f;
        }
        /// <summary>注册三版本配方；现代头部额外消耗铅芯。</summary>
        public override void AddRecipes()
        {
            AddSilvaRecipes(30, 8, 2, 5, 5, 5, 6, 14, 14, 2, true, false, false);
        }
    }
}
