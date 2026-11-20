using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems
{
    /// <summary>注册盾牌冲刺链的守护状态快捷键。</summary>
    internal sealed class ShieldSlamKeybinds : ModSystem
    {
        /// <summary>默认守护状态按键。</summary>
        private const string ElysianDefaultKey = "N";
        /// <summary>极乐之庇护守护状态快捷键。</summary>
        public static ModKeybind ElysianHotKey { get; private set; }
        /// <summary>当前绑定键的显示文本。</summary>
        public static string ElysianKeyDisplay
        {
            get
            {
                List<string> assigned = ElysianHotKey?.GetAssignedKeys(InputMode.Keyboard);
                return assigned != null && assigned.Count > 0
                    ? string.Join("/", assigned) : ElysianDefaultKey;
            }
        }
        /// <summary>注册默认 N 键。</summary>
        public override void Load()
        {
            ElysianHotKey = KeybindLoader.RegisterKeybind(Mod, "Elysian", ElysianDefaultKey);
        }
        /// <summary>卸载时清空静态按键引用。</summary>
        public override void Unload()
        {
            ElysianHotKey = null;
        }
    }
}
