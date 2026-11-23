using CalamityRebirth.Content.Items.Armors.Bloodflare;
using CalamityRebirth.Content.Items.Armors.GodSlayer;
using CalamityRebirth.Content.Items.Armors.Silva;
using CalamityRebirth.Content.Items.Armors.Tarragon;
using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.AuricTesla
{
    /// <summary>金羽十杀盔：古圣金源盗贼头，复合四套盗贼效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class AuricTeslaPlumedHelm : AuricTeslaArmorItem
    {
        /// <summary>设置 34 防御和 1 铂金 80 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(34, 180);
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
        /// <summary>置位四套盗贼分支、金源套装与 160 最大潜行。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareThrowing = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerThrowing = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaThrowing = true;
            modPlayer.auricSet = true;
            CRUtil.ApplyRogueStats(player, 0f, 0, 0f, 1.6f, 0f, 0f);
            ApplyCommonSetBonuses(player, false);
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay);
        }
        /// <summary>增加盗贼伤害、暴击和移动速度。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().auricBoost = true;
            CRUtil.ApplyRogueStats(player, 0.20f, 20, 0f, 0f, 0f, 0f);
            player.moveSpeed += 0.25f;
        }
        /// <summary>注册三版本盗贼头配方。</summary>
        public override void AddRecipes()
        {
            AddHeadRecipes<TarragonHelmet, BloodflareHelm, SilvaMask, GodSlayerMask>();
        }
    }
}
