using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 血肉图腾：让一次敌怪接触伤害减半，随后进入 20 秒冷却。
    /// </summary>
    public class FleshTotem : ModItem
    {
        /// <summary>触发一次接触减半后的冷却帧数，1200 帧等于 20 秒。</summary>
        public const int ContactDamageCooldown = 1200;
        /// <summary>设置贴图尺寸、现代版 60 金售价和黄色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            RarityUtil.Apply(Item, ItemRarityID.Yellow);
            Item.accessory = true;
        }
        /// <summary>点亮本帧的血肉图腾状态；血神核心也会直接点亮同一个标记。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().fleshTotem = true;
        }
    }
}
