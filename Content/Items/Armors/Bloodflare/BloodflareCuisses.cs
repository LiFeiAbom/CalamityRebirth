using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>血炎护腿：提供移动速度、通用伤害与暴击。</summary>
    [AutoloadEquip(EquipType.Legs)]
    internal sealed class BloodflareCuisses : BloodflareArmorItem
    {
        /// <summary>设置 29 防御与 36 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(29, 36);
        }
        /// <summary>增加 30% 移速、10% 通用伤害和暴击。</summary>
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.30f;
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 10f;
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddBloodflareRecipes(35, 15, 13, 3);
        }
    }
}
