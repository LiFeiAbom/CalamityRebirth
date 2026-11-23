using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Silva
{
    /// <summary>始源林海胸甲：提高生命上限、移动速度和通用伤害/暴击。</summary>
    [AutoloadEquip(EquipType.Body)]
    internal sealed class SilvaArmor : SilvaArmorItem
    {
        /// <summary>设置 44 防御和 72 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(44, 72);
        }
        /// <summary>增加 80 生命、20% 移速以及 18% 通用伤害和暴击。</summary>
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 80;
            player.moveSpeed += 0.20f;
            player.GetDamage(DamageClass.Generic) += 0.18f;
            player.GetCritChance(DamageClass.Generic) += 18f;
        }
        /// <summary>注册三版本胸甲配方；胸甲在三版本中都消耗铅芯。</summary>
        public override void AddRecipes()
        {
            AddSilvaRecipes(90, 12, 4, 10, 10, 10, 12, 16, 16, 3, true, true, true);
        }
    }
}
