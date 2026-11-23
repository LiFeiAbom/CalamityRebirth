using CalamityRebirth.Content.Items.Armors.Bloodflare;
using CalamityRebirth.Content.Items.Armors.GodSlayer;
using CalamityRebirth.Content.Items.Armors.Silva;
using CalamityRebirth.Content.Items.Armors.Tarragon;
using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.AuricTesla
{
    /// <summary>金源耀日盔：古圣金源近战头，复合四套近战效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class AuricTeslaHelm : AuricTeslaArmorItem
    {
        /// <summary>设置 54 防御和 1 铂金 80 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(54, 180);
        }
        /// <summary>要求古圣金源胸甲与腿甲。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AuricTeslaBodyArmor>()
                && legs.type == ModContent.ItemType<AuricTeslaCuisses>();
        }
        /// <summary>套装期间显示角色残影。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        /// <summary>置位四套近战分支与金源套装标记。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraMelee = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMelee = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerMelee = true;
            modPlayer.godSlayerDamage = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaMelee = true;
            modPlayer.auricSet = true;
            ApplyCommonSetBonuses(player, true);
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay);
        }
        /// <summary>增加近战伤害、暴击、攻速并置位金源静止增伤。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().auricBoost = true;
            player.GetDamage(DamageClass.Melee) += 0.20f;
            player.GetCritChance(DamageClass.Melee) += 20f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.28f;
        }
        /// <summary>注册三版本近战头配方。</summary>
        public override void AddRecipes()
        {
            AddHeadRecipes<TarragonHelm, BloodflareMask, SilvaHelm, GodSlayerHelm>();
        }
    }
}
