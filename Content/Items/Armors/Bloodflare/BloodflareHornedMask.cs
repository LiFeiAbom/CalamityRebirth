using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>血炎法师的九头盔：增强魔法面板，并触发幽灵魔弹与暴击火焰爆炸。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class BloodflareHornedMask : BloodflareArmorItem
    {
        /// <summary>设置 22 防御与 60 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(22, 60);
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
        /// <summary>置位通用与法师套装标记，并开启猩红再生。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMage = true;
            player.crimsonRegen = true;
            player.setBonus = this.GetLocalizedValue("SetBonus");
        }
        /// <summary>增加魔法伤害、暴击、最大法力和蓝耗减免，并应用头部生存效果。</summary>
        public override void UpdateEquip(Player player)
        {
            ApplyHeadProtection(player);
            player.GetDamage(DamageClass.Magic) += 0.10f;
            player.GetCritChance(DamageClass.Magic) += 10f;
            player.statManaMax2 += 100;
            player.manaCost *= 0.83f;
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddBloodflareRecipes(25, 10, 11, 2);
        }
    }
}
