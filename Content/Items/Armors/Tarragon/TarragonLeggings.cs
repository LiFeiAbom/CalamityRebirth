using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿护腿：提供移动速度和通用伤害/暴击，半血以下额外增加移动速度。
    /// </summary>
    [AutoloadEquip(EquipType.Legs)]
    internal sealed class TarragonLeggings : TarragonArmorItem
    {
        /// <summary>设置 32 防御与 30 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(32, 30);
        }
        /// <summary>穿戴时增加 20% 移速、10% 通用伤害与暴击；半血以下再增加 15% 移速。</summary>
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.20f;
            if (player.statLife <= player.statLifeMax2 * 0.5f)
                player.moveSpeed += 0.15f;
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 10f;
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddTarragonRecipes(18, 11, 12);
        }
    }
}
