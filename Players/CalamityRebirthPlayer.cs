using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Items.Accessories.Combat.Magic;
using CalamityRebirth.Content.Items.Accessories.Combat.Ranged;
using CalamityRebirth.Content.Items.Accessories.Combat.Rogue;
using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions;
using CalamityRebirth.Content.Projectiles.Magic;
using CalamityRebirth.Content.Projectiles.Rogue;
using CalamityRebirth.Content.Projectiles.Typeless;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
namespace CalamityRebirth.Players
{
    /// <summary>
    /// 模组玩家状态与统一属性结算入口。
    /// </summary>
    public partial class CalamityRebirthPlayer : ModPlayer
    {
        /// <summary>天界洋葱永久解锁状态在自定义网络消息中的消息码。</summary>
        public const byte MsgPermanentUnlock = 0;
        /// <summary>客户端请求服务器结算受限治疗效果的消息码。</summary>
        public const byte MsgRequestHeal = 1;
        /// <summary>盾牌冲刺开始或结束的同步消息码。</summary>
        public const byte MsgShieldSlamDash = 2;
        /// <summary>盾牌冲刺撞击粒子表现的同步消息码。</summary>
        public const byte MsgShieldSlamDashHit = 3;
        /// <summary>晶光头骨潜行治疗来源。</summary>
        public const byte HealRequestCrystalSkull = 0;
        /// <summary>新生药水受击治疗来源。</summary>
        public const byte HealRequestRevivify = 1;
        /// <summary>光合作用红心治疗来源。</summary>
        public const byte HealRequestPhotosynthesis = 2;
        /// <summary>纳米技术潜行打击治疗来源。</summary>
        public const byte HealRequestNanotech = 3;
        /// <summary>
        /// 魔君牌兴奋剂提供的亚利姆之力标记。
        /// </summary>
        public bool yharimPower;
        /// <summary>本帧是否受到龙舌兰日出增益影响。</summary>
        public bool tequilaSunrise;
        /// <summary>本帧累计的酒醉等级；各酒类增益与原生酒醉分别计数。</summary>
        public int alcoholPoisonLevel;
        /// <summary>本帧是否处于酒精中毒提示状态。</summary>
        public bool alcoholPoisoning;
        /// <summary>本帧是否受到血腥玛丽影响。</summary>
        public bool bloodyMary;
        /// <summary>本帧是否受到加勒比朗姆酒影响。</summary>
        public bool caribbeanRum;
        /// <summary>本帧是否受到摩闪酒影响。</summary>
        public bool moonshine;
        /// <summary>本帧是否受到莫斯科骡子影响。</summary>
        public bool moscowMule;
        /// <summary>本帧是否受到朗姆酒影响。</summary>
        public bool rum;
        /// <summary>本帧是否受到伏特加影响。</summary>
        public bool vodka;
        /// <summary>本帧是否受到护甲崩裂药水影响。</summary>
        public bool armorCrumbling;
        /// <summary>本帧是否受到护甲粉碎药水影响。</summary>
        public bool armorShattering;
        /// <summary>本帧是否受到弹跳药水影响。</summary>
        public bool bounding;
        /// <summary>本帧是否受到韵律药水影响。</summary>
        public bool cadence;
        /// <summary>本帧是否受到钙质药水影响。</summary>
        public bool calcium;
        /// <summary>本帧是否受到无尽饥渴药水影响。</summary>
        public bool ceaselessHunger;
        /// <summary>本帧是否受到神圣之怒药水影响。</summary>
        public bool holyWrath;
        /// <summary>本帧是否受到光合作用药水影响。</summary>
        public bool photosynthesis;
        /// <summary>本帧是否受到渎神之怒药水影响。</summary>
        public bool profanedRage;
        /// <summary>本帧是否受到新生药水影响。</summary>
        public bool revivify;
        /// <summary>本帧是否受到翱翔药水影响。</summary>
        public bool soaring;
        /// <summary>本帧是否受到泰坦之鳞药水影响。</summary>
        public bool titanScale;
        /// <summary>本帧是否受到胜利药水影响。</summary>
        public bool triumph;
        /// <summary>本帧是否装备了血炎晶核。</summary>
        public bool bloodflareCore;
        /// <summary>本帧是否装备了血契。</summary>
        public bool bloodPact;
        /// <summary>本帧是否装备了血肉图腾，或其上位血神核心。</summary>
        public bool fleshTotem;
        /// <summary>本帧是否装备了血腥蠕虫围巾。</summary>
        public bool bloodyWormScarf;
        /// <summary>本帧是否装备了血腥蠕虫牙。</summary>
        public bool bloodyWormTooth;
        /// <summary>本帧是否装备了血神核心。</summary>
        public bool coreOfTheBloodGod;
        /// <summary>
        /// 本帧是否装备烦恼项链，用于低生命动态增伤与 NPC 命中附加毒液。
        /// </summary>
        public bool vexation;
        /// <summary>血肉图腾接触减伤剩余冷却帧数。</summary>
        private int fleshTotemCooldown;
        /// <summary>本帧是否装备了战士徽章EX，用于近战命中附加燃烧III。</summary>
        public bool warriorEmblemEX;
        /// <summary>本帧是否装备了烈火手套EX，用于复合增益和附加两种近战减益。</summary>
        public bool fireGauntletEX;
        /// <summary>本帧是否装备魔君之证章，用于经典近战面板、低生命增伤与圣火命中。</summary>
        public bool yharimsInsignia;
        /// <summary>本帧是否装备元素之握，用于近战面板、命中与 PvP 结算。</summary>
        public bool elementalGauntlet;
        /// <summary>本帧是否装备了游侠徽章EX，用于远程省弹和弹速结算。</summary>
        public bool rangerEmblemEX;
        /// <summary>本帧是否装备了代达罗斯纹章，用于 20% 概率不消耗弹药。</summary>
        public bool daedalusEmblem;
        /// <summary>本帧是否装备了元素箭袋，用于省弹、箭矢分裂与非箭弹幕增伤。</summary>
        public bool elementalQuiver;
        /// <summary>本帧是否装备了狙击镜EX，用于远程省弹、弹速和瞄准能力。</summary>
        public bool sniperScopeEX;
        /// <summary>本帧是否装备了法师徽章EX，用于额外回蓝。</summary>
        public bool sorcererEmblemEX;
        /// <summary>本帧是否装备了天界纹章EX，用于高阶回蓝与隐藏兼容能力。</summary>
        public bool celestialEmblemEX;
        /// <summary>本帧是否装备了召唤师徽章EX，用于召唤穿防。</summary>
        public bool summonerEmblemEX;
        /// <summary>本帧是否装备了甲虫莎草纸EX，用于召唤穿防。</summary>
        public bool papyrusScarabEX;
        /// <summary>本帧是否装备了始源暗影炎，用于仆从命中附加暗影焰。</summary>
        public bool theFirstShadowflame;
        /// <summary>本帧是否装备斯塔提斯的祝福。</summary>
        public bool statisBlessing;
        /// <summary>本帧是否装备斯塔提斯的诅咒。</summary>
        public bool statisCurse;
        /// <summary>本帧是否装备斯塔提斯诅咒腰带。</summary>
        public bool statisBeltOfCurses;
        /// <summary>本帧是否装备了盗贼徽章EX，用于晚装阶段应用三版本盗贼属性。</summary>
        public bool rogueEmblemEX;
        /// <summary>本帧是否装备了晶光头骨，用于盗贼属性、潜行追击与闪电结算。</summary>
        public bool crystalSkull;
        /// <summary>本帧是否装备欺诈硬币。</summary>
        public bool coinofDeceit;
        /// <summary>本帧是否装备毁灭徽章。</summary>
        public bool ruinMedallion;
        /// <summary>本帧是否装备静默剑鞘。</summary>
        public bool silencingSheath;
        /// <summary>本帧是否装备幻影魔镜。</summary>
        public bool mirageMirror;
        /// <summary>本帧是否装备深渊魔镜。</summary>
        public bool abyssalMirror;
        /// <summary>本帧是否装备暗物质剑鞘。</summary>
        public bool darkMatterSheath;
        /// <summary>本帧是否装备蚀日魔镜。</summary>
        public bool eclipseMirror;
        /// <summary>本帧是否装备蚀日尊戒；属性在 PostUpdateMiscEffects 中统一结算。</summary>
        public bool darkSunRing;
        /// <summary>本帧是否装备腐烂之脑。</summary>
        public bool rottenBrain;
        /// <summary>本帧是否装备聚合之脑。</summary>
        public bool amalgamatedBrain;
        /// <summary>本帧是否装备灾厄之环。</summary>
        public bool calamityRing;
        /// <summary>本帧是否装备焦热地狱。</summary>
        public bool gehenna;
        /// <summary>本帧是否装备终结虚空。</summary>
        public bool voidofExtinction;
        /// <summary>本帧是否装备利维坦龙涎香。</summary>
        public bool leviathanAmbergris;
        /// <summary>本帧是否装备真菌团块或其顶点融合之脑。</summary>
        public bool fungalClump;
        /// <summary>本帧是否装备融合之脑。</summary>
        public bool theAmalgam;
        /// <summary>焦热地狱下轮硫磺火齐射倒计时。</summary>
        private int gehennaFireCountdown;
        /// <summary>终结虚空下轮硫磺火齐射倒计时。</summary>
        private int voidFireCountdown;
        /// <summary>融合之脑下轮硫磺火齐射倒计时。</summary>
        private int amalgamFireCountdown;
        /// <summary>本帧是否装备纳米技术。</summary>
        public bool nanotech;
        /// <summary>本帧玩家是否受到触电II影响。</summary>
        public bool electrifiedII;
        /// <summary>本帧玩家是否受到元素混合II影响。</summary>
        public bool elementalMixII;
        /// <summary>本帧玩家是否受到本模组自定义暗影焰影响。</summary>
        public bool shadowflameII;
        /// <summary>本帧是否装备王冠宝石；实际再生由 UpdateLifeRegen 统一结算。</summary>
        public bool crownJewel;
        /// <summary>本帧是否装备感染宝石；实际再生与动态防御由 UpdateLifeRegen 统一结算。</summary>
        public bool infectedJewel;
        /// <summary>本帧是否装备无暇粹魂晶；优先级高于感染宝石与王冠宝石。</summary>
        public bool purity;
        /// <summary>本帧是否装备寒霜壁垒。</summary>
        public bool frigidBulwark;
        /// <summary>本帧是否装备神圣护符。</summary>
        public bool deificAmulet;
        /// <summary>本帧是否装备神之壁垒；该顶点优先覆盖两件前置的重复效果。</summary>
        public bool rampartOfDeities;
        /// <summary>是否继承甘露安瓿古链的常驻蜂蜜式再生。</summary>
        public bool alwaysHoneyRegen;
        /// <summary>是否将病症、火系及其他目标战斗减益的持续时间减半。</summary>
        public bool honeyDewHalveDebuffs;
        /// <summary>Whether honey-turbo regeneration is active.</summary>
        public bool honeyTurboRegen;
        /// <summary>Whether fire debuffs are also halved.</summary>
        public bool livingDewHalveDebuffs;
        /// <summary>Whether Radiant Ooze (Ancient) is equipped.</summary>
        public bool rOoze;
        /// <summary>Whether Ambrosial Ampoule (Ancient) is equipped.</summary>
        public bool aAmpoule;
        /// <summary>
        /// 无暇粹魂晶带减益直接回血的节拍惩罚累计帧数。
        /// 该值跨帧保存，只在减益清空时逐帧回落，并在死亡时归零。
        /// </summary>
        private int purityHealSlowdownFrames;
        /// <summary>
        /// 宝石系动态防御的跨帧存量，减益消退后每 60 帧降低 1 点。
        /// 该值不能放进 ResetEffects，否则每帧清零后无法形成持续回退效果。
        /// </summary>
        private int jewelBonusDefense;
        /// <summary>禅心剑每次挥砍只播放一次命中音效，此标记在首次近战命中后清零。</summary>
        public bool ataraxiaHitSound;
        /// <summary>玩家是否已经食用天界洋葱并永久获得额外饰品槽。</summary>
        public bool extraAccessoryML;
        /// <summary>法力饰品的逐帧计时器，累计到 60 帧后统一回复每秒法力值。</summary>
        public int passiveManaRegenTimer;
        /// <summary>晶光头骨闪电的剩余冷却帧数。</summary>
        private int crystalSkullLightningCooldown;
        /// <summary>本次潜行打击已经触发电火花的命中次数。</summary>
        private int crystalSkullStealthHitCount;
        /// <summary>本次潜行打击是否已经触发过治疗。</summary>
        private bool crystalSkullHealedThisStrike;
        /// <summary>镜子闪避的共享冷却帧数。</summary>
        private int mirrorDodgeCooldown;
        /// <summary>纳米技术潜行火花冷却帧数。</summary>
        public int nanotechFlareCooldown;
        /// <summary>本次潜行打击是否已经触发纳米技术治疗。</summary>
        private bool nanotechHealedThisStrike;
        /// <summary>
        /// 在所有装备效果重新计算前清空本帧缓存。
        /// UpdateAccessory 随后只重新点亮当前实际装备的饰品，避免卸下后仍保留效果。
        /// </summary>
        public override void ResetEffects()
        {
            yharimPower = false;
            tequilaSunrise = false;
            alcoholPoisonLevel = 0;
            alcoholPoisoning = false;
            bloodyMary = false;
            caribbeanRum = false;
            moonshine = false;
            moscowMule = false;
            rum = false;
            vodka = false;
            armorCrumbling = false;
            armorShattering = false;
            bounding = false;
            cadence = false;
            calcium = false;
            ceaselessHunger = false;
            holyWrath = false;
            photosynthesis = false;
            profanedRage = false;
            revivify = false;
            soaring = false;
            titanScale = false;
            triumph = false;
            bloodflareCore = false;
            bloodPact = false;
            fleshTotem = false;
            bloodyWormScarf = false;
            bloodyWormTooth = false;
            coreOfTheBloodGod = false;
            vexation = false;
            warriorEmblemEX = false;
            fireGauntletEX = false;
            yharimsInsignia = false;
            elementalGauntlet = false;
            rangerEmblemEX = false;
            daedalusEmblem = false;
            elementalQuiver = false;
            sniperScopeEX = false;
            sorcererEmblemEX = false;
            celestialEmblemEX = false;
            summonerEmblemEX = false;
            papyrusScarabEX = false;
            theFirstShadowflame = false;
            statisBlessing = false;
            statisCurse = false;
            statisBeltOfCurses = false;
            rogueEmblemEX = false;
            crystalSkull = false;
            coinofDeceit = false;
            ruinMedallion = false;
            silencingSheath = false;
            mirageMirror = false;
            abyssalMirror = false;
            darkMatterSheath = false;
            eclipseMirror = false;
            darkSunRing = false;
            rottenBrain = false;
            amalgamatedBrain = false;
            calamityRing = false;
            gehenna = false;
            voidofExtinction = false;
            leviathanAmbergris = false;
            fungalClump = false;
            theAmalgam = false;
            nanotech = false;
            electrifiedII = false;
            elementalMixII = false;
            shadowflameII = false;
            crownJewel = false;
            infectedJewel = false;
            purity = false;
            frigidBulwark = false;
            deificAmulet = false;
            rampartOfDeities = false;
            alwaysHoneyRegen = false;
            honeyDewHalveDebuffs = false;
            honeyTurboRegen = false;
            livingDewHalveDebuffs = false;
            rOoze = false;
            aAmpoule = false;
            ResetShieldSlamEffects();
            ResetAfflictionEffects();
            ResetSpongeChainEffects();
			ResetNebulousCoreEffects();
			ResetElementalChainEffects();
			ResetWingChainEffects();
        }
        /// <summary>物品直接攻击 NPC 时应用召唤物无视防御。</summary>
        /// <summary>
        /// 死亡时清空宝石系的跨帧战斗状态，避免复活后继承上一轮减益造成的回血节拍惩罚和防御。
        /// </summary>
        public override void UpdateDead()
        {
            purityHealSlowdownFrames = 0;
            jewelBonusDefense = 0;
            ResetShieldSlamDashState();
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            ApplySummonDefenseIgnore(item.DamageType, ref modifiers);
            ApplyCrystalSkullStealthModifiers(item.DamageType, ref modifiers);
        }
        /// <summary>弹幕攻击 NPC 时应用召唤物无视防御与版本专属原版伤害回调。</summary>
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            ApplySummonDefenseIgnore(proj.DamageType, ref modifiers);
            ApplyCrystalSkullStealthModifiers(proj.DamageType, ref modifiers);
            ApplyRogueMirrorHitModifiers(proj, ref modifiers);
            ApplyElementalQuiverProjectileBonus(proj, ref modifiers);
            VanillaNerfRevertSystem.RestoreProjectileDamage(Player, proj, ref modifiers);
        }
        /// <summary>
        /// 在灾厄完成跑速倍率结算后，撤销当前版本对原版移动属性的削减。
        /// </summary>
        public override void PostUpdateRunSpeeds()
        {
            VanillaNerfRevertSystem.RestoreRunSpeedPenalty(Player);
            ApplyShieldSlamRunSpeedEffects();
            ShieldSlamDashMovement();
        }
        /// <summary>
        /// 将召唤饰品的无视防御比例转换为 DefenseEffectiveness 乘数。
        /// 多个饰品的比例先相加，再一次性乘算，避免重复走命中修饰器。
        /// </summary>
        private void ApplySummonDefenseIgnore(DamageClass damageType, ref NPC.HitModifiers modifiers)
        {
            if (!damageType.CountsAsClass(DamageClass.Summon))
                return;
            float ignore = 0f;
            if (summonerEmblemEX)
                ignore += SummonerEmblemEX.DefenseIgnore;
            if (papyrusScarabEX)
                ignore += PapyrusScarabEX.DefenseIgnore;
            if (ignore > 0f)
                modifiers.DefenseEffectiveness *= 1f - ignore;
        }
        /// <summary>
        /// 晶光头骨继承电工手套的潜行打击穿甲与增伤。
        /// 只对当前灾厄版本确认处于潜行打击的盗贼伤害生效。
        /// </summary>
        private void ApplyCrystalSkullStealthModifiers(DamageClass damageType, ref NPC.HitModifiers modifiers)
        {
            if (!crystalSkull || !damageType.CountsAsClass(DamageClass.Throwing) || !CRUtil.IsStealthStrike(Player))
                return;
            modifiers.ArmorPenetration += CrystalSkull.StealthArmorPenetration;
            modifiers.FinalDamage *= 1f + CrystalSkull.StealthDamageBonus;
        }
        /// <summary>
        /// 暗物质剑鞘与蚀日魔镜让盗贼潜行打击必定暴击。
        /// 纳米技术再给潜行打击 20 穿甲和 5% 最终增伤。
        /// </summary>
        private void ApplyRogueMirrorHitModifiers(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!CRUtil.IsRogueDamageType(projectile.DamageType) || !CRUtil.IsStealthStrike(Player))
                return;
            if (darkMatterSheath || eclipseMirror)
                modifiers.SetCrit();
            if (nanotech)
            {
                modifiers.ArmorPenetration += 20;
                modifiers.FinalDamage *= 1.05f;
            }
        }
        /// <summary>
        /// 元素箭袋让非箭类远程弹幕获得 5% 最终乘算伤害。
        /// 只接受本玩家的友好远程弹幕，避免召唤、近战、盗贼或被敌方控制的弹幕误享加成。
        /// </summary>
        private void ApplyElementalQuiverProjectileBonus(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!elementalQuiver || projectile.owner != Player.whoAmI || !projectile.friendly
                || projectile.hostile || projectile.arrow
                || !projectile.DamageType.CountsAsClass(DamageClass.Ranged))
                return;
            modifiers.FinalDamage *= ElementalQuiver.NonArrowDamageMultiplier;
        }
        /// <summary>物品近战命中后补挂燃烧III与破晓II。</summary>
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyMeleeHitDebuffs(target, hit.DamageType);
            ApplyPotionHitDebuffs(target);
            ApplyCrystalSkullOnHit(target, hit, -1);
        }
        /// <summary>近战弹幕命中后补挂燃烧III与破晓II。</summary>
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyMeleeHitDebuffs(target, hit.DamageType);
            ApplySummonAccessoryHitDebuffs(proj, target);
            ApplyPotionHitDebuffs(target);
            ApplyCrystalSkullOnHit(target, hit, proj.type);
            ApplyNanotechOnHit(proj, hit);
            TrySpawnAbaddonCrit(proj, target, hit);
        }
        /// <summary>纳米技术的盗贼潜行打击在每次潜行攻击中提供一次 1 HP 治疗。</summary>
        private void ApplyNanotechOnHit(Projectile projectile, NPC.HitInfo hit)
        {
            if (!nanotech || nanotechHealedThisStrike || !CRUtil.IsRogueDamageType(hit.DamageType)
                || !CRUtil.IsStealthStrike(Player) || Player.whoAmI != Main.myPlayer)
                return;
            nanotechHealedThisStrike = true;
            RequestHeal(Player, HealRequestNanotech);
        }
        /// <summary>
        /// 始源暗影炎只给真正的仆从与哨兵弹幕附加暗影焰。
        /// 不把鞭子、法师弹幕或盗贼弹幕误判为仆从攻击。
        /// </summary>
        private void ApplySummonAccessoryHitDebuffs(Projectile projectile, NPC target)
        {
            if (theFirstShadowflame && (projectile.minion || projectile.sentry))
                target.AddBuff(BuffID.ShadowFlame, TheFirstShadowflame.ShadowflameDuration);
            if (!projectile.DamageType.CountsAsClass(DamageClass.Summon)
                && !projectile.DamageType.CountsAsClass(DamageClass.SummonMeleeSpeed))
                return;
            int temporalSadnessDuration = statisBeltOfCurses ? 120
                : statisCurse || statisBlessing ? 60 : 0;
            if (temporalSadnessDuration > 0)
            {
                CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "TemporalSadness", temporalSadnessDuration);
                CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "TemporalSadness", temporalSadnessDuration);
                CalamityCompatUtil.TryAddBuff(target, "CalRD", "TemporalSadness", temporalSadnessDuration);
            }
            if (statisCurse || statisBeltOfCurses)
                target.AddBuff(BuffID.ShadowFlame, 300);
        }
        /// <summary>
        /// 将神圣之怒和两种碎甲药水的命中减益应用到 NPC。
        /// 所有外部 buff 都通过弱依赖查找，缺失的灾厄版本只跳过对应分支。
        /// </summary>
        private void ApplyPotionHitDebuffs(NPC target)
        {
            if (holyWrath)
            {
                CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "HolyFlames", 120);
                CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "HolyLight", 120);
                CalamityCompatUtil.TryAddBuff(target, "CalRD", "HolyFlames", 120);
            }
            if (armorShattering || armorCrumbling)
            {
                CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "ArmorCrunch", 240);
                CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "ArmorCrunch", 240);
                CalamityCompatUtil.TryAddBuff(target, "CalRD", "ArmorCrunch", 240);
            }
        }
        /// <summary>
        /// 仅对近战伤害附加减益：任意 EX 近战饰品提供燃烧III，烈火手套EX额外提供破晓II。
        /// </summary>
        private void ApplyMeleeHitDebuffs(NPC target, DamageClass damageType)
        {
            if (!damageType.CountsAsClass(DamageClass.Melee))
                return;
            if (warriorEmblemEX || fireGauntletEX)
                target.AddBuff(ModContent.BuffType<BurningIII>(), BurningIII.Duration);
            if (fireGauntletEX)
                target.AddBuff(ModContent.BuffType<DaybreakII>(), DaybreakII.SpreadDuration);
            if (yharimsInsignia)
                ApplyYharimsInsigniaHolyFire(target);
            if (elementalGauntlet)
                ApplyElementalGauntletDebuffs(target);
        }
        /// <summary>
        /// 元素之握只施加本工程的强化减益元素混合II。
        /// NPC 与 PvP 玩家分别使用重载，避免命中路径遗漏。
        /// </summary>
        private static void ApplyElementalGauntletDebuffs(NPC target)
        {
            target.AddBuff(ModContent.BuffType<ElementalMixII>(), ElementalGauntlet.HitDebuffDuration);
        }
        /// <summary>PvP 版本保持与 NPC 命中完全相同的元素混合II施加逻辑。</summary>
        private static void ApplyElementalGauntletDebuffs(Player target)
        {
            target.AddBuff(ModContent.BuffType<ElementalMixII>(), ElementalGauntlet.HitDebuffDuration);
        }
        /// <summary>
        /// 处理晶光头骨在盗贼命中后的全部效果。
        /// 闪电对所有盗贼命中独立判定；潜行火花、治疗与穿甲则由潜行打击状态触发。
        /// </summary>
        private void ApplyCrystalSkullOnHit(NPC target, NPC.HitInfo hit, int triggerProjectileType)
        {
            if (!crystalSkull || !hit.DamageType.CountsAsClass(DamageClass.Throwing)
                || Player.whoAmI != Main.myPlayer)
                return;
            TrySpawnCrystalSkullLightning(target, hit, triggerProjectileType);
            bool stealthStrike = CRUtil.IsStealthStrike(Player);
            if (!stealthStrike || crystalSkullStealthHitCount >= CrystalSkull.MaxSparkHitTriggers)
                return;
            crystalSkullStealthHitCount++;
            if (!crystalSkullHealedThisStrike)
            {
                RequestHeal(Player, HealRequestCrystalSkull);
                crystalSkullHealedThisStrike = true;
            }
            int sparkType = ModContent.ProjectileType<CrystalSkullSpark>();
            int sparkDamage = CRUtil.GetRogueDamage(Player, CrystalSkull.SparkBaseDamage);
            for (int i = 0; i < CrystalSkull.SparksPerTrigger; i++)
            {
                Vector2 velocity = new Vector2(5f, 0f).RotatedByRandom(MathHelper.TwoPi)
                    * Main.rand.NextFloat(0.65f, 1.15f);
                Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, velocity, sparkType
                    , sparkDamage, 0f, Player.whoAmI);
            }
        }
        /// <summary>
        /// 按 10% 概率召唤晶光头骨闪电，并用短冷却限制高频穿透弹幕。
        /// 闪电自身被排除，防止一次闪电继续递归触发新的闪电。
        /// </summary>
        private void TrySpawnCrystalSkullLightning(NPC target, NPC.HitInfo hit, int triggerProjectileType)
        {
            int lightningType = ModContent.ProjectileType<CrystalSkullLightning>();
            if (crystalSkullLightningCooldown > 0 || triggerProjectileType == lightningType
                || Main.rand.NextFloat() >= CrystalSkull.LightningChance)
                return;
            crystalSkullLightningCooldown = CrystalSkull.LightningCooldown;
            int damage = Math.Max(1, (int)(hit.Damage * CrystalSkull.LightningDamageMultiplier));
            Projectile.NewProjectile(Player.GetSource_OnHit(target), target.Center, Vector2.Zero, lightningType
                , damage, 0f, Player.whoAmI, target.whoAmI);
        }
        /// <summary>
        /// PvP 版本的目标是玩家而非 NPC，因此从攻击者的 ModPlayer 读取装备状态并调用 AddBuff。
        /// 该入口同时供物品和弹幕全局钩子复用。
        /// </summary>
        public static void TryApplyMeleeHitDebuffsOnPvP(Player attacker, Player target, DamageClass damageType)
        {
            if (attacker is null || !attacker.active || !target.active)
                return;
            if (!damageType.CountsAsClass(DamageClass.Melee))
                return;
            CalamityRebirthPlayer attackerPlayer = attacker.GetModPlayer<CalamityRebirthPlayer>();
            if (attackerPlayer.warriorEmblemEX || attackerPlayer.fireGauntletEX)
                target.AddBuff(ModContent.BuffType<BurningIII>(), BurningIII.Duration);
            if (attackerPlayer.fireGauntletEX)
                target.AddBuff(ModContent.BuffType<DaybreakII>(), DaybreakII.SpreadDuration);
            if (attackerPlayer.yharimsInsignia)
                ApplyYharimsInsigniaHolyFire(target);
            if (attackerPlayer.elementalGauntlet)
                ApplyElementalGauntletDebuffs(target);
        }
        /// <summary>
        /// 生成一次魔君圣火持续时间。
        /// 1/4 概率为 360 帧，否则再以 1/2 概率选择 240 帧，其余为 120 帧。
        /// </summary>
        private static int RollYharimsHolyDuration()
        {
            if (Main.rand.NextBool(4))
                return YharimsInsignia.HolyLongDuration;
            if (Main.rand.NextBool(2))
                return YharimsInsignia.HolyMediumDuration;
            return YharimsInsignia.HolyShortDuration;
        }
        /// <summary>
        /// 直接尝试施加当前灾厄版本提供的圣火，不建立显式版本分支。
        /// 未安装的版本由弱依赖查找安全跳过。
        /// </summary>
        private static void ApplyYharimsInsigniaHolyFire(NPC target)
        {
            int duration = RollYharimsHolyDuration();
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "HolyFlames", duration);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "HolyLight", duration);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "HolyFlames", duration);
        }
        /// <summary>PvP 版本使用同一持续时间与直接施加路径。</summary>
        private static void ApplyYharimsInsigniaHolyFire(Player target)
        {
            int duration = RollYharimsHolyDuration();
            CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "HolyFlames", duration);
            CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "HolyLight", duration);
            CalamityCompatUtil.TryAddBuff(target, "CalRD", "HolyFlames", duration);
        }
        /// <summary>
        /// PvP 版本的晶光头骨闪电判定。
        /// 闪电以负 NPC 索引生成，并在命中玩家时由弹幕自身施加触电II。
        /// </summary>
        public static void TryApplyCrystalSkullLightningOnPvP(Player attacker, Player target
            , DamageClass damageType, int sourceDamage)
        {
            if (attacker is null || target is null || !attacker.active || !target.active
                || attacker.whoAmI != Main.myPlayer || !damageType.CountsAsClass(DamageClass.Throwing))
                return;
            CalamityRebirthPlayer attackerPlayer = attacker.GetModPlayer<CalamityRebirthPlayer>();
            if (!attackerPlayer.crystalSkull || attackerPlayer.crystalSkullLightningCooldown > 0
                || Main.rand.NextFloat() >= CrystalSkull.LightningChance)
                return;
            attackerPlayer.crystalSkullLightningCooldown = CrystalSkull.LightningCooldown;
            int damage = Math.Max(1, (int)(sourceDamage * CrystalSkull.LightningDamageMultiplier));
            Projectile.NewProjectile(attacker.GetSource_OnHit(target), target.Center, Vector2.Zero
                , ModContent.ProjectileType<CrystalSkullLightning>(), damage, 0f, attacker.whoAmI, -1f);
        }
        /// <summary>
        /// PvP 版本的神圣之怒与碎甲药水命中入口。
        /// 从攻击者玩家状态读取药水标记，再对目标玩家施加当前灾厄版本提供的对应 buff。
        /// </summary>
        public static void TryApplyPotionHitDebuffsOnPvP(Player attacker, Player target)
        {
            if (attacker is null || target is null || !attacker.active || !target.active)
                return;
            CalamityRebirthPlayer modPlayer = attacker.GetModPlayer<CalamityRebirthPlayer>();
            if (modPlayer.holyWrath)
            {
                CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "HolyFlames", 120);
                CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "HolyLight", 120);
                CalamityCompatUtil.TryAddBuff(target, "CalRD", "HolyFlames", 120);
            }
            if (modPlayer.armorShattering || modPlayer.armorCrumbling)
            {
                CalamityCompatUtil.TryAddBuff(target, "CalamityMod", "ArmorCrunch", 240);
                CalamityCompatUtil.TryAddBuff(target, "CalamityModClassicPreTrailer", "ArmorCrunch", 240);
                CalamityCompatUtil.TryAddBuff(target, "CalRD", "ArmorCrunch", 240);
            }
        }
        /// <summary>
        /// 玩家侧持续伤害结算。任一负面减益存在时先清零自然再生和再生计时，
        /// 再按各减益的原版 lifeRegen 口径叠加损失。
        /// </summary>
        public override void UpdateBadLifeRegen()
        {
            if (!Player.HasBuff<BurningIII>() && !Player.HasBuff<DaybreakII>()
                && !Player.HasBuff<ElectrifiedII>() && !Player.HasBuff<ElementalMixII>()
                && !Player.HasBuff<ShadowflameII>())
                return;
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;
            if (Player.HasBuff<BurningIII>())
                Player.lifeRegen -= BurningIII.BurnLifeRegen;
            if (Player.HasBuff<DaybreakII>())
                Player.lifeRegen -= DaybreakII.BurnLifeRegen;
            if (Player.HasBuff<ElectrifiedII>())
                Player.lifeRegen -= ElectrifiedII.PlayerLifeRegenLoss;
            if (Player.HasBuff<ElementalMixII>())
                Player.lifeRegen -= ElementalMixII.PlayerLifeRegenLoss;
            if (Player.HasBuff<ShadowflameII>())
                Player.lifeRegen -= 30;
        }
        /// <summary>
        /// 结算龙舌兰日出的生命再生惩罚与通用酒醉等级。
        /// 酒醉超过三级时挂两帧酒精中毒提示，并清零正再生、追加等级相关负再生。
        /// </summary>
        public override void UpdateLifeRegen()
        {
            if (tequilaSunrise)
                Player.lifeRegen -= TequilaSunrise.LifeRegenLoss;
            if (bloodyMary)
                Player.lifeRegen -= 8;
            if (moscowMule)
                Player.lifeRegen -= 4;
            if (rum || caribbeanRum)
                Player.lifeRegen += 2;
            if (moonshine || vodka)
                Player.lifeRegen -= 1;
            // 光合作用：完全静止时按是否存在 Boss 使用既定的再生与时间上限，晚上效果为五分之一。
            if (photosynthesis && Math.Abs(Player.velocity.X) < 0.05 && Math.Abs(Player.velocity.Y) < 0.05
                && Player.itemAnimation == 0)
            {
                bool bossAlive = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].boss)
                    {
                        bossAlive = true;
                        break;
                    }
                }
                int regenTimeLimit = bossAlive ? 900 : 3600;
                int regenStep = bossAlive ? 2 : 8;
                float timeRegenCap = bossAlive ? 16f : 60f;
                if (!Main.dayTime)
                {
                    regenTimeLimit /= 5;
                    regenStep /= 5;
                    timeRegenCap /= 5f;
                }
                if (Player.lifeRegenTime > 90 && Player.lifeRegenTime < regenTimeLimit)
                    Player.lifeRegenTime = regenTimeLimit;
                Player.lifeRegenTime += regenStep;
                Player.lifeRegen += regenStep;
                float timeBonus = Player.lifeRegenTime * 2.5f / 300f;
                if (timeBonus > 0f)
                {
                    if (timeBonus > timeRegenCap)
                        timeBonus = timeRegenCap;
                    Player.lifeRegen += (int)timeBonus;
                }
            }
            if (Player.HasBuff(BuffID.Tipsy))
                alcoholPoisonLevel++;
            if (alcoholPoisonLevel <= 3)
                return;
            Player.nebulaLevelLife = 0;
            if (Player.whoAmI == Main.myPlayer)
                Player.AddBuff(ModContent.BuffType<AlcoholPoisoning>(), 2, false);
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
            Player.lifeRegenTime = 0;
            Player.lifeRegen -= 3 * alcoholPoisonLevel;
            ApplyJewelAccessoryLifeRegen();
        }
        /// <summary>
        /// 结算无暇粹魂晶宝石系的再生、减益时长、直接回血与动态防御。
        /// 三件按无暇粹魂晶、感染宝石、王冠宝石的优先级互斥，并使用 else-if 链。
        /// </summary>
        private void ApplyJewelAccessoryLifeRegen()
        {
            int debuffCount = CountJewelDebuffs();
            if (purity && Player.lifeRegen < 0 && debuffCount > 0)
            {
                Player.lifeRegen += 24;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
            }
            if (alwaysHoneyRegen && !Player.honey)
            {
                Player.lifeRegen += 2;
                Player.lifeRegenTime++;
                if (Player.lifeRegen < 0)
                {
                    Player.lifeRegen += 2;
                    if (Player.lifeRegen > 0)
                        Player.lifeRegen = 0;
                }
            }
            if (honeyDewHalveDebuffs || livingDewHalveDebuffs || purity)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (Player.buffTime[i] <= 2)
                        continue;
                    int buffType = Player.buffType[i];
                    bool halve = purity
                        || (honeyDewHalveDebuffs && BuffUtil.IsSicknessDebuff(buffType))
                        || (livingDewHalveDebuffs && BuffUtil.IsFireDebuff(buffType));
                    if (halve)
                        Player.buffTime[i]--;
                }
            }
            bool stationary = Math.Abs(Player.velocity.X) < 0.05f
                && Math.Abs(Player.velocity.Y) < 0.05f && Player.itemAnimation == 0;
            if (honeyTurboRegen && Player.honeyWet && stationary)
                ApplyHoneyStationaryRegen(3, 90);
            if (aAmpoule && stationary)
                ApplyHoneyStationaryRegen(3, 90);
            if (purity && stationary)
                ApplyHoneyStationaryRegen(4, 60);
            if ((rOoze || aAmpoule || purity) && Player.statLifeMax2 > 0)
            {
                float missingLifeRatio = (Player.statLifeMax2 - Player.statLife)
                    / (float)Player.statLifeMax2;
                Player.lifeRegen += (int)MathHelper.Lerp(
                    purity ? 6f : 4f, purity ? 14f : 12f, missingLifeRatio);
            }
            if (purity)
            {
                int intendedDefense = 0;
                if (debuffCount > 0)
                {
                    int punishmentFrames = purityHealSlowdownFrames - 300;
                    int speedPenalty = punishmentFrames < 0 ? 0 : punishmentFrames / 15;
                    int healFrameCadence = Math.Min(180, 12 + speedPenalty);
                    if (Main.netMode != NetmodeID.MultiplayerClient
                        && Player.miscCounter % healFrameCadence == healFrameCadence - 1)
                    {
                        Player.Heal(1);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null
                                , Player.whoAmI, Player.statLife, Player.statMana);
                        }
                    }
                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;
                    intendedDefense = 20 + (debuffCount - 1) * 8;
                    if (jewelBonusDefense < intendedDefense)
                        jewelBonusDefense = intendedDefense;
                    purityHealSlowdownFrames++;
                }
                if (Player.miscCounter % 60 == 0 && jewelBonusDefense > intendedDefense)
                    jewelBonusDefense--;
                if (debuffCount <= 0 && purityHealSlowdownFrames > 0)
                    purityHealSlowdownFrames--;
                Player.statDefense += jewelBonusDefense;
            }
            else if (infectedJewel)
            {
                Player.lifeRegen += 2;
                int intendedDefense = 0;
                if (debuffCount > 0)
                {
                    Player.lifeRegen += 4;
                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;
                    intendedDefense = 16 + (debuffCount - 1) * 5;
                    if (jewelBonusDefense < intendedDefense)
                        jewelBonusDefense = intendedDefense;
                }
                if (Player.miscCounter % 60 == 0 && jewelBonusDefense > intendedDefense)
                    jewelBonusDefense--;
                Player.statDefense += jewelBonusDefense;
            }
            else if (crownJewel)
            {
                Player.lifeRegen += 2;
                if (debuffCount > 0)
                {
                    Player.lifeRegen += 3;
                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;
                }
            }
        }
        /// <summary>
        /// 统计当前玩家身上的目标有害减益数量。
        /// 只统计配表中明确列出的战斗减益，酒类、药水病等持续型状态不会误触发宝石强化。
        /// </summary>
        /// <summary>
        /// 应用古代蜂蜜链的站桩再生档位：先削弱负再生，再提高再生时间与点数。
        /// threshold 越大代表需要站立越久才进入该档位。
        /// </summary>
        private void ApplyHoneyStationaryRegen(int regen, int threshold)
        {
            if (Player.lifeRegen < 0)
                Player.lifeRegen /= 2;
            if (Player.lifeRegenTime > threshold && Player.lifeRegenTime < 1800)
                Player.lifeRegenTime = 1800;
            Player.lifeRegen += regen;
            Player.lifeRegenTime += regen;
        }
        private int CountJewelDebuffs()
        {
            int count = 0;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Player.buffTime[i] > 0 && BuffUtil.Contains(Player.buffType[i]))
                    count++;
            }
            return count;
        }
        /// <summary>
        /// 装备属性全部计算后的统一收尾：
        /// 处理负面减益减防、法系饰品的每秒回蓝，以及三版本盗贼兼容属性。
        /// </summary>
        public override void PostUpdateEquips()
        {
            if (Player.HasBuff<BurningIII>())
                Player.statDefense -= (int)BurningIII.DefenseLoss;
            if (Player.HasBuff<DaybreakII>())
                Player.statDefense -= (int)DaybreakII.DefenseLoss;
            if (Player.HasBuff<ElectrifiedII>())
                Player.statDefense -= ElectrifiedII.PlayerDefenseLoss;
            if (Player.HasBuff<ElementalMixII>())
                Player.statDefense -= (int)ElementalMixII.DefenseLoss;
            int passiveManaPerSecond = 0;
            // 两个法系饰品可同时装备，因此回蓝速度先相加，再由同一个 60 帧计时器发放。
            if (sorcererEmblemEX)
                passiveManaPerSecond += SorcererEmblemEX.ManaPerSecond;
            if (celestialEmblemEX)
                passiveManaPerSecond += CelestialEmblemEX.ManaPerSecond;
            if (passiveManaPerSecond > 0 && Player.whoAmI == Main.myPlayer)
            {
                // 仅在本地玩家身上推进计时，避免多人模式下每个客户端都修改同一份法力。
                passiveManaRegenTimer++;
                if (passiveManaRegenTimer >= 60)
                {
                    // 用减法而不是归零，确保高帧率累积不会丢失整除后的节拍。
                    passiveManaRegenTimer -= 60;
                    if (Player.statMana < Player.statManaMax2)
                        Player.statMana += passiveManaPerSecond;
                    if (Player.statMana > Player.statManaMax2)
                        Player.statMana = Player.statManaMax2;
                }
            }
            else
            {
                passiveManaRegenTimer = 0;
            }
            if (rogueEmblemEX)
            {
                // 盗贼职业由三版灾厄分别提供不同 API，统一交给 CRUtil 在运行时桥接。
                CRUtil.ApplyRogueStats(
                    Player,
                    RogueEmblemEX.RogueDamageBonus,
                    RogueEmblemEX.RogueCritBonus,
                    RogueEmblemEX.RogueSpeedBonus,
                    RogueEmblemEX.StealthBonus,
                    0.10f,
                    0.05f);
            }
            ApplyRogueAccessoryStealthEffects();
            if (crystalSkull)
            {
                CRUtil.ApplyCrystalSkullStats(
                    Player,
                    CrystalSkull.RogueDamageBonus,
                    CrystalSkull.RogueCritBonus,
                    CrystalSkull.RogueSpeedBonus,
                    CrystalSkull.MaxStealthBonus,
                    CrystalSkull.StealthRegenBonus);
            }
            ApplyGenericStealthStrikeCost();
        }
        /// <summary>
        /// 在灾厄读取潜行字段前汇总镜片链的潜行上限、恢复和消耗档位。
        /// 半价档优先于欺诈硬币的 75% 档，避免低阶件反向覆盖高阶效果。
        /// </summary>
        private void ApplyRogueAccessoryStealthEffects()
        {
            float stealthBonus = 0f;
            float standstillRegen = 0f;
            float movingRegen = 0f;
            if (ruinMedallion)
                stealthBonus += 0.10f;
            if (silencingSheath)
            {
                stealthBonus += 0.20f;
                standstillRegen += 0.15f;
                movingRegen += 0.15f;
            }
            if (mirageMirror)
            {
                stealthBonus += 0.15f;
                standstillRegen += 0.30f;
                movingRegen += 0.20f;
            }
            if (abyssalMirror)
            {
                stealthBonus += 0.20f;
                standstillRegen += 0.30f;
                movingRegen += 0.20f;
            }
            if (darkMatterSheath)
                stealthBonus += 0.20f;
            if (eclipseMirror)
            {
                stealthBonus += 0.25f;
                standstillRegen += 0.20f;
            }
            if (stealthBonus > 0f || standstillRegen > 0f || movingRegen > 0f)
                CRUtil.ApplyRogueStats(Player, 0f, 0, 0f, stealthBonus, standstillRegen, movingRegen);
            if (darkMatterSheath || eclipseMirror)
            {
                CRUtil.SetStealthStrikeAlwaysCrits(Player);
                CRUtil.SetStealthAcceleration(Player, eclipseMirror);
            }
        }
        /// <summary>
        /// 每帧只选择一个最低的潜行攻击消耗档位。
        /// 任意 50% 档存在时统一使用 50%，否则才使用 75%，不会与旧字段叠加。
        /// </summary>
        private void ApplyGenericStealthStrikeCost()
        {
            float costFraction = 1f;
            if (rogueEmblemEX || coinofDeceit)
                costFraction = CRUtil.StealthStrikeCost75;
            if (crystalSkull || ruinMedallion || darkMatterSheath || eclipseMirror)
                costFraction = CRUtil.StealthStrikeCost50;
            if (costFraction < 1f)
                CRUtil.SetStealthStrikeCost(Player, costFraction);
        }
        /// <summary>
        /// 维护晶光头骨闪电冷却与每次潜行打击的命中计数。
        /// </summary>
        public override void PostUpdate()
        {
            if (crystalSkullLightningCooldown > 0)
                crystalSkullLightningCooldown--;
            if (mirrorDodgeCooldown > 0)
                mirrorDodgeCooldown--;
            if (nanotechFlareCooldown > 0)
                nanotechFlareCooldown--;
            if (crystalSkull && !CRUtil.IsStealthStrike(Player))
            {
                crystalSkullStealthHitCount = 0;
                crystalSkullHealedThisStrike = false;
            }
            bool nanotechStealthStrike = nanotech && CRUtil.IsStealthStrike(Player);
            if (!nanotechStealthStrike)
                nanotechHealedThisStrike = false;
        }
        // 与原版 CalamityPlayerDrawEffects 相同：仅在正常绘制通道触发，避免残影把同一份粒子重复生成。
        /// <summary>
        /// 为破晓II和自定义暗影焰生成玩家粒子。
        /// shadow 为 0 可排除残影绘制，防止一次画面更新重复消耗粒子和视觉预算。
        /// </summary>
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0f && Player.HasBuff<DaybreakII>())
                DaybreakII.DrawEffects(drawInfo);
            if (drawInfo.shadow == 0f && Player.HasBuff<ShadowflameII>())
                ShadowflameII.DrawEffects(drawInfo);
            if (drawInfo.shadow == 0f && Player.HasBuff<ElectrifiedII>())
                ElectrifiedII.SpawnElectricVisuals(Player);
            if (drawInfo.shadow == 0f && Player.HasBuff<ElementalMixII>())
                ElementalMixII.DrawEffects(drawInfo);
        }
        /// <summary>
        /// 玩家受到 NPC 接触伤害前结算血肉图腾与胜利药水。
        /// 两者都只通过本次 HurtModifiers 乘算，不修改 NPC 本体伤害。
        /// </summary>
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (fleshTotem && fleshTotemCooldown <= 0)
            {
                fleshTotemCooldown = FleshTotem.ContactDamageCooldown;
                modifiers.FinalDamage *= 0.5f;
            }
            if (triumph)
            {
                // 目标当前生命越低，减伤越高，最高 25%。
                double reduction = 0.25 * (1D - (double)npc.life / npc.lifeMax);
                modifiers.FinalDamage *= 1f - (float)reduction;
            }
        }
        /// <summary>
        /// 玩家受到伤害前应用燃烧III、破晓II与元素混合II的易伤倍率，并结算血契暴击惩罚。
        /// 各减益同时存在时保留独立乘算；血契自身再进行一次 25% 概率判定。
        /// </summary>
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HasBuff<BurningIII>())
                modifiers.FinalDamage *= BurningIII.DamageTakenMultiplier;
            if (Player.HasBuff<DaybreakII>())
                modifiers.FinalDamage *= DaybreakII.DamageTakenMultiplier;
            if (Player.HasBuff<ElementalMixII>())
                modifiers.FinalDamage *= ElementalMixII.DamageTakenMultiplier;
            // NextFloat 的上界不包含 1，因此这里与该概率区间一一对应。
            if (bloodPact && Main.rand.NextFloat() < BloodPact.CriticalHitChance)
                modifiers.FinalDamage *= BloodPact.CriticalHitDamageMultiplier;
            ApplySpongeAbsorbHeal(ref modifiers);
        }
        /// <summary>新生药水：实际受伤后按本次伤害的十五分之一回复生命。</summary>
        public override void OnHurt(Player.HurtInfo info)
        {
            if (revivify)
                RequestHeal(Player, HealRequestRevivify, info.Damage);
            ApplySpongeChainOnHurt();
            if (amalgamatedBrain || theAmalgam)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly)
                        continue;
                    float range = Main.rand.Next(200 + info.Damage / 2, 301 + info.Damage * 2);
                    if (range > 500f)
                        range = 500f + (range - 500f) * 0.75f;
                    if (range > 700f)
                        range = 700f + (range - 700f) * 0.5f;
                    if (range > 900f)
                        range = 900f + (range - 900f) * 0.25f;
                    if ((npc.Center - Player.Center).Length() >= range)
                        continue;
                    int duration = Main.rand.Next(90 + info.Damage / 3, 300 + info.Damage / 2);
                    npc.AddBuff(BuffID.Confused, duration, false);
                }
                if (Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis()
                        , Player.Center.X + Main.rand.Next(-40, 40)
                        , Player.Center.Y - Main.rand.Next(20, 60)
                        , Player.velocity.X * 0.3f, Player.velocity.Y * 0.3f
                        , ProjectileID.BrainOfConfusion, 0, 0f, Player.whoAmI);
                }
            }
        }
        /// <summary>
        /// 结算神圣护符与神之壁垒的受击反击。
        /// 弹幕只在受伤玩家的主人客户端生成，避免服务端和其他客户端为其重复创建星辰。
        /// 神之壁垒同时装备时只触发顶点档，神圣护符不再额外追加三枚星辰。
        /// </summary>
        public override void PostHurt(Player.HurtInfo info)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            ApplySpongeChainPostHurt(info);
            ApplyWingChainPostHurt(info);
            if (rampartOfDeities)
            {
                Player.immuneTime += info.Damage == 1 ? 15 : 30;
                SpawnHallowStars(6);
                return;
            }
            if (deificAmulet)
            {
                Player.immuneTime += info.Damage == 1 ? 10 : 20;
                SpawnHallowStars(3);
            }
        }
        /// <summary>
        /// 从玩家上方随机生成指定数量的神圣之星，并以固定二十九像素每帧的速度朝玩家附近落下。
        /// 伤害使用通用伤害结算一百三十点基础值，星级使用五帧本地 NPC 命中冷却。
        /// </summary>
        private void SpawnHallowStars(int count)
        {
            int damage = (int)Player.GetDamage(DamageClass.Generic).ApplyTo(130);
            for (int i = 0; i < count; i++)
            {
                Vector2 spawnPosition = new(
                    Player.position.X + Main.rand.Next(-400, 400),
                    Player.position.Y - Main.rand.Next(500, 800));
                Vector2 direction = Player.Center - spawnPosition;
                direction.X += Main.rand.Next(-100, 101);
                direction.Normalize();
                direction *= 29f;
                int star = Projectile.NewProjectile(Player.GetSource_FromThis(), spawnPosition, direction
                    , ProjectileID.HallowStar, damage, 4f, Player.whoAmI);
                if (!Main.projectile.IndexInRange(star))
                    continue;
                Main.projectile[star].usesLocalNPCImmunity = true;
                Main.projectile[star].localNPCHitCooldown = 5;
            }
        }
        /// <summary>
        /// 深渊魔镜与蚀日魔镜共享 90 秒闪避冷却；同时装备时蚀日魔镜优先。
        /// 闪避恢复潜行，并在主人客户端生成对应的镜片特效弹幕。
        /// </summary>
        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (amalgamatedBrain && Main.rand.NextBool(AmalgamatedBrain.DodgeChanceDenominator))
                return true;
            if (theAmalgam && Main.rand.NextBool(TheAmalgam.DodgeChanceDenominator))
                return true;
            if (mirrorDodgeCooldown > 0 || (!eclipseMirror && !abyssalMirror))
                return false;
            mirrorDodgeCooldown = 90 * 60;
            Player.immune = true;
            Player.immuneTime = Math.Max(Player.immuneTime, Player.longInvince ? 100 : 60);
            Player.noKnockback = true;
            if (eclipseMirror)
            {
                CRUtil.RestoreRogueStealth(Player, 1f);
                SoundEngine.PlaySound(SoundID.Item68, Player.Center);
                if (Player.whoAmI == Main.myPlayer)
                {
                    int damage = CRUtil.GetRogueDamage(Player, 2750);
                    int burst = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center
                        , Vector2.Zero, ModContent.ProjectileType<EclipseMirrorBurst>(), damage, 0f, Player.whoAmI);
                    if (Main.projectile.IndexInRange(burst))
                        Main.projectile[burst].DamageType = DamageClass.Generic;
                }
            }
            else
            {
                CRUtil.RestoreRogueStealth(Player, 0.5f);
                SoundEngine.PlaySound(SoundID.Item29, Player.Center);
                if (Player.whoAmI == Main.myPlayer)
                {
                    int damage = CRUtil.GetRogueDamage(Player, 55);
                    for (int i = 0; i < 10; i++)
                    {
                        int fluid = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center
                            , new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f))
                            , ModContent.ProjectileType<AbyssalMirrorProjectile>(), damage, 0f, Player.whoAmI);
                        if (!Main.projectile.IndexInRange(fluid))
                            continue;
                        Main.projectile[fluid].rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        Main.projectile[fluid].frame = Main.rand.Next(3);
                    }
                }
            }
            if (Player.whoAmI == Main.myPlayer)
                NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f);
            return true;
        }
        /// <summary>
        /// 请求一次由服务器验证的治疗。
        /// 多人客户端只发送来源与伤害参数；单机或服务器本地直接执行同一验证函数。
        /// </summary>
        public static void RequestHeal(Player player, byte reason, int damageSource = 0)
        {
            if (player is null || !player.active)
                return;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = ModContent.GetInstance<global::CalamityRebirth.CalamityRebirth>().GetPacket();
                packet.Write(MsgRequestHeal);
                packet.Write(reason);
                packet.Write(damageSource);
                packet.Send();
                return;
            }
            ApplyServerHealRequest(player, reason, damageSource);
        }
        /// <summary>
        /// 服务器根据玩家当前 buff 与装备状态验证并执行治疗，避免客户端直接改写生命。
        /// </summary>
        public static void ApplyServerHealRequest(Player player, byte reason, int damageSource)
        {
            if (player is null || !player.active)
                return;
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            int heal = reason switch
            {
                HealRequestCrystalSkull when modPlayer.crystalSkull => CrystalSkull.StealthHeal,
                HealRequestRevivify when player.HasBuff<Revivify>() => (int)(Math.Clamp(damageSource, 0, 1500) / 15D),
                HealRequestPhotosynthesis when player.HasBuff<Photosynthesis>() => 5,
                HealRequestNanotech when modPlayer.nanotech => 1,
                _ => 0
            };
            if (heal <= 0)
                return;
            player.Heal(heal);
            player.HealEffect(heal, true);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null, player.whoAmI
                    , player.statLife, player.statMana);
            }
        }
        /// <summary>
        /// 对远程武器执行概率省弹。返回 false 表示本次弹药不被消耗。
        /// 游侠徽章和狙击镜独立判定，因此同时装备时理论省弹率高于单件。
        /// </summary>
        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            if (rangerEmblemEX && Main.rand.NextFloat() < RangerEmblemEX.AmmoSaveChance)
                return false;
            if (daedalusEmblem && weapon.CountsAsClass(DamageClass.Ranged)
                && Main.rand.NextFloat() < DaedalusEmblem.AmmoSaveChance)
                return false;
            if (elementalQuiver && weapon.CountsAsClass(DamageClass.Ranged)
                && Main.rand.NextFloat() < ElementalQuiver.AmmoSaveChance)
                return false;
            if (sniperScopeEX && Main.rand.NextFloat() < SniperScopeEX.AmmoSaveChance)
                return false;
            return true;
        }
        /// <summary>
        /// 只处理真正消耗弹药的远程射击，避免把法杖、召唤武器或无限弹药武器误判为远程弹幕。
        /// 两件饰品的弹速倍率连续乘算。
        /// </summary>
        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (item.useAmmo == AmmoID.None || !item.CountsAsClass(DamageClass.Ranged))
                return;
            if (rangerEmblemEX)
                velocity *= 1f + RangerEmblemEX.AmmoVelocityBonus;
            if (sniperScopeEX)
                velocity *= 1f + SniperScopeEX.ProjectileVelocityBonus;
        }
        /// <summary>
        /// 龙舌兰日出在昼夜均提供 +3% 武器击退，白天再追加 +7%。
        /// 使用 StatModifier 加法口径，确保多个来源的加成按加法结算。
        /// </summary>
        public override void ModifyWeaponKnockback(Item item, ref StatModifier knockback)
        {
            if (moscowMule)
                knockback += MoscowMule.KnockbackBoost;
            if (!tequilaSunrise)
                return;
            knockback += TequilaSunrise.GeneralKnockbackBoost;
            if (Main.dayTime)
                knockback += TequilaSunrise.DayKnockbackBoost;
        }
        /// <summary>
        /// 在杂项效果阶段统一结算药水、血神核心系列和血肉图腾冷却。
        /// 这些数值放在同一入口，避免增益 Update 与装备结算互相覆盖。
        /// </summary>
        public override void PostUpdateMiscEffects()
        {
            if (fleshTotemCooldown > 0)
                fleshTotemCooldown--;
            if (Player.dead)
                fleshTotemCooldown = 0;
            if (vexation && Player.statLifeMax2 > 0)
            {
                // 现代烦恼项链按已损失生命比例提供连续通用增伤：满血 0%，空血 30%。
                // 每次都基于当前最终最大生命计算，避免固定半血档位覆盖治疗或最大生命变化后的结果。
                float missingLifeRatio = 1f - Player.statLife / (float)Player.statLifeMax2;
                Player.GetDamage(DamageClass.Generic) += NecklaceofVexation.MaximumDamageBonus
                    * MathHelper.Clamp(missingLifeRatio, 0f, 1f);
            }
            if (yharimsInsignia)
            {
                Player.GetDamage(DamageClass.Melee) += YharimsInsignia.MeleeDamageBonus;
                Player.GetCritChance(DamageClass.Melee) += YharimsInsignia.MeleeCritBonus;
                Player.GetAttackSpeed(DamageClass.Melee) += YharimsInsignia.MeleeSpeedBonus;
                Player.kbGlove = true;
                Player.longInvince = true;
                Player.lavaMax += YharimsInsignia.LavaImmunityFrames;
                // 低生命增伤每次按当前最终最大生命重新判断，避免治疗或最大生命变化后沿用旧状态。
                if (Player.statLife > 0
                    && Player.statLife <= Player.statLifeMax2 * YharimsInsignia.LowLifeThreshold)
                    Player.GetDamage(DamageClass.Generic) += YharimsInsignia.LowLifeDamageBonus;
            }
            if (elementalGauntlet)
            {
                Player.GetDamage(DamageClass.Melee) += ElementalGauntlet.MeleeBonus;
                Player.GetCritChance(DamageClass.Melee) += ElementalGauntlet.MeleeCritBonus;
                Player.GetAttackSpeed(DamageClass.Melee) += ElementalGauntlet.MeleeBonus;
                Player.autoReuseGlove = true;
                Player.meleeScaleGlove = true;
                Player.kbGlove = true;
                Player.longInvince = true;
                Player.magmaStone = true;
                Player.lavaMax += ElementalGauntlet.LavaImmunityFrames;
            }
            ApplyRogueAccessoryStats();
            ApplyGenericStealthStrikeCost();
            ApplyStatisSummonEffects();
			ApplyNebulousCoreEffects();
			ApplyElementalChainEffects();
			ApplyWingChainEffects();
            ApplyBloodGodAccessoryEffects();
            ApplyDarkSunRingEffects();
            ApplyBrainAccessoryEffects();
            ApplyAmalgamChainAccessoryEffects();
            ApplyRampartChainEffects();
            ApplyShieldSlamAccessoryEffects();
            if (yharimPower)
            {
                Player.GetDamage<GenericDamageClass>() += 0.22f;
                Player.GetCritChance<GenericDamageClass>() += 10;
                Player.GetAttackSpeed<MeleeDamageClass>() += 0.12f;
                Player.statDefense += 10;
                Player.lifeRegen += 2; // Terraria 中每 2 点 lifeRegen 约等于 1 HP/s。
                Player.endurance += 0.06f;
                Player.GetKnockback<SummonDamageClass>() *= 1.5f;
                Player.moveSpeed += 0.15f;
            }
            if (tequilaSunrise)
            {
                Player.GetDamage<GenericDamageClass>() += TequilaSunrise.GeneralDamageBoost;
                Player.GetCritChance<GenericDamageClass>() += TequilaSunrise.GeneralCritBoost;
                Player.endurance += TequilaSunrise.GeneralDamageReduction;
                if (Main.dayTime)
                {
                    Player.GetDamage<GenericDamageClass>() += TequilaSunrise.DayDamageBoost;
                    Player.GetCritChance<GenericDamageClass>() += TequilaSunrise.DayCritBoost;
                    Player.statDefense += TequilaSunrise.DayDefenseBoost;
                    Player.endurance += TequilaSunrise.DayDamageReduction;
                }
            }
            ApplyPotionEffects();
            ApplyBloodGodThresholdEffects();
            ApplyAfflictionEffects();
            ApplySpongeChainEffects();
        }
        /// <summary>
        /// 结算蚀日尊戒的旧版复合面板。通用伤害、暴击、近战攻速、召唤击退、仆从栏与挖掘速度常驻；
        /// 白昼额外提供约 1.5 HP/s 再生，夜晚额外提供 30 防御。日食仍被 Terraria 视作白昼，
        /// 因此两项昼夜效果会在日食期间同时生效。
        /// </summary>
        private void ApplyDarkSunRingEffects()
        {
            if (!darkSunRing)
                return;
            Player.maxMinions += DarkSunRing.MaxMinionBonus;
            Player.GetDamage(DamageClass.Generic) += DarkSunRing.DamageBonus;
            Player.GetCritChance(DamageClass.Generic) += DarkSunRing.CritBonus;
            Player.GetAttackSpeed(DamageClass.Melee) += DarkSunRing.MeleeSpeedBonus;
            Player.GetKnockback(DamageClass.Summon).Base += DarkSunRing.MinionKnockbackBonus;
            Player.pickSpeed -= DarkSunRing.MiningSpeedBonus;
            if (Main.dayTime)
                Player.lifeRegen += DarkSunRing.DaylightLifeRegenBonus;
            if (!Main.dayTime || Main.eclipse)
                Player.statDefense += DarkSunRing.NightDefenseBonus;
        }
        /// <summary>
        /// 结算腐烂之脑与聚合之脑的常驻面板及无敌期间暗影雨。
        /// 每帧只有拥有者客户端生成雨滴，避免多人环境下所有客户端重复创建弹幕。
        /// </summary>
        private void ApplyBrainAccessoryEffects()
        {
            if (rottenBrain)
            {
                if (Player.statLife <= Player.statLifeMax2 * RottenBrain.DamageLifeThreshold)
                    Player.GetDamage(DamageClass.Generic) += RottenBrain.LowLifeDamageBonus;
                if (Player.statLife <= Player.statLifeMax2 * RottenBrain.MoveSpeedLifeThreshold)
                    Player.moveSpeed -= RottenBrain.LowLifeMoveSpeedPenalty;
                if (Player.immune && Player.whoAmI == Main.myPlayer
                    && Main.rand.NextBool(RottenBrain.AuraRainChanceDenominator))
                    SpawnAuraRain(RottenBrain.AuraRainBaseDamage);
            }
            if (amalgamatedBrain)
            {
                Player.GetDamage(DamageClass.Generic) += AmalgamatedBrain.DamageBonus;
                Player.GetCritChance(DamageClass.Generic) += AmalgamatedBrain.CritBonus;
                if (Player.immune && Player.whoAmI == Main.myPlayer
                    && Main.rand.NextBool(AmalgamatedBrain.AuraRainChanceDenominator))
                    SpawnAuraRain(AmalgamatedBrain.AuraRainBaseDamage);
            }
        }
        /// <summary>
        /// 从玩家上方随机位置生成一滴暗影雨，并以固定速度朝玩家中心附近落下。
        /// ai[1] 记录生成时玩家的纵向位置，供原版雨云 AI 判断何时允许落地碰撞。
        /// </summary>
        private void SpawnAuraRain(int baseDamage)
        {
            float spawnX = Player.position.X + Main.rand.Next(-400, 400);
            float spawnY = Player.position.Y - Main.rand.Next(500, 800);
            Vector2 spawnPosition = new(spawnX, spawnY);
            Vector2 direction = Player.Center - spawnPosition;
            direction.X += Main.rand.Next(-100, 101);
            direction.Normalize();
            direction *= 22f;
            int damage = (int)Player.GetDamage(DamageClass.Generic).ApplyTo(baseDamage);
            int projectileIndex = Projectile.NewProjectile(Player.GetSource_FromThis(), spawnPosition
                , direction, ModContent.ProjectileType<AuraRain>(), damage, 2f, Player.whoAmI);
            if (!Main.projectile.IndexInRange(projectileIndex))
                return;
            Main.projectile[projectileIndex].ai[1] = Player.position.Y;
            Main.projectile[projectileIndex].tileCollide = false;
        }
        /// <summary>
        /// 结算灾厄之环、焦热地狱、终结虚空、利维坦龙涎香与融合之脑。
        /// 顶点与组件互斥，因此各分支可分别保留各自的原始数值，不会同时叠加。
        /// </summary>
        private void ApplyAmalgamChainAccessoryEffects()
        {
            if (calamityRing)
            {
                Player.GetDamage(DamageClass.Generic) += CalamityRing.DamageBonus;
                if (Player.immune && Player.whoAmI == Main.myPlayer
                    && Main.rand.NextBool(CalamityRing.StandingFireChanceDenominator))
                    SpawnStandingFire(CalamityRing.StandingFireBaseDamage, false);
            }
            if (gehenna)
            {
                UpdateBrimstoneBarrage(ref gehennaFireCountdown, Gehenna.BarrageCooldownFrames
                    , Gehenna.FireballBaseDamage);
            }
            if (voidofExtinction)
            {
                Player.GetDamage(DamageClass.Generic) += VoidofExtinction.DamageBonus;
                Player.lavaRose = true;
                Player.lavaMax += VoidofExtinction.LavaImmunityFrames;
                if (Player.lavaWet)
                    Player.GetDamage(DamageClass.Generic) += VoidofExtinction.LavaDamageBonus;
                if (Player.immune && Player.whoAmI == Main.myPlayer
                    && Main.rand.NextBool(VoidofExtinction.StandingFireChanceDenominator))
                    SpawnStandingFire(VoidofExtinction.StandingFireBaseDamage);
                UpdateBrimstoneBarrage(ref voidFireCountdown, VoidofExtinction.BarrageCooldownFrames
                    , VoidofExtinction.FireballBaseDamage);
            }
            if (leviathanAmbergris)
            {
                ApplyAmalgamWaterEffects(LeviathanAmbergris.DryDamageReduction
                    , LeviathanAmbergris.DryDamageBonus, LeviathanAmbergris.WetDamageBonus
                    , LeviathanAmbergris.WetDefenseBonus, LeviathanAmbergris.WetMoveSpeedBonus
                    , LeviathanAmbergris.SeawaterBaseDamage, LeviathanAmbergris.AuraDamage);
            }
            if (theAmalgam)
            {
                Player.GetDamage(DamageClass.Generic) += TheAmalgam.DamageBonus;
                Player.GetCritChance(DamageClass.Generic) += TheAmalgam.CritBonus;
                Player.lavaRose = true;
                Player.lavaMax += 240;
                if (Player.lavaWet)
                    Player.GetDamage(DamageClass.Generic) += 0.25f;
                if (Player.immune && Player.whoAmI == Main.myPlayer && Main.rand.NextBool(10))
                {
                    if (Main.rand.NextBool(2))
                        SpawnStandingFire(TheAmalgam.InvincibilityProjectileDamage);
                    else
                        SpawnAuraRain(TheAmalgam.InvincibilityProjectileDamage);
                }
                UpdateBrimstoneBarrage(ref amalgamFireCountdown, 600, TheAmalgam.BarrageBaseDamage);
                int seawaterDamage = (int)Player.GetDamage(DamageClass.Generic)
                    .ApplyTo(TheAmalgam.SeawaterBaseDamage);
                ApplyAmalgamWaterEffects(0.10f, 0.10f, 0.10f, 40, 0.75f
                    , seawaterDamage, 80);
            }
        }
        /// <summary>
        /// 维护一轮硫磺火齐射计时。倒计时从 600 开始，到零后从玩家上空向瞄准方向发射四枚火球。
        /// </summary>
        private void UpdateBrimstoneBarrage(ref int countdown, int cooldown, int baseDamage)
        {
            if (countdown == 0)
                countdown = cooldown;
            countdown--;
            if (countdown > 0 || Player.whoAmI != Main.myPlayer)
                return;
            Vector2 baseSpawn = new(Player.Center.X + Main.rand.Next(-500, 500), Player.Center.Y - 1000f);
            Vector2 baseVelocity = (Player.Center - baseSpawn).SafeNormalize(-Vector2.UnitY) * 25f;
            int damage = (int)Player.GetDamage(DamageClass.Generic).ApplyTo(baseDamage);
            for (int i = 0; i < 4; i++)
            {
                Vector2 spawn = baseSpawn;
                spawn.X += i * 30 - 60;
                Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(
                    -60f + 120f * i / 4f));
                velocity.X += 3f * Main.rand.NextFloat() - 1.5f;
                int projectileIndex = Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, velocity
                    , ModContent.ProjectileType<BrimstoneHellfireballFriendly2>(), damage, 5f, Player.whoAmI);
                if (!Main.projectile.IndexInRange(projectileIndex))
                    continue;
                Main.projectile[projectileIndex].tileCollide = false;
                Main.projectile[projectileIndex].timeLeft = 50;
            }
        }
        /// <summary>从玩家上方随机位置生成一枚站火，并让原版雨云 AI 识别玩家纵向位置。</summary>
        private void SpawnStandingFire(int baseDamage, bool applyDamageBonus = true)
        {
            Vector2 spawnPosition = new(Player.Center.X + Main.rand.Next(-400, 400)
                , Player.Center.Y - Main.rand.Next(500, 800));
            Vector2 direction = (Player.Center - spawnPosition).SafeNormalize(-Vector2.UnitY) * 22f;
            int damage = applyDamageBonus
                ? (int)Player.GetDamage(DamageClass.Generic).ApplyTo(baseDamage)
                : baseDamage;
            int projectileIndex = Projectile.NewProjectile(Player.GetSource_FromThis(), spawnPosition
                , direction, ModContent.ProjectileType<StandingFire>(), damage, 5f, Player.whoAmI);
            if (!Main.projectile.IndexInRange(projectileIndex))
                return;
            Main.projectile[projectileIndex].ai[1] = Player.position.Y;
        }
        /// <summary>
        /// 结算利维坦式水下战斗：干燥时提供少量减伤与增伤，浸水时提供更高伤害、防御与移速；
        /// 移动时留下毒海水，浸水无敌期间按 1/5 检查节拍毒害并灼烧近身敌人。
        /// </summary>
        private void ApplyAmalgamWaterEffects(float dryEndurance, float dryDamage, float wetDamage
            , int wetDefense, float wetMoveSpeed, int seawaterDamage, int auraDamage)
        {
            Player.ignoreWater = true;
            if (!Player.lavaWet && !Player.honeyWet)
            {
                if (!Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir))
                {
                    Player.endurance += dryEndurance;
                    Player.GetDamage(DamageClass.Generic) += dryDamage;
                }
                else
                {
                    Player.GetDamage(DamageClass.Generic) += wetDamage;
                    Player.statDefense += wetDefense;
                    Player.moveSpeed += wetMoveSpeed;
                }
            }
            if (Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y) > 0.1f
                && Player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero
                    , ModContent.ProjectileType<PoisonousSeawater>(), seawaterDamage, 5f, Player.whoAmI);
            }
            Lighting.AddLight(Player.Center, 0f, 0.5f, 1.25f);
            if (Player.whoAmI != Main.myPlayer || Player.immune
                && Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir)
                && Main.rand.NextBool(5))
            {
                ApplyUnderwaterAura(auraDamage);
            }
        }
        /// <summary>对 200 像素内可中毒敌人施加毒液，并造成由调用方指定的光环伤害。</summary>
        private void ApplyUnderwaterAura(int damage)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage
                    || npc.buffImmune[BuffID.Venom]
                    || Vector2.DistanceSquared(Player.Center, npc.Center) > 200f * 200f)
                    continue;
                if (npc.FindBuffIndex(BuffID.Venom) == -1)
                    npc.AddBuff(BuffID.Venom, 300, false);
                Player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
            }
        }
        /// <summary>汇总镜片链与纳米技术的盗贼伤害、暴击、弹速、仇恨和纳米生存面板。</summary>
        private void ApplyRogueAccessoryStats()
        {
            float damageBonus = 0f;
            int critBonus = 0;
            if (coinofDeceit)
            {
                damageBonus += 0.03f;
                critBonus += 3;
            }
            if (ruinMedallion)
            {
                damageBonus += 0.04f;
                critBonus += 4;
            }
            if (silencingSheath)
            {
                damageBonus += 0.02f;
                critBonus += 2;
            }
            if (mirageMirror)
            {
                damageBonus += 0.02f;
                critBonus += 2;
                Player.aggro -= 200;
            }
            if (abyssalMirror)
            {
                damageBonus += 0.05f;
                critBonus += 5;
                Player.aggro -= 450;
            }
            if (darkMatterSheath)
            {
                damageBonus += 0.06f;
                critBonus += 6;
            }
            if (eclipseMirror)
            {
                damageBonus += 0.10f;
                critBonus += 10;
                Player.aggro -= 700;
            }
            if (nanotech)
            {
                damageBonus += 0.20f;
                critBonus += 20;
                CRUtil.AddRogueVelocity(Player, 0.15f);
                if (CRUtil.IsRogueDamageType(Player.HeldItem.DamageType))
                {
                    Player.statDefense += 30;
                    Player.endurance += 0.10f;
                }
            }
            if (damageBonus <= 0f && critBonus == 0)
                return;
            DamageClass rogue = CRUtil.GetRogueDamageClass();
            Player.GetDamage(rogue) += damageBonus;
            Player.GetCritChance(rogue) += critBonus;
        }
        /// <summary>
        /// 斯塔提斯召唤系列只结算所装备的最高档，避免祝福、诅咒和腰带同时装备时重复叠加。
        /// 诅咒腰带额外保留盗贼双职业面板。
        /// </summary>
        private void ApplyStatisSummonEffects()
        {
            if (statisBeltOfCurses)
            {
                Player.GetKnockback(DamageClass.Summon).Base += 2.5f;
                Player.GetDamage(DamageClass.Summon) += 0.2f;
                Player.maxMinions += 4;
                Player.whipRangeMultiplier += 0.2f;
                Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.2f;
                CRUtil.ApplyRogueStats(Player, 0.2f, 20, 0f, 0f, 0f, 0f);
                return;
            }
            if (statisCurse)
            {
                Player.GetKnockback(DamageClass.Summon).Base += 2.5f;
                Player.GetDamage(DamageClass.Summon) += 0.1f;
                Player.maxMinions += 3;
                Player.whipRangeMultiplier += 0.1f;
                Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.1f;
                return;
            }
            if (statisBlessing)
            {
                Player.GetKnockback(DamageClass.Summon).Base += 2.5f;
                Player.GetDamage(DamageClass.Summon) += 0.1f;
                Player.maxMinions += 3;
            }
        }
        /// <summary>
        /// 结算寒霜壁垒、神圣护符与神之壁垒的常驻生存属性。
        /// 顶点已经完整包含两件前置，因此存在顶点时直接跳过前置分支，避免重复穿甲和护盾结算。
        /// </summary>
        private void ApplyRampartChainEffects()
        {
            if (rampartOfDeities)
            {
                ApplyDeificAmuletEffects(50, useDrownCollision: true);
                ApplyFrigidBulwarkEffects();
                return;
            }
            if (deificAmulet)
                ApplyDeificAmuletEffects(25, useDrownCollision: false);
            if (frigidBulwark)
                ApplyFrigidBulwarkEffects();
        }
        /// <summary>
        /// 应用神圣护符系的恐慌加速、魔力星吸附、魔法手铐、穿甲与水下照明。
        /// 神之壁垒使用完整溺水碰撞判定，神圣护符沿用其旧版 wet 判定。
        /// </summary>
        private void ApplyDeificAmuletEffects(int armorPenetration, bool useDrownCollision)
        {
            Player.panic = true;
            Player.manaMagnet = true;
            Player.magicCuffs = true;
            Player.GetArmorPenetration(DamageClass.Generic) += armorPenetration;
            bool underwater = useDrownCollision
                ? Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir)
                : Player.wet;
            if (underwater)
                Lighting.AddLight(Player.Center / 16f, 1.35f, 0.3f, 0.9f);
        }
        /// <summary>
        /// 应用寒霜壁垒的击退免疫、圣骑士护盾、半血冰屏障与残血额外减伤。
        /// 圣骑士护盾仅在高于四分之一生命时生效，并每十帧向八百像素内的同队玩家补一次盾。
        /// </summary>
        private void ApplyFrigidBulwarkEffects()
        {
            Player.noKnockback = true;
            if (Player.statLife > Player.statLifeMax2 * RampartofDeities.PaladinShieldLifeThreshold)
            {
                Player.hasPaladinShield = true;
                SharePaladinShield();
            }
            if (Player.statLife <= Player.statLifeMax2 * RampartofDeities.IceBarrierLifeThreshold)
                Player.AddBuff(BuffID.IceBarrier, 5, true);
            if (Player.statLife <= Player.statLifeMax2 * RampartofDeities.EmergencyEnduranceLifeThreshold)
                Player.endurance += RampartofDeities.EmergencyEnduranceBonus;
        }
        /// <summary>
        /// 让被观察的队伍成员向本地玩家补圣骑士护盾。
        /// 原版多人逻辑只在非本机玩家每十帧执行一次，并限制同队且距离小于八百像素。
        /// </summary>
        private void SharePaladinShield()
        {
            if (Player.whoAmI == Main.myPlayer || Player.miscCounter % 10 != 0)
                return;
            Player localPlayer = Main.player[Main.myPlayer];
            if (Player.team == 0 || localPlayer.team != Player.team)
                return;
            if (Vector2.DistanceSquared(Player.Center, localPlayer.Center)
                >= RampartofDeities.PaladinShieldShareDistance * RampartofDeities.PaladinShieldShareDistance)
                return;
            localPlayer.AddBuff(BuffID.PaladinsShield, 20, true);
        }
        /// <summary>
        /// 结算 Agentia 与其余酒类药水的面板属性。
        /// 属性药水先加攻防和机动，酒类固定值减防最后执行，保证“当前防御百分比”口径正确。
        /// </summary>
        private void ApplyPotionEffects()
        {
            if (titanScale)
            {
                Player.endurance += 0.05f;
                Player.statDefense += 5;
                Player.kbBuff = true;
            }
            if (cadence)
            {
                Player.discountAvailable = true;
                Player.lifeMagnet = true;
                Player.calmed = true;
                Player.loveStruck = true;
                Player.lifeRegen += 4;
                Player.statLifeMax2 += (int)(Player.statLifeMax2 * 0.2f);
            }
            if (holyWrath)
            {
                Player.GetDamage<GenericDamageClass>() += 0.12f;
                Player.moveSpeed += 0.05f;
            }
            if (armorShattering)
            {
                Player.GetCritChance<GenericDamageClass>() += 8;
                Player.GetDamage<GenericDamageClass>() += 0.08f;
            }
            if (armorCrumbling)
                Player.GetCritChance<GenericDamageClass>() += 5;
            if (bounding)
            {
                Player.jumpSpeedBoost += 0.5f;
                Player.jumpHeight += 10;
                Player.extraFall += 25;
            }
            if (soaring)
                Player.wingTimeMax = (int)(Player.wingTimeMax * 1.1);
            if (profanedRage)
            {
                Player.GetCritChance<GenericDamageClass>() += 12;
                Player.moveSpeed += 0.05f;
            }
            if (ceaselessHunger)
                MagnetizeDroppedItems();
            if (calcium)
                Player.noFallDmg = true;
            if (bloodyMary)
            {
                Player.GetDamage<GenericDamageClass>() += 0.15f;
                Player.GetCritChance<GenericDamageClass>() += 5;
                Player.GetAttackSpeed<MeleeDamageClass>() += 0.15f;
                Player.moveSpeed += 0.15f;
                Player.statDefense -= 4;
                Player.endurance -= 0.04f;
            }
            if (moscowMule)
            {
                Player.GetDamage<GenericDamageClass>() += MoscowMule.DamageBoost;
                Player.GetCritChance<GenericDamageClass>() += MoscowMule.CritBoost;
            }
            if (rum)
                Player.moveSpeed += 0.1f;
            if (caribbeanRum)
                Player.moveSpeed += 0.1f;
            if (moonshine)
            {
                Player.statDefense += 10;
                Player.endurance += 0.03f;
            }
            if (vodka)
            {
                Player.GetDamage<GenericDamageClass>() += 0.06f;
                Player.GetCritChance<GenericDamageClass>() += 2;
            }
            if (rum)
                Player.statDefense -= 8;
            if (caribbeanRum)
            {
                if (Player.statDefense > 0)
                    Player.statDefense -= (int)(Player.statDefense * 0.1f);
                if (!Player.slowFall && !Player.mount.Active)
                    Player.velocity.Y *= 1.01f;
            }
            if (vodka)
                Player.statDefense -= 5;
        }
        /// <summary>
        /// 把世界内尚未进入拾取延迟的掉落物向玩家加速。
        /// 仅处理允许该玩家拾取的物品，并限制在超大矩形范围内。
        /// </summary>
        private void MagnetizeDroppedItems()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            int range = Main.maxTilesX;
            Rectangle attractionArea = new((int)Player.position.X - range, (int)Player.position.Y - range
                , Player.width + range * 2, Player.height + range * 2);
            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];
                if (!item.active || item.noGrabDelay != 0
                    || (item.playerIndexTheItemIsReservedFor != Player.whoAmI
                        && item.playerIndexTheItemIsReservedFor != 255))
                    continue;
                if (!attractionArea.Intersects(new Rectangle((int)item.position.X, (int)item.position.Y
                    , item.width, item.height)))
                    continue;
                item.beingGrabbed = true;
                if (item.playerIndexTheItemIsReservedFor == 255)
                    item.playerIndexTheItemIsReservedFor = Player.whoAmI;
                float horizontal = Player.Center.X > item.Center.X ? 4.5f : -4.5f;
                if ((horizontal > 0f && item.velocity.X < 40f + Player.velocity.X)
                    || (horizontal < 0f && item.velocity.X > -40f + Player.velocity.X))
                    item.velocity.X += horizontal;
                if ((horizontal > 0f && item.velocity.X < 0f) || (horizontal < 0f && item.velocity.X > 0f))
                    item.velocity.X += horizontal * 0.75f;
                float vertical = Player.Center.Y > item.Center.Y ? 4.5f : -4.5f;
                if ((vertical > 0f && item.velocity.Y < 40f) || (vertical < 0f && item.velocity.Y > -40f))
                    item.velocity.Y += vertical;
                if ((vertical > 0f && item.velocity.Y < 0f) || (vertical < 0f && item.velocity.Y > 0f))
                    item.velocity.Y += vertical * 0.75f;
            }
        }
        /// <summary>
        /// 结算血神核心系列的全部装备属性。
        /// 血契先翻倍生命，再由血神核心按当前上限追加 10%，保证阈值读取最终上限。
        /// </summary>
        private void ApplyBloodGodAccessoryEffects()
        {
            if (bloodflareCore)
                SpawnBloodGodAura(BloodflareCore.AuraBaseDamage, BloodflareCore.AuraHitCooldown);
            if (bloodPact)
                Player.statLifeMax2 += Player.statLifeMax2;
            if (bloodyWormScarf)
            {
                Player.GetDamage<MeleeDamageClass>() += BloodyWormScarf.MeleeDamageBonus;
                Player.GetAttackSpeed<MeleeDamageClass>() += BloodyWormScarf.MeleeSpeedBonus;
                Player.endurance += BloodyWormScarf.DamageReduction;
            }
            if (coreOfTheBloodGod)
            {
                Player.statLifeMax2 += (int)(Player.statLifeMax2 * CoreOfTheBloodGod.MaxLifeBonus);
                Player.GetDamage<GenericDamageClass>() += CoreOfTheBloodGod.DamageBonus;
                Player.GetCritChance<GenericDamageClass>() += CoreOfTheBloodGod.CritBonus;
                Player.GetAttackSpeed<MeleeDamageClass>() += CoreOfTheBloodGod.MeleeSpeedBonus;
                Player.endurance += CoreOfTheBloodGod.DamageReduction;
                SpawnBloodGodAura(CoreOfTheBloodGod.AuraBaseDamage, CoreOfTheBloodGod.AuraHitCooldown);
            }
        }
        /// <summary>
        /// 在所有生命上限与防御修改完成后结算血神核心系列的动态阈值。
        /// 这样血契翻倍、血神核心/Cadence 生命上限和所有药水防御修正都会进入最终判定。
        /// </summary>
        private void ApplyBloodGodThresholdEffects()
        {
            if (bloodflareCore)
            {
                if (Player.statDefense < 100)
                    Player.GetDamage<GenericDamageClass>() += BloodflareCore.LowDefenseDamageBonus;
                if (Player.statLife < Player.statLifeMax2 * 0.15f)
                {
                    Player.endurance += BloodflareCore.LowLifeDamageReduction;
                    Player.GetDamage<GenericDamageClass>() += BloodflareCore.LowLifeDamageBonus;
                    Player.GetCritChance<GenericDamageClass>() += BloodflareCore.LowLifeCritBonus;
                }
                else if (Player.statLife <= Player.statLifeMax2 * 0.5f)
                {
                    Player.endurance += BloodflareCore.HalfLifeDamageReduction;
                    Player.GetDamage<GenericDamageClass>() += BloodflareCore.HalfLifeDamageBonus;
                    Player.GetCritChance<GenericDamageClass>() += BloodflareCore.HalfLifeCritBonus;
                }
            }
            if (bloodyWormTooth)
            {
                float bonus = Player.statLife < Player.statLifeMax2 * 0.5f
                    ? BloodyWormTooth.LowLifeBonus
                    : BloodyWormTooth.NormalBonus;
                Player.GetDamage<MeleeDamageClass>() += bonus;
                Player.GetAttackSpeed<MeleeDamageClass>() += bonus;
                Player.endurance += bonus;
            }
            if (coreOfTheBloodGod && Player.statDefense < 100)
                Player.GetDamage<GenericDamageClass>() += CoreOfTheBloodGod.LowDefenseDamageBonus;
        }
        /// <summary>
        /// 为血炎晶核或血神核心生成一帧灵魂虹吸光环。
        /// 只在本地所属客户端生成，避免服务器与远端玩家重复创建同一来源的弹幕。
        /// </summary>
        private void SpawnBloodGodAura(int damage, int hitCooldown)
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            int drain = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero
                , ProjectileID.SoulDrain, damage, 0f, Player.whoAmI, 0f, 0f);
            if (!Main.projectile.IndexInRange(drain))
                return;
            Main.projectile[drain].usesLocalNPCImmunity = true;
            Main.projectile[drain].localNPCHitCooldown = hitCooldown;
        }
        /// <summary>将天界洋葱永久解锁状态写入玩家存档。</summary>
        public override void SaveData(TagCompound tag)
        {
            tag["celestialOnionUnlocked"] = extraAccessoryML;
        }
        /// <summary>从玩家存档恢复天界洋葱永久解锁状态。</summary>
        public override void LoadData(TagCompound tag)
        {
            extraAccessoryML = tag.GetBool("celestialOnionUnlocked");
        }
        /// <summary>
        /// 将永久解锁状态复制到客户端基准副本。
        /// tModLoader 随后通过 SendClientChanges 比较并上报变化。
        /// </summary>
        public override void CopyClientState(ModPlayer targetCopy)
        {
            CalamityRebirthPlayer copy = (CalamityRebirthPlayer)targetCopy;
            copy.extraAccessoryML = extraAccessoryML;
        }
        /// <summary>
        /// 非主机客户端食用天界洋葱后，将新状态发送给服务器。
        /// 服务器会校验发送者身份并广播给其他客户端。
        /// </summary>
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            CalamityRebirthPlayer old = (CalamityRebirthPlayer)clientPlayer;
            if (old.extraAccessoryML == extraAccessoryML)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write(MsgPermanentUnlock);
            packet.Write(Player.whoAmI);
            packet.Write(extraAccessoryML);
            packet.Send();
        }
    }
}
