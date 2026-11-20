using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 三版本盗贼兼容分部。
    /// 通过软依赖、Mod.Call 与反射避免在编译期引用任何一个灾厄版本的玩家类型，
    /// 使同一构建可在现代 2.2.2、经典 1.4.2.101 和 CalRD 1.4.5.7 下安全运行。
    /// </summary>
    internal static partial class CRUtil
    {
        /// <summary>保证版本探测与反射缓存只建立一次。</summary>
        private static bool initialized;
        /// <summary>现代灾厄 Mod 实例，用于潜行相关 Mod.Call。</summary>
        private static Mod modernMod;
        /// <summary>现代灾厄真实 RogueDamageClass。</summary>
        private static DamageClass modernRogueClass;
        /// <summary>现代灾厄 CalamityPlayer 类型。</summary>
        private static Type modernPlayerType;
        /// <summary>现代版“潜行打击只消耗 75%”布尔字段。</summary>
        private static FieldInfo modernStealth75CostField;
        /// <summary>现代版静止潜行恢复加成字段。</summary>
        private static FieldInfo modernStealthStandstillField;
        /// <summary>现代版移动潜行恢复加成字段。</summary>
        private static FieldInfo modernStealthMovingField;
        /// <summary>现代版当前潜行值字段。</summary>
        private static FieldInfo modernStealthCurrentField;
        /// <summary>现代版是否启用盗贼潜行系统的字段。</summary>
        private static FieldInfo modernWearingRogueArmorField;
        /// <summary>现代版潜行打击消耗一半上限字段。</summary>
        private static FieldInfo modernStealthHalfCostField;
        /// <summary>现代版潜行打击必定暴击字段。</summary>
        private static FieldInfo modernStealthAlwaysCritsField;
        /// <summary>现代版暗物质剑鞘移动潜行加速字段。</summary>
        private static FieldInfo modernDarkGodSheathField;
        /// <summary>现代版蚀日魔镜移动潜行加速字段。</summary>
        private static FieldInfo modernEclipseMirrorField;
        /// <summary>现代版盗贼弹速字段。</summary>
        private static FieldInfo modernRogueVelocityField;
        /// <summary>现代版最大潜行值字段。</summary>
        private static FieldInfo modernStealthMaxField;
        /// <summary>现代版本帧是否触发潜行打击的字段。</summary>
        private static FieldInfo modernStealthStrikeThisFrameField;
        /// <summary>现代版泛型 GetModPlayer&lt;CalamityPlayer&gt; 方法缓存。</summary>
        private static MethodInfo modernPlayerGetter;
        /// <summary>CalRD 1.4.5.7 Mod 实例。</summary>
        private static Mod classic1457Mod;
        /// <summary>CalRD 1.4.5.7 玩家类型。</summary>
        private static Type classic1457PlayerType;
        /// <summary>CalRD 自定义投掷伤害字段。</summary>
        private static FieldInfo classic1457DamageField;
        /// <summary>CalRD 自定义投掷暴击字段。</summary>
        private static FieldInfo classic1457CritField;
        /// <summary>CalRD 自定义投掷弹速字段。</summary>
        private static FieldInfo classic1457VelocityField;
        /// <summary>CalRD 最大潜行字段。</summary>
        private static FieldInfo classic1457StealthField;
        /// <summary>CalRD 是否穿盗贼套装标记。</summary>
        private static FieldInfo classic1457WearingArmorField;
        /// <summary>CalRD 潜行打击 75% 消耗字段。</summary>
        private static FieldInfo classic1457Stealth75CostField;
        /// <summary>CalRD 静止潜行恢复字段。</summary>
        private static FieldInfo classic1457StealthStandstillField;
        /// <summary>CalRD 移动潜行恢复字段。</summary>
        private static FieldInfo classic1457StealthMovingField;
        /// <summary>CalRD 当前潜行值字段。</summary>
        private static FieldInfo classic1457StealthCurrentField;
        /// <summary>CalRD 潜行打击消耗一半上限字段。</summary>
        private static FieldInfo classic1457StealthHalfCostField;
        /// <summary>CalRD 潜行打击必定暴击字段。</summary>
        private static FieldInfo classic1457StealthAlwaysCritsField;
        /// <summary>CalRD 暗物质剑鞘移动潜行加速字段。</summary>
        private static FieldInfo classic1457DarkGodSheathField;
        /// <summary>CalRD 蚀日魔镜移动潜行加速字段。</summary>
        private static FieldInfo classic1457EclipseMirrorField;
        /// <summary>CalRD 本帧是否触发潜行打击的字段。</summary>
        private static FieldInfo classic1457StealthStrikeThisFrameField;
        /// <summary>CalRD 玩家读取方法缓存。</summary>
        private static MethodInfo classic1457PlayerGetter;
        /// <summary>经典 1.4.2.101 Mod 实例。</summary>
        private static Mod classic142Mod;
        /// <summary>经典 1.4.2.101 的自定义投掷伤害玩家类型。</summary>
        private static Type classic142ThrowingType;
        /// <summary>经典 1.4.2.101 投掷伤害字段。</summary>
        private static FieldInfo classic142DamageField;
        /// <summary>经典 1.4.2.101 投掷暴击字段。</summary>
        private static FieldInfo classic142CritField;
        /// <summary>经典 1.4.2.101 投掷弹速字段。</summary>
        private static FieldInfo classic142VelocityField;
        /// <summary>经典 1.4.2.101 投掷玩家读取方法。</summary>
        private static MethodInfo classic142ThrowingGetter;
        /// <summary>经典 1.4.2.101 的 CalamityPlayerPreTrailer 类型。</summary>
        private static Type classic142PlayerType;
        /// <summary>经典 1.4.2.101 最大潜行字段。</summary>
        private static FieldInfo classic142StealthField;
        /// <summary>经典 1.4.2.101 当前潜行字段。</summary>
        private static FieldInfo classic142StealthCurrentField;
        /// <summary>经典 1.4.2.101 玩家读取方法。</summary>
        private static MethodInfo classic142PlayerGetter;
        /// <summary>
        /// 应用不绑定固定消耗档位的盗贼面板与潜行恢复，供完整盗贼饰品链组合使用。
        /// </summary>
        public static void ApplyRogueStats(Player player, float damageBonus, int critBonus, float speedBonus
            , float stealthBonus, float standstillRegen, float movingRegen)
        {
            Initialize();
            stealthBonus = NormalizeStealthBonus(stealthBonus);
            standstillRegen = NormalizeStealthBonus(standstillRegen);
            movingRegen = NormalizeStealthBonus(movingRegen);
            if (modernRogueClass != null)
            {
                player.GetDamage(modernRogueClass) += damageBonus;
                player.GetCritChance(modernRogueClass) += critBonus;
                player.GetAttackSpeed(modernRogueClass) += speedBonus;
                if (modernMod != null)
                {
                    modernMod.Call("AddMaxStealth", player, stealthBonus);
                    modernMod.Call("SetWearingRogueArmor", player, true);
                }
                object modPlayer = InvokePlayerGetter(modernPlayerGetter, player);
                SetBool(modernWearingRogueArmorField, modPlayer, true);
                AddFloat(modernStealthStandstillField, modPlayer, standstillRegen);
                AddFloat(modernStealthMovingField, modPlayer, movingRegen);
                return;
            }
            player.GetAttackSpeed(DamageClass.Throwing) += speedBonus;
            if (classic1457PlayerGetter != null)
            {
                object modPlayer = classic1457PlayerGetter.Invoke(player, null);
                AddFloat(classic1457DamageField, modPlayer, damageBonus);
                AddInt(classic1457CritField, modPlayer, critBonus);
                AddFloat(classic1457StealthField, modPlayer, stealthBonus);
                SetBool(classic1457WearingArmorField, modPlayer, true);
                AddFloat(classic1457StealthStandstillField, modPlayer, standstillRegen);
                AddFloat(classic1457StealthMovingField, modPlayer, movingRegen);
                return;
            }
            if (classic142ThrowingGetter != null)
            {
                object throwingPlayer = classic142ThrowingGetter.Invoke(player, null);
                AddFloat(classic142DamageField, throwingPlayer, damageBonus);
                AddInt(classic142CritField, throwingPlayer, critBonus);
            }
            if (classic142PlayerGetter != null)
            {
                object modPlayer = classic142PlayerGetter.Invoke(player, null);
                AddFloat(classic142StealthField, modPlayer, stealthBonus);
            }
        }
        /// <summary>75% 潜行消耗通用档位。</summary>
        public const float StealthStrikeCost75 = 0.75f;
        /// <summary>50% 潜行消耗通用档位。</summary>
        public const float StealthStrikeCost50 = 0.50f;
        /// <summary>
        /// 设置当前版本唯一的潜行打击消耗档位。
        /// 同时只允许一个消耗布尔字段为真，杜绝不同档位乘算。
        /// </summary>
        public static void SetStealthStrikeCost(Player player, float costFraction)
        {
            Initialize();
            bool halfCost = costFraction <= StealthStrikeCost50 + 0.001f;
            bool seventyFiveCost = !halfCost && costFraction <= StealthStrikeCost75 + 0.001f;
            if (!halfCost && !seventyFiveCost)
                return;
            if (modernPlayerGetter != null)
            {
                object modPlayer = InvokePlayerGetter(modernPlayerGetter, player);
                SetBool(modernStealthHalfCostField, modPlayer, halfCost);
                SetBool(modernStealth75CostField, modPlayer, seventyFiveCost);
                return;
            }
            if (classic1457PlayerGetter != null)
            {
                object modPlayer = classic1457PlayerGetter.Invoke(player, null);
                SetBool(classic1457StealthHalfCostField, modPlayer, halfCost);
                SetBool(classic1457Stealth75CostField, modPlayer, seventyFiveCost);
            }
        }
        /// <summary>启用现代与 CalRD 支持的潜行打击必定暴击。</summary>
        public static void SetStealthStrikeAlwaysCrits(Player player)
        {
            Initialize();
            if (modernPlayerGetter != null)
            {
                SetBool(modernStealthAlwaysCritsField, InvokePlayerGetter(modernPlayerGetter, player), true);
                return;
            }
            if (classic1457PlayerGetter != null)
                SetBool(classic1457StealthAlwaysCritsField, classic1457PlayerGetter.Invoke(player, null), true);
        }
        /// <summary>启用暗物质剑鞘或蚀日魔镜对应的移动潜行恢复加速。</summary>
        public static void SetStealthAcceleration(Player player, bool eclipse)
        {
            Initialize();
            if (modernPlayerGetter != null)
            {
                object modPlayer = InvokePlayerGetter(modernPlayerGetter, player);
                SetBool(eclipse ? modernEclipseMirrorField : modernDarkGodSheathField, modPlayer, true);
                return;
            }
            if (classic1457PlayerGetter != null)
            {
                object modPlayer = classic1457PlayerGetter.Invoke(player, null);
                SetBool(eclipse ? classic1457EclipseMirrorField : classic1457DarkGodSheathField, modPlayer, true);
            }
        }
        /// <summary>增加现代与 CalRD 支持的盗贼弹速。</summary>
        public static void AddRogueVelocity(Player player, float velocityBonus)
        {
            Initialize();
            if (modernRogueClass != null)
            {
                AddFloat(modernRogueVelocityField, InvokePlayerGetter(modernPlayerGetter, player), velocityBonus);
                return;
            }
            if (classic1457PlayerGetter != null)
            {
                AddFloat(classic1457VelocityField, classic1457PlayerGetter.Invoke(player, null), velocityBonus);
                return;
            }
            if (classic142ThrowingGetter != null)
                AddFloat(classic142VelocityField, classic142ThrowingGetter.Invoke(player, null), velocityBonus);
        }
        /// <summary>判断伤害类型是否属于当前版本的盗贼职业。</summary>
        public static bool IsRogueDamageType(DamageClass damageType)
        {
            Initialize();
            return modernRogueClass != null
                ? damageType.CountsAsClass(modernRogueClass)
                : damageType.CountsAsClass(DamageClass.Throwing);
        }
        /// <summary>取得当前版本的盗贼伤害类型。</summary>
        public static DamageClass GetRogueDamageClass()
        {
            Initialize();
            return modernRogueClass ?? DamageClass.Throwing;
        }
        /// <summary>
        /// 应用晶光头骨专用的盗贼属性。
        /// 本方法增加 120 点最大潜行和 10% 潜行恢复；
        /// 潜行攻击消耗由统一的 50% 通用档位处理，不再使用固定损耗减免。
        /// </summary>
        public static void ApplyCrystalSkullStats(Player player, float damageBonus, int critBonus
            , float speedBonus, float stealthBonus, float stealthRegenBonus)
        {
            Initialize();
            stealthBonus = NormalizeStealthBonus(stealthBonus);
            stealthRegenBonus = NormalizeStealthBonus(stealthRegenBonus);
            if (modernRogueClass != null)
            {
                player.GetDamage(modernRogueClass) += damageBonus;
                player.GetCritChance(modernRogueClass) += critBonus;
                player.GetAttackSpeed(modernRogueClass) += speedBonus;
                if (modernMod != null)
                {
                    modernMod.Call("AddMaxStealth", player, stealthBonus);
                    modernMod.Call("SetWearingRogueArmor", player, true);
                }
                object modPlayer = InvokePlayerGetter(modernPlayerGetter, player);
                SetBool(modernWearingRogueArmorField, modPlayer, true);
                AddFloat(modernStealthStandstillField, modPlayer, stealthRegenBonus);
                AddFloat(modernStealthMovingField, modPlayer, stealthRegenBonus);
                return;
            }
            player.GetAttackSpeed(DamageClass.Throwing) += speedBonus;
            if (classic1457PlayerGetter != null)
            {
                object modPlayer = classic1457PlayerGetter.Invoke(player, null);
                AddFloat(classic1457DamageField, modPlayer, damageBonus);
                AddInt(classic1457CritField, modPlayer, critBonus);
                AddFloat(classic1457StealthField, modPlayer, stealthBonus);
                SetBool(classic1457WearingArmorField, modPlayer, true);
                AddFloat(classic1457StealthStandstillField, modPlayer, stealthRegenBonus);
                AddFloat(classic1457StealthMovingField, modPlayer, stealthRegenBonus);
                return;
            }
            if (classic142ThrowingGetter != null)
            {
                object throwingPlayer = classic142ThrowingGetter.Invoke(player, null);
                AddFloat(classic142DamageField, throwingPlayer, damageBonus);
                AddInt(classic142CritField, throwingPlayer, critBonus);
            }
            if (classic142PlayerGetter != null)
            {
                object modPlayer = classic142PlayerGetter.Invoke(player, null);
                AddFloat(classic142StealthField, modPlayer, stealthBonus);
            }
        }
        /// <summary>读取当前灾厄版本是否在本帧触发潜行打击。</summary>
        public static bool IsStealthStrike(Player player)
        {
            Initialize();
            if (modernStealthStrikeThisFrameField != null)
            {
                object modPlayer = InvokePlayerGetter(modernPlayerGetter, player);
                if (TryGetBool(modernStealthStrikeThisFrameField, modPlayer, out bool modernValue))
                    return modernValue;
            }
            if (classic1457StealthStrikeThisFrameField != null)
            {
                object modPlayer = classic1457PlayerGetter?.Invoke(player, null);
                if (TryGetBool(classic1457StealthStrikeThisFrameField, modPlayer, out bool classicValue))
                    return classicValue;
            }
            return false;
        }
        /// <summary>把当前潜行值设置为最大潜行的指定比例，供镜子闪避恢复。</summary>
        public static void RestoreRogueStealth(Player player, float remainingFraction)
        {
            Initialize();
            remainingFraction = MathHelper.Clamp(remainingFraction, 0f, 1f);
            if (modernStealthCurrentField != null && modernStealthMaxField != null)
            {
                object modPlayer = InvokePlayerGetter(modernPlayerGetter, player);
                float maximum = GetFloat(modernStealthMaxField, modPlayer);
                if (maximum > 0f)
                    SetFloat(modernStealthCurrentField, modPlayer, maximum * remainingFraction);
                return;
            }
            if (classic1457StealthCurrentField != null && classic1457StealthField != null)
            {
                object modPlayer = classic1457PlayerGetter?.Invoke(player, null);
                float maximum = GetFloat(classic1457StealthField, modPlayer);
                if (maximum > 0f)
                    SetFloat(classic1457StealthCurrentField, modPlayer, maximum * remainingFraction);
            }
        }
        /// <summary>按当前灾厄版本取得盗贼基础伤害经过玩家倍率后的实际伤害。</summary>
        public static int GetRogueDamage(Player player, int baseDamage)
        {
            Initialize();
            if (modernRogueClass != null)
                return (int)player.GetTotalDamage(modernRogueClass).ApplyTo(baseDamage);
            return (int)player.GetTotalDamage(DamageClass.Throwing).ApplyTo(baseDamage);
        }
        /// <summary>
        /// 首次使用时探测三版灾厄并缓存类型、字段和泛型读取方法。
        /// 所有查找都允许失败，以支持弱依赖未安装或内部名称变化的情况。
        /// </summary>
        private static void Initialize()
        {
            if (initialized
                && (modernMod == null || modernPlayerGetter != null)
                && (classic1457Mod == null || classic1457PlayerGetter != null)
                && (classic142Mod == null || classic142PlayerGetter != null))
                return;
            initialized = true;
            if (ModLoader.TryGetMod("CalamityMod", out modernMod)
                && ModContent.TryFind("CalamityMod", "RogueDamageClass", out DamageClass rogueClass))
            {
                modernRogueClass = rogueClass;
            }
            if (modernMod != null)
            {
                // 现代版潜行核心位于 CalamityPlayer，字段用于复刻 75% 消耗和恢复速度。
                modernPlayerType = FindModPlayerType(modernMod, "CalamityPlayer");
                if (modernPlayerType != null)
                {
                    modernStealth75CostField = FindField(modernPlayerType, "stealthStrike75Cost");
                    modernStealthStandstillField = FindField(modernPlayerType, "stealthGenStandstill");
                    modernStealthMovingField = FindField(modernPlayerType, "stealthGenMoving");
                    modernStealthCurrentField = FindField(modernPlayerType, "rogueStealth");
                    modernWearingRogueArmorField = FindField(modernPlayerType, "wearingRogueArmor");
                    modernStealthMaxField = FindField(modernPlayerType, "rogueStealthMax");
                    modernStealthHalfCostField = FindField(modernPlayerType, "stealthStrikeHalfCost");
                    modernStealthAlwaysCritsField = FindField(modernPlayerType, "stealthStrikeAlwaysCrits");
                    modernDarkGodSheathField = FindField(modernPlayerType, "darkGodSheath");
                    modernEclipseMirrorField = FindField(modernPlayerType, "eclipseMirror");
                    modernRogueVelocityField = FindField(modernPlayerType, "rogueVelocity");
                    modernStealthStrikeThisFrameField = FindField(modernPlayerType, "stealthStrikeThisFrame");
                    modernPlayerGetter = CreatePlayerGetter(modernPlayerType);
                }
            }
            if (ModLoader.TryGetMod("CalRD", out classic1457Mod))
            {
                // CalRD 的 CalamityPlayer 同时承载投掷三围与完整潜行字段。
                classic1457PlayerType = FindModPlayerType(classic1457Mod, "CalamityPlayer");
                if (classic1457PlayerType != null)
                {
                    classic1457DamageField = FindField(classic1457PlayerType, "throwingDamage");
                    classic1457CritField = FindField(classic1457PlayerType, "throwingCrit");
                    classic1457VelocityField = FindField(classic1457PlayerType, "throwingVelocity");
                    classic1457StealthField = FindField(classic1457PlayerType, "rogueStealthMax");
                    classic1457WearingArmorField = FindField(classic1457PlayerType, "wearingRogueArmor");
                    classic1457Stealth75CostField = FindField(classic1457PlayerType, "stealthStrike75Cost");
                    classic1457StealthStandstillField = FindField(classic1457PlayerType, "stealthGenStandstill");
                    classic1457StealthMovingField = FindField(classic1457PlayerType, "stealthGenMoving");
                    classic1457StealthCurrentField = FindField(classic1457PlayerType, "rogueStealth");
                    classic1457StealthHalfCostField = FindField(classic1457PlayerType, "stealthStrikeHalfCost");
                    classic1457StealthAlwaysCritsField = FindField(classic1457PlayerType, "stealthStrikeAlwaysCrits");
                    classic1457DarkGodSheathField = FindField(classic1457PlayerType, "darkGodSheath");
                    classic1457EclipseMirrorField = FindField(classic1457PlayerType, "eclipseMirror");
                    classic1457StealthStrikeThisFrameField = FindField(classic1457PlayerType, "stealthStrikeThisFrame");
                    classic1457PlayerGetter = CreatePlayerGetter(classic1457PlayerType);
                }
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out classic142Mod))
            {
                // 1.4.2.101 把投掷伤害/暴击拆到独立 ModPlayer，潜行仍在 CalamityPlayerPreTrailer。
                classic142ThrowingType = FindModPlayerType(classic142Mod, "CalamityCustomThrowingDamagePlayer");
                if (classic142ThrowingType != null)
                {
                    classic142DamageField = FindField(classic142ThrowingType, "throwingDamage");
                    classic142CritField = FindField(classic142ThrowingType, "throwingCrit");
                    classic142VelocityField = FindField(classic142ThrowingType, "throwingVelocity");
                    classic142ThrowingGetter = CreatePlayerGetter(classic142ThrowingType);
                }
                classic142PlayerType = FindModPlayerType(classic142Mod, "CalamityPlayerPreTrailer");
                if (classic142PlayerType != null)
                {
                    classic142StealthField = FindField(classic142PlayerType, "rogueStealthMax");
                    classic142StealthCurrentField = FindField(classic142PlayerType, "rogueStealth");
                    classic142PlayerGetter = CreatePlayerGetter(classic142PlayerType);
                }
            }
        }
        /// <summary>
        /// 在模组程序集中按短类型名查找 ModPlayer。
        /// ReflectionTypeLoadException 发生时使用已成功加载的类型继续搜索。
        /// </summary>
        private static Type FindModPlayerType(Mod mod, string typeName)
        {
            try
            {
                foreach (ModPlayer modPlayer in mod.GetContent<ModPlayer>())
                {
                    if (modPlayer.GetType().Name == typeName)
                        return modPlayer.GetType();
                }
            }
            catch
            {
                // 内容枚举失败时继续使用程序集类型反射兜底。
            }
            try
            {
                return mod.Code.GetTypes().FirstOrDefault(type =>
                    type.Name == typeName && type.IsSubclassOf(typeof(ModPlayer)));
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.FirstOrDefault(type =>
                    type != null && type.Name == typeName && type.IsSubclassOf(typeof(ModPlayer)));
            }
        }
        /// <summary>忽略可见性限制查找实例字段，并对缺失字段返回 null。</summary>
        private static FieldInfo FindField(Type type, string fieldName) =>
            type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        /// <summary>构建 Player.GetModPlayer&lt;T&gt; 的封闭泛型方法，避免编译期引用灾厄类型。</summary>
        private static MethodInfo CreatePlayerGetter(Type playerType)
        {
            MethodInfo method = typeof(Player).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(candidate => candidate.Name == "GetModPlayer"
                    && candidate.IsGenericMethodDefinition
                    && candidate.GetParameters().Length == 0);
            return method?.MakeGenericMethod(playerType);
        }
        /// <summary>调用缓存的泛型 GetModPlayer 方法并返回目标 ModPlayer 或 null。</summary>
        private static object InvokePlayerGetter(MethodInfo getter, Player player)
        {
            if (getter == null || player == null)
                return null;
            return getter.Invoke(player, null);
        }
        /// <summary>若字段存在且当前值为 float，则在其基础上追加浮点值。</summary>
        private static void AddFloat(FieldInfo field, object target, float value)
        {
            if (field == null)
                return;
            if (field.GetValue(target) is float current)
                field.SetValue(target, current + value);
        }
        /// <summary>读取字段中的浮点值；字段缺失或类型不符时返回 0。</summary>
        private static float GetFloat(FieldInfo field, object target)
        {
            if (field == null || target == null)
                return 0f;
            return field.GetValue(target) is float value ? value : 0f;
        }
        /// <summary>在字段为 float 时直接写入指定浮点值。</summary>
        private static void SetFloat(FieldInfo field, object target, float value)
        {
            if (field != null && target != null && field.FieldType == typeof(float))
                field.SetValue(target, value);
        }
        /// <summary>在字段为 int 时直接写入指定整数值。</summary>
        private static void SetInt(FieldInfo field, object target, int value)
        {
            if (field != null && target != null && field.FieldType == typeof(int))
                field.SetValue(target, value);
        }
        /// <summary>尝试从布尔字段读取值，用于跨版本判断潜行打击状态。</summary>
        private static bool TryGetBool(FieldInfo field, object target, out bool value)
        {
            if (field != null && target != null && field.GetValue(target) is bool result)
            {
                value = result;
                return true;
            }
            value = false;
            return false;
        }
        /// <summary>
        /// 若字段为 int 或 float，则追加整数加成。
        /// 兼容不同版本可能把暴击字段从整数改成浮点实现。
        /// </summary>
        private static void AddInt(FieldInfo field, object target, int value)
        {
            if (field == null)
                return;
            if (field.GetValue(target) is int current)
                field.SetValue(target, current + value);
            else if (field.GetValue(target) is float currentFloat)
                field.SetValue(target, currentFloat + value);
        }
        /// <summary>仅在字段为 bool 时写入布尔值，防止版本差异导致类型异常。</summary>
        private static void SetBool(FieldInfo field, object target, bool value)
        {
            if (field != null && field.FieldType == typeof(bool))
                field.SetValue(target, value);
        }
        /// <summary>
        /// 统一约束项目内部潜行增量。灾厄潜行 1f 对应显示值 100，本工程合法上限远低于 5；
        /// 将误传的整数点数限制在安全范围内，避免出现几百或几千点潜行。
        /// </summary>
        private static float NormalizeStealthBonus(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                return 0f;
            return MathHelper.Clamp(value, 0f, 5f);
        }
    }
}
