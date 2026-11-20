using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.PermanentPowerUps
{
    /// <summary>
    /// 天界洋葱：专家/大师模式消耗品，使用后永久开启一个额外饰品栏。
    /// 解锁状态保存在玩家数据中，并同步到多人游戏中的服务器与其他客户端。
    /// </summary>
    internal sealed class CelestialOnion : ModItem
    {
        /// <summary>设置贴图尺寸、专家标记、堆叠和举升使用方式。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.expert = true;
            Item.maxStack = 99;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item4;
            Item.consumable = true;
        }
        /// <summary>
        /// 仅在专家或大师世界、且玩家尚未解锁额外饰品栏时允许使用。
        /// 返回 false 时物品不会被消耗。
        /// </summary>
        public override bool CanUseItem(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            return Main.expertMode && !modPlayer.extraAccessoryML;
        }
        /// <summary>
        /// 在使用动画的首次结算帧写入永久解锁标记。
        /// 标记由 ModPlayer 保存，并由 SendClientChanges 将多人状态同步给服务器。
        /// </summary>
        public override bool? UseItem(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (player.itemAnimation > 0 && !modPlayer.extraAccessoryML && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                modPlayer.extraAccessoryML = true;
            }
            return true;
        }
    }
    /// <summary>
    /// 天界洋葱提供的额外饰品槽。
    /// 只有玩家已经使用天界洋葱后才显示和启用。
    /// </summary>
    internal sealed class CelestialOnionSlot : ModAccessorySlot
    {
        /// <summary>
        /// 玩家有效且永久解锁标记存在时启用。
        /// 加载早期 Player.active 可能为 false，因此先执行安全检查。
        /// </summary>
        public override bool IsEnabled()
        {
            if (!Player.active)
                return false;
            return Player.GetModPlayer<CalamityRebirthPlayer>().extraAccessoryML;
        }
        /// <summary>未解锁且槽位为空时隐藏，避免占用饰品界面空间。</summary>
        public override bool IsHidden() => IsEmpty && !IsEnabled();
    }
}
