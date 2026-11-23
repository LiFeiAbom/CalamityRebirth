using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Immunity
{
    /// <summary>
    /// 铅芯：功能型饰品，装备后免疫当前灾厄版本提供的辐照减益。
    /// </summary>
    internal sealed class LeadCore : ModItem
    {
        /// <summary>设置 26×26 判定尺寸、绿色稀有度、3 金价格与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.accessory = true;
        }
        /// <summary>逐版本弱依赖查找 Irradiated，任一已加载分支存在时授予对应免疫。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int modern = CalamityCompatUtil.GetBuffType("CalamityMod", "Irradiated");
            int classic = CalamityCompatUtil.GetBuffType("CalamityModClassicPreTrailer", "Irradiated");
            int calRD = CalamityCompatUtil.GetBuffType("CalRD", "Irradiated");
            if (modern > 0)
                player.buffImmune[modern] = true;
            if (classic > 0)
                player.buffImmune[classic] = true;
            if (calRD > 0)
                player.buffImmune[calRD] = true;
        }
    }
}
