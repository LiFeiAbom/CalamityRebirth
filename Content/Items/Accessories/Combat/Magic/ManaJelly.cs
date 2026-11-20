using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Magic
{
    /// <summary>法力凝胶：提高最大魔力，并在静止时提高魔力再生。</summary>
    internal sealed class ManaJelly : ModItem
    {
        /// <summary>设置 20×24 判定、六金价格、蓝色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().manaJelly = true;
        }
    }
}
