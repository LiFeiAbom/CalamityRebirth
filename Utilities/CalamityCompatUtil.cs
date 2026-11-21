using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 血神核心系列使用到的三版本灾厄弱依赖查询。
    /// 这里只通过 tModLoader 的内容查找与反射读取外部模组成员，不建立编译期强引用。
    /// </summary>
    internal static class CalamityCompatUtil
    {
        /// <summary>现代灾厄内部模组名。</summary>
        private const string ModernModName = "CalamityMod";
        /// <summary>经典 1.4.2.101 内部模组名。</summary>
        private const string ClassicModName = "CalamityModClassicPreTrailer";
        /// <summary>经典 1.4.5.7 内部模组名。</summary>
        private const string CalRDModName = "CalRD";
        /// <summary>
        /// 缓存各灾厄版本读取玩家星陨群系所需的反射成员。
        /// 键由模组名和 ModPlayer 类型名组成，避免弹幕每帧重复扫描程序集。
        /// </summary>
        private static readonly Dictionary<string, (MethodInfo Getter, PropertyInfo Property)> AstralBindings = new();
        /// <summary>缓存外部模组的 buff 类型，避免每帧重复查找。</summary>
        private static readonly Dictionary<(string ModName, string BuffName), int> BuffTypeCache = new();
        /// <summary>缓存外部 ModPlayer 的布尔字段写入绑定。</summary>
        private static readonly Dictionary<string, (MethodInfo Getter, FieldInfo Field)> PlayerBoolBindings = new();
        /// <summary>
        /// 将一个布尔状态镜像到指定灾厄 ModPlayer 字段。
        /// 用于让外部模组继续消费本工程饰品的等价兼容标记。
        /// </summary>
        internal static bool TrySetPlayerBool(Player player, string modName
            , string playerTypeName, string fieldName, bool value)
        {
            if (player is null || !ModLoader.TryGetMod(modName, out Mod mod))
                return false;
            string key = modName + "|" + playerTypeName + "|" + fieldName;
            if (!PlayerBoolBindings.TryGetValue(key, out (MethodInfo Getter, FieldInfo Field) binding))
            {
                Type playerType = FindModPlayerType(mod, playerTypeName);
                MethodInfo getter = playerType is null ? null : CreatePlayerGetter(playerType);
                FieldInfo field = playerType?.GetField(fieldName
                    , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (getter is null || field is null || field.FieldType != typeof(bool))
                    return false;
                binding = (getter, field);
                PlayerBoolBindings[key] = binding;
            }
            object modPlayer = binding.Getter.Invoke(player, null);
            if (modPlayer is null)
                return false;
            binding.Field.SetValue(modPlayer, value);
            return true;
        }
        /// <summary>把炽天使翼靴的金源排斥与危险砖块免疫标记镜像到现代灾厄玩家。</summary>
        internal static bool MirrorSeraphTracersToCalamity(Player player)
        {
            return TrySetPlayerBool(player, ModernModName, "CalamityPlayer"
                , "seraphTracers", true);
        }
        /// <summary>
        /// 判断玩家是否位于任意一个已加载灾厄版本的星陨感染群系。
        /// 三个版本使用不同的 ModPlayer 类型名，但都提供只读的 ZoneAstral 属性。
        /// </summary>
        internal static bool IsInAstralInfection(Player player)
        {
            return TryGetZoneAstral(player, ModernModName, "CalamityPlayer")
                || TryGetZoneAstral(player, ClassicModName, "CalamityPlayerPreTrailer")
                || TryGetZoneAstral(player, CalRDModName, "CalamityPlayer");
        }
        /// <summary>
        /// 判断当前世界是否已经击败亵渎天神。
        /// 三个灾厄分支分别把进度存放在不同静态成员中，缺失时安全返回 false。
        /// </summary>
        internal static bool IsProvidenceDowned()
        {
            return TryGetStaticBool(ModernModName, "CalamityMod.DownedBossSystem", "downedProvidence")
                || TryGetStaticBool(ClassicModName, "CalamityModClassicPreTrailer.CalamityWorldPreTrailer", "downedProvidence")
                || TryGetStaticBool(CalRDModName, "CalRD.World.CalamityWorld", "downedProvidence");
        }
        /// <summary>
        /// 判断当前世界是否已经击败神明吞噬者。
        /// 三个灾厄版本使用不同进度容器，缺失成员时安全返回 false。
        /// </summary>
        internal static bool IsDevourerOfGodsDowned()
        {
            return TryGetStaticBool(ModernModName, "CalamityMod.DownedBossSystem", "downedDoG")
                || TryGetStaticBool(ClassicModName
                    , "CalamityModClassicPreTrailer.CalamityWorldPreTrailer", "downedDoG")
                || TryGetStaticBool(CalRDModName, "CalRD.World.CalamityWorld", "downedDoG");
        }
        /// <summary>
        /// 判断当前世界是否已经击败犽戎。
        /// 供元素仆从随进度缩短本地命中冷却。
        /// </summary>
        internal static bool IsYharonDowned()
        {
            return TryGetStaticBool(ModernModName, "CalamityMod.DownedBossSystem", "downedYharon")
                || TryGetStaticBool(ClassicModName
                    , "CalamityModClassicPreTrailer.CalamityWorldPreTrailer", "downedYharon")
                || TryGetStaticBool(CalRDModName, "CalRD.World.CalamityWorld", "downedYharon");
        }
        /// <summary>尝试给 NPC 施加指定外部模组的 buff，缺失时安全返回 false。</summary>
        internal static bool TryAddBuff(NPC target, string modName, string buffName, int duration)
        {
            if (target is null || !target.active || !TryFindBuff(modName, buffName, out ModBuff buff))
                return false;
            target.AddBuff(buff.Type, duration);
            return true;
        }
        /// <summary>尝试给玩家施加指定外部模组的 buff，供 PvP 药水命中复用。</summary>
        internal static bool TryAddBuff(Player target, string modName, string buffName, int duration)
        {
            if (target is null || !target.active || !TryFindBuff(modName, buffName, out ModBuff buff))
                return false;
            target.AddBuff(buff.Type, duration);
            return true;
        }
        /// <summary>取得外部模组 buff 的类型；模组或 buff 缺失时返回零。</summary>
        internal static int GetBuffType(string modName, string buffName)
        {
            if (BuffTypeCache.TryGetValue((modName, buffName), out int cached))
                return cached;
            int type = ModLoader.TryGetMod(modName, out Mod mod)
                && mod.TryFind(buffName, out ModBuff buff) ? buff.Type : 0;
            BuffTypeCache[(modName, buffName)] = type;
            return type;
        }
        /// <summary>清空跨模组缓存，防止热重载后继续引用旧类型。</summary>
        internal static void ClearBuffTypeCache() => BuffTypeCache.Clear();
        /// <summary>清空外部 ModPlayer 布尔字段绑定缓存。</summary>
        internal static void ClearPlayerBoolBindings() => PlayerBoolBindings.Clear();
        /// <summary>在外部模组中按名称查找 ModBuff。</summary>
        private static bool TryFindBuff(string modName, string buffName, out ModBuff buff)
        {
            buff = null;
            return ModLoader.TryGetMod(modName, out Mod mod)
                && mod.TryFind(buffName, out buff);
        }
        /// <summary>
        /// 从指定外部程序集读取静态 bool 字段或同名属性。
        /// 类型、成员或值类型不匹配时返回 false，避免灾厄小版本变化导致加载失败。
        /// </summary>
        private static bool TryGetStaticBool(string modName, string typeName, string memberName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod))
                return false;
            Type worldType = mod.Code?.GetType(typeName);
            FieldInfo field = worldType?.GetField(memberName, BindingFlags.Public | BindingFlags.Static);
            if (field?.GetValue(null) is bool fieldValue)
                return fieldValue;
            PropertyInfo property = worldType?.GetProperty(memberName, BindingFlags.Public | BindingFlags.Static);
            return property?.GetValue(null) is true;
        }
        /// <summary>
        /// 从指定灾厄版本读取玩家 ModPlayer 的 ZoneAstral 只读属性。
        /// 类型、属性或泛型 GetModPlayer 方法缺失时返回 false，避免外部版本变化导致本模组加载失败。
        /// </summary>
        private static bool TryGetZoneAstral(Player player, string modName, string playerTypeName)
        {
            if (player is null || !ModLoader.TryGetMod(modName, out Mod mod))
                return false;
            string cacheKey = modName + "|" + playerTypeName;
            if (!AstralBindings.TryGetValue(cacheKey, out (MethodInfo Getter, PropertyInfo Property) binding))
            {
                Type playerType = FindModPlayerType(mod, playerTypeName);
                MethodInfo getter = playerType is null ? null : CreatePlayerGetter(playerType);
                PropertyInfo property = playerType?.GetProperty("ZoneAstral"
                    , BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (getter is null || property is null || property.PropertyType != typeof(bool))
                    return false;
                binding = (getter, property);
                AstralBindings[cacheKey] = binding;
            }
            object modPlayer = binding.Getter.Invoke(player, null);
            return modPlayer is not null && binding.Property.GetValue(modPlayer) is true;
        }
        /// <summary>
        /// 在外部模组程序集中查找指定名称的 ModPlayer 子类。
        /// 优先使用正常类型枚举；若出现部分类型加载失败，则从异常携带的成功类型中继续查找。
        /// </summary>
        private static Type FindModPlayerType(Mod mod, string playerTypeName)
        {
            try
            {
                return mod.Code?.GetTypes().FirstOrDefault(type =>
                    type.Name == playerTypeName && type.IsSubclassOf(typeof(ModPlayer)));
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.FirstOrDefault(type =>
                    type is not null && type.Name == playerTypeName && type.IsSubclassOf(typeof(ModPlayer)));
            }
        }
        /// <summary>
        /// 为外部 ModPlayer 类型构造 Player.GetModPlayer&lt;T&gt; 的封闭泛型方法。
        /// 通过反射调用可保持三版本弱依赖，不产生灾厄类型的编译期引用。
        /// </summary>
        private static MethodInfo CreatePlayerGetter(Type playerType)
        {
            MethodInfo genericMethod = typeof(Player).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(method => method.Name == "GetModPlayer"
                    && method.IsGenericMethodDefinition
                    && method.GetParameters().Length == 0);
            return genericMethod?.MakeGenericMethod(playerType);
        }
    }
}
