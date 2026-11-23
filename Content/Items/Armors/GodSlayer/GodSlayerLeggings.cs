using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.GodSlayer
{
    /// <summary>神裁者护腿：提供高额移动速度和通用攻击面板。</summary>
    [AutoloadEquip(EquipType.Legs)]
    internal sealed class GodSlayerLeggings : GodSlayerArmorItem
    {
        /// <summary>设置 35 防御和 45 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(35, 45);
        }
        /// <summary>增加 35% 移速以及 11% 通用伤害和暴击。</summary>
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.35f;
            player.GetDamage(DamageClass.Generic) += 0.11f;
            player.GetCritChance(DamageClass.Generic) += 11f;
        }
        /// <summary>注册三版本护腿配方。</summary>
        public override void AddRecipes()
        {
            AddGodSlayerRecipes(15, 3, 18, 9, 9, 2);
        }
    }
}
