using CalamityRebirth.Content.Items.Armors.Bloodflare;
using CalamityRebirth.Content.Items.Armors.GodSlayer;
using CalamityRebirth.Content.Items.Armors.Silva;
using CalamityRebirth.Content.Items.Armors.Tarragon;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.AuricTesla
{
    /// <summary>古圣金源腿甲：高移动、通用面板与魔法飞毯效果。</summary>
    [AutoloadEquip(EquipType.Legs)]
    internal sealed class AuricTeslaCuisses : AuricTeslaArmorItem
    {
        /// <summary>设置 44 防御和 1 铂金 8 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(44, 108);
        }
        /// <summary>增加 50% 移速、通用伤害/暴击并启用飞毯。</summary>
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.50f;
            player.carpet = true;
            player.GetDamage(DamageClass.Generic) += 0.14f;
            player.GetCritChance(DamageClass.Generic) += 14f;
        }
        /// <summary>注册三版本腿甲合成。</summary>
        public override void AddRecipes()
        {
            AddLegRecipes<TarragonLeggings, BloodflareCuisses, SilvaLeggings, GodSlayerLeggings>();
        }
    }
}
