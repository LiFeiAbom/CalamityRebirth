using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Demonshade
{
    /// <summary>魔影护腿：提供通用面板与黑红暗影速度。</summary>
    [AutoloadEquip(EquipType.Legs)]
    internal sealed class DemonshadeGreaves : DemonshadeArmorItem
    {
        /// <summary>设置五十防御和三铂金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(300);
        }
        /// <summary>增加通用面板并启用奔跑速度与加速度倍率。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().demonshadeShadowSpeed = true;
            player.GetDamage(DamageClass.Generic) += 0.30f;
            player.GetCritChance(DamageClass.Generic) += 15f;
        }
        /// <summary>注册三版本四十五枚魔影锭配方。</summary>
        public override void AddRecipes()
        {
            AddShadowspecRecipe(45);
        }
    }
}
