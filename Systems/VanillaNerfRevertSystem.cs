using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems
{
    /// <summary>
    /// 经典灾厄原版削弱回调的统一数据入口。
    /// 数值覆盖 1.4.2.101 与 1.4.5.7 两个已加载分支，
    /// 现代版本的同类功能已由对应环境自行实现，不在本类重复处理。
    /// </summary>
    internal static class VanillaNerfRevertSystem
    {
        /// <summary>扩展模组内部名，用于无强引用识别当前加载的经典灾厄分支。</summary>
        private const string ClassicModName = "CalamityModClassicPreTrailer";
        /// <summary>CalRD 1.4.5.7 的扩展模组内部名。</summary>
        private const string CalRDModName = "CalRD";
        /// <summary>
        /// 当前是否启用了原版削弱回调。
        /// </summary>
        internal static bool Enabled => VanillaNerfRevertConfig.Instance?.RevertVanillaNerfs == true;
        /// <summary>
        /// 是否加载了 1.4.2.101 经典灾厄。
        /// </summary>
        internal static bool ClassicLoaded => ModLoader.HasMod(ClassicModName);
        /// <summary>
        /// 是否加载了 CalRD 1.4.5.7。
        /// </summary>
        internal static bool CalRDLoaded => ModLoader.HasMod(CalRDModName);
        /// <summary>
        /// 是否加载了任一需要处理的经典灾厄版本。
        /// </summary>
        internal static bool AnyClassicLoaded => ClassicLoaded || CalRDLoaded;
        /// <summary>
        /// 还原经典灾厄在 SetDefaults 阶段直接缩放的武器基础伤害。
        /// 源码使用整数截断，逆向除系数会在 28.5 一类结果上少还原 1 点，因此直接写回原版精确值。
        /// </summary>
        internal static void RestoreItemDefaults(Item item)
        {
            if (!Enabled)
                return;
            if (ClassicLoaded)
            {
                RestoreClassicPreTrailerItemDefaults(item);
                return;
            }
            if (CalRDLoaded)
                RestoreCalRDItemDefaults(item);
        }
        /// <summary>
        /// 还原 1.4.2.101 的武器伤害削弱。
        /// 源码先执行 Razorpine 与 LastPrism 两项单独缩放，其余四件统一乘 0.75。
        /// </summary>
        private static void RestoreClassicPreTrailerItemDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.DaedalusStormbow:
                    item.damage = 38;
                    break;
                case ItemID.PhoenixBlaster:
                    item.damage = 24;
                    break;
                case ItemID.VenusMagnum:
                    item.damage = 50;
                    break;
                case ItemID.BlizzardStaff:
                    item.damage = 58;
                    break;
                case ItemID.Razorpine:
                    item.damage = 48;
                    break;
                case ItemID.LastPrism:
                    item.damage = 100;
                    break;
            }
        }
        /// <summary>
        /// 还原 CalRD 1.4.5.7 的武器伤害削弱。
        /// 除 Phantasm 的 10% 削弱外，其余条目对应源码中的 0.75、0.7、0.65 与 0.5 系数。
        /// 与 1.4.2.101 相同，这里写入原版精确值以避免源码整数截断造成的一至两点误差。
        /// </summary>
        private static void RestoreCalRDItemDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.Phantasm:
                    item.damage = 50;
                    break;
                case ItemID.DaedalusStormbow:
                    item.damage = 38;
                    break;
                case ItemID.PhoenixBlaster:
                    item.damage = 24;
                    break;
                case ItemID.DD2BetsyBow:
                    item.damage = 39;
                    break;
                case ItemID.InfluxWaver:
                    item.damage = 100;
                    break;
                case ItemID.Xenopopper:
                    item.damage = 45;
                    break;
                case ItemID.ElectrosphereLauncher:
                    item.damage = 40;
                    break;
                case ItemID.OpticStaff:
                    item.damage = 24;
                    break;
                case ItemID.Razorpine:
                    item.damage = 48;
                    break;
                case ItemID.BlizzardStaff:
                    item.damage = 58;
                    break;
                case ItemID.LaserMachinegun:
                    item.damage = 60;
                    break;
                case ItemID.StardustDragonStaff:
                    item.damage = 40;
                    break;
            }
        }
        /// <summary>
        /// 还原 1.4.2.101 为盗贼化改造而扣除的原版塔防护甲单件属性。
        /// CalRD 1.4.5.7 对应源码只追加盗贼属性，没有保留这些负数修正，故无需补偿。
        /// </summary>
        internal static void RestoreArmorPieceStats(Item item, Player player)
        {
            if (!Enabled || !ClassicLoaded)
                return;
            switch (item.type)
            {
                case ItemID.ApprenticeRobe:
                    player.GetDamage<SummonDamageClass>() += 0.2f;
                    break;
                case ItemID.SquirePlating:
                    player.GetDamage<SummonDamageClass>() += 0.15f;
                    break;
                case ItemID.HuntressJerkin:
                case ItemID.MonkShirt:
                    player.GetDamage<SummonDamageClass>() += 0.2f;
                    break;
                case ItemID.ApprenticeAltShirt:
                    player.GetDamage<SummonDamageClass>() += 0.3f;
                    player.GetDamage<MagicDamageClass>() += 0.15f;
                    break;
                case ItemID.MonkAltShirt:
                    player.GetDamage<SummonDamageClass>() += 0.2f;
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.2f;
                    break;
                case ItemID.HuntressAltShirt:
                    player.GetDamage<SummonDamageClass>() += 0.25f;
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.2f;
                    break;
                case ItemID.SquireAltShirt:
                    player.GetDamage<SummonDamageClass>() += 0.3f;
                    break;
                case ItemID.ApprenticeTrousers:
                    player.GetCritChance<MagicDamageClass>() += 20;
                    break;
                case ItemID.SquireGreaves:
                    player.GetDamage<SummonDamageClass>() += 0.15f;
                    player.GetCritChance<MeleeDamageClass>() += 20;
                    break;
                case ItemID.HuntressPants:
                    player.GetDamage<SummonDamageClass>() += 0.1f;
                    break;
                case ItemID.MonkPants:
                    player.GetDamage<SummonDamageClass>() += 0.1f;
                    player.GetCritChance<MeleeDamageClass>() += 10;
                    break;
                case ItemID.ApprenticeAltPants:
                    player.GetDamage<SummonDamageClass>() += 0.2f;
                    player.GetCritChance<MagicDamageClass>() += 25;
                    break;
                case ItemID.MonkAltPants:
                    player.GetDamage<SummonDamageClass>() += 0.2f;
                    player.GetCritChance<MeleeDamageClass>() += 20;
                    break;
                case ItemID.HuntressAltPants:
                    player.GetDamage<SummonDamageClass>() += 0.25f;
                    break;
                case ItemID.SquireAltPants:
                    player.GetDamage<SummonDamageClass>() += 0.2f;
                    player.GetCritChance<MeleeDamageClass>() += 20;
                    break;
            }
        }
        /// <summary>
        /// 还原两版经典灾厄共同削减的原版饰品效果。
        /// 蠕虫围巾被扣除了 7% 减伤；Precise 与 Lucky 词缀则为盗贼职业让出了 1% 与 2% 三职业暴击。
        /// </summary>
        internal static void RestoreAccessoryStats(Item item, Player player)
        {
            if (!Enabled || !AnyClassicLoaded)
                return;
            if (item.type == ItemID.WormScarf)
                player.endurance += 0.07f;
            if (!item.accessory)
                return;
            if (item.prefix == PrefixID.Precise)
                AddVanillaCritPrefix(player, 1);
            else if (item.prefix == PrefixID.Lucky)
                AddVanillaCritPrefix(player, 2);
        }
        /// <summary>
        /// 把经典灾厄从近战、远程和魔法暴击词缀挪走的点数补回三职业。
        /// 不扣除灾厄额外给予盗贼职业的补偿，因为回调目标是撤销削弱而不是取消其职业补偿。
        /// </summary>
        private static void AddVanillaCritPrefix(Player player, int amount)
        {
            player.GetCritChance<MeleeDamageClass>() += amount;
            player.GetCritChance<RangedDamageClass>() += amount;
            player.GetCritChance<MagicDamageClass>() += amount;
        }
        /// <summary>
        /// 还原经典灾厄削弱过的原版增益。
        /// 1.4.2.101 把星云伤害从四职业挪给盗贼；CalRD 则直接从通用伤害扣除。
        /// 两版都削减了星云生命回复，并把冰障的 10% 伤害减免完全扣除。
        /// </summary>
        internal static void RestoreBuffStats(int type, Player player)
        {
            if (!Enabled || !AnyClassicLoaded)
                return;
            switch (type)
            {
                case BuffID.IceBarrier:
                    player.endurance += 0.1f;
                    return;
                case BuffID.NebulaUpDmg1:
                case BuffID.NebulaUpDmg2:
                case BuffID.NebulaUpDmg3:
                    float nebulaDamage = 0.075f * player.nebulaLevelDamage;
                    if (ClassicLoaded)
                    {
                        player.GetDamage<MeleeDamageClass>() += nebulaDamage;
                        player.GetDamage<RangedDamageClass>() += nebulaDamage;
                        player.GetDamage<MagicDamageClass>() += nebulaDamage;
                        player.GetDamage<SummonDamageClass>() += nebulaDamage;
                    }
                    else
                    {
                        player.GetDamage<GenericDamageClass>() += nebulaDamage;
                    }
                    return;
                case BuffID.NebulaUpLife1:
                case BuffID.NebulaUpLife2:
                case BuffID.NebulaUpLife3:
                    player.lifeRegen += 5 * player.nebulaLevelLife;
                    return;
            }
        }
        /// <summary>
        /// 还原两版经典灾厄对三种原版远程弹幕的直接伤害缩减。
        /// 使用 FinalDamage 的逆系数抵消命中阶段的修改，不会递归改变弹幕自身的后续伤害。
        /// </summary>
        internal static void RestoreProjectileDamage(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!Enabled || !AnyClassicLoaded || projectile.npcProj || projectile.trap
                || !projectile.DamageType.CountsAsClass(DamageClass.Ranged))
                return;
            float reduction = projectile.type switch
            {
                ProjectileID.CrystalShard => 0.6f,
                ProjectileID.ChlorophyteBullet => 0.8f,
                ProjectileID.HallowStar => 0.7f,
                _ => 0f,
            };
            if (reduction > 0f)
                modifiers.FinalDamage /= reduction;
        }
        /// <summary>
        /// 还原复仇模式对原版跑鞋持续奔跑速度与冰面靴加速的全局惩罚。
        /// 1.4.2.101 只削跑速；CalRD 还把“滑冰鞋加速 + 光滑地形”的加速度乘 2/3。
        /// 反射读取两个经典灾厄分支各自的复仇标志，避免为扩展模组增加强引用。
        /// </summary>
        internal static void RestoreRunSpeedPenalty(Player player)
        {
            if (!Enabled || !AnyClassicLoaded)
                return;
            bool icyAcceleration = CalRDLoaded && (player.slippy || player.slippy2) && player.iceSkate;
            if ((!player.powerrun && !icyAcceleration) || !IsRevengeanceActive())
                return;
            if (player.powerrun)
                player.maxRunSpeed /= 0.6666667f;
            if (icyAcceleration)
                player.runAcceleration /= 0.6666667f;
        }
        /// <summary>
        /// 读取当前经典灾厄的复仇模式开关。
        /// 1.4.2.101 使用 CalamityWorldPreTrailer.revenge；CalRD 使用 CalamityWorld.revenge。
        /// 两个参考分支都将该值暴露为 public static bool 字段。
        /// </summary>
        private static bool IsRevengeanceActive()
        {
            if (ClassicLoaded
                && TryGetStaticBool(ClassicModName, "CalamityModClassicPreTrailer.CalamityWorldPreTrailer", "revenge"))
                return true;
            return CalRDLoaded
                && TryGetStaticBool(CalRDModName, "CalRD.World.CalamityWorld", "revenge");
        }
        /// <summary>
        /// 从指定扩展程序集读取静态 bool 字段；同时兼容未来分支把它改成同名属性的情况。
        /// 类型、成员缺失或值类型不符时安全返回 false，不会让回调因外部模组版本变化而抛错。
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
    }
}
