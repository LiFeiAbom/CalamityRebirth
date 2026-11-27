using CalamityRebirth.Players;
using CalamityRebirth.Systems.Graphic;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Demonshade
{
    /// <summary>魔影胸甲：生命魔力、通用面板、弹药与静止再生核心。</summary>
    [AutoloadEquip(EquipType.Body)]
    internal sealed class DemonshadeBreastplate : DemonshadeArmorItem, IDrawFrontArmOverShoulderpad
    {
        /// <summary>肩甲上方重绘所需的前臂贴图。</summary>
        public string FrontArmTexture =>
            "CalamityRebirth/Content/Items/Armors/Demonshade/DemonshadeBreastplate_Arms";
        /// <summary>设置五十防御和四铂金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(400);
        }
        /// <summary>增加通用面板、生命魔力、反伤、省弹与鞭子属性。</summary>
        public override void UpdateEquip(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.demonshadeRegen = true;
            modPlayer.demonshadeAmmoSaving = true;
            player.GetDamage(DamageClass.Generic) += 0.40f;
            player.GetCritChance(DamageClass.Generic) += 20f;
            player.statLifeMax2 += 300;
            player.statManaMax2 += 300;
            player.thorns = 100f;
            player.manaCost *= 0.5f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.30f;
            player.whipRangeMultiplier += 0.30f;
        }
        /// <summary>注册三版本五十枚魔影锭配方。</summary>
        public override void AddRecipes()
        {
            AddShadowspecRecipe(50);
        }
    }
}
