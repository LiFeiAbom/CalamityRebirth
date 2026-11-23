using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>血炎近战头：专精近战攻速，并通过连续命中进入血之狂怒。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class BloodflareMask : BloodflareArmorItem
    {
        /// <summary>设置 49 防御与 60 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(49, 60);
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
        /// <summary>置位通用与近战套装标记，并提高仇恨与猩红再生。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMelee = true;
            player.crimsonRegen = true;
            player.aggro += 900;
            player.setBonus = this.GetLocalizedValue("SetBonus");
        }
        /// <summary>增加近战伤害、暴击、攻速与头部生存效果。</summary>
        public override void UpdateEquip(Player player)
        {
            ApplyHeadProtection(player);
            player.GetDamage(DamageClass.Melee) += 0.10f;
            player.GetCritChance(DamageClass.Melee) += 10f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.18f;
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddBloodflareRecipes(25, 10, 11, 2);
        }
    }
}
