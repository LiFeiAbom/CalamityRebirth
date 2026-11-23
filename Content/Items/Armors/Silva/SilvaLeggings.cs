using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Silva
{
    /// <summary>始源林海护胫：提供高额移动速度和通用攻击面板。</summary>
    [AutoloadEquip(EquipType.Legs)]
    internal sealed class SilvaLeggings : SilvaArmorItem
    {
        /// <summary>设置 39 防御和 54 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(39, 54);
        }
        /// <summary>增加 45% 移速以及 12% 通用伤害和暴击。</summary>
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.45f;
            player.GetDamage(DamageClass.Generic) += 0.12f;
            player.GetCritChance(DamageClass.Generic) += 12f;
        }
        /// <summary>注册三版本护胫配方；只有现代配方额外消耗铅芯。</summary>
        public override void AddRecipes()
        {
            AddSilvaRecipes(60, 10, 3, 7, 7, 7, 9, 15, 15, 2, true, false, false);
        }
    }
}
