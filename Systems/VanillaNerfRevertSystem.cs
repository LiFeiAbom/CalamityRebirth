using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems
{
    /// <summary>
    /// 灾厄原版削弱回调的统一数据入口。
    /// 数值覆盖现代 2.2.2、经典 1.4.2.101 与 CalRD 1.4.5.7，
    /// 并按实际加载的灾厄分支只执行对应版本的恢复逻辑。
    /// </summary>
    internal static class VanillaNerfRevertSystem
    {
        /// <summary>现代灾厄的扩展模组内部名。</summary>
        private const string ModernModName = "CalamityMod";
        /// <summary>扩展模组内部名，用于无强引用识别当前加载的经典灾厄分支。</summary>
        private const string ClassicModName = "CalamityModClassicPreTrailer";
        /// <summary>CalRD 1.4.5.7 的扩展模组内部名。</summary>
        private const string CalRDModName = "CalRD";
        /// <summary>现代 2.2.2 中被灾厄调低的原版斧力，内部值以 5% 为单位。</summary>
        private static readonly Dictionary<int, int> ModernVanillaAxe = new()
        {
            { ItemID.AcornAxe, 30 },
            { ItemID.AdamantiteChainsaw, 20 },
            { ItemID.MoltenHamaxe, 30 },
            { ItemID.MythrilChainsaw, 17 },
            { ItemID.OrichalcumChainsaw, 18 },
            { ItemID.PalladiumChainsaw, 15 },
            { ItemID.SawtoothShark, 14 },
            { ItemID.TitaniumChainsaw, 21 },
        };
        /// <summary>现代 2.2.2 中被灾厄调低的原版镐力。</summary>
        private static readonly Dictionary<int, int> ModernVanillaPick = new()
        {
            { ItemID.LaserDrill, 230 },
            { ItemID.LeadPickaxe, 43 },
            { ItemID.OrichalcumDrill, 165 },
            { ItemID.OrichalcumPickaxe, 165 },
            { ItemID.PlatinumPickaxe, 59 },
            { ItemID.TitaniumDrill, 190 },
            { ItemID.TitaniumPickaxe, 190 },
        };
        /// <summary>现代 2.2.2 中被灾厄调低的原版锤力。</summary>
        private static readonly Dictionary<int, int> ModernVanillaHammer = new()
        {
            { ItemID.AshWoodHammer, 45 },
            { ItemID.BorealWoodHammer, 35 },
            { ItemID.EbonwoodHammer, 40 },
            { ItemID.PalmWoodHammer, 35 },
            { ItemID.PearlwoodHammer, 55 },
            { ItemID.RichMahoganyHammer, 35 },
            { ItemID.Rockfish, 70 },
            { ItemID.ShadewoodHammer, 40 },
            { ItemID.TinHammer, 38 },
        };
        /// <summary>现代 2.2.2 中被灾厄调低的原版伤害。</summary>
        private static readonly Dictionary<int, int> ModernVanillaDamage = new()
        {
            { ItemID.AquaScepter, 27 },
            { ItemID.BeesKnees, 23 },
            { ItemID.BlizzardStaff, 58 },
            { ItemID.Boomstick, 14 },
            { ItemID.ChristmasTreeSword, 86 },
            { ItemID.Code2, 54 },
            { ItemID.CrystalBullet, 9 },
            { ItemID.DaedalusStormbow, 38 },
            { ItemID.DayBreak, 150 },
            { ItemID.DD2SquireBetsySword, 180 },
            { ItemID.DemonBow, 14 },
            { ItemID.DemonScythe, 35 },
            { ItemID.EmpressBlade, 90 },
            { ItemID.Flamarang, 49 },
            { ItemID.Flamethrower, 35 },
            { ItemID.FlowerofFire, 48 },
            { ItemID.Gradient, 49 },
            { ItemID.IchorBullet, 13 },
            { ItemID.InfluxWaver, 100 },
            { ItemID.Kraken, 95 },
            { ItemID.LaserMachinegun, 60 },
            { ItemID.MagicMissile, 35 },
            { ItemID.LastPrism, 100 },
            { ItemID.Minishark, 6 },
            { ItemID.MoonlordBullet, 20 },
            { ItemID.MoonlordTurretStaff, 100 },
            { ItemID.Musket, 31 },
            { ItemID.NailGun, 85 },
            { ItemID.RainbowRod, 50 },
            { ItemID.RavenStaff, 55 },
            { ItemID.Razorpine, 48 },
            { ItemID.RedsYoyo, 70 },
            { ItemID.ScourgeoftheCorruptor, 70 },
            { ItemID.Seedler, 50 },
            { ItemID.SilverBullet, 9 },
            { ItemID.StardustDragonStaff, 40 },
            { ItemID.TendonBow, 19 },
            { ItemID.Terragrim, 17 },
            { ItemID.Terrarian, 190 },
            { ItemID.TheEyeOfCthulhu, 115 },
            { ItemID.TheUndertaker, 19 },
            { ItemID.ThunderStaff, 20 },
            { ItemID.Tsunami, 53 },
            { ItemID.TungstenBullet, 9 },
            { ItemID.UnholyTrident, 88 },
            { ItemID.ValkyrieYoyo, 70 },
            { ItemID.Yelets, 60 },
        };
        /// <summary>现代 2.2.2 中被灾厄调低的原版防御。</summary>
        private static readonly Dictionary<int, int> ModernVanillaDefense = new()
        {
            { ItemID.SquireAltHead, 20 },
            { ItemID.SquireAltPants, 24 },
            { ItemID.SquireAltShirt, 24 },
            { ItemID.SquireGreatHelm, 13 },
            { ItemID.SquireGreaves, 18 },
            { ItemID.SquirePlating, 27 },
        };
        /// <summary>现代 2.2.2 中被灾厄调低的原版弹速。</summary>
        private static readonly Dictionary<int, float> ModernVanillaShootSpeed = new()
        {
            { ItemID.IceBoomerang, 11.5f },
            { ItemID.SpiritFlame, 3f },
        };
        /// <summary>现代 2.2.2 中被灾厄调低的原版使用时间。</summary>
        private static readonly Dictionary<int, int> ModernVanillaUseTime = new()
        {
            { ItemID.AdamantiteWaraxe, 8 },
            { ItemID.Beenade, 15 },
            { ItemID.BeesKnees, 23 },
            { ItemID.ChlorophyteClaymore, 26 },
            { ItemID.ChlorophyteJackhammer, 4 },
            { ItemID.ChristmasTreeSword, 23 },
            { ItemID.CoinGun, 8 },
            { ItemID.DayBreak, 16 },
            { ItemID.Handgun, 15 },
            { ItemID.IceBoomerang, 20 },
            { ItemID.InfluxWaver, 20 },
            { ItemID.MoltenFury, 22 },
            { ItemID.MythrilWaraxe, 10 },
            { ItemID.NebulaDrill, 2 },
            { ItemID.OrichalcumWaraxe, 9 },
            { ItemID.PhoenixBlaster, 14 },
            { ItemID.PsychoKnife, 8 },
            { ItemID.Sandgun, 16 },
            { ItemID.SolarFlareDrill, 2 },
            { ItemID.StarCannon, 12 },
            { ItemID.StardustDrill, 2 },
            { ItemID.TitaniumPickaxe, 7 },
            { ItemID.TitaniumWaraxe, 7 },
            { ItemID.VortexDrill, 2 },
        };
        /// <summary>现代 2.2.2 中被灾厄归零或调低的原版物品价值。</summary>
        private static readonly Dictionary<int, int> ModernVanillaValue = new()
        {
            { ItemID.Mushroom, Item.sellPrice(silver: 2, copper: 50) },
            { ItemID.GlowingMushroom, Item.sellPrice(copper: 10) },
            { ItemID.VileMushroom, Item.sellPrice(copper: 10) },
            { ItemID.ViciousMushroom, Item.sellPrice(copper: 10) },
            { ItemID.EncumberingStone, Item.sellPrice(gold: 1) },
            { ItemID.UncumberingStone, Item.sellPrice(gold: 1) },
            { ItemID.PortableStool, Item.sellPrice(silver: 50) },
        };
        /// <summary>
        /// 当前是否启用了原版削弱回调。
        /// </summary>
        internal static bool Enabled => VanillaNerfRevertConfig.Instance?.RevertVanillaNerfs == true;
        /// <summary>
        /// 是否加载了现代灾厄。
        /// </summary>
        internal static bool ModernLoaded => ModLoader.HasMod(ModernModName);
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
        /// 还原灾厄在 SetDefaults 阶段直接改写的物品基础数值。
        /// 源码使用整数截断，逆向除系数会在 28.5 一类结果上少还原 1 点，因此直接写回原版精确值。
        /// </summary>
        internal static void RestoreItemDefaults(Item item)
        {
            if (!Enabled)
                return;
            if (ModernLoaded)
                RestoreModernItemDefaults(item);
            if (ClassicLoaded)
                RestoreClassicPreTrailerItemDefaults(item);
            if (CalRDLoaded)
                RestoreCalRDItemDefaults(item);
        }
        /// <summary>
        /// 还原现代灾厄对原版武器、弹药、工具、护甲和价值的基础字段修改。
        /// 同一物品可能同时出现在多个表中，因此逐项检查而不提前返回。
        /// </summary>
        private static void RestoreModernItemDefaults(Item item)
        {
            if (ModernVanillaDamage.TryGetValue(item.type, out int damage))
                item.damage = damage;
            if (ModernVanillaUseTime.TryGetValue(item.type, out int useTime))
            {
                item.useTime = useTime;
                item.useAnimation = useTime;
            }
            if (ModernVanillaShootSpeed.TryGetValue(item.type, out float shootSpeed))
                item.shootSpeed = shootSpeed;
            if (ModernVanillaDefense.TryGetValue(item.type, out int defense))
                item.defense = defense;
            if (ModernVanillaAxe.TryGetValue(item.type, out int axe))
                item.axe = axe;
            if (ModernVanillaPick.TryGetValue(item.type, out int pick))
                item.pick = pick;
            if (ModernVanillaHammer.TryGetValue(item.type, out int hammer))
                item.hammer = hammer;
            if (ModernVanillaValue.TryGetValue(item.type, out int value))
                item.value = value;
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
        /// 按加载版本还原原版护甲单件被扣除的属性。
        /// 现代版与 1.4.2.101 各不相同，CalRD 1.4.5.7 不需要补偿这些单件属性。
        /// </summary>
        internal static void RestoreArmorPieceStats(Item item, Player player)
        {
            if (!Enabled)
                return;
            if (ModernLoaded)
                RestoreModernArmorPieceStats(item, player);
            if (ClassicLoaded)
                RestoreClassicArmorPieceStats(item, player);
        }
        /// <summary>还原 1.4.2.101 为盗贼化改造而扣除的塔防护甲单件属性。</summary>
        private static void RestoreClassicArmorPieceStats(Item item, Player player)
        {
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
        /// 还原现代灾厄对原版头、胸甲和腿甲单件扣除的魔法、召唤、远程和近战属性。
        /// 法袍的魔法消耗降低值按宝石等级逐级恢复。
        /// </summary>
        private static void RestoreModernArmorPieceStats(Item item, Player player)
        {
            switch (item.type)
            {
                case ItemID.MagicHat:
                    player.GetDamage<MagicDamageClass>() += 0.06f;
                    break;
                case ItemID.JungleHat:
                case ItemID.AncientCobaltHelmet:
                    player.statManaMax2 += 20;
                    player.GetCritChance<MagicDamageClass>() += 3;
                    break;
                case ItemID.JunglePants:
                case ItemID.AncientCobaltLeggings:
                    player.GetCritChance<MagicDamageClass>() += 3;
                    break;
                case ItemID.AmethystRobe:
                    player.manaCost -= 0.01f;
                    break;
                case ItemID.TopazRobe:
                    player.statManaMax2 += 20;
                    player.manaCost -= 0.02f;
                    break;
                case ItemID.SapphireRobe:
                    player.manaCost -= 0.03f;
                    break;
                case ItemID.EmeraldRobe:
                    player.statManaMax2 += 20;
                    player.manaCost -= 0.04f;
                    break;
                case ItemID.RubyRobe:
                case ItemID.AmberRobe:
                    player.manaCost -= 0.05f;
                    break;
                case ItemID.DiamondRobe:
                    player.statManaMax2 += 20;
                    player.manaCost -= 0.06f;
                    break;
                case ItemID.Gi:
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
                    break;
                case ItemID.ShroomiteBreastplate:
                    player.GetDamage<RangedDamageClass>() += 0.05f;
                    player.GetCritChance<RangedDamageClass>() += 5;
                    break;
                case ItemID.SquireAltShirt:
                    player.GetDamage<SummonDamageClass>() += 0.1f;
                    break;
                case ItemID.SquireAltPants:
                    player.GetCritChance<MeleeDamageClass>() += 5;
                    player.GetDamage<SummonDamageClass>() += 0.05f;
                    break;
                case ItemID.SolarFlareHelmet:
                    player.GetCritChance<MeleeDamageClass>() += 6;
                    break;
                case ItemID.VortexHelmet:
                    player.GetDamage<RangedDamageClass>() += 0.06f;
                    player.GetCritChance<RangedDamageClass>() += 2;
                    break;
            }
        }
        /// <summary>
        /// 还原现代灾厄对原版套装效果施加的数值削减。
        /// 钴、精金、丛林、熔岩和日耀分别补回攻速、法力消耗、近战伤害或减伤。
        /// </summary>
        internal static void RestoreArmorSetStats(Player player, string set)
        {
            if (!Enabled || !ModernLoaded)
                return;
            switch (set)
            {
                case "WizardHat":
                    player.GetCritChance<MagicDamageClass>() += 6;
                    break;
                case "MagicHat":
                    player.statManaMax2 += 20;
                    break;
                case "Adamantite":
                    if (player.armor[0].type == ItemID.AdamantiteHelmet)
                        player.GetAttackSpeed<MeleeDamageClass>() += 0.05f;
                    break;
                case "Cobalt":
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.05f;
                    break;
                case "Jungle":
                    player.manaCost -= 0.06f;
                    break;
                case "Molten":
                    player.GetDamage<MeleeDamageClass>() += 0.03f;
                    break;
                case "SolarFlare":
                    player.endurance += 0.12f;
                    break;
            }
        }
        /// <summary>
        /// 还原灾厄对原版饰品效果施加的版本专属削减。
        /// 现代版恢复侦察镜、狙击镜、爪类与日月石系属性；经典版恢复蠕虫围巾和词缀暴击。
        /// </summary>
        internal static void RestoreAccessoryStats(Item item, Player player)
        {
            if (!Enabled)
                return;
            if (ModernLoaded)
            {
                RestoreModernAccessoryStats(item, player);
                return;
            }
            if (!AnyClassicLoaded)
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
        /// 还原现代灾厄调低的原版饰品属性。
        /// 手套系只补野性爪净损失的 2% 近战攻速，其他手套由灾厄自身按等级重新发放，不能重复叠加。
        /// </summary>
        private static void RestoreModernAccessoryStats(Item item, Player player)
        {
            switch (item.type)
            {
                case ItemID.ReconScope:
                    player.GetCritChance<RangedDamageClass>() += 5;
                    break;
                case ItemID.SniperScope:
                    player.GetDamage<RangedDamageClass>() += 0.1f;
                    break;
                case ItemID.FeralClaws:
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.02f;
                    break;
                case ItemID.SunStone:
                    if (Main.dayTime)
                        player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
                    break;
                case ItemID.MoonStone:
                    if (!Main.dayTime || Main.eclipse)
                        player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
                    break;
                case ItemID.CelestialStone:
                case ItemID.CelestialShell:
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
                    break;
            }
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
        /// 按加载版本还原灾厄削弱过的原版增益。
        /// 1.4.2.101 把星云伤害从四职业挪给盗贼；CalRD 则直接从通用伤害扣除。
        /// 现代版还削减了箭术、魔法力量、移动、挖掘、食物和甲虫增益。
        /// </summary>
        internal static void RestoreBuffStats(int type, Player player)
        {
            if (!Enabled)
                return;
            if (ModernLoaded)
            {
                RestoreModernBuffStats(type, player);
                return;
            }
            if (!AnyClassicLoaded)
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
        /// 还原现代灾厄对原版增益施加的逐帧数值削减。
        /// 甲虫耐力把现代版的每球 10% 加算换算回原版乘算 15%，避免高额减伤叠加时超补。
        /// </summary>
        private static void RestoreModernBuffStats(int type, Player player)
        {
            switch (type)
            {
                case BuffID.Archery:
                    player.arrowDamage /= 0.955f;
                    break;
                case BuffID.MagicPower:
                    player.GetDamage<MagicDamageClass>() += 0.1f;
                    break;
                case BuffID.SugarRush:
                    player.moveSpeed += 0.1f;
                    player.pickSpeed -= 0.1f;
                    break;
                case BuffID.Mining:
                    player.pickSpeed -= 0.1f;
                    break;
                case BuffID.Swiftness:
                    player.moveSpeed += 0.1f;
                    break;
                case BuffID.Tipsy:
                    player.GetCritChance<MeleeDamageClass>() += 2;
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.2f;
                    player.GetDamage<MeleeDamageClass>() += 0.1f;
                    break;
                case BuffID.WellFed:
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.05f;
                    player.moveSpeed += 0.15f;
                    break;
                case BuffID.WellFed2:
                    player.pickSpeed -= 0.025f;
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.075f;
                    player.moveSpeed += 0.225f;
                    break;
                case BuffID.WellFed3:
                    player.pickSpeed -= 0.05f;
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
                    player.moveSpeed += 0.3f;
                    break;
                case BuffID.Werewolf:
                    player.GetAttackSpeed<MeleeDamageClass>() += 0.051f;
                    break;
                case BuffID.Rabies:
                    player.GetDamage<GenericDamageClass>() += 0.2f;
                    break;
                case BuffID.NebulaUpDmg1:
                    player.GetDamage<MagicDamageClass>() += 0.075f;
                    break;
                case BuffID.NebulaUpDmg2:
                    player.GetDamage<MagicDamageClass>() += 0.15f;
                    break;
                case BuffID.NebulaUpDmg3:
                    player.GetDamage<MagicDamageClass>() += 0.225f;
                    break;
                case BuffID.NebulaUpLife1:
                    player.lifeRegen += 2;
                    break;
                case BuffID.NebulaUpLife2:
                    player.lifeRegen += 4;
                    break;
                case BuffID.NebulaUpLife3:
                    player.lifeRegen += 6;
                    break;
            }
            if (type >= BuffID.BeetleMight1 && type <= BuffID.BeetleMight3 && player.beetleOffense)
            {
                int orbs = Math.Clamp(player.beetleOrbs, 0, 3);
                player.GetAttackSpeed<MeleeDamageClass>() += 0.05f * orbs;
            }
            else if (type >= BuffID.BeetleEndurance1 && type <= BuffID.BeetleEndurance3
                && player.beetleDefense)
            {
                int orbs = Math.Clamp(player.beetleOrbs, 0, 3);
                if (orbs > 0)
                {
                    float other = Math.Max(0f, player.endurance - 0.1f * orbs);
                    float target = 1f - (1f - other) * (1f - 0.15f * orbs);
                    player.endurance += target - player.endurance;
                }
            }
        }
        /// <summary>
        /// 还原灾厄在弹幕命中阶段施加的原版伤害修正。
        /// 现代版恢复召唤弹幕在手持其他职业武器时的 25% 惩罚，经典版恢复三种原版远程弹幕伤害缩减。
        /// 使用 FinalDamage 的逆系数抵消命中阶段的修改，不会递归改变弹幕自身的后续伤害。
        /// </summary>
        internal static void RestoreProjectileDamage(Player player, Projectile projectile
            , ref NPC.HitModifiers modifiers)
        {
            if (!Enabled || projectile.npcProj || projectile.trap)
                return;
            if (ModernLoaded)
                RestoreModernSummonPenalty(player, projectile, ref modifiers);
            if (AnyClassicLoaded && projectile.DamageType.CountsAsClass(DamageClass.Ranged))
            {
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
        }
        /// <summary>
        /// 抵消现代灾厄对跨职业召唤弹幕施加的 0.75 倍最终伤害惩罚。
        /// 只处理玩家手持其他职业武器且不处于禁忌套装或撒旦军事件的常规攻击状态。
        /// </summary>
        private static void RestoreModernSummonPenalty(Player player, Projectile projectile
            , ref NPC.HitModifiers modifiers)
        {
            if (!projectile.friendly || projectile.hostile
                || !projectile.DamageType.CountsAsClass<SummonDamageClass>())
                return;
            Item heldItem = player.HeldItem;
            bool forbiddenWithMagicWeapon = player.armor[0].type == ItemID.AncientBattleArmorHat
                && player.armor[1].type == ItemID.AncientBattleArmorShirt
                && player.armor[2].type == ItemID.AncientBattleArmorPants
                && heldItem.CountsAsClass(DamageClass.Magic);
            bool crossClassPenaltyDisabled = forbiddenWithMagicWeapon
                || Terraria.GameContent.Events.DD2Event.Ongoing;
            bool heldClassedWeapon = !heldItem.CountsAsClass<SummonDamageClass>()
                && (heldItem.CountsAsClass<MeleeDamageClass>()
                    || heldItem.CountsAsClass<RangedDamageClass>()
                    || heldItem.CountsAsClass<MagicDamageClass>()
                    || heldItem.CountsAsClass<ThrowingDamageClass>());
            bool heldIsTool = heldItem.pick > 0 || heldItem.axe > 0 || heldItem.hammer > 0;
            if (heldClassedWeapon && heldItem.useStyle != ItemUseStyleID.None && !heldIsTool
                && !heldItem.accessory && heldItem.ammo == AmmoID.None && !crossClassPenaltyDisabled)
                modifiers.FinalDamage /= 0.75f;
        }
        /// <summary>
        /// 还原现代灾厄对暗影护甲和腾飞徽章移动属性的削减，或还原经典复仇模式的跑速惩罚。
        /// 现代分支同时补回腾飞徽章的跳跃提速和无限飞行时间。
        /// </summary>
        internal static void RestoreRunSpeedPenalty(Player player)
        {
            if (!Enabled)
                return;
            if (ModernLoaded)
            {
                RestoreModernRunSpeedPenalty(player);
                return;
            }
            RestoreClassicRunSpeedPenalty(player);
        }
        /// <summary>
        /// 以灾厄当前结算后的最终速度为基准恢复现代版削减的暗影护甲与腾飞徽章属性。
        /// 暗影护甲的魔光叠加状态保持灾厄原有判定，避免和玩家主动装备效果重复计算。
        /// </summary>
        private static void RestoreModernRunSpeedPenalty(Player player)
        {
            if (player.shadowArmor && !(player.hasMagiluminescence && player.velocity.Y == 0f))
            {
                player.runAcceleration *= 1.75f / 1.25f;
                player.maxRunSpeed *= 1.15f / 1.05f;
                player.accRunSpeed *= 1.15f / 1.05f;
                player.runSlowdown *= 1.75f / 1.5f;
            }
            if (player.empressBrooch)
            {
                player.runAcceleration *= 1.75f / 1.25f;
                player.jumpSpeedBoost += 1.3f;
                player.wingTime = player.wingTimeMax;
            }
        }
        /// <summary>
        /// 还原经典灾厄复仇模式对原版跑鞋持续奔跑速度与冰面靴加速的全局惩罚。
        /// 1.4.2.101 只削跑速；CalRD 还把“滑冰鞋加速 + 光滑地形”的加速度乘 2/3。
        /// 反射读取两个经典灾厄分支各自的复仇标志，避免为扩展模组增加强引用。
        /// </summary>
        private static void RestoreClassicRunSpeedPenalty(Player player)
        {
            if (!AnyClassicLoaded)
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
        /// 还原现代灾厄对蝴蝶翅膀和幽灵翅膀上升倍率的削减。
        /// 灾厄同处提供的其他加强保持不变。
        /// </summary>
        internal static void RestoreVerticalWingSpeeds(Item item, ref float maxAscentMultiplier)
        {
            if (!Enabled || !ModernLoaded)
                return;
            if (item.type == ItemID.ButterflyWings)
                maxAscentMultiplier /= 0.6667f;
            else if (item.type == ItemID.GhostWings)
                maxAscentMultiplier /= 0.6625f;
        }
        /// <summary>
        /// 在灾厄完成翅膀静态数据注册后恢复被调低的飞行时间、水平速度、悬浮速度和加速度。
        /// 必须由 PostSetupContent 调用，确保修改命中的是最终使用的翅膀属性表。
        /// </summary>
        internal static void ApplyWingStatOverrides()
        {
            if (!Enabled || !ModernLoaded)
                return;
            var stats = ArmorIDs.Wing.Sets.Stats;
            if (stats.Length <= 45)
                return;
            stats[9].FlyTime = 160;
            stats[14].AccRunSpeedOverride = 7.5f;
            stats[44].FlyTime = 150;
            stats[28].DownHoverSpeedOverride = 12f;
            stats[28].DownHoverAccelerationMult = 12f;
            stats[33].DownHoverSpeedOverride = 12f;
            stats[33].DownHoverAccelerationMult = 12f;
            stats[35].DownHoverSpeedOverride = 12f;
            stats[35].DownHoverAccelerationMult = 12f;
            stats[37].DownHoverSpeedOverride = 12f;
            stats[37].DownHoverAccelerationMult = 12f;
            stats[45].AccRunAccelerationMult = 4.5f;
            stats[45].DownHoverSpeedOverride = 16f;
            stats[45].DownHoverAccelerationMult = 16f;
        }
        /// <summary>
        /// 读取当前经典灾厄的复仇模式开关。
        /// 1.4.2.101 使用 CalamityWorldPreTrailer.revenge；CalRD 使用 CalamityWorld.revenge。
        /// 两个经典兼容分支都提供可读取的静态布尔字段。
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
