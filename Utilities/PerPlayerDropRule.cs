using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 为每名玩家生成独立实例的掉落规则。
    /// 专用于永久解锁类物品，避免多人共同击杀时全队只有一人获得掉落。
    /// </summary>
    internal sealed class PerPlayerDropRule : CommonDrop
    {
        /// <summary>实例物品保留 15 分钟，避免被其他玩家抢走。</summary>
        private const int ProtectionTime = 18000;
        /// <summary>创建必定掉落一枚的每玩家规则。</summary>
        public PerPlayerDropRule(int itemId) : base(itemId, 1)
        {
        }
        /// <summary>服务器为每个玩家发送独立的 InstancedItem 数据。</summary>
        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            if (Main.dedServ)
            {
                NPC npc = info.npc;
                int index = Item.NewItem(npc.GetSource_Loot(), npc.Center, itemId, 1, true, -1);
                if (index < Main.maxItems)
                {
                    Main.timeItemSlotCannotBeReusedFor[index] = ProtectionTime;
                    foreach (Player player in Main.ActivePlayers)
                        NetMessage.SendData(MessageID.InstancedItem, player.whoAmI, -1, null, index);
                    Main.item[index].active = false;
                }
            }
            else
            {
                CommonCode.DropItem(info, itemId, 1);
            }
            return new ItemDropAttemptResult { State = ItemDropAttemptResultState.Success };
        }
    }
}
