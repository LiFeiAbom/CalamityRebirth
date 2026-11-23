using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>血炎盗贼头：提高盗贼面板，并按生命阈值提供防御或伤害。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class BloodflareHelm : BloodflareArmorItem
    {
        /// <summary>设置 28 防御与 60 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(28, 60);
        }
        /// <summary>三件套要求胸甲与护腿均为血炎部件。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BloodflareBodyArmor>()
                && legs.type == ModContent.ItemType<BloodflareCuisses>();
        }
        /// <summary>套装期间显示细微残影。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
        }
        /// <summary>置位通用与盗贼套装标记，增加 135 潜行上限并开启猩红再生。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareThrowing = true;
            player.crimsonRegen = true;
            CRUtil.ApplyRogueStats(player, 0f, 0, 0f, 1.35f, 0f, 0f);
            player.setBonus = this.GetLocalizedValue("SetBonus");
        }
        /// <summary>增加盗贼伤害、暴击与头部生存效果。</summary>
        public override void UpdateEquip(Player player)
        {
            ApplyHeadProtection(player);
            CRUtil.ApplyRogueStats(player, 0.10f, 10, 0f, 0f, 0f, 0f);
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddBloodflareRecipes(25, 10, 11, 2);
        }
    }
}
