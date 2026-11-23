using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Tarragon
{
    /// <summary>
    /// 龙蒿头盔：盗贼向头部，提供盗贼面板、减伤、潜行上限与暴击免伤。
    /// </summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class TarragonHelmet : TarragonArmorItem
    {
        /// <summary>设置 15 防御与 50 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(15, 50);
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
        /// <summary>置位通用与盗贼套装标记，并通过三版本桥接增加 130 潜行上限。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            CRUtil.ApplyRogueStats(player, 0f, 0, 0f, 1.30f, 0f, 0f);
            player.setBonus = this.GetLocalization("SetBonus").Format(TarragonKeybinds.KeyDisplay);
        }
        /// <summary>增加盗贼伤害/暴击、减伤与头部元素防护。</summary>
        public override void UpdateEquip(Player player)
        {
            CRUtil.ApplyRogueStats(player, 0.10f, 10, 0f, 0f, 0f, 0f);
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
