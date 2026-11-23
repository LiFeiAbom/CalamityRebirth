using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>血炎召唤头：提高仆从输出，并周期性生成环绕自身的幽灵地雷。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class BloodflareHelmet : BloodflareArmorItem
    {
        /// <summary>设置 16 防御与 60 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(16, 60);
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
        /// <summary>置位通用与召唤套装标记，立即增加 55% 召唤伤害并开启猩红再生。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareSummon = true;
            player.crimsonRegen = true;
            player.GetDamage(DamageClass.Summon) += 0.55f;
            player.setBonus = this.GetLocalizedValue("SetBonus");
        }
        /// <summary>增加仆从栏、召唤伤害、鞭子范围/攻速与头部生存效果。</summary>
        public override void UpdateEquip(Player player)
        {
            ApplyHeadProtection(player);
            player.maxMinions += 3;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.whipRangeMultiplier += 0.12f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddBloodflareRecipes(25, 10, 11, 2);
        }
    }
}
