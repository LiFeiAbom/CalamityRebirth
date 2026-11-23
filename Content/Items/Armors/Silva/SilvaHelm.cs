using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Silva
{
    /// <summary>始源林海战盔：近战向头部，提供近战面板与近战套装状态。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class SilvaHelm : SilvaArmorItem
    {
        /// <summary>设置 52 防御和 90 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(52, 90);
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
        /// <summary>置位通用套装与近战分支标记，并写入套装说明。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.silvaSet = true;
            modPlayer.silvaMelee = true;
            player.setBonus = this.GetLocalization("SetBonus").Value;
        }
        /// <summary>增加近战伤害、暴击和攻击速度。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.13f;
            player.GetCritChance(DamageClass.Melee) += 13f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.19f;
        }
        /// <summary>注册三版本配方；现代头部额外消耗铅芯。</summary>
        public override void AddRecipes()
        {
            AddSilvaRecipes(30, 8, 2, 5, 5, 5, 6, 14, 14, 2, true, false, false);
        }
    }
}
