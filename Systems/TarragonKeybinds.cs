using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems
{
    /// <summary>注册龙蒿近战套装的生命能量护罩快捷键。</summary>
    internal sealed class TarragonKeybinds : ModSystem
    {
        /// <summary>默认快捷键 Y。</summary>
        private const string DefaultKey = "Y";
        /// <summary>龙蒿护罩快捷键。</summary>
        public static ModKeybind HotKey { get; private set; }
        /// <summary>当前绑定键显示文本。</summary>
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
            HotKey = KeybindLoader.RegisterKeybind(Mod, "Tarragon", DefaultKey);
        }
        /// <summary>卸载时清空静态引用。</summary>
        public override void Unload()
        {
            HotKey = null;
        }
    }
}
