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
    /// <summary>金虚万象盔：古圣金源法师头，复合四套法师效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class AuricTeslaWireHemmedVisage : AuricTeslaArmorItem
    {
        /// <summary>设置 24 防御和 1 铂金 80 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(24, 180);
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
        /// <summary>置位四套法师分支与金源套装标记。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraMage = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMage = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerMage = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaMage = true;
            modPlayer.auricSet = true;
            ApplyCommonSetBonuses(player, false);
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay);
        }
        /// <summary>增加魔法伤害、暴击、最大魔力并降低蓝耗。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().auricBoost = true;
            player.GetDamage(DamageClass.Magic) += 0.30f;
            player.GetCritChance(DamageClass.Magic) += 20f;
            player.statManaMax2 += 100;
            player.manaCost *= 0.80f;
        }
        /// <summary>注册三版本法师头配方。</summary>
        public override void AddRecipes()
        {
            AddHeadRecipes<TarragonMask, BloodflareHornedMask, SilvaMaskedCap, GodSlayerVisage>();
        }
    }
}
