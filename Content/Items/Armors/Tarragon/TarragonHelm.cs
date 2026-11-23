using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿战盔：近战向头部，提供近战面板、减伤、元素防护与近战套装状态。
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class TarragonHelm : TarragonArmorItem
    {
        /// <summary>设置 33 防御与 50 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(33, 50);
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
        /// <summary>置位通用与近战套装标记并写入快捷键提示。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraMelee = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(TarragonKeybinds.KeyDisplay);
        }
        /// <summary>增加近战伤害、暴击、攻速、减伤与头部元素防护。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetCritChance(DamageClass.Melee) += 10f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
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
