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
    /// <summary>金兜铁面盔：古圣金源射手头，复合四套远程效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class AuricTeslaHoodedFacemask : AuricTeslaArmorItem
    {
        /// <summary>设置 40 防御和 1 铂金 80 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(40, 180);
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
        /// <summary>置位四套远程分支与金源套装标记。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraRanged = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaRanged = true;
            modPlayer.auricSet = true;
            ApplyCommonSetBonuses(player, false);
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay);
        }
        /// <summary>增加远程伤害、暴击并置位金源静止增伤。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().auricBoost = true;
            player.GetDamage(DamageClass.Ranged) += 0.30f;
            player.GetCritChance(DamageClass.Ranged) += 30f;
        }
        /// <summary>注册三版本射手头配方。</summary>
        public override void AddRecipes()
        {
            AddHeadRecipes<TarragonVisage, BloodflareHornedHelm, SilvaHornedHelm, GodSlayerHelmet>();
        }
    }
}
