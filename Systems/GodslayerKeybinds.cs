using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems
{
    /// <summary>注册弑神者套装的八方向冲刺快捷键。</summary>
    internal sealed class GodslayerKeybinds : ModSystem
    {
        /// <summary>默认快捷键 H。</summary>
        private const string DefaultKey = "H";
        /// <summary>弑神者冲刺快捷键。</summary>
        public static ModKeybind DashHotKey { get; private set; }
        /// <summary>当前绑定键的显示文本。</summary>
        public static string KeyDisplay
        {
            get
            {
                List<string> assigned = DashHotKey?.GetAssignedKeys(InputMode.Keyboard);
                return assigned is { Count: > 0 } ? string.Join("/", assigned) : DefaultKey;
            }
        }
        /// <summary>注册默认 H 键。</summary>
        public override void Load()
        {
            DashHotKey = KeybindLoader.RegisterKeybind(Mod, "GodslayerDash", DefaultKey);
        }
        /// <summary>卸载时清空静态引用。</summary>
        public override void Unload()
        {
            DashHotKey = null;
        }
    }
}
