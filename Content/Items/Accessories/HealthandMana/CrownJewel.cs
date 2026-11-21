using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>
    /// 王冠宝石：无暇粹魂晶宝石系的下位饰品，以基础生命再生和带减益时的强化再生为核心。
    /// 本件没有配方，史莱姆王普通与专家模式各按 10% 掉落。
    /// </summary>
    public class CrownJewel : ModItem
    {
        /// <summary>
        /// 设置物品尺寸、四点防御、蓝色稀有度与 1 金购买价，并标记为饰品。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.defense = 4;
            RarityUtil.Apply(Item, ItemRarityID.Blue);
            Item.accessory = true;
        }
        /// <summary>
        /// 点亮本帧的王冠宝石状态，实际再生结算统一放在玩家类中执行。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().crownJewel = true;
        }
    }
}
