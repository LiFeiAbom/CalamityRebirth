using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿胸甲：提供生命上限、生命再生和通用伤害/暴击。
    /// </summary>
    [AutoloadEquip(EquipType.Body)]
    internal sealed class TarragonBreastplate : TarragonArmorItem
    {
        /// <summary>设置 37 防御、40 金与自带 2 点生命再生。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(37, 40);
            Item.lifeRegen = 2;
        }
        /// <summary>穿戴时增加 40 生命、1 点生命再生以及 10% 通用伤害和暴击；物品自身另提供 2 点再生。</summary>
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.lifeRegen += 1;
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 10f;
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddTarragonRecipes(24, 15, 18);
        }
    }
}
