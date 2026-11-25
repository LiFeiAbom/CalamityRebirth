using Terraria;
namespace CalamityRebirth.Systems.Graphic
{
    /// <summary>
    /// 允许胸甲把前方手臂重新绘制在肩甲之上，避免身体贴图挡住前臂。
    /// </summary>
    internal interface IDrawArmOverShoulderpad
    {
        /// <summary>前臂贴图的完整资源路径。</summary>
        string FrontArmTexture { get; }
        /// <summary>需要区分多套身体外观时返回对应装备槽名。</summary>
        string EquipSlotName(Player drawPlayer) => "";
    }
}
