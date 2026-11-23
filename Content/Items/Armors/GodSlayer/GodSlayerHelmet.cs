using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.GodSlayer
{
    /// <summary>弑神者战盔：远程向头部，提供远程面板和远程套装效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class GodSlayerHelmet : GodSlayerArmorItem
    {
        /// <summary>设置 35 防御和 75 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(35, 75);
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
        /// <summary>置位通用与远程套装标记。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay, 100);
        }
        /// <summary>增加远程伤害和暴击。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.14f;
            player.GetCritChance(DamageClass.Ranged) += 14f;
        }
        /// <summary>注册三版本头部配方。</summary>
        public override void AddRecipes()
        {
            AddGodSlayerRecipes(10, 2, 14, 8, 8, 1);
        }
    }
}
