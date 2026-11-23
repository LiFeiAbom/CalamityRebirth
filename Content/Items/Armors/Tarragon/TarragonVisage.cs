using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿面甲：远程向头部，触发暴击树叶与远程弹幕消失时的生命能量。
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class TarragonVisage : TarragonArmorItem
    {
        /// <summary>设置 21 防御与 50 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(21, 50);
        }
        /// <summary>三件套要求胸甲与护腿均为龙蒿部件。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TarragonBreastplate>()
                && legs.type == ModContent.ItemType<TarragonLeggings>();
        }
        /// <summary>套装期间显示柔和残影与轮廓。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }
        /// <summary>置位通用与远程套装标记并写入快捷键提示。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraRanged = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(TarragonKeybinds.KeyDisplay);
        }
        /// <summary>增加远程伤害、暴击、减伤与头部元素防护。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.GetCritChance(DamageClass.Ranged) += 10f;
            player.endurance += 0.05f;
            ApplyHeadProtection(player);
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddTarragonRecipes(12, 7, 6);
        }
    }
}
