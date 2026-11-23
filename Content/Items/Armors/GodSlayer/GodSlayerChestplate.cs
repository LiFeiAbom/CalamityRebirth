using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.GodSlayer
{
    /// <summary>神裁者胸甲：提供生命、移速、通用面板、反伤和防御型闪避。</summary>
    [AutoloadEquip(EquipType.Body)]
    internal sealed class GodSlayerChestplate : GodSlayerArmorItem
    {
        /// <summary>设置 41 防御和 60 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(41, 60);
        }
        /// <summary>置位概率闪避与低伤保护，并增加生命、移速、通用面板和反伤。</summary>
        public override void UpdateEquip(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.godSlayerReflect = true;
            modPlayer.godSlayerDamageProtect = true;
            player.thorns += 0.9f;
            player.statLifeMax2 += 60;
            player.moveSpeed += 0.15f;
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetCritChance(DamageClass.Generic) += 15f;
        }
        /// <summary>注册三版本胸甲配方。</summary>
        public override void AddRecipes()
        {
            AddGodSlayerRecipes(20, 4, 23, 11, 11, 2);
        }
    }
}
