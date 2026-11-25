using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
namespace CalamityRebirth.Systems.Graphic
{
    /// <summary>
    /// 允许头部装备额外绘制一张超出标准五十六像素高度的装饰层。
    /// 魔影头盔的兜帽和角使用该接口叠加在头部层之后。
    /// </summary>
    internal interface IExtendedHat
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
