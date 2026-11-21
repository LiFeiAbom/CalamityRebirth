using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
namespace CalamityRebirth.Systems
{
    /// <summary>
    /// 灾厄原版削弱回调配置。
    /// 该选项按当前加载版本处理现代 2.2.2、经典 1.4.2.101 与 CalRD 1.4.5.7
    /// 中明确存在的原版内容削弱，不修改灾厄自身内容。
    /// </summary>
    internal sealed class VanillaNerfRevertConfig : ModConfig
    {
        /// <summary>
        /// 配置载入后的共享实例，供物品、增益、玩家和弹幕钩子统一读取。
        /// </summary>
        internal static VanillaNerfRevertConfig Instance;
        /// <summary>
        /// 回调会改变所有客户端与服务器共享的战斗数值，因此采用服务器端配置。
        /// </summary>
        public override ConfigScope Mode => ConfigScope.ServerSide;
        /// <summary>
        /// 是否还原当前灾厄分支对原版内容施加的削弱。
        /// 物品默认数值在加载期写入，启用或关闭后必须重载模组才能完整生效。
        /// </summary>
        [Header("Gameplay")]
        [BackgroundColor(209, 196, 160, 192)]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool RevertVanillaNerfs { get; set; }
        /// <summary>
        /// 保存当前配置实例，空引用即代表配置尚未载入或回调处于关闭状态。
        /// </summary>
        public override void OnLoaded()
        {
            Instance = this;
        }
    }
}
