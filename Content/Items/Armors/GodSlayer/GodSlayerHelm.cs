using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.GodSlayer
{
    /// <summary>神裁者头盔：近战向头部，提供近战面板和近战套装效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class GodSlayerHelm : GodSlayerArmorItem
    {
        /// <summary>设置 48 防御和 75 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(48, 75);
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
        /// <summary>置位通用、近战和低伤压制标记，并增加 2.5 反伤。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerMelee = true;
            modPlayer.godSlayerDamage = true;
            player.thorns += 2.5f;
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay, 100);
        }
        /// <summary>增加近战伤害、暴击和攻击速度。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.14f;
            player.GetCritChance(DamageClass.Melee) += 14f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.20f;
        }
        /// <summary>注册三版本头部配方。</summary>
        public override void AddRecipes()
        {
            AddGodSlayerRecipes(10, 2, 14, 8, 8, 1);
        }
    }
}
