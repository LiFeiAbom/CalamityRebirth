using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Silva
{
    /// <summary>始源林海面具：盗贼向头部，提供盗贼面板、潜行上限与盗贼套装状态。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class SilvaMask : SilvaArmorItem
    {
        /// <summary>设置 30 防御和 90 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(30, 90);
        }
        /// <summary>三件套要求胸甲与护胫均为始源林海部件。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>()
                && legs.type == ModContent.ItemType<SilvaLeggings>();
        }
        /// <summary>套装期间显示角色残影。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        /// <summary>置位通用与盗贼套装标记，并增加 150 最大潜行值。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.silvaSet = true;
            modPlayer.silvaThrowing = true;
            CRUtil.ApplyRogueStats(player, 0f, 0, 0f, 1.5f, 0f, 0f);
            player.setBonus = this.GetLocalization("SetBonus").Value;
        }
        /// <summary>增加盗贼伤害、暴击与移动速度。</summary>
        public override void UpdateEquip(Player player)
        {
            CRUtil.ApplyRogueStats(player, 0.13f, 13, 0f, 0f, 0f, 0f);
            player.moveSpeed += 0.20f;
        }
        /// <summary>注册三版本配方；现代头部额外消耗铅芯。</summary>
        public override void AddRecipes()
        {
            AddSilvaRecipes(30, 8, 2, 5, 5, 5, 6, 14, 14, 2, true, false, false);
        }
    }
}
