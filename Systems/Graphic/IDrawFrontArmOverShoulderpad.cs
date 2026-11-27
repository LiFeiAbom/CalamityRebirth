using Terraria;
namespace CalamityRebirth.Systems.Graphic
{
    /// <summary>
    /// 本工程专用的胸甲前臂重绘接口；独占名称用于避免与弱依赖模组中的同名全局接口冲突。
    /// </summary>
    internal interface IDrawFrontArmOverShoulderpad
    {
        /// <summary>前臂贴图的完整资源路径。</summary>
        string FrontArmTexture { get; }
        /// <summary>需要区分多套身体外观时返回对应装备槽名。</summary>
        string EquipSlotName(Player drawPlayer) => "";
    }
}
