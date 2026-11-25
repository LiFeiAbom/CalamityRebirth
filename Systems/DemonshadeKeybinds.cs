using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems
{
    /// <summary>注册魔影套装的狂怒技能快捷键。</summary>
    internal sealed class DemonshadeKeybinds : ModSystem
    {
        /// <summary>默认快捷键 Y。</summary>
        private const string DefaultKey = "Y";
        /// <summary>魔影套狂怒技能快捷键。</summary>
        public static ModKeybind HotKey { get; private set; }
        /// <summary>当前绑定键的显示文本。</summary>
        public static string KeyDisplay
        {
            get
            {
                List<string> assigned = HotKey?.GetAssignedKeys(InputMode.Keyboard);
                return assigned is { Count: > 0 } ? string.Join("/", assigned) : DefaultKey;
            }
        }
        /// <summary>注册默认 Y 键。</summary>
        public override void Load()
        {
            HotKey = KeybindLoader.RegisterKeybind(Mod, "Demonshade", DefaultKey);
        }
        /// <summary>卸载时清除静态按键引用。</summary>
        public override void Unload()
        {
            HotKey = null;
        }
    }
}
