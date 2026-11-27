using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
namespace CalamityRebirth.Systems.Graphic
{
    /// <summary>
    /// 本工程专用的头部装饰层接口；独占名称用于避免与弱依赖模组中的同名全局接口冲突。
    /// 魔影头盔的兜帽和角使用该接口叠加在头部层之后。
    /// </summary>
    internal interface IHatExtensionDrawer
    {
        /// <summary>附加层贴图的完整资源路径。</summary>
        string ExtensionTexture { get; }
        /// <summary>返回附加层相对标准头部绘制位置的像素偏移。</summary>
        Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo);
        /// <summary>返回 true 时由通用绘制层绘制附加贴图。</summary>
        bool PreDrawExtension(PlayerDrawSet drawInfo) => true;
        /// <summary>需要区分同一物品的多套头部外观时返回对应装备槽名。</summary>
        string EquipSlotName(Player drawPlayer) => "";
    }
}
