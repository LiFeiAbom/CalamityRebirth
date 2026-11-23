using CalamityRebirth.Content.Items.Armors.Bloodflare;
using CalamityRebirth.Content.Items.Armors.GodSlayer;
using CalamityRebirth.Content.Items.Armors.Silva;
using CalamityRebirth.Content.Items.Armors.Tarragon;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.AuricTesla
{
    /// <summary>古圣金源盔甲：高生命、全通用面板、反伤、闪避与披风外观。</summary>
    [AutoloadEquip(EquipType.Body)]
    internal sealed class AuricTeslaBodyArmor : AuricTeslaArmorItem
    {
        /// <summary>设置 48 防御和 1 铂金 44 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(48, 144);
        }
        /// <summary>在客户端注册独立披风装备贴图。</summary>
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                EquipLoader.AddEquipTexture(Mod, Texture + "_Back", EquipType.Back, this);
        }
        /// <summary>穿戴胸甲时切换为古圣金源披风。</summary>
        public override void EquipFrameEffects(Player player, EquipType type)
        {
            if (player.body == Item.bodySlot)
                player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, Name, EquipType.Back);
        }
        /// <summary>增加生命、移速和通用面板，并继承寒霜屏障与弑神防御。</summary>
        public override void UpdateEquip(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.frostBarrier = true;
            modPlayer.godSlayerReflect = true;
            modPlayer.godSlayerDamageProtect = true;
            player.statLifeMax2 += 100;
            player.moveSpeed += 0.25f;
            player.GetCritChance(DamageClass.Generic) += 22f;
            player.GetDamage(DamageClass.Generic) += 0.22f;
        }
        /// <summary>注册三版本胸甲合成。</summary>
        public override void AddRecipes()
        {
            AddBodyRecipes<TarragonBreastplate, BloodflareBodyArmor, SilvaArmor, GodSlayerChestplate>();
        }
    }
}
