using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.GodSlayer
{
    /// <summary>弑神者面具：盗贼向头部，提供盗贼面板、潜行上限和盗贼套装效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class GodSlayerMask : GodSlayerArmorItem
    {
        /// <summary>设置 29 防御和 75 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(29, 75);
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
        /// <summary>置位通用与盗贼套装标记，并增加 140 潜行上限及当前上限的七分之一。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerThrowing = true;
            CRUtil.ApplyRogueStats(player, 0f, 0, 0f, 1.4f, 0f, 0f);
            CRUtil.AddRogueStealthRatio(player, 1f / 7f);
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay, 100);
        }
        /// <summary>增加盗贼伤害、暴击与移动速度。</summary>
        public override void UpdateEquip(Player player)
        {
            CRUtil.ApplyRogueStats(player, 0.14f, 14, 0f, 0f, 0f, 0f);
            player.moveSpeed += 0.18f;
        }
        /// <summary>注册三版本头部配方。</summary>
        public override void AddRecipes()
        {
            AddGodSlayerRecipes(10, 2, 14, 8, 8, 1);
        }
    }
}
