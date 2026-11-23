using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>血炎射手的魔颅盔：通过套装键释放血炎之魂，并让远程射击概率追加血液炸弹。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class BloodflareHornedHelm : BloodflareArmorItem
    {
        /// <summary>设置 34 防御与 60 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(34, 60);
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
        /// <summary>置位通用与远程套装标记，并写入套装键提示。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            player.crimsonRegen = true;
            player.setBonus = this.GetLocalization("SetBonus").Format(TarragonKeybinds.KeyDisplay);
        }
        /// <summary>增加远程伤害、暴击与头部生存效果。</summary>
        public override void UpdateEquip(Player player)
        {
            ApplyHeadProtection(player);
            player.GetDamage(DamageClass.Ranged) += 0.10f;
            player.GetCritChance(DamageClass.Ranged) += 10f;
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddBloodflareRecipes(25, 10, 11, 2);
        }
    }
}
