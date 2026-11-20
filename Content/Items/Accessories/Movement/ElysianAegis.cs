using CalamityRebirth.Enums;
using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
    /// <summary>极乐之庇护：神圣烈焰盾牌冲刺与可切换的守护充能状态。</summary>
    [AutoloadEquip(EquipType.Shield)]
    internal sealed class ElysianAegis : ModItem
    {
        /// <summary>设置判定尺寸、六十金价格、专家标记、青色稀有度与十六点防御。</summary>
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 42;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.expert = true;
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 16;
            Item.accessory = true;
        }
        /// <summary>置位被动、守护状态与神圣盾牌冲刺。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.elysianAegis = true;
            modPlayer.elysianAegisPower = true;
            modPlayer.ProvideShieldSlamDash(ShieldSlamDash.ElysianAegis);
        }
        /// <summary>把说明行中的按键占位符替换成当前绑定键。</summary>
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault(line =>
                line.Mod == "Terraria" && line.Text.Contains("[KEY]"));
            if (line != null)
                line.Text = line.Text.Replace("[KEY]", ShieldSlamKeybinds.ElysianKeyDisplay);
        }
    }
}
