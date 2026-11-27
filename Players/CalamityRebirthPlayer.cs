using CalamityRebirth.Content.Items.Accessories.Combat.Magic;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Content.Items.Accessories.Combat.Ranged;
using CalamityRebirth.Content.Items.Accessories.Combat.Rogue;
using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions;
using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Content.Particles;
using CalamityRebirth.Content.Projectiles.Healing;
using CalamityRebirth.Content.Projectiles.Magic;
using CalamityRebirth.Content.Projectiles.Melee;
using CalamityRebirth.Content.Projectiles.Rogue;
using CalamityRebirth.Content.Projectiles.Summon;
using CalamityRebirth.Content.Projectiles.Typeless;
using CalamityRebirth.Enums;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System.Collections.Generic;
using System.IO;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;
namespace CalamityRebirth.Players
{
    /// <summary>
    /// 模组玩家状态与统一属性结算入口。
    /// </summary>
    public class CalamityRebirthPlayer : ModPlayer
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
        /// <summary>龙蒿法师套装法弹治疗来源。</summary>
        public const byte HealRequestTarragon = 4;
        /// <summary>血炎近战套装命中治疗来源。</summary>
        public const byte HealRequestBloodflareMelee = 5;
        /// <summary>血炎盗贼套装暴击治疗来源。</summary>
        public const byte HealRequestBloodflareRogue = 6;
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
        /// <summary>本帧是否装备王冠宝石；实际再生由 UpdateLifeRegen 统一结算。</summary>
        public bool crownJewel;
        /// <summary>本帧是否装备感染宝石；实际再生与动态防御由 UpdateLifeRegen 统一结算。</summary>
        public bool infectedJewel;
        /// <summary>本帧是否装备无暇粹魂晶；优先级高于感染宝石与王冠宝石。</summary>
        public bool purity;
        /// <summary>本帧是否穿着完整魔影三件套。</summary>
        public bool demonshadeSetBonus;
        /// <summary>本帧是否装备魔影胸甲，用于静止暗影再生。</summary>
        public bool demonshadeRegen;
        /// <summary>本帧是否装备魔影胸甲，用于远程省弹。</summary>
        public bool demonshadeAmmoSaving;
        /// <summary>本帧是否装备魔影护腿，用于提高跑速与加速度。</summary>
        public bool demonshadeShadowSpeed;
        /// <summary>玩家是否持有狂怒状态。</summary>
        public bool enraged;
        /// <summary>魔影套装是否要求维持红魔弹幕。</summary>
        public bool redDevil;
        /// <summary>红魔增益是否确认对应弹幕仍然存活。</summary>
        public bool redDevil2;
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
            crownJewel = false;
            infectedJewel = false;
            purity = false;
            demonshadeSetBonus = false;
            demonshadeRegen = false;
            demonshadeAmmoSaving = false;
            demonshadeShadowSpeed = false;
            enraged = false;
            redDevil = false;
            redDevil2 = false;
            frigidBulwark = false;
            deificAmulet = false;
            rampartOfDeities = false;
            alwaysHoneyRegen = false;
            honeyDewHalveDebuffs = false;
            honeyTurboRegen = false;
            livingDewHalveDebuffs = false;
            rOoze = false;
            aAmpoule = false;
            ResetAuricTeslaAccessoryEffects();
            ResetTarragonEffects();
            ResetBloodflareEffects();
            ResetSilvaEffects();
            ResetGodSlayerEffects();
            ResetAuricTeslaSetEffects();
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
            ResetAuricTeslaAccessoryStateOnDeath();
            ResetTarragonStateOnDeath();
            ResetBloodflareStateOnDeath();
            ResetSilvaStateOnDeath();
            ResetGodSlayerStateOnDeath();
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (enraged)
                modifiers.FinalDamage *= 2.25f;
            ApplySummonDefenseIgnore(item.DamageType, ref modifiers);
            ApplyCrystalSkullStealthModifiers(item.DamageType, ref modifiers);
            ApplySilvaMeleeDamage(item, ref modifiers);
        }
        /// <summary>弹幕攻击 NPC 时应用召唤物无视防御与版本专属原版伤害回调。</summary>
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (enraged)
                modifiers.FinalDamage *= 2.25f;
            ApplySummonDefenseIgnore(proj.DamageType, ref modifiers);
            ApplyCrystalSkullStealthModifiers(proj.DamageType, ref modifiers);
            ApplyRogueMirrorHitModifiers(proj, ref modifiers);
            ApplyElementalQuiverProjectileBonus(proj, ref modifiers);
            ApplySilvaProjectileDamage(proj, ref modifiers);
            ApplyGodSlayerRangedCrit(proj, ref modifiers);
            ApplyAuricTeslaProjectileDamage(proj, ref modifiers);
            VanillaNerfRevertSystem.RestoreProjectileDamage(Player, proj, ref modifiers);
        }
        /// <summary>
        /// 在灾厄完成跑速倍率结算后，撤销当前版本对原版移动属性的削减。
        /// </summary>
        public override void PostUpdateRunSpeeds()
        {
            VanillaNerfRevertSystem.RestoreRunSpeedPenalty(Player);
            ApplySilvaRunSpeed();
            ApplyAuricTeslaRunSpeed();
            ApplyDemonshadeRunSpeed();
            ApplyShieldSlamRunSpeedEffects();
            GodSlayerDashMovement();
            ShieldSlamDashMovement();
        }
        /// <summary>魔影护腿把最终跑动加速度与最大跑速各提高五十百分点。</summary>
        private void ApplyDemonshadeRunSpeed()
        {
            if (!demonshadeShadowSpeed)
                return;
            Player.runAcceleration *= 1.50f;
            Player.maxRunSpeed *= 1.50f;
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
            ApplyDemonshadeBurn(target);
            ApplyCrystalSkullOnHit(target, hit, -1);
            if (item.CountsAsClass(DamageClass.Melee) || item.CountsAsClass(DamageClass.MeleeNoSpeed))
                ApplyBloodflareMeleeHit(target);
            ApplyGodSlayerItemHit(item, hit);
        }
        /// <summary>近战弹幕命中后补挂燃烧III与破晓II。</summary>
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyMeleeHitDebuffs(target, hit.DamageType);
            ApplySummonAccessoryHitDebuffs(proj, target);
            ApplyPotionHitDebuffs(target);
            ApplyDemonshadeBurn(target);
            ApplyCrystalSkullOnHit(target, hit, proj.type);
            ApplyNanotechOnHit(proj, hit);
            TrySpawnAbaddonCrit(proj, target, hit);
            ApplyTarragonProjectileHit(proj, target, hit);
            ApplyBloodflareProjectileHit(proj, target, hit);
            ApplySilvaProjectileHit(proj, target, hit, damageDone);
            ApplyGodSlayerProjectileHit(proj, target, hit);
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
        /// <summary>魔影套装持有者的任意直接攻击统一施加五秒魔影灼燃。</summary>
        private void ApplyDemonshadeBurn(NPC target)
        {
            if (!demonshadeSetBonus)
                return;
            target.AddBuff(ModContent.BuffType<DemonshadeBurn>(), DemonshadeBurn.Duration);
        }
        /// <summary>PvP 中按攻击者的魔影套装状态施加同一种魔影灼燃。</summary>
        public static void TryApplyDemonshadeBurnOnPvP(Player attacker, Player target)
        {
            if (attacker is null || target is null || !attacker.active || !target.active
                || !attacker.GetModPlayer<CalamityRebirthPlayer>().demonshadeSetBonus)
                return;
            target.AddBuff(ModContent.BuffType<DemonshadeBurn>(), DemonshadeBurn.Duration);
        }
        /// <summary>
        /// 玩家侧持续伤害结算。任一负面减益存在时先清零自然再生和再生计时，
        /// 再按各减益的原版 lifeRegen 口径叠加损失。
        /// </summary>
        public override void UpdateBadLifeRegen()
        {
            ApplySilvaDebuffRegenProtection();
            if (!Player.HasBuff<BurningIII>() && !Player.HasBuff<DaybreakII>()
                && !Player.HasBuff<ElectrifiedII>() && !Player.HasBuff<ElementalMixII>()
                && !Player.HasBuff<DemonshadeBurn>())
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
            if (Player.HasBuff<DemonshadeBurn>())
                Player.lifeRegen -= DemonshadeBurn.DamagePerSecond * 2;
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
            if (demonshadeRegen)
                ApplyDemonshadeRegen();
            if (tarraLifeRegen)
                Player.lifeRegen += 10;
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
        /// 魔影胸甲在完全静止且未使用物品时快速提高再生计时；
        /// Boss 战使用较低上限，非 Boss 战允许累积到更高的再生档位。
        /// </summary>
        private void ApplyDemonshadeRegen()
        {
            if (Math.Abs(Player.velocity.X) >= 0.05f
                || Math.Abs(Player.velocity.Y) >= 0.05f
                || Player.itemAnimation != 0)
                return;
            bool bossAlive = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.boss)
                    continue;
                bossAlive = true;
                break;
            }
            int timeLimit = bossAlive ? 900 : 3600;
            int regenStep = bossAlive ? 2 : 8;
            float timeRegenCap = bossAlive ? 16f : 60f;
            if (Player.lifeRegenTime > 90 && Player.lifeRegenTime < timeLimit)
                Player.lifeRegenTime = timeLimit;
            Player.lifeRegenTime += regenStep;
            Player.lifeRegen += regenStep;
            float timeBonus = Player.lifeRegenTime * 2.5f / 300f;
            Player.lifeRegen += (int)Math.Min(timeBonus, timeRegenCap);
            if (Player.lifeRegen <= 0 || Player.statLife >= Player.statLifeMax2)
                return;
            Player.lifeRegenCount++;
            if (Main.rand.Next(30000) >= Player.lifeRegenTime && Main.rand.Next(30) != 0)
                return;
            Dust dust = Dust.NewDustPerfect(Player.Center, DustID.ShadowbeamStaff
                , Main.rand.NextVector2Circular(2f, 2f), 200, default, 1f);
            dust.noGravity = true;
            dust.velocity *= 0.75f;
            dust.fadeIn = 1.3f;
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
            UpdateAuricCarpetTexture();
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
        // 仅在正常绘制通道触发，避免残影把同一份粒子重复生成。
        /// <summary>
        /// 为破晓II生成玩家粒子。
        /// shadow 为 0 可排除残影绘制，防止一次画面更新重复消耗粒子和视觉预算。
        /// </summary>
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0f && Player.HasBuff<DaybreakII>())
                DaybreakII.DrawEffects(drawInfo);
            if (drawInfo.shadow == 0f && Player.HasBuff<ElectrifiedII>())
                ElectrifiedII.SpawnElectricVisuals(Player);
            if (drawInfo.shadow == 0f && Player.HasBuff<ElementalMixII>())
                ElementalMixII.DrawEffects(drawInfo);
            if (drawInfo.shadow == 0f && Player.HasBuff<DemonshadeBurn>())
                SpawnDemonshadeBurnPlayerVisuals();
            ApplyAuricTeslaVisuals(drawInfo, ref r, ref g, ref b, ref fullBright);
            ApplyDemonshadeVisuals(drawInfo, ref r, ref g, ref b, ref fullBright);
        }
        /// <summary>玩家承受魔影灼燃时持续生成黑芯紫色火花和紫焰火尘。</summary>
        private void SpawnDemonshadeBurnPlayerVisuals()
        {
            Vector2 spot = Player.position + Main.rand.NextVector2Square(0f, Player.width);
            spot.Y += Main.rand.NextFloat(Player.height);
            if (Main.rand.NextBool())
            {
                Vector2 velocity = new(0f, Main.rand.NextFloat(-10f, -6f));
                DemonshadeBurnSparkSystem.Spawn(spot, velocity
                    , Main.rand.NextBool() ? DemonshadeBurnParticleSet.BrightColor
                        : Color.Lerp(DemonshadeBurnParticleSet.BrightColor, Color.Magenta, 0.55f)
                    , Main.rand.Next(18, 27), Main.rand.NextFloat(0.015f, 0.045f));
            }
            Dust dust = Dust.NewDustPerfect(spot, Main.rand.NextBool()
                ? DustID.AncientLight : DustID.Shadowflame);
            dust.scale = Main.rand.NextFloat(0.4f, 0.62f);
            dust.velocity = new Vector2(0f, Main.rand.NextFloat(-2.5f, -0.5f))
                .RotatedByRandom(0.6f);
            dust.noGravity = true;
            if (dust.type == DustID.AncientLight)
                dust.color = DemonshadeBurnParticleSet.BrightColor;
        }
        /// <summary>
        /// 魔影套装移动时生成暗影焰尘并压成黑红色高亮外观。
        /// 该表现不读取盗贼潜行，也不因城镇 NPC 数量改变。
        /// </summary>
        private void ApplyDemonshadeVisuals(PlayerDrawSet drawInfo, ref float r, ref float g
            , ref float b, ref bool fullBright)
        {
            if (!demonshadeSetBonus || Player.mount.Active
                || (Math.Abs(Player.velocity.X) <= 0.05f
                    && Math.Abs(Player.velocity.Y) <= 0.05f))
                return;
            if (drawInfo.shadow == 0f && Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(drawInfo.Position + Main.rand.NextVector2Square(
                    0f, Player.width), DustID.Shadowflame, Player.velocity * 0.4f
                    , 100, DemonshadeDashFlameColor, 1.5f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
            }
            r *= 0.15f;
            g *= 0.025f;
            b *= 0.10f;
            fullBright = true;
        }
        /// <summary>近战挥砍期间有三分之一概率生成黑红暗影焰尘。</summary>
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (!demonshadeSetBonus || !Main.rand.NextBool(3))
                return;
            Vector2 position = new(hitbox.X + Main.rand.NextFloat(hitbox.Width)
                , hitbox.Y + Main.rand.NextFloat(hitbox.Height));
            Dust dust = Dust.NewDustPerfect(position, DustID.Shadowflame
                , Player.velocity * 0.2f + new Vector2(Player.direction * 3f, 0f)
                , 100, DemonshadeDashFlameColor, 2.5f);
            dust.noGravity = true;
        }
        /// <summary>
        /// 玩家受到 NPC 接触伤害前结算血肉图腾与胜利药水。
        /// 两者都只通过本次 HurtModifiers 乘算，不修改 NPC 本体伤害。
        /// </summary>
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            ApplyTarragonContactReduction(ref modifiers);
            ApplyBloodflareContactReduction(ref modifiers);
            ApplySilvaContactReduction(ref modifiers);
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
            if (enraged)
                modifiers.FinalDamage *= 1.25f;
            // NextFloat 的上界不包含 1，因此这里与该概率区间一一对应。
            if (bloodPact && Main.rand.NextFloat() < BloodPact.CriticalHitChance)
                modifiers.FinalDamage *= BloodPact.CriticalHitDamageMultiplier;
            ApplyGodSlayerLowDamage(ref modifiers);
            ApplySpongeAbsorbHeal(ref modifiers);
        }
        /// <summary>新生药水：实际受伤后按本次伤害的十五分之一回复生命。</summary>
        public override void OnHurt(Player.HurtInfo info)
        {
            ApplyFrostBarrierOnHurt(info);
            ApplyTarragonMeleeOnHurt();
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
            ApplyGodSlayerPostHurt(info);
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
            if (TryGodSlayerFreeDodge(info))
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
                HealRequestTarragon when modPlayer.tarraMage && player.lifeSteal > 0f
                    => Math.Clamp(damageSource, 0, 50000) / (modPlayer.auricSet ? 100 : 50),
                HealRequestBloodflareMelee when modPlayer.bloodflareMelee
                    => Math.Clamp(damageSource, 1, 3),
                HealRequestBloodflareRogue when modPlayer.bloodflareThrowing && player.lifeSteal > 0f => 1,
                _ => 0
            };
            if (heal <= 0)
                return;
            if (reason == HealRequestTarragon)
                player.lifeSteal -= Math.Max(1f, damageSource * 0.045f);
            if (reason == HealRequestBloodflareRogue)
                player.lifeSteal -= 1f;
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
            if (demonshadeAmmoSaving && weapon.CountsAsClass(DamageClass.Ranged)
                && Main.rand.NextFloat() < 0.50f)
                return false;
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
            if (tequilaSunrise)
            {
                knockback += TequilaSunrise.GeneralKnockbackBoost;
                if (Main.dayTime)
                    knockback += TequilaSunrise.DayKnockbackBoost;
            }
            if (auricBoost)
                knockback *= 1f + (1f - psychoticStealth) * 0.5f;
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
            ApplyAuricTeslaAccessoryEffects();
            ApplyTarragonEffects();
            ApplyBloodflareEffects();
            ApplySilvaEffects();
            ApplyGodSlayerEffects();
            ApplyAuricTeslaSetEffects();
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
        /// <summary>本帧是否装备灾劫之尖啸。</summary>
        public bool affliction;
        /// <summary>本帧是否受到侵蚀增益。</summary>
        public bool afflicted;
        /// <summary>清空本帧装备与增益状态。</summary>
        public void ResetAfflictionEffects()
        {
            affliction = false;
            afflicted = false;
        }
        /// <summary>
        /// 结算灾劫之尖啸与侵蚀共用的属性，并每十帧刷新一次同队玩家的侵蚀增益。
        /// </summary>
        public void ApplyAfflictionEffects()
        {
            if (!affliction && !afflicted)
                return;
            Player.noKnockback = true;
            Player.GetDamage(DamageClass.Generic) += Affliction.DamageBonus;
            Player.statDefense += Affliction.DefenseBonus;
            Player.endurance += Affliction.DamageReduction;
            Player.statLifeMax2 += (int)(Player.statLifeMax2
                * Affliction.MaxLifeBonus);
            Player.lifeRegen += Affliction.LifeRegenBonus;
            if (!affliction || Player.miscCounter % Affliction.TeamAuraInterval != 0)
                return;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player teammate = Main.player[i];
                if (!teammate.active || teammate.whoAmI == Player.whoAmI
                    || Player.team == 0 || teammate.team != Player.team)
                    continue;
                teammate.AddBuff(ModContent.BuffType<Afflicted>()
                    , Affliction.TeamBuffDuration, true);
            }
        }
        /// <summary>本帧是否装备寒霜屏障。</summary>
        public bool frostBarrier;
        /// <summary>本帧是否装备疯狂护符。</summary>
        public bool psychoticAmulet;
        /// <summary>疯狂护符的潜行值；1 为完全暴露，0 为完全潜行。</summary>
        private float psychoticStealth = 1f;
        /// <summary>攻击后阻止潜行继续下降的剩余帧数。</summary>
        private int psychoticStealthAttackDelay;
        /// <summary>清空两件前置饰品的本帧装备标记；潜行值本身跨帧保留。</summary>
        private void ResetAuricTeslaAccessoryEffects()
        {
            frostBarrier = false;
            psychoticAmulet = false;
        }
        /// <summary>
        /// 死亡时恢复疯狂护符的暴露状态并清除攻击延迟，避免复活后继承上一轮潜行进度。
        /// </summary>
        private void ResetAuricTeslaAccessoryStateOnDeath()
        {
            psychoticStealth = 1f;
            psychoticStealthAttackDelay = 0;
        }
        /// <summary>
        /// 结算寒霜屏障免疫与疯狂护符常驻属性。潜行动态值放在所有装备属性之后，
        /// 以便按本帧最终潜行状态计算远程伤害、暴击和仇恨。
        /// </summary>
        private void ApplyAuricTeslaAccessoryEffects()
        {
            if (frostBarrier)
                Player.buffImmune[BuffID.Chilled] = true;
            if (!auricBoost)
                ApplyPsychoticAmuletEffects();
        }
        /// <summary>
        /// 疯狂护符每帧提供 5% 远程伤害与 5% 远程暴击，并启用蘑菇矿潜行。
        /// 静止且不在攻击动作中时潜行值每帧下降 0.015；移动时按速度恢复。
        /// 完全潜行额外提供 20% 远程伤害、10% 远程暴击并降低 750 仇恨。
        /// </summary>
        private void ApplyPsychoticAmuletEffects()
        {
            if (!psychoticAmulet)
            {
                psychoticStealth = 1f;
                psychoticStealthAttackDelay = 0;
                return;
            }
            Player.GetDamage(DamageClass.Ranged) += PsychoticAmulet.BaseRangedDamageBonus;
            Player.GetCritChance(DamageClass.Ranged) += PsychoticAmulet.BaseRangedCritBonus;
            Player.shroomiteStealth = true;
            if (Player.itemAnimation > 0)
                psychoticStealthAttackDelay = 5;
            bool standingStill = Math.Abs(Player.velocity.X) < 0.1
                && Math.Abs(Player.velocity.Y) < 0.1
                && !Player.mount.Active;
            if (standingStill)
            {
                if (psychoticStealthAttackDelay == 0 && psychoticStealth > 0f)
                {
                    psychoticStealth -= 0.015f;
                    if (psychoticStealth <= 0f)
                    {
                        psychoticStealth = 0f;
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI);
                    }
                }
            }
            else
            {
                float speed = Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y);
                psychoticStealth = Math.Min(1f, psychoticStealth + speed * 0.0075f);
                if (Player.mount.Active)
                    psychoticStealth = 1f;
            }
            float stealthStrength = 1f - psychoticStealth;
            Player.GetDamage(DamageClass.Ranged) += PsychoticAmulet.MaximumStealthDamageBonus * stealthStrength;
            Player.GetCritChance(DamageClass.Ranged) += (int)(PsychoticAmulet.MaximumStealthCritBonus * stealthStrength);
            Player.aggro -= (int)(PsychoticAmulet.MaximumStealthAggroReduction * stealthStrength);
            if (psychoticStealthAttackDelay > 0)
                psychoticStealthAttackDelay--;
        }
        /// <summary>
        /// 寒霜屏障受击反制：按伤害提高判定半径并冻结附近敌怪。
        /// 依次尝试三个灾厄分支的 GlacialState；均缺失时回退原版 Frozen。
        /// </summary>
        private void ApplyFrostBarrierOnHurt(Player.HurtInfo info)
        {
            if (!frostBarrier || info.Damage <= 0)
                return;
            SoundEngine.PlaySound(SoundID.Item27, Player.position);
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.active || npc.friendly)
                    continue;
                float range = Main.rand.Next(200 + info.Damage / 2, 301 + info.Damage * 2);
                if (range > 500f)
                    range = 500f + (range - 500f) * 0.75f;
                if (range > 700f)
                    range = 700f + (range - 700f) * 0.5f;
                if (range > 900f)
                    range = 900f + (range - 900f) * 0.25f;
                if (Vector2.Distance(npc.Center, Player.Center) >= range)
                    continue;
                int duration = Main.rand.Next(90 + info.Damage / 3, 240 + info.Damage / 2);
                if (CalamityCompatUtil.TryAddBuff(npc, "CalamityMod", "GlacialState", duration)
                    || CalamityCompatUtil.TryAddBuff(npc, "CalamityModClassicPreTrailer", "GlacialState", duration)
                    || CalamityCompatUtil.TryAddBuff(npc, "CalRD", "GlacialState", duration))
                    continue;
                npc.AddBuff(BuffID.Frozen, duration);
            }
        }
        /// <summary>本帧是否穿着任意古圣金源头部；启用静止潜行式通用增伤。</summary>
        public bool auricBoost;
        /// <summary>本帧是否穿着完整古圣金源三件套。</summary>
        public bool auricSet;
        /// <summary>清空古圣金源每帧装备与套装标记。</summary>
        private void ResetAuricTeslaSetEffects()
        {
            auricBoost = false;
            auricSet = false;
        }
        /// <summary>结算古圣金源静止增伤与近战满血增伤。</summary>
        private void ApplyAuricTeslaSetEffects()
        {
            if (auricBoost)
                ApplyAuricBoostStealth();
            if (auricSet && silvaMelee && Player.statLifeMax2 > 0)
            {
                float healthRatio = Player.statLife / (float)Player.statLifeMax2;
                Player.GetDamage(DamageClass.Melee) += healthRatio * 0.20f;
            }
        }
        /// <summary>
        /// 古圣金源头部共用的潜行式静止增益：静止时最多获得 20% 通用伤害与 10 通用暴击。
        /// 移动时按速度恢复暴露值，攻击后短暂停止继续潜行。
        /// </summary>
        private void ApplyAuricBoostStealth()
        {
            if (Player.itemAnimation > 0)
                psychoticStealthAttackDelay = 5;
            bool standingStill = Math.Abs(Player.velocity.X) < 0.1f
                && Math.Abs(Player.velocity.Y) < 0.1f && !Player.mount.Active;
            if (standingStill)
            {
                if (psychoticStealthAttackDelay == 0 && psychoticStealth > 0f)
                {
                    psychoticStealth = Math.Max(0f, psychoticStealth - 0.015f);
                    if (psychoticStealth <= 0f && Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI);
                }
            }
            else
            {
                float speed = Math.Abs(Player.velocity.X) + Math.Abs(Player.velocity.Y);
                psychoticStealth = Math.Min(1f, psychoticStealth + speed * 0.0075f);
                if (Player.mount.Active)
                    psychoticStealth = 1f;
            }
            float stealthStrength = 1f - psychoticStealth;
            Player.GetDamage(DamageClass.Generic) += stealthStrength * 0.20f;
            Player.GetCritChance(DamageClass.Generic) += (int)(stealthStrength * 10f);
            if (psychoticStealthAttackDelay > 0)
                psychoticStealthAttackDelay--;
        }
        /// <summary>古圣金源三件套额外提高 10% 最大跑速与加速度。</summary>
        private void ApplyAuricTeslaRunSpeed()
        {
            if (!auricSet)
                return;
            Player.maxRunSpeed *= 1.10f;
            Player.runAcceleration *= 1.10f;
        }
        /// <summary>金源近战弹幕按当前生命比例获得至多 20% 最终伤害。</summary>
        private void ApplyAuricTeslaProjectileDamage(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!auricSet || !silvaMelee || Player.statLifeMax2 <= 0
                || !projectile.CountsAsClass(DamageClass.Melee))
                return;
            float healthRatio = Player.statLife / (float)Player.statLifeMax2;
            modifiers.FinalDamage *= 1f + healthRatio * 0.20f;
        }
        /// <summary>把飞毯贴图切换为金源披风纹样；离装后恢复原版贴图。</summary>
        private void UpdateAuricCarpetTexture()
        {
            if (Main.dedServ)
                return;
            if (auricSet)
            {
                TextureAssets.FlyingCarpet = ModContent.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(
                    "CalamityRebirth/Assets/ExtraTextures/AuricCarpet");
            }
            else
            {
                TextureAssets.FlyingCarpet = Main.Assets.Request<Microsoft.Xna.Framework.Graphics.Texture2D>(
                    "Images/FlyingCarpet");
            }
        }
        /// <summary>古圣金源移动时生成金色拖影并把角色染成金绿色高光。</summary>
        private void ApplyAuricTeslaVisuals(PlayerDrawSet drawInfo, ref float r, ref float g
            , ref float b, ref bool fullBright)
        {
            if (!auricSet || Player.mount.Active
                || (Math.Abs(Player.velocity.X) <= 0.05f && Math.Abs(Player.velocity.Y) <= 0.05f))
                return;
            if (drawInfo.shadow == 0f)
            {
                int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f)
                    , Player.width + 4, Player.height + 4
                    , Main.rand.NextBool() ? DustID.CopperCoin : DustID.GoldCoin
                    , Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.5f;
            }
            r *= 0.025f;
            g *= 0.15f;
            b *= 0.035f;
            fullBright = true;
        }
        /// <summary>本帧是否穿着任意血炎三件套。</summary>
        public bool bloodflareSet;
        /// <summary>本帧是否穿着血炎近战头套装。</summary>
        public bool bloodflareMelee;
        /// <summary>本帧是否穿着血炎射手套装。</summary>
        public bool bloodflareRanged;
        /// <summary>本帧是否穿着血炎法师套装。</summary>
        public bool bloodflareMage;
        /// <summary>本帧是否穿着血炎召唤套装。</summary>
        public bool bloodflareSummon;
        /// <summary>本帧是否穿着血炎盗贼套装。</summary>
        public bool bloodflareThrowing;
        /// <summary>血炎近战连续命中计数。</summary>
        private int bloodflareMeleeHits;
        /// <summary>血之狂怒剩余帧数。</summary>
        private int bloodflareFrenzyTimer;
        /// <summary>血之狂怒冷却。</summary>
        private int bloodflareFrenzyCooldown;
        /// <summary>红心掉落冷却。</summary>
        public int bloodflareHeartTimer = 180;
        /// <summary>魔力星掉落冷却。</summary>
        public int bloodflareManaTimer = 180;
        /// <summary>法师火焰爆炸冷却。</summary>
        private int bloodflareMageCooldown;
        /// <summary>射手灵魂爆发冷却。</summary>
        private int bloodflareRangedCooldown;
        /// <summary>召唤地雷生成冷却。</summary>
        private int bloodflareSummonTimer;
        /// <summary>清空血炎套装的逐帧职业标记。</summary>
        private void ResetBloodflareEffects()
        {
            bloodflareSet = false;
            bloodflareMelee = false;
            bloodflareRanged = false;
            bloodflareMage = false;
            bloodflareSummon = false;
            bloodflareThrowing = false;
        }
        /// <summary>死亡时清空血炎套装的全部跨帧计数与冷却。</summary>
        private void ResetBloodflareStateOnDeath()
        {
            bloodflareMeleeHits = 0;
            bloodflareFrenzyTimer = 0;
            bloodflareFrenzyCooldown = 0;
            bloodflareHeartTimer = 0;
            bloodflareManaTimer = 0;
            bloodflareMageCooldown = 0;
            bloodflareRangedCooldown = 0;
            bloodflareSummonTimer = 0;
        }
        /// <summary>
        /// 结算血炎通用掉落冷却、盗贼生命阈值、召唤阈值与地雷、近战狂怒、
        /// 射手灵魂和法师爆炸冷却。
        /// </summary>
        private void ApplyBloodflareEffects()
        {
            if (bloodflareSet)
            {
                if (bloodflareHeartTimer > 0)
                    bloodflareHeartTimer--;
                if (bloodflareManaTimer > 0)
                    bloodflareManaTimer--;
            }
            ApplyBloodflareRogueEffects();
            ApplyBloodflareSummonEffects();
            ApplyBloodflareMeleeEffects();
            if (bloodflareRangedCooldown > 0)
                bloodflareRangedCooldown--;
            if (bloodflareMageCooldown > 0)
                bloodflareMageCooldown--;
        }
        /// <summary>盗贼生命高于 80% 时获得防御/暴击，否则获得盗贼伤害。</summary>
        private void ApplyBloodflareRogueEffects()
        {
            if (!bloodflareThrowing)
                return;
            if (Player.statLife > (int)(Player.statLifeMax2 * 0.8f))
            {
                CRUtil.ApplyRogueStats(Player, 0f, 5, 0f, 0f, 0f, 0f);
                Player.statDefense += 30;
            }
            else
            {
                CRUtil.ApplyRogueStats(Player, 0.10f, 0, 0f, 0f, 0f, 0f);
            }
        }
        /// <summary>召唤生命阈值加成，并每 15 秒生成三枚环绕地雷。</summary>
        private void ApplyBloodflareSummonEffects()
        {
            if (!bloodflareSummon)
                return;
            if (Player.statLife >= (int)(Player.statLifeMax2 * 0.9f))
                Player.GetDamage(DamageClass.Summon) += 0.10f;
            else if (Player.statLife <= (int)(Player.statLifeMax2 * 0.5f))
            {
                Player.statDefense += 20;
                Player.lifeRegen += 2;
            }
            if (bloodflareSummonTimer > 0)
                bloodflareSummonTimer--;
            if (Player.whoAmI != Main.myPlayer || bloodflareSummonTimer > 0)
                return;
            bloodflareSummonTimer = 900;
            int mineBaseDamage = auricSet ? 15000 : 5000;
            int mineDamage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(mineBaseDamage);
            for (int i = 0; i < 3; i++)
            {
                float angle = i * 120f;
                int mine = Projectile.NewProjectile(Player.GetSource_FromThis()
                    , Player.Center, Vector2.Zero, ModContent.ProjectileType<GhostlyMine>()
                    , mineDamage, 1f, Player.whoAmI, angle);
                if (!Main.projectile.IndexInRange(mine))
                    continue;
                Main.projectile[mine].originalDamage = mineBaseDamage;
                Main.projectile[mine].DamageType = DamageClass.Generic;
            }
        }
        /// <summary>累计 15 次真近战命中后开启 5 秒血之狂怒，并在结束后进入 30 秒冷却。</summary>
        private void ApplyBloodflareMeleeEffects()
        {
            if (!bloodflareMelee)
                return;
            if (bloodflareMeleeHits >= 15)
            {
                bloodflareMeleeHits = 0;
                bloodflareFrenzyTimer = 300;
            }
            if (bloodflareFrenzyTimer > 0)
            {
                bloodflareFrenzyTimer--;
                if (bloodflareFrenzyTimer <= 0)
                    bloodflareFrenzyCooldown = 1800;
                Player.GetCritChance(DamageClass.Melee) += 25f;
                Player.GetDamage(DamageClass.Melee) += 0.25f;
                SpawnBloodflareFrenzyDust();
            }
            if (bloodflareFrenzyCooldown > 0)
                bloodflareFrenzyCooldown--;
        }
        /// <summary>生成狂怒状态下的血尘。</summary>
        private void SpawnBloodflareFrenzyDust()
        {
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood
                    , 0f, 0f, 100, default, 2f);
                Main.dust[dust].position += new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21));
                Main.dust[dust].velocity *= 0.9f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale *= 1f + Main.rand.Next(40) * 0.01f;
                Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
            }
        }
        /// <summary>血炎近战狂怒期间将接触伤害减半。</summary>
        private void ApplyBloodflareContactReduction(ref Player.HurtModifiers modifiers)
        {
            if (bloodflareMelee && bloodflareFrenzyTimer > 0)
                modifiers.FinalDamage *= 0.5f;
        }
        /// <summary>
        /// 血炎近战命中：累计真近战次数，并对可吸血目标请求 1～3 点服务器验证治疗。
        /// </summary>
        private void ApplyBloodflareMeleeHit(NPC target)
        {
            if (!bloodflareMelee)
                return;
            if (bloodflareMeleeHits < 15 && bloodflareFrenzyTimer <= 0 && bloodflareFrenzyCooldown <= 0)
                bloodflareMeleeHits++;
            if (!target.canGhostHeal || Player.moonLeech || Player.whoAmI != Main.myPlayer)
                return;
            RequestHeal(Player, HealRequestBloodflareMelee, Main.rand.Next(1, 4));
        }
        /// <summary>
        /// 血炎弹幕命中：盗贼暴击概率回血，法师暴击按冷却生成三枚火球。
        /// </summary>
        private void ApplyBloodflareProjectileHit(Projectile projectile, NPC target, NPC.HitInfo hit)
        {
            if (projectile.owner != Player.whoAmI || Player.whoAmI != Main.myPlayer)
                return;
            if (bloodflareMelee && projectile.CountsAsClass(DamageClass.MeleeNoSpeed))
                ApplyBloodflareMeleeHit(target);
            if (bloodflareThrowing && hit.Crit && CRUtil.IsRogueDamageType(projectile.DamageType)
                && target.canGhostHeal && Player.whoAmI == Main.myPlayer && Player.lifeSteal > 0f
                && Main.rand.NextBool(2))
                RequestHeal(Player, HealRequestBloodflareRogue);
            if (!bloodflareMage || bloodflareMageCooldown > 0 || !hit.Crit
                || !projectile.CountsAsClass(DamageClass.Magic))
                return;
            bloodflareMageCooldown = 120;
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7f, 10f);
                int fire = Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center
                    , velocity, ProjectileID.BallofFire, (int)(projectile.damage * 0.5f)
                    , 0f, projectile.owner);
                if (Main.projectile.IndexInRange(fire))
                    Main.projectile[fire].netUpdate = true;
            }
        }
        /// <summary>按套装键释放 16 枚血炎之魂，冷却 30 秒。</summary>
        private void TryActivateBloodflareSoulBurst()
        {
            if (!bloodflareRanged || bloodflareRangedCooldown > 0)
                return;
            bloodflareRangedCooldown = 1800;
            SoundEngine.PlaySound(SoundID.Zombie104, Player.position);
            for (int i = 0; i < 64; i++)
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height
                    , DustID.RedTorch, 0f, 0f, 0, default, 1f);
                Main.dust[dust].velocity *= 3f;
                Main.dust[dust].scale *= 1.15f;
            }
            const int ringDustCount = 36;
            Vector2 ringBase = Player.velocity.SafeNormalize(Vector2.UnitY)
                * new Vector2(Player.width / 2f, Player.height) * 0.75f;
            for (int i = 0; i < ringDustCount; i++)
            {
                Vector2 position = ringBase.RotatedBy(
                    (i - (ringDustCount / 2 - 1)) * MathHelper.TwoPi / ringDustCount) + Player.Center;
                Vector2 offset = position - Player.Center;
                int dust = Dust.NewDust(position + offset, 0, 0, DustID.RedTorch
                    , offset.X * 1.5f, offset.Y * 1.5f, 100, default, 1.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].velocity = offset;
            }
            if (Player.whoAmI != Main.myPlayer)
                return;
            const float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2f;
            double deltaAngle = spread / 8f;
            for (int i = 0; i < 8; i++)
            {
                float turnRate = Main.rand.NextFloat() + 0.5f;
                double angle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center
                    , new Vector2((float)Math.Sin(angle) * 5f
                    , (float)Math.Cos(angle) * 5f + Main.rand.Next(1, 7))
                    , ModContent.ProjectileType<BloodflareSoul>(), 800, 0f, Player.whoAmI
                    , 0f, turnRate);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center
                    , new Vector2((float)-Math.Sin(angle) * 5f
                    , (float)-Math.Cos(angle) * 5f + Main.rand.Next(1, 7))
                    , ModContent.ProjectileType<BloodflareSoul>(), 800, 0f, Player.whoAmI
                    , 0f, turnRate);
            }
        }
		/// <summary>本帧是否装备气之石。</summary>
		public bool aeroStone;
		/// <summary>本帧是否装备寒晶石。</summary>
		public bool cryoStone;
		/// <summary>本帧是否装备混沌石。</summary>
		public bool chaosStone;
		/// <summary>本帧是否装备花绽石。</summary>
		public bool bloomStone;
		/// <summary>本帧是否装备玫瑰石。</summary>
		public bool roseStone;
		/// <summary>玫瑰石是否显示外观；隐藏时关闭粉色照明和硫火元素。</summary>
		public bool roseStoneVisible;
		/// <summary>本帧是否装备元灵之心。</summary>
		public bool heartoftheElements;
		/// <summary>元灵之心是否隐藏外观；隐藏档使用略高的基础属性且不召唤元素。</summary>
		public bool heartoftheElementsHideVisual;
		/// <summary>元灵之心是否允许全部五种元素仆从存活。</summary>
		public bool allWaifus;
		/// <summary>本帧是否装备风暴之眼。</summary>
		public bool eyeoftheStorm;
		/// <summary>本帧是否装备魅惑之饵。</summary>
		public bool lureofEnthrallment;
		/// <summary>本帧是否装备瓶装沙元素。</summary>
		public bool wifeinaBottle;
		/// <summary>本帧是否装备稀有瓶装沙元素。</summary>
		public bool wifeinaBottlewithBoobs;
		/// <summary>硫火元素在场标记，由召唤增益维护。</summary>
		public bool brimstoneWaifu;
		/// <summary>云元素在场标记，由召唤增益维护。</summary>
		public bool cloudWaifu;
		/// <summary>塞壬在场标记，由召唤增益维护。</summary>
		public bool sirenLureWaifu;
		/// <summary>沙元素在场标记，由召唤增益维护。</summary>
		public bool sandyWaifu;
		/// <summary>治疗型沙元素在场标记，由召唤增益维护。</summary>
		public bool drewsSandyWaifu;
		/// <summary>清空元素链的全部逐帧装备与召唤状态。</summary>
		public void ResetElementalChainEffects()
		{
			aeroStone = false;
			cryoStone = false;
			chaosStone = false;
			bloomStone = false;
			roseStone = false;
			roseStoneVisible = false;
			heartoftheElements = false;
			heartoftheElementsHideVisual = false;
			allWaifus = false;
			eyeoftheStorm = false;
			lureofEnthrallment = false;
			wifeinaBottle = false;
			wifeinaBottlewithBoobs = false;
			brimstoneWaifu = false;
			cloudWaifu = false;
			sirenLureWaifu = false;
			sandyWaifu = false;
			drewsSandyWaifu = false;
		}
		/// <summary>
		/// 结算元素石、玫瑰石与元灵之心的逐帧面板，并处理照明、诅咒光环和植物生长。
		/// </summary>
		public void ApplyElementalChainEffects()
		{
			if (roseStone)
			{
				if (roseStoneVisible)
					Lighting.AddLight(Player.Center, 0.6f, 0f, 0.25f);
				Player.lifeRegen += 2;
				Player.statLifeMax2 += 20;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (aeroStone)
			{
				Lighting.AddLight(Player.Center, 0f, 0.425f, 0.425f);
				Player.moveSpeed += 0.1f;
				Player.jumpSpeedBoost += 2f;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (cryoStone)
			{
				Lighting.AddLight(Player.Center, 0f, 0.25f, 0.6f);
				Player.endurance += 0.05f;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (chaosStone)
			{
				Lighting.AddLight(Player.Center, 0.85f, 0f, 0f);
				Player.statManaMax2 += 50;
				Player.manaCost *= 0.95f;
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (bloomStone)
			{
				Lighting.AddLight(Player.Center, 0.25f, 0.4f, 0.2f);
				Player.GetDamage(DamageClass.Generic) += 0.02f;
				Player.GetCritChance(DamageClass.Generic) += 2;
				ApplyElementalCurseAura();
			}
			if (heartoftheElements)
			{
				ApplyHeartBaseStats();
				ApplyElementalCurseAura();
			}
			if ((bloomStone || heartoftheElements) && Player.whoAmI == Main.myPlayer)
				GrowElementalPlants();
		}
		/// <summary>应用元灵之心两档共用的再生、魔力效率与通用暴击。</summary>
		private void ApplyHeartBaseStats()
		{
			Player.lifeRegen += 4;
			Player.manaRegenBonus += 2;
			Player.manaCost *= 0.95f;
			Player.GetCritChance(DamageClass.Generic) += 10;
			if (!heartoftheElementsHideVisual)
			{
				Player.statLifeMax2 += 20;
				Player.statManaMax2 += 50;
				Player.moveSpeed += 0.1f;
				Player.jumpSpeedBoost += 2f;
				Player.endurance += 0.05f;
				Player.GetDamage(DamageClass.Generic) += 0.1f;
			}
			else
			{
				Player.statLifeMax2 += 25;
				Player.statManaMax2 += 60;
				Player.moveSpeed += 0.12f;
				Player.jumpSpeedBoost += 2.2f;
				Player.endurance += 0.06f;
				Player.GetDamage(DamageClass.Generic) += 0.12f;
			}
		}
		/// <summary>
		/// 每帧按十分之一概率诅咒一百五十像素内敌怪，并造成十点通用伤害。
		/// 只在拥有者客户端结算，避免多人环境重复施加与伤害。
		/// </summary>
		private void ApplyElementalCurseAura()
		{
			if (Player.whoAmI != Main.myPlayer || !Main.rand.NextBool(10))
				return;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.active || npc.friendly || npc.damage <= 0 || npc.dontTakeDamage
					|| npc.buffImmune[BuffID.BetsysCurse]
					|| Vector2.DistanceSquared(Player.Center, npc.Center) > 150f * 150f)
					continue;
				npc.AddBuff(BuffID.BetsysCurse, 120);
				npc.SimpleStrikeNPC(10, 0, false, 0f, DamageClass.Generic);
			}
		}
		/// <summary>
		/// 玩家站稳且未抓钩时，在下方的泥土、草地、神圣草地或丛林草地上生成随机植物。
		/// 只在本地玩家端修改方块，并在多人环境发送单格方块更新。
		/// </summary>
		private void GrowElementalPlants()
		{
			if (Player.velocity.Y != 0f || Player.grappling[0] != -1 || Player.dead)
				return;
			int x = (int)Player.Center.X / 16;
			int y = (int)(Player.position.Y + Player.height - 1f) / 16;
			if (!WorldGen.InWorld(x, y, 2) || Main.tile[x, y].HasTile
				|| Main.tile[x, y].LiquidAmount != 0 || !WorldGen.SolidTile(x, y + 1))
				return;
			Tile below = Main.tile[x, y + 1];
			bool changed = false;
			if (below.TileType == TileID.Dirt)
			{
				if (Main.rand.NextBool(1000))
				{
					SetElementalPlant(x, y, TileID.DyePlants
						, (short)(34 * Main.rand.Next(1, 13)));
					changed = true;
				}
			}
			else if (below.TileType == TileID.Grass)
			{
				if (Main.rand.NextBool(2))
					SetElementalPlant(x, y, TileID.Plants
						, (short)(18 * Main.rand.Next(6, 11)));
				else
					SetElementalPlant(x, y, TileID.Plants2
						, (short)(18 * Main.rand.Next(6, 21)));
				changed = true;
			}
			else if (below.TileType == TileID.HallowedGrass)
			{
				if (Main.rand.NextBool(2))
					SetElementalPlant(x, y, TileID.HallowedPlants
						, (short)(18 * Main.rand.Next(4, 7)));
				else
					SetElementalPlant(x, y, TileID.HallowedPlants2
						, (short)(18 * Main.rand.Next(2, 8)));
				changed = true;
			}
			else if (below.TileType == TileID.JungleGrass)
			{
				SetElementalPlant(x, y, TileID.JunglePlants2
					, (short)(18 * Main.rand.Next(9, 17)));
				changed = true;
			}
			if (changed && Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
		}
		/// <summary>写入一格无坡度的植物，并清除可能继承的锤击状态。</summary>
		private static void SetElementalPlant(int x, int y, ushort tileType, short frameX)
		{
			Tile tile = Main.tile[x, y];
			tile.HasTile = true;
			tile.TileType = tileType;
			tile.TileFrameX = frameX;
			tile.TileFrameY = 0;
			tile.Get<TileWallWireStateData>().Slope = 0;
			tile.Get<TileWallWireStateData>().IsHalfBlock = false;
		}
        /// <summary>同步弑神者冲刺开始的网络消息码。</summary>
        public const byte MsgGodSlayerDash = 5;
        /// <summary>同步弑神者冲刺命中的网络消息码。</summary>
        public const byte MsgGodSlayerDashHit = 6;
        /// <summary>客户端请求服务器给魔影狂怒范围内的敌怪施加状态的网络消息码。</summary>
        public const byte MsgDemonshadeEnrage = 7;
        /// <summary>冲刺起步速度。</summary>
        private const float GodSlayerDashStartSpeed = 80f;
        /// <summary>冲刺中段速度阈值。</summary>
        private const float GodSlayerDashMidSpeed = 40f;
        /// <summary>冲刺中段以上每帧速度衰减。</summary>
        private const float GodSlayerDashFastDecay = 0.985f;
        /// <summary>冲刺降到跑速后每帧速度衰减。</summary>
        private const float GodSlayerDashRunDecay = 0.8f;
        /// <summary>冲刺持续帧数。</summary>
        private const int GodSlayerDashDuration = 25;
        /// <summary>冲刺结束后的原版冲刺延迟。</summary>
        private const int GodSlayerDashEndDelay = 30;
        /// <summary>冲刺命中基础伤害。</summary>
        private const int GodSlayerDashDamage = 3000;
        /// <summary>冲刺命中基础击退。</summary>
        private const float GodSlayerDashKnockback = 15f;
        /// <summary>冲刺命中互斥无敌帧。</summary>
        private const int GodSlayerDashHitImmunity = 12;
        /// <summary>冲刺附加神谕狱火时长。</summary>
        private const int GodSlayerDashInfernoFrames = 300;
        /// <summary>冲刺龙颚光片的初始缩放。</summary>
        private const float GodSlayerDashJawsStartSize = 2.2f;
        /// <summary>弑神者冲刺起步专用音效。</summary>
        private static readonly SoundStyle GodSlayerDashStartSound =
            new("CalamityRebirth/Sounds/Custom/DevourerDeath");
        /// <summary>弑神者冲刺命中专用音效。</summary>
        private static readonly SoundStyle GodSlayerDashImpactSound =
            new("CalamityRebirth/Sounds/Custom/DevourerDeathImpact");
        /// <summary>魔影冲刺主火光色。</summary>
        private static readonly Color DemonshadeDashFlameColor = new(225, 30, 34);
        /// <summary>魔影冲刺暗红内焰色。</summary>
        private static readonly Color DemonshadeDashDarkColor = new(92, 0, 8);
        /// <summary>魔影冲刺高亮橙红色。</summary>
        private static readonly Color DemonshadeDashCoreColor = new(255, 104, 72);
        /// <summary>本帧是否穿着任意弑神者三件套。</summary>
        public bool godSlayer;
        /// <summary>本帧是否处于弑神保命冷却。</summary>
        public bool godSlayerCooldown;
        /// <summary>本帧是否穿着弑神者近战头。</summary>
        public bool godSlayerMelee;
        /// <summary>本帧是否穿着弑神者射手脚盔。</summary>
        public bool godSlayerRanged;
        /// <summary>本帧是否穿着弑神者法师面甲。</summary>
        public bool godSlayerMage;
        /// <summary>本帧是否穿着弑神者召唤角盔。</summary>
        public bool godSlayerSummon;
        /// <summary>本帧是否穿着弑神者盗贼面具。</summary>
        public bool godSlayerThrowing;
        /// <summary>本帧是否获得胸甲概率完全闪避。</summary>
        public bool godSlayerReflect;
        /// <summary>本帧是否启用低伤压制。</summary>
        public bool godSlayerDamage;
        /// <summary>本帧是否启用动态低伤保护。</summary>
        public bool godSlayerDamageProtect;
        /// <summary>动态低伤保护上限，最高 80，触发后重置为 20。</summary>
        public int godSlayerDamageProtectMax = 80;
        /// <summary>近战命中触发弑神飞镖的剩余冷却。</summary>
        private int godSlayerMeleeFireCooldown;
        /// <summary>法师命中触发弑神光球的剩余冷却。</summary>
        private int godSlayerMageFireCooldown;
        /// <summary>远程武器追加破片弹的剩余冷却。</summary>
        private int godSlayerShrapnelCooldown;
        /// <summary>召唤幻影节流预算，每帧衰减 2.5。</summary>
        private float godSlayerPhantomBudget;
        /// <summary>机械蠕虫是否在场。</summary>
        public bool mWorm;
        /// <summary>本帧是否请求冲刺。</summary>
        private bool godSlayerDashQueued;
        /// <summary>本机冲刺已持续帧数。</summary>
        private int godSlayerDashElapsed;
        /// <summary>本机冲刺视觉相位。</summary>
        private int godSlayerDashTime;
        /// <summary>本机龙颚光片当前缩放。</summary>
        private float godSlayerDashJawsSize;
        /// <summary>本机冲刺起步音效句柄，用于让声源跟随玩家。</summary>
        private SlotId godSlayerDashSoundSlot;
        /// <summary>本机冲刺是否已播放命中音效。</summary>
        private bool godSlayerDashHitSounded;
        /// <summary>远端冲刺已持续帧数。</summary>
        private int remoteGodSlayerDashElapsed;
        /// <summary>远端冲刺视觉相位。</summary>
        private int remoteGodSlayerDashTime;
        /// <summary>远端龙颚光片当前缩放。</summary>
        private float remoteGodSlayerDashJawsSize;
        /// <summary>远端冲刺起步音效句柄。</summary>
        private SlotId remoteGodSlayerDashSoundSlot;
        /// <summary>远端冲刺是否已播放命中音效。</summary>
        private bool remoteGodSlayerDashHitSounded;
        /// <summary>清空弑神者每帧装备与套装标记。</summary>
        private void ResetGodSlayerEffects()
        {
            godSlayer = false;
            godSlayerCooldown = false;
            godSlayerMelee = false;
            godSlayerRanged = false;
            godSlayerMage = false;
            godSlayerSummon = false;
            godSlayerThrowing = false;
            godSlayerReflect = false;
            godSlayerDamage = false;
            godSlayerDamageProtect = false;
            mWorm = false;
        }
        /// <summary>死亡时重置冲刺、命中冷却、保护上限和幻影预算。</summary>
        private void ResetGodSlayerStateOnDeath()
        {
            godSlayerDamageProtectMax = 80;
            godSlayerMeleeFireCooldown = 0;
            godSlayerMageFireCooldown = 0;
            godSlayerShrapnelCooldown = 0;
            godSlayerPhantomBudget = 0f;
            godSlayerDashQueued = false;
            godSlayerDashElapsed = 0;
            godSlayerDashTime = 0;
            godSlayerDashJawsSize = 0f;
            godSlayerDashHitSounded = false;
            remoteGodSlayerDashElapsed = 0;
            remoteGodSlayerDashTime = 0;
            remoteGodSlayerDashJawsSize = 0f;
            remoteGodSlayerDashHitSounded = false;
            Player.dashDelay = 0;
        }
        /// <summary>
        /// 结算弑神者攻击冷却、保护上限恢复、冷却增伤、盗贼满血属性和机械蠕虫维护。
        /// </summary>
        private void ApplyGodSlayerEffects()
        {
            godSlayerPhantomBudget = Math.Max(0f, godSlayerPhantomBudget - 2.5f);
            if (godSlayerThrowing && Player.statLife >= Player.statLifeMax2)
            {
                CRUtil.ApplyRogueStats(Player, 0.10f, 10, 0f, 0f, 0f, 0f);
                CRUtil.AddRogueVelocity(Player, 0.10f);
            }
            if (godSlayerSummon)
                UpdateGodSlayerMechworm();
            if (godSlayerDamageProtect && godSlayerDamageProtectMax < 80)
                godSlayerDamageProtectMax++;
            if (godSlayerCooldown)
                Player.GetDamage(DamageClass.Generic) += 0.10f;
            if (godSlayerMeleeFireCooldown > 0)
                godSlayerMeleeFireCooldown--;
            if (godSlayerMageFireCooldown > 0)
                godSlayerMageFireCooldown--;
            if (godSlayerShrapnelCooldown > 0)
                godSlayerShrapnelCooldown--;
        }
        /// <summary>触发弑神保命：回复 100 生命并附加 45 秒冷却。</summary>
        private bool TryActivateGodSlayerRevival()
        {
            if (!godSlayer || godSlayerCooldown)
                return false;
            SoundEngine.PlaySound(SoundID.Item67, Player.position);
            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height
                    , DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
                Main.dust[dust].position += new Vector2(Main.rand.Next(-20, 21)
                    , Main.rand.Next(-20, 21));
                Main.dust[dust].velocity *= 0.9f;
                Main.dust[dust].scale *= 1f + Main.rand.Next(40) * 0.01f;
                Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
            }
            Player.statLife = Math.Min(Player.statLifeMax2, Player.statLife + 100);
            Player.HealEffect(100);
            Player.AddBuff(ModContent.BuffType<GodSlayerCooldown>(), 2700);
            return true;
        }
        /// <summary>胸甲动态低伤保护和 2% 完全闪避。</summary>
        private bool TryGodSlayerFreeDodge(Player.HurtInfo info)
        {
            if (godSlayerDamageProtect && info.Damage <= godSlayerDamageProtectMax)
            {
                godSlayerDamageProtectMax = 20;
                Player.immune = true;
                Player.immuneTime = 15;
                return true;
            }
            if (!godSlayerReflect || !Main.rand.NextBool(50))
                return false;
            Player.immune = true;
            Player.immuneTime = 15;
            return true;
        }
        /// <summary>将不超过 80 点的基础伤害压制为 1。</summary>
        private void ApplyGodSlayerLowDamage(ref Player.HurtModifiers modifiers)
        {
            if ((godSlayerDamage && modifiers.SourceDamage.Base <= 80f)
                || modifiers.SourceDamage.Base < 1f)
                modifiers.SourceDamage.Base = 1f;
        }
        /// <summary>法师套装受击释放 1200 伤害弑神烈焰，盗贼套装高伤追加 30 帧无敌。</summary>
        private void ApplyGodSlayerPostHurt(Player.HurtInfo info)
        {
            if (godSlayerMelee && info.Damage > 80)
            {
                SoundEngine.PlaySound(SoundID.Item73, Player.Center);
                if (Player.whoAmI == Main.myPlayer)
                    SpawnGodSlayerCounterDarts();
            }
            if (godSlayerMage && info.Damage > 0)
            {
                SoundEngine.PlaySound(SoundID.Item74, Player.position);
                if (Player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center
                        , Vector2.Zero, ModContent.ProjectileType<GodSlayerBlaze>(), 1200, 1f
                        , Player.whoAmI);
                }
            }
            if (godSlayerThrowing && Player.whoAmI == Main.myPlayer && info.Damage > 80)
                Player.immuneTime += 30;
        }
        /// <summary>受到 80 点以上伤害时向四周释放四对高伤害弑神飞镖。</summary>
        private void SpawnGodSlayerCounterDarts()
        {
            float spread = MathHelper.ToRadians(45f);
            double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2f;
            double deltaAngle = spread / 8f;
            int damage = (int)Player.GetDamage(DamageClass.Melee).ApplyTo(1500);
            for (int i = 0; i < 4; i++)
            {
                double angle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                Vector2 velocity = new((float)Math.Sin(angle) * 10f
                    , (float)Math.Cos(angle) * 10f);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, velocity
                    , ModContent.ProjectileType<GodSlayerDart>(), damage, 5f, Player.whoAmI, 1f, 0f);
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, -velocity
                    , ModContent.ProjectileType<GodSlayerDart>(), damage, 5f, Player.whoAmI, 1f, 0f);
            }
        }
        /// <summary>近战物品命中时每 60 帧生成一枚挂载弑神飞镖。</summary>
        private void ApplyGodSlayerItemHit(Item item, NPC.HitInfo hit)
        {
            if (!godSlayerMelee || godSlayerMeleeFireCooldown > 0
                || Player.whoAmI != Main.myPlayer
                || (!hit.DamageType.CountsAsClass(DamageClass.Melee)
                    && !hit.DamageType.CountsAsClass(DamageClass.MeleeNoSpeed)))
                return;
            SpawnGodSlayerMountedDart();
        }
        /// <summary>近战弹幕、法师、召唤和通用召唤物命中效果统一入口。</summary>
        private void ApplyGodSlayerProjectileHit(Projectile projectile, NPC target, NPC.HitInfo hit)
        {
            if (projectile.owner != Player.whoAmI || Player.whoAmI != Main.myPlayer)
                return;
            if (godSlayerMelee && godSlayerMeleeFireCooldown <= 0
                && (projectile.CountsAsClass(DamageClass.Melee)
                    || projectile.CountsAsClass(DamageClass.MeleeNoSpeed)))
                SpawnGodSlayerMountedDart();
            if (godSlayerMage && godSlayerMageFireCooldown <= 0
                && projectile.CountsAsClass(DamageClass.Magic))
                TrySpawnGodSlayerMageOrb(projectile, target);
            if (godSlayerSummon && (projectile.minion || projectile.sentry))
                TrySpawnGodSlayerPhantom(projectile);
        }
        /// <summary>生成八方向挂载飞镖发射器，并开启 60 帧冷却。</summary>
        private void SpawnGodSlayerMountedDart()
        {
            int damage = 500 + Player.HeldItem.damage / 2;
            Vector2 velocity = new Vector2(1f, 1f).SafeNormalize(Vector2.UnitY) * 4f;
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, velocity
                , ModContent.ProjectileType<GodSlayerDart>(), damage, 0f, Player.whoAmI);
            godSlayerMeleeFireCooldown = 60;
        }
        /// <summary>生成随机方向弑神光球，并在吸血预算可用时生成固定治疗叶球。</summary>
        private void TrySpawnGodSlayerMageOrb(Projectile source, NPC target)
        {
            godSlayerMageFireCooldown = 2;
            int damage = (400 + Player.HeldItem.damage / 2) * 5;
            Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(12f, 16f);
            Projectile.NewProjectile(source.GetSource_FromThis(), source.Center, velocity
                , ModContent.ProjectileType<GodSlayerOrb>(), damage, source.knockBack
                , Player.whoAmI);
            if (!target.canGhostHeal || Player.lifeSteal <= 0f)
                return;
            int heal = Main.rand.Next(5, 11);
            Player.lifeSteal -= heal * 2f;
            int healTarget = FindLargestMissingLifeTarget(source, 3000f);
            Projectile.NewProjectile(source.GetSource_FromThis(), source.Center, Vector2.Zero
                , ModContent.ProjectileType<GodSlayerHealOrb>(), 0, 0f, Player.whoAmI
                , healTarget, heal);
        }
        /// <summary>节流预算归零且有近敌时生成弑神幻影。</summary>
        private void TrySpawnGodSlayerPhantom(Projectile source)
        {
            if (godSlayerPhantomBudget > 0f)
                return;
            bool nearby = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(source))
                    continue;
                float distance = Math.Abs(npc.Center.X - source.Center.X)
                    + Math.Abs(npc.Center.Y - source.Center.Y);
                if (distance < 800f)
                {
                    nearby = true;
                    break;
                }
            }
            if (!nearby)
                return;
            int damage = source.damage / 2;
            godSlayerPhantomBudget += damage;
            Vector2 velocity = Main.rand.NextVector2Unit() * 15f;
            Projectile.NewProjectile(source.GetSource_FromThis(), source.Center, velocity
                , ModContent.ProjectileType<GodSlayerPhantom>(), damage * 2, 0f
                , Player.whoAmI, 0f, Main.rand.NextFloat() + 0.5f);
        }
        /// <summary>远程射击每 2.5 秒追加一枚破片弹。</summary>
        internal void TryShootGodSlayerRound(Item item, Vector2 position, Vector2 velocity, int damage)
        {
            if (!godSlayerRanged || godSlayerShrapnelCooldown > 0
                || !item.CountsAsClass(DamageClass.Ranged) || item.channel
                || Player.whoAmI != Main.myPlayer)
                return;
            godSlayerShrapnelCooldown = 150;
            int roundDamage = DamageSoftCap(damage * 2f, 1500);
            Projectile.NewProjectile(Player.GetSource_FromThis(), position, velocity * 1.25f
                , ModContent.ProjectileType<GodSlayerShrapnelRound>(), roundDamage, 2f
                , Player.whoAmI);
        }
        /// <summary>远程暴击触发再次暴击，并把本次暴击倍率提高到四倍。</summary>
        private void ApplyGodSlayerRangedCrit(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!godSlayerRanged || !projectile.CountsAsClass(DamageClass.Ranged))
                return;
            int excessCrit = (int)Player.GetTotalCritChance(DamageClass.Ranged) - 100;
            bool againCrit = excessCrit > 1
                ? Main.rand.Next(1, 101) <= excessCrit
                : Main.rand.NextBool(20);
            if (againCrit)
                modifiers.CritDamage *= 2f;
        }
        /// <summary>寻找指定范围内生命缺口最大的友方玩家。</summary>
        private int FindLargestMissingLifeTarget(Projectile source, float range)
        {
            int targetIndex = Player.whoAmI;
            int missingLife = 0;
            foreach (Player candidate in Main.player)
            {
                if (!candidate.active || candidate.dead)
                    continue;
                bool friendly = (!Player.hostile && !candidate.hostile)
                    || Player.team == candidate.team;
                if (!friendly)
                    continue;
                float distance = Math.Abs(candidate.Center.X - source.Center.X)
                    + Math.Abs(candidate.Center.Y - source.Center.Y);
                int missing = candidate.statLifeMax2 - candidate.statLife;
                if (distance >= range || missing <= missingLife)
                    continue;
                missingLife = missing;
                targetIndex = candidate.whoAmI;
            }
            return targetIndex;
        }
        /// <summary>维护只生成一次的噬神机械蠕虫四节链。</summary>
        private void UpdateGodSlayerMechworm()
        {
            if (Player.whoAmI != Main.myPlayer)
                return;
            int headType = ModContent.ProjectileType<MechwormHead>();
            if (Player.ownedProjectileCounts[headType] > 0)
                return;
            int buffType = ModContent.BuffType<Content.ModBuffs.Buffs.Mechworm>();
            if (Player.FindBuffIndex(buffType) < 0)
                Player.AddBuff(buffType, 3600, true);
            int bodyType = ModContent.ProjectileType<MechwormBody>();
            int bodyType2 = ModContent.ProjectileType<MechwormBody2>();
            int tailType = ModContent.ProjectileType<MechwormTail>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || projectile.owner != Player.whoAmI)
                    continue;
                if (projectile.type == headType || projectile.type == bodyType
                    || projectile.type == bodyType2 || projectile.type == tailType)
                    projectile.Kill();
            }
            int maxMinionScale = Math.Min(Player.maxMinions, 10);
            StatModifier summonStat = Player.GetTotalDamage(DamageClass.Summon);
            float summonDamage = summonStat.Additive + summonStat.Multiplicative;
            int damage = (int)(35f * (summonDamage * 5f / 3f
                + summonDamage * 0.46f * (maxMinionScale - 1)));
            Vector2 spawn = Main.MouseWorld;
            Vector2 fromPlayer = spawn - Player.RotatedRelativePoint(Player.MountedCenter, true);
            Player.ChangeDir(fromPlayer.X >= 0f ? 1 : -1);
            int head = Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero
                , headType, damage, 1f, Player.whoAmI);
            int body = Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero
                , bodyType, damage, 1f, Player.whoAmI, head);
            int body2 = Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero
                , bodyType2, damage, 1f, Player.whoAmI, body);
            if (Main.projectile.IndexInRange(body))
            {
                Main.projectile[body].localAI[1] = body2;
                Main.projectile[body].netUpdate = true;
            }
            int tail = Projectile.NewProjectile(Player.GetSource_FromThis(), spawn, Vector2.Zero
                , tailType, damage, 1f, Player.whoAmI, body2);
            if (Main.projectile.IndexInRange(body2))
            {
                Main.projectile[body2].localAI[1] = tail;
                Main.projectile[body2].netUpdate = true;
            }
        }
        /// <summary>按下快捷键时记录弑神者冲刺请求。</summary>
        public void RequestGodSlayerDash()
        {
            if (!Player.active || Player.dead || Player.CCed
                || (!godSlayer && !demonshadeSetBonus) || Player.whoAmI != Main.myPlayer
                || Player.HasBuff(ModContent.BuffType<GodSlayerDashCooldown>())
                || Player.HasBuff(ModContent.BuffType<DemonshadeDashCooldown>())
                || Player.pulley || Player.grappling[0] != -1 || Player.tongued
                || Player.mount.Active || Player.dashDelay != 0 || shieldSlamDashElapsed > 0)
                return;
            if (!Player.controlUp && !Player.controlDown
                && !Player.controlLeft && !Player.controlRight)
                return;
            godSlayerDashQueued = true;
        }
        /// <summary>冲刺状态机：起步、命中、视觉和速度衰减。</summary>
        public void GodSlayerDashMovement()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                GodSlayerRemoteVisuals();
                return;
            }
            if (godSlayerDashQueued)
            {
                godSlayerDashQueued = false;
                if (shieldSlamDashElapsed <= 0)
                    StartGodSlayerDash();
            }
            if (godSlayerDashElapsed <= 0)
                return;
            if (!Player.active || Player.dead)
            {
                EndGodSlayerDash(false);
                return;
            }
            if (SoundEngine.TryGetActiveSound(godSlayerDashSoundSlot, out var dashSound)
                && dashSound.IsPlaying)
                dashSound.Position = Player.Center;
            BlockVanillaDash();
            Player.maxFallSpeed = 50f;
            GodSlayerDashHits();
            GodSlayerDashVisuals(Player.Center, Player.velocity, ref godSlayerDashTime
                , ref godSlayerDashJawsSize);
            godSlayerDashElapsed++;
            if (godSlayerDashElapsed > GodSlayerDashDuration)
                EndGodSlayerDash(true);
        }
        /// <summary>按八方向输入启动冲刺并广播视觉。</summary>
        private void StartGodSlayerDash()
        {
            Vector2 direction = Vector2.Zero;
            if (Player.controlUp)
                direction.Y--;
            if (Player.controlDown)
                direction.Y++;
            if (Player.controlLeft)
                direction.X--;
            if (Player.controlRight)
                direction.X++;
            direction = direction.SafeNormalize(new Vector2(Player.direction, 0f));
            Player.velocity = direction * GodSlayerDashStartSpeed;
            Player.dashDelay = -1;
            godSlayerDashElapsed = 1;
            godSlayerDashTime = 0;
            godSlayerDashJawsSize = GodSlayerDashJawsStartSize;
            godSlayerDashHitSounded = false;
            if (demonshadeSetBonus)
                Player.AddBuff(ModContent.BuffType<DemonshadeDashCooldown>(), 15 * 60);
            else
                Player.AddBuff(ModContent.BuffType<GodSlayerDashCooldown>(), 45 * 60);
            godSlayerDashSoundSlot = SoundEngine.PlaySound(GodSlayerDashStartSound, Player.Center);
            GodSlayerDashStartVisuals();
            SendGodSlayerDashPacket(false);
            float horizontalSign = direction.X == 0f ? 1f : MathF.Sign(direction.X);
            Vector2 horizontalProbeOffset = new(horizontalSign * (Player.width / 2f + 2f), 0f);
            Vector2 upperProbeOffset = horizontalProbeOffset + new Vector2(0f
                , -Player.gravDir * (Player.height / 2f + 2f));
            Point upwardTile = (Player.Center + upperProbeOffset).ToTileCoordinates();
            Point aheadTile = (Player.Center + horizontalProbeOffset).ToTileCoordinates();
            if (WorldGen.SolidOrSlopedTile(upwardTile.X, upwardTile.Y)
                || WorldGen.SolidOrSlopedTile(aheadTile.X, aheadTile.Y))
                Player.velocity.X /= 2f;
        }
        /// <summary>
        /// 统一收尾冲刺状态；正常结束时保留短时间的方向输入锁，异常中断则立即恢复原版冲刺闸门。
        /// </summary>
        private void EndGodSlayerDash(bool applyEndDelay)
        {
            if (applyEndDelay)
            {
                Player.dashDelay = GodSlayerDashEndDelay;
                Player.velocity *= 0.2f;
            }
            else
            {
                Player.dashDelay = 0;
            }
            godSlayerDashElapsed = 0;
            godSlayerDashTime = 0;
            godSlayerDashJawsSize = 0f;
            godSlayerDashHitSounded = false;
        }
        /// <summary>以玩家前后扩展矩形命中敌人，并施加神谕狱火。</summary>
        private void GodSlayerDashHits()
        {
            Rectangle hitbox = new((int)(Player.position.X + Player.velocity.X * 0.5f - 4f)
                , (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4f)
                , Player.width + 8, Player.height + 8);
            int direction = Player.velocity.X == 0f ? Player.direction : Math.Sign(Player.velocity.X);
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if ((Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[npc.type])
                    || npc.dontTakeDamage || npc.friendly || npc.immune[Player.whoAmI] > 0
                    || !hitbox.Intersects(npc.getRect()) || !(npc.noTileCollide || Player.CanHit(npc)))
                    continue;
                int damage = (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(GodSlayerDashDamage);
                float knockback = Player.GetTotalKnockback(DamageClass.Generic)
                    .ApplyTo(GodSlayerDashKnockback);
                bool crit = Main.rand.Next(100) < Player.GetTotalCritChance(DamageClass.Generic);
                Player.ApplyDamageToNPC(npc, damage, knockback, direction, crit, DamageClass.Generic);
                npc.immune[Player.whoAmI] = Math.Max(npc.immune[Player.whoAmI], GodSlayerDashHitImmunity);
                Player.immuneTime = Math.Max(Player.immuneTime, GodSlayerDashHitImmunity);
                ApplyDashInferno(npc);
                if (!godSlayerDashHitSounded)
                {
                    godSlayerDashHitSounded = true;
                    SoundEngine.PlaySound(GodSlayerDashImpactSound, Player.Center);
                }
                GodSlayerDashHitVisuals();
                SendGodSlayerDashPacket(true);
            }
        }
        /// <summary>按三版本弱依赖施加冲刺神谕狱火。</summary>
        private static void ApplyDashInferno(NPC npc)
        {
            CalamityCompatUtil.TryAddBuff(npc, "CalamityMod", "GodSlayerInferno"
                , GodSlayerDashInfernoFrames);
            CalamityCompatUtil.TryAddBuff(npc, "CalamityModClassicPreTrailer"
                , "GodSlayerInferno", GodSlayerDashInfernoFrames);
            CalamityCompatUtil.TryAddBuff(npc, "CalRD", "GodSlayerInferno"
                , GodSlayerDashInfernoFrames);
        }
        /// <summary>生成冲刺起步的紫黑尘环。</summary>
        private void GodSlayerDashStartVisuals()
        {
            Color pulseColor = demonshadeSetBonus ? DemonshadeDashFlameColor : Color.Orchid;
            GodSlayerDashParticleSystem.SpawnDirectionalPulse(Player.Center, Vector2.Zero, pulseColor
                , new Vector2(2f), Main.rand.NextFloat(12f, 25f), 0.1f, 12f, 18);
            for (int i = 0; i < 16; i++)
            {
                int dustType = demonshadeSetBonus ? DustID.LifeDrain : DustID.GiantCursedSkullBolt;
                Dust dust = Dust.NewDustPerfect(Player.position, dustType
                    , -Player.velocity.RotatedByRandom(MathHelper.ToRadians(35f))
                    * Main.rand.NextFloat(0.3f, 0.9f), 0
                    , demonshadeSetBonus ? DemonshadeDashDarkColor : default
                    , Main.rand.NextFloat(3.1f, 3.9f));
                dust.noGravity = false;
            }
        }
        /// <summary>生成龙颚、尘环、随体尾迹、双色火花和中段定向脉冲。</summary>
        private void GodSlayerDashVisuals(Vector2 center, Vector2 velocity, ref int time, ref float size)
        {
            Color primaryColor = demonshadeSetBonus ? DemonshadeDashFlameColor : Color.Fuchsia;
            Color secondaryColor = demonshadeSetBonus ? DemonshadeDashCoreColor : Color.Aqua;
            Color darkColor = demonshadeSetBonus ? DemonshadeDashDarkColor : Color.Fuchsia;
            int outerDust = demonshadeSetBonus ? DustID.LifeDrain : DustID.GiantCursedSkullBolt;
            int trailDust = demonshadeSetBonus ? DustID.CrimsonTorch : DustID.CorruptTorch;
            time++;
            size -= 0.04f;
            if (time < 20)
            {
                GodSlayerDashParticleSystem.SpawnJaws(center + velocity * 0.5f, velocity, primaryColor
                    , new Vector2(0.8f, 1f), velocity.ToRotation() + MathHelper.PiOver2
                    , size, size, 2);
                GodSlayerDashParticleSystem.SpawnJaws(center + velocity * 0.45f, velocity, secondaryColor
                    , new Vector2(0.8f, 1f), velocity.ToRotation() + MathHelper.PiOver2
                    , size - 0.3f, size - 0.3f, 2);
            }
            float radius = MathHelper.Lerp(0f, 1f
                , Utils.GetLerpValue(2f, 2.5f, time, true));
            for (int i = 0; i < 9; i++)
            {
                float angle = velocity.ToRotation() + time / 20f;
                float ringRadius = (30f + MathF.Cos(time / 3f) * 24f) * radius;
                Vector2 position = center + velocity * 0.8f
                    + angle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * ringRadius;
                Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(5)
                    ? outerDust : trailDust, default, 0, demonshadeSetBonus
                    ? DemonshadeDashFlameColor : default);
                dust.noGravity = true;
                dust.velocity = velocity * 0.5f;
                dust.scale = Main.rand.NextFloat(2.7f, 3f);
                Dust trail = Dust.NewDustPerfect(center
                    + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                    + velocity * 0.5f
                    , Main.rand.NextBool(14) ? trailDust : outerDust
                    , -velocity.RotatedByRandom(MathHelper.ToRadians(30f))
                        * Main.rand.NextFloat(0.1f, 0.8f), 0, demonshadeSetBonus
                        ? DemonshadeDashDarkColor : default
                    , Main.rand.NextFloat(2.7f, 3.9f));
                trail.noGravity = true;
            }
            float sparkScale = size * 1.3f;
            Vector2 firstSparkVelocity = velocity.RotatedBy(Player.direction * -4f) * 0.08f
                - velocity / 2f;
            GodSlayerDashParticleSystem.SpawnGodSlayerDashSpark(center + velocity.RotatedBy(
                2f * Player.direction) * 1.2f, firstSparkVelocity
                , Main.rand.NextBool(3) ? secondaryColor : primaryColor
                , Main.rand.Next(11, 13), sparkScale);
            Vector2 secondSparkVelocity = velocity.RotatedBy(Player.direction * 4f) * 0.08f
                - velocity / 2f;
            GodSlayerDashParticleSystem.SpawnGodSlayerDashSpark(center + velocity.RotatedBy(
                -2f * Player.direction) * 1.2f, secondSparkVelocity
                , Main.rand.NextBool(3) ? secondaryColor : primaryColor
                , Main.rand.Next(11, 13), sparkScale);
            if (time > 20 && time < 100)
            {
                GodSlayerDashParticleSystem.SpawnDirectionalPulse(center - velocity * 0.52f
                    , velocity / 1.5f, darkColor, new Vector2(1f, 2f), velocity.ToRotation()
                    , 0.82f, 0.32f, 60);
                GodSlayerDashParticleSystem.SpawnDirectionalPulse(center - velocity * 0.4f
                    , velocity / 1.5f * 0.9f, secondaryColor, new Vector2(0.8f, 1.5f)
                    , velocity.ToRotation(), 0.58f, 0.28f, 50);
                time = 111;
            }
        }
        /// <summary>命中时喷出 26 枚电光尘。</summary>
        private void GodSlayerDashHitVisuals()
        {
            int firstDust = demonshadeSetBonus ? DustID.LifeDrain : DustID.Electric;
            int secondDust = demonshadeSetBonus ? DustID.CrimsonTorch : DustID.WitherLightning;
            for (int i = 0; i < 26; i++)
            {
                Dust dust = Dust.NewDustPerfect(Player.position, Main.rand.NextBool(3)
                    ? firstDust : secondDust
                    , Player.velocity.RotatedByRandom(MathHelper.ToRadians(15f))
                    * Main.rand.NextFloat(0.1f, 0.5f), 0
                    , demonshadeSetBonus ? DemonshadeDashFlameColor : default
                    , Main.rand.NextFloat(2.1f, 2.9f));
                dust.noGravity = false;
            }
        }
        /// <summary>远端玩家仅播放冲刺尘效，不模拟物理。</summary>
        private void GodSlayerRemoteVisuals()
        {
            if (Main.dedServ || remoteGodSlayerDashElapsed <= 0)
                return;
            if (!Player.active || Player.dead)
            {
                remoteGodSlayerDashElapsed = 0;
                return;
            }
            if (SoundEngine.TryGetActiveSound(remoteGodSlayerDashSoundSlot, out var dashSound)
                && dashSound.IsPlaying)
                dashSound.Position = Player.Center;
            GodSlayerDashVisuals(Player.Center, Player.velocity, ref remoteGodSlayerDashTime
                , ref remoteGodSlayerDashJawsSize);
            if (++remoteGodSlayerDashElapsed > GodSlayerDashDuration)
                remoteGodSlayerDashElapsed = 0;
        }
        /// <summary>广播本机冲刺开始或命中表现。</summary>
        private void SendGodSlayerDashPacket(bool hit)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write(hit ? MsgGodSlayerDashHit : MsgGodSlayerDash);
            packet.Write((byte)Player.whoAmI);
            if (Main.netMode == NetmodeID.Server)
                packet.Send(-1, Player.whoAmI);
            else
                packet.Send();
        }
        /// <summary>处理本机或转发的冲刺开始消息。</summary>
        internal static void HandleGodSlayerDashPacket(BinaryReader reader, int whoAmI)
        {
            int playerIndex = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = ModContent.GetInstance<global::CalamityRebirth.CalamityRebirth>().GetPacket();
                packet.Write(MsgGodSlayerDash);
                packet.Write((byte)whoAmI);
                packet.Send(-1, whoAmI);
                return;
            }
            if (!Main.player.IndexInRange(playerIndex))
                return;
            CalamityRebirthPlayer modPlayer = Main.player[playerIndex]
                .GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.remoteGodSlayerDashElapsed = 1;
            modPlayer.remoteGodSlayerDashTime = 0;
            modPlayer.remoteGodSlayerDashJawsSize = GodSlayerDashJawsStartSize;
            modPlayer.remoteGodSlayerDashHitSounded = false;
            modPlayer.remoteGodSlayerDashSoundSlot = SoundEngine.PlaySound(
                GodSlayerDashStartSound, Main.player[playerIndex].Center);
            modPlayer.GodSlayerDashStartVisuals();
        }
        /// <summary>处理本机或转发的冲刺命中消息。</summary>
        internal static void HandleGodSlayerDashHitPacket(BinaryReader reader, int whoAmI)
        {
            int playerIndex = reader.ReadByte();
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = ModContent.GetInstance<global::CalamityRebirth.CalamityRebirth>().GetPacket();
                packet.Write(MsgGodSlayerDashHit);
                packet.Write((byte)whoAmI);
                packet.Send(-1, whoAmI);
                return;
            }
            if (!Main.player.IndexInRange(playerIndex))
                return;
            CalamityRebirthPlayer modPlayer = Main.player[playerIndex]
                .GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.GodSlayerDashHitVisuals();
            if (modPlayer.remoteGodSlayerDashHitSounded)
                return;
            modPlayer.remoteGodSlayerDashHitSounded = true;
            SoundEngine.PlaySound(GodSlayerDashImpactSound, Main.player[playerIndex].Center);
        }
		/// <summary>本帧是否装备星云核心。</summary>
		public bool nebulousCore;
		/// <summary>本帧是否显示星云核心外观；隐藏时停止生成并清除已有星星。</summary>
		public bool nebulousCoreVisible;
		/// <summary>清空星云核心的逐帧装备状态。</summary>
		public void ResetNebulousCoreEffects()
		{
			nebulousCore = false;
			nebulousCoreVisible = false;
		}
		/// <summary>结算星云核心的通用伤害与通用暴击。</summary>
		public void ApplyNebulousCoreEffects()
		{
			if (!nebulousCore)
				return;
			Player.GetDamage(DamageClass.Generic) += NebulousCore.DamageBonus;
			Player.GetCritChance(DamageClass.Generic) += NebulousCore.CritBonus;
		}
		/// <summary>
		/// 致命伤前进行十分之一概率判定，成功时播放紫黑光束粒子并回复一百点生命。
		/// 返回 false 取消本次死亡，且该效果没有额外冷却。
		/// </summary>
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound
			, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (!nebulousCore
				|| !Main.rand.NextBool(NebulousCore.ReviveChanceDenominator))
			{
				if (TryActivateGodSlayerRevival())
					return false;
				return !TryActivateSilvaRevival();
			}
			SoundEngine.PlaySound(SoundID.Item67, Player.position);
			for (int i = 0; i < 25; i++)
			{
				int dustIndex = Dust.NewDust(Player.position, Player.width, Player.height
					, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
				Dust dust = Main.dust[dustIndex];
				dust.position.X += Main.rand.Next(-20, 21);
				dust.position.Y += Main.rand.Next(-20, 21);
				dust.velocity *= 0.9f;
				dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
				dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
				if (Main.rand.NextBool(2))
					dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
			}
			Player.statLife += NebulousCore.ReviveHeal;
			Player.HealEffect(NebulousCore.ReviveHeal);
			if (Player.statLife > Player.statLifeMax2)
				Player.statLife = Player.statLifeMax2;
			return false;
		}
        /// <summary>冲刺速度高于过程上限时的每帧衰减。</summary>
        private const float ShieldSpeedDeceleration = 0.985f;
        /// <summary>冲刺速度落入跑步区间后的每帧衰减。</summary>
        private const float ShieldRunSpeedDeceleration = 0.94f;
        /// <summary>基础冲刺的过程速度上限。</summary>
        private const float DefaultMidDashSpeed = 12f;
        /// <summary>冲刺结束后的冷却帧数。</summary>
        private const int ShieldSlamCooldown = 30;
        /// <summary>方向键双击判定窗口。</summary>
        private const int DoubleTapWindow = 15;
        /// <summary>冲刺命中后玩家获得的碰撞免疫帧数。</summary>
        private const int ShieldHitImmunityFrames = 12;
        /// <summary>冲刺命中后敌人至少获得的冲刺免疫帧数。</summary>
        private const int MinShieldNPCImmunityFrames = 12;
        /// <summary>远端冲刺表现的最大兜底帧数。</summary>
        private const int RemoteDashMaxFrames = 120;
        /// <summary>本帧是否装备阿斯加德之英勇。</summary>
        public bool asgardsValor;
        /// <summary>本帧是否装备阿斯加德之庇护。</summary>
        public bool asgardianAegis;
        /// <summary>本帧是否装备华丽盾。</summary>
        public bool ornateShield;
        /// <summary>本帧是否装备海之盾。</summary>
        public bool shieldOfTheOcean;
        /// <summary>本帧是否装备极乐之庇护。</summary>
        public bool elysianAegis;
        /// <summary>本帧是否允许切换守护状态。</summary>
        public bool elysianAegisPower;
        /// <summary>守护状态是否开启。</summary>
        public bool elysianGuard;
        /// <summary>当前护盾充能，五为充满、零为耗尽。</summary>
        public float shieldInvinc = 5f;
        /// <summary>本帧是否装备阿巴顿。</summary>
        public bool abaddon;
        /// <summary>阿巴顿暴击爆炸的剩余冷却帧数。</summary>
        private int abaddonCritCooldown;
        /// <summary>当前生效的盾牌冲刺类型。</summary>
        public ShieldSlamDash shieldSlamDash;
        /// <summary>本地玩家累计的屏幕震动强度。</summary>
        public float GeneralScreenShakePower;
        /// <summary>本地冲刺已经持续的帧数。</summary>
        private int shieldSlamDashElapsed;
        /// <summary>本地冲刺剩余冷却帧数。</summary>
        private int shieldSlamDashCooldown;
        /// <summary>左右方向键的双击窗口，正数表示右键、负数表示左键。</summary>
        private int shieldSlamDashTimeMod;
        /// <summary>本次冲刺是否已经触发首次震屏。</summary>
        private bool shieldSlamDashScreenShook;
        /// <summary>本地冲刺的表现相位。</summary>
        private int shieldSlamDashTime;
        /// <summary>远端玩家正在表现的冲刺类型。</summary>
        private ShieldSlamDash remoteShieldSlamDash;
        /// <summary>远端冲刺表现已经持续的帧数。</summary>
        private int remoteShieldSlamElapsed;
        /// <summary>远端冲刺的表现相位。</summary>
        private int remoteShieldSlamTime;
        /// <summary>仅以更高优先级覆盖当前冲刺，避免饰品更新顺序改变顶点结果。</summary>
        public void ProvideShieldSlamDash(ShieldSlamDash dash)
        {
            if (dash > shieldSlamDash)
                shieldSlamDash = dash;
        }
        /// <summary>在每帧装备效果重置时清空阿斯加德系列标记与当前冲刺。</summary>
        public void ResetShieldSlamEffects()
        {
            asgardsValor = false;
            asgardianAegis = false;
            ornateShield = false;
            shieldOfTheOcean = false;
            elysianAegis = false;
            elysianAegisPower = false;
            abaddon = false;
            shieldSlamDash = ShieldSlamDash.None;
        }
        /// <summary>死亡时清空全部冲刺计时、冷却与远端表现状态。</summary>
        public void ResetShieldSlamDashState()
        {
            shieldSlamDashElapsed = 0;
            shieldSlamDashCooldown = 0;
            shieldSlamDashTimeMod = 0;
            shieldSlamDashScreenShook = false;
            shieldSlamDashTime = 0;
            remoteShieldSlamDash = ShieldSlamDash.None;
            remoteShieldSlamElapsed = 0;
            remoteShieldSlamTime = 0;
            GeneralScreenShakePower = 0f;
            elysianGuard = false;
            shieldInvinc = 5f;
            abaddonCritCooldown = 0;
        }
        /// <summary>结算盾牌冲刺链全部常驻效果。</summary>
        public void ApplyShieldSlamAccessoryEffects()
        {
            if (abaddonCritCooldown > 0)
                abaddonCritCooldown--;
            if (ornateShield)
                ApplyOrnateShieldEffects();
            if (shieldOfTheOcean && Collision.DrownCollision(
                Player.position, Player.width, Player.height, Player.gravDir))
                Player.statDefense += 5;
            if (abaddon)
            {
                Player.GetCritChance(DamageClass.Generic) += 8;
                AddImmunity(Player, "CalamityMod", "BrimstoneFlames");
                AddImmunity(Player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
                AddImmunity(Player, "CalRD", "BrimstoneFlames");
            }
            if (asgardsValor)
                ApplyAsgardsValorEffects();
            if (elysianAegis)
                ApplyElysianAegisEffects();
            if (elysianAegisPower)
                UpdateElysianGuard();
            else
                elysianGuard = false;
            if (asgardianAegis)
                ApplyAsgardianAegisEffects();
        }
        /// <summary>应用华丽盾的生命、再生、残血防御与冲刺禁用。</summary>
        private void ApplyOrnateShieldEffects()
        {
            Player.dashType = 0;
            Player.lifeRegen += 2;
            Player.statLifeMax2 += 20;
            if (Player.statLife < Player.statLifeMax2 * 0.25f)
                Player.statDefense += 4;
        }
        /// <summary>应用阿斯加德之英勇的生命、移动与减益免疫。</summary>
        private void ApplyAsgardsValorEffects()
        {
            Player.dashType = 0;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.statLifeMax2 += 50;
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Weak] = true;
            Player.buffImmune[BuffID.BrokenArmor] = true;
            Player.buffImmune[BuffID.Bleeding] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Slow] = true;
            Player.buffImmune[BuffID.Confused] = true;
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.Darkness] = true;
            Player.buffImmune[BuffID.WindPushed] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.Daybreak] = true;
            Player.buffImmune[BuffID.OnFire] = true;
            Player.buffImmune[BuffID.OnFire3] = true;
            AddValorImmunities(Player);
            ApplySubmergedEndurance();
        }
        /// <summary>应用极乐之庇护的生命、恢复、火焰免疫与守护状态标记。</summary>
        private void ApplyElysianAegisEffects()
        {
            Player.dashType = 0;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.statLifeMax2 += 30;
            Player.lifeRegen += 3;
            Player.buffImmune[BuffID.CursedInferno] = true;
            Player.buffImmune[BuffID.ShadowFlame] = true;
            Player.buffImmune[BuffID.Daybreak] = true;
            Player.buffImmune[BuffID.OnFire] = true;
            Player.buffImmune[BuffID.OnFire3] = true;
            AddImmunity(Player, "CalamityMod", "HolyFlames");
            AddImmunity(Player, "CalamityMod", "BrimstoneFlames");
            AddImmunity(Player, "CalamityMod", "WeakPetrification");
            AddImmunity(Player, "CalamityMod", "Nightwither");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "GlacialState");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "HolyLight");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
            AddImmunity(Player, "CalamityModClassicPreTrailer", "WhisperingDeath");
            AddImmunity(Player, "CalRD", "GlacialState");
            AddImmunity(Player, "CalRD", "HolyFlames");
            AddImmunity(Player, "CalRD", "HolyLight");
            AddImmunity(Player, "CalRD", "BrimstoneFlames");
        }
        /// <summary>按护盾充能更新守护状态的伤害、暴击、仇恨、防御与移动惩罚。</summary>
        private void UpdateElysianGuard()
        {
            bool changed = false;
            if (elysianGuard)
            {
                float previous = shieldInvinc;
                shieldInvinc = Math.Max(0f, shieldInvinc - 0.08f);
                changed = previous != shieldInvinc;
                if (changed && shieldInvinc <= 0f && Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI);
                float consumed = 5f - shieldInvinc;
                Player.GetDamage(DamageClass.Generic) += consumed * 0.03f;
                Player.GetCritChance(DamageClass.Generic) += (int)(consumed * 2f);
                Player.aggro += (int)(consumed * 220f);
                Player.statDefense += (int)(consumed * 4f);
                Player.moveSpeed *= 0.85f;
                if (Player.mount.Active)
                    elysianGuard = false;
            }
            else
            {
                float previous = shieldInvinc;
                shieldInvinc = Math.Min(5f, shieldInvinc + 0.08f);
                changed = previous != shieldInvinc;
                if (changed && shieldInvinc >= 5f && Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.PlayerStealth, -1, -1, null, Player.whoAmI);
            }
            if (!changed)
                return;
            if (Main.rand.NextBool(2))
                SpawnGuardChargeDust(DustID.CopperCoin, 1.5707964f, 4f, 5, 11);
            if (Main.rand.NextBool(2))
                SpawnGuardChargeDust(DustID.GoldCoin, -1.5707964f, 2f, 12, 12);
        }
        /// <summary>生成守护充能时环绕玩家的金币色粒子。</summary>
        private void SpawnGuardChargeDust(int dustType, float angle, float speed, int minRadius, int maxRadius)
        {
            Vector2 direction = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
            Dust dust = Dust.NewDustPerfect(Player.Center - direction * maxRadius, dustType);
            dust.noGravity = true;
            dust.position = Player.Center - direction * Main.rand.Next(minRadius, maxRadius + 1);
            dust.velocity = direction.RotatedBy(angle) * speed;
            dust.scale = 0.5f + Main.rand.NextFloat();
            dust.fadeIn = 0.5f;
        }
        /// <summary>应用阿斯加德之庇护的生命、恢复、残血防御与广泛减益免疫。</summary>
        private void ApplyAsgardianAegisEffects()
        {
            Player.dashType = 0;
            Player.noKnockback = true;
            Player.fireWalk = true;
            Player.statLifeMax2 += 40;
            Player.lifeRegen += 5;
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Weak] = true;
            Player.buffImmune[BuffID.BrokenArmor] = true;
            Player.buffImmune[BuffID.Bleeding] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Slow] = true;
            Player.buffImmune[BuffID.Confused] = true;
            Player.buffImmune[BuffID.Silenced] = true;
            Player.buffImmune[BuffID.Cursed] = true;
            Player.buffImmune[BuffID.Darkness] = true;
            Player.buffImmune[BuffID.WindPushed] = true;
            Player.buffImmune[BuffID.Stoned] = true;
            Player.buffImmune[BuffID.Daybreak] = true;
            Player.buffImmune[BuffID.OnFire] = true;
            Player.buffImmune[BuffID.OnFire3] = true;
            Player.buffImmune[BuffID.CursedInferno] = true;
            Player.buffImmune[BuffID.ShadowFlame] = true;
            Player.buffImmune[BuffID.Venom] = true;
            Player.buffImmune[BuffID.Webbed] = true;
            Player.buffImmune[BuffID.Blackout] = true;
            AddAsgardianImmunities(Player);
            ApplySubmergedEndurance();
            if (Player.statLife < Player.statLifeMax2 * 0.25f)
                Player.statDefense += 10;
        }
        /// <summary>为阿斯加德之英勇追加对应分支的减益免疫。</summary>
        private static void AddValorImmunities(Player player)
        {
            AddImmunity(player, "CalamityMod", "SearingLava");
            AddImmunity(player, "CalamityMod", "HolyFlames");
            AddImmunity(player, "CalamityMod", "BrimstoneFlames");
            AddImmunity(player, "CalamityModClassicPreTrailer", "GlacialState");
            AddImmunity(player, "CalamityModClassicPreTrailer", "HolyLight");
            AddImmunity(player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
            AddImmunity(player, "CalRD", "GlacialState");
            AddImmunity(player, "CalRD", "HolyFlames");
            AddImmunity(player, "CalRD", "HolyLight");
            AddImmunity(player, "CalRD", "BrimstoneFlames");
        }
        /// <summary>为阿斯加德之庇护追加广泛的外部减益免疫。</summary>
        private static void AddAsgardianImmunities(Player player)
        {
            AddImmunity(player, "CalamityMod", "HolyFlames");
            AddImmunity(player, "CalamityMod", "BrimstoneFlames");
            AddImmunity(player, "CalamityMod", "WeakPetrification");
            AddImmunity(player, "CalamityMod", "Nightwither");
            AddImmunity(player, "CalamityMod", "GodSlayerInferno");
            AddImmunity(player, "CalamityMod", "ArmorCrunch");
            AddImmunity(player, "CalamityMod", "SulphuricPoisoning");
            AddImmunity(player, "CalamityMod", "BrainRot");
            AddImmunity(player, "CalamityMod", "BurningBlood");
            AddImmunity(player, "CalamityModClassicPreTrailer", "GlacialState");
            AddImmunity(player, "CalamityModClassicPreTrailer", "HolyLight");
            AddImmunity(player, "CalamityModClassicPreTrailer", "BrimstoneFlames");
            AddImmunity(player, "CalamityModClassicPreTrailer", "WhisperingDeath");
            AddImmunity(player, "CalamityModClassicPreTrailer", "GodSlayerInferno");
            AddImmunity(player, "CalamityModClassicPreTrailer", "ArmorCrunch");
            AddImmunity(player, "CalRD", "GlacialState");
            AddImmunity(player, "CalRD", "HolyFlames");
            AddImmunity(player, "CalRD", "HolyLight");
            AddImmunity(player, "CalRD", "BrimstoneFlames");
            AddImmunity(player, "CalRD", "WhisperingDeath");
            AddImmunity(player, "CalRD", "GodSlayerInferno");
            AddImmunity(player, "CalRD", "ArmorCrunch");
        }
        /// <summary>向玩家写入指定外部减益的免疫。</summary>
        private static void AddImmunity(Player player, string modName, string buffName)
        {
            int type = CalamityCompatUtil.GetBuffType(modName, buffName);
            if (type > 0)
                player.buffImmune[type] = true;
        }
        /// <summary>在完整溺水碰撞环境中提供十二百分点伤害减免。</summary>
        private void ApplySubmergedEndurance()
        {
            if (Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir))
                Player.endurance += 0.12f;
        }
        /// <summary>每帧执行盾牌冲刺状态机。</summary>
        public void ShieldSlamDashMovement()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                UpdateRemoteShieldSlamVisuals();
                return;
            }
            if (shieldSlamDashCooldown > 0)
                shieldSlamDashCooldown--;
            if (shieldSlamDashTimeMod != 0)
                shieldSlamDashTimeMod -= shieldSlamDashTimeMod > 0 ? 1 : -1;
            bool startedThisFrame = false;
            if (shieldSlamDashElapsed <= 0)
            {
                UpdateShieldSlamInput();
                if (shieldSlamDashElapsed <= 0)
                    return;
                startedThisFrame = true;
            }
            BlockVanillaDash();
            if (startedThisFrame)
                return;
            ResolveShieldSlamHits();
            UpdateShieldSlamMotion();
        }
        /// <summary>把本地累积的震动强度转换为屏幕偏移并逐渐衰减。</summary>
        public override void ModifyScreenPosition()
        {
            if (Player.whoAmI != Main.myPlayer || GeneralScreenShakePower <= 0f)
                return;
            Main.screenPosition += Main.rand.NextVector2Circular(GeneralScreenShakePower, GeneralScreenShakePower);
            GeneralScreenShakePower = MathHelper.Clamp(GeneralScreenShakePower - 0.185f, 0f, 20f);
        }
        /// <summary>守护状态额外降低跑动加速度与最大跑速。</summary>
        public void ApplyShieldSlamRunSpeedEffects()
        {
            if (!elysianGuard)
                return;
            Player.runAcceleration *= 0.85f;
            Player.maxRunSpeed *= 0.85f;
        }
        /// <summary>读取守护状态快捷键；骑乘时禁止开启。</summary>
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (GodslayerKeybinds.DashHotKey?.JustPressed == true)
                RequestGodSlayerDash();
            if (DemonshadeKeybinds.HotKey?.JustPressed == true && demonshadeSetBonus
                && Player.whoAmI == Main.myPlayer)
                ActivateDemonshadeEnrage();
            if (ShieldSlamKeybinds.ElysianHotKey?.JustPressed == true
                && elysianAegisPower && !Player.mount.Active)
                elysianGuard = !elysianGuard;
            if (TarragonKeybinds.HotKey?.JustPressed == true)
            {
                TryActivateTarragonCloak();
                TryActivateBloodflareSoulBurst();
            }
        }
        /// <summary>播放狂怒爆发表现，给玩家自身挂十秒狂怒并请求服务器同步附近敌怪。</summary>
        private void ActivateDemonshadeEnrage()
        {
            SoundEngine.PlaySound(SoundID.Zombie104, Player.position);
            for (int i = 0; i < 36; i++)
            {
                Dust dust = Dust.NewDustPerfect(Player.position + Main.rand.NextVector2Square(
                    0f, Player.width), DustID.LifeDrain, Main.rand.NextVector2Circular(3f, 3f));
                dust.velocity *= 3f;
                dust.scale *= 1.15f;
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 offset = Main.rand.NextVector2Unit()
                    * new Vector2(Player.width / 2f, Player.height) * 0.75f;
                Dust dust = Dust.NewDustPerfect(Player.Center + offset * 2f, DustID.LifeDrain
                    , offset * 1.5f, 100, default, 1.4f);
                dust.noGravity = true;
                dust.noLight = true;
            }
            Player.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write(MsgDemonshadeEnrage);
                packet.Send();
                return;
            }
            ApplyDemonshadeEnrageToNearbyNPCs(Player, 600);
        }
        /// <summary>服务端或单机给三千像素内的合法敌怪施加十秒狂怒。</summary>
        public static void ApplyDemonshadeEnrageToNearbyNPCs(Player player, int duration)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || player is null || !player.active)
                return;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.active || npc.friendly || npc.dontTakeDamage
                    || Vector2.Distance(player.Center, npc.Center) > 3000f)
                    continue;
                npc.AddBuff(ModContent.BuffType<Enraged>(), duration, false);
            }
        }
        /// <summary>更新远端玩家的拖尾表现，结束包丢失时自动过期。</summary>
        private void UpdateRemoteShieldSlamVisuals()
        {
            if (Main.dedServ || remoteShieldSlamElapsed <= 0)
                return;
            if (!Player.active || Player.dead)
            {
                ReceiveShieldSlamDash(ShieldSlamDash.None);
                return;
            }
            SpawnShieldSlamTrailDust(remoteShieldSlamDash, ref remoteShieldSlamTime);
            if (++remoteShieldSlamElapsed > RemoteDashMaxFrames)
                ReceiveShieldSlamDash(ShieldSlamDash.None);
        }
        /// <summary>接收其他玩家的冲刺开始或结束广播。</summary>
        public void ReceiveShieldSlamDash(ShieldSlamDash dash)
        {
            remoteShieldSlamDash = dash;
            remoteShieldSlamElapsed = dash == ShieldSlamDash.None ? 0 : 1;
            remoteShieldSlamTime = 0;
        }
        /// <summary>接收其他玩家的撞击广播并在目标位置生成表现粒子。</summary>
        public void ReceiveShieldSlamDashHit(ShieldSlamDash dash, int npcIndex)
        {
            if (npcIndex < 0 || npcIndex >= Main.maxNPCs || !Main.npc[npcIndex].active)
                return;
            SpawnShieldSlamHitParticles(dash, Main.npc[npcIndex].Center);
        }
        /// <summary>广播本地冲刺开始或结束。</summary>
        private void SendShieldSlamDashPacket(ShieldSlamDash dash)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write(MsgShieldSlamDash);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)dash);
            if (Main.netMode == NetmodeID.Server)
                packet.Send(-1, Player.whoAmI);
            else
                packet.Send();
        }
        /// <summary>广播本地撞击，供其他客户端同步粒子表现。</summary>
        private void SendShieldSlamHitPacket(ShieldSlamDash dash, int npcIndex)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write(MsgShieldSlamDashHit);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)dash);
            packet.Write((short)npcIndex);
            if (Main.netMode == NetmodeID.Server)
                packet.Send(-1, Player.whoAmI);
            else
                packet.Send();
        }
        /// <summary>读取左右方向键十五帧内的同向双击。</summary>
        private void UpdateShieldSlamInput()
        {
            if (shieldSlamDash == ShieldSlamDash.None || shieldSlamDashCooldown > 0
                || Player.mount.Active || godSlayerDashQueued || godSlayerDashElapsed > 0)
                return;
            bool rightInput = Player.controlRight && Player.releaseRight;
            bool leftInput = Player.controlLeft && Player.releaseLeft;
            if (rightInput)
            {
                if (shieldSlamDashTimeMod > 0)
                {
                    shieldSlamDashTimeMod = 0;
                    StartShieldSlamDash(1);
                }
                else
                    shieldSlamDashTimeMod = DoubleTapWindow;
            }
            else if (leftInput)
            {
                if (shieldSlamDashTimeMod < 0)
                {
                    shieldSlamDashTimeMod = 0;
                    StartShieldSlamDash(-1);
                }
                else
                    shieldSlamDashTimeMod = -DoubleTapWindow;
            }
        }
        /// <summary>设置水平冲刺速度，并在斜前方有实心块时减半。</summary>
        private void StartShieldSlamDash(int direction)
        {
            ShieldSlamProfile profile = GetShieldSlamProfile(shieldSlamDash);
            Player.velocity = new Vector2(direction * profile.StartSpeed, Player.velocity.Y);
            Point upwardPoint = (Player.Center + new Vector2(direction * Player.width / 2f + 2f
                , Player.gravDir * -Player.height / 2f + Player.gravDir * 2f)).ToTileCoordinates();
            Point aheadPoint = (Player.Center + new Vector2(direction * Player.width / 2f + 2f, 0f))
                .ToTileCoordinates();
            if (WorldGen.SolidOrSlopedTile(upwardPoint.X, upwardPoint.Y)
                || WorldGen.SolidOrSlopedTile(aheadPoint.X, aheadPoint.Y))
                Player.velocity.X *= 0.5f;
            Player.dashDelay = -1;
            shieldSlamDashElapsed = 1;
            shieldSlamDashTime = 0;
            shieldSlamDashScreenShook = false;
            SendShieldSlamDashPacket(shieldSlamDash);
        }
        /// <summary>更新拖尾、朝向和两级速度衰减，并在速度落入跑步区间时结束冲刺。</summary>
        private void UpdateShieldSlamMotion()
        {
            SpawnShieldSlamTrailDust(shieldSlamDash, ref shieldSlamDashTime);
            Player.vortexStealthActive = false;
            if (Player.velocity.X != 0f)
                Player.ChangeDir(Math.Sign(Player.velocity.X));
            ShieldSlamProfile profile = GetShieldSlamProfile(shieldSlamDash);
            float runSpeed = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
            if (Math.Abs(Player.velocity.X) > profile.MidDashSpeed)
                Player.velocity.X *= profile.FastDecay;
            else if (Math.Abs(Player.velocity.X) > runSpeed)
                Player.velocity.X *= profile.RunDecay;
            else
                EndShieldSlamDash(runSpeed);
        }
        /// <summary>按冲刺类型生成护盾染色的拖尾尘。</summary>
        private void SpawnShieldSlamTrailDust(ShieldSlamDash dash, ref int time)
        {
            switch (dash)
            {
                case ShieldSlamDash.AsgardianAegis:
                {
                    time += 2;
                    float radiusFactor = MathHelper.Lerp(0f, 1f, Utils.GetLerpValue(2f, 2.5f, time, true));
                    for (int i = 0; i < 3; i++)
                    {
                        float angle = Player.velocity.ToRotation() + time / 5f;
                        float radius = (15f + MathF.Cos(time / 3f) * 12f) * radiusFactor;
                        Vector2 position = Player.Center - Player.velocity * 2f;
                        position += angle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi) * radius;
                        Dust ring = Dust.NewDustPerfect(position, Main.rand.NextBool(5)
                            ? DustID.GiantCursedSkullBolt : 295);
                        ring.alpha = 220;
                        ring.noGravity = true;
                        ring.velocity = Player.velocity * 0.8f;
                        ring.scale = Main.rand.NextFloat(1.7f, 2f);
                        ring.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        Dust trail = Dust.NewDustPerfect(
                            Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                                + Player.velocity * 1.5f
                            , Main.rand.NextBool(8) ? 180 : 295
                            , -Player.velocity.RotatedByRandom(MathHelper.ToRadians(30f))
                                * Main.rand.NextFloat(0.1f, 0.8f)
                            , 0, default, Main.rand.NextFloat(1.7f, 1.9f));
                        trail.alpha = 170;
                        trail.noGravity = true;
                        trail.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                    }
                    break;
                }
                case ShieldSlamDash.Demonshade:
                {
                    time += 2;
                    float radiusFactor = MathHelper.Lerp(0f, 1f
                        , Utils.GetLerpValue(2f, 2.5f, time, true));
                    for (int i = 0; i < 5; i++)
                    {
                        float angle = Player.velocity.ToRotation() + time / 5f;
                        float radius = (18f + MathF.Cos(time / 3f) * 14f) * radiusFactor;
                        Vector2 position = Player.Center - Player.velocity * 2f;
                        position += angle.ToRotationVector2().RotatedBy(i / 5f * MathHelper.TwoPi)
                            * radius;
                        Dust ring = Dust.NewDustPerfect(position, Main.rand.NextBool(3)
                            ? DustID.LifeDrain : DustID.CrimsonTorch);
                        ring.alpha = 200;
                        ring.noGravity = true;
                        ring.velocity = Player.velocity * 0.8f;
                        ring.scale = Main.rand.NextFloat(1.9f, 2.3f);
                        ring.color = DemonshadeDashFlameColor;
                        Dust trail = Dust.NewDustPerfect(
                            Player.Center + new Vector2(
                                Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-18f, 18f)) + Player.velocity * 1.5f,
                            Main.rand.NextBool(4) ? DustID.Shadowflame : DustID.LifeDrain,
                            -Player.velocity.RotatedByRandom(MathHelper.ToRadians(30f))
                                * Main.rand.NextFloat(0.1f, 0.9f),
                            150, DemonshadeDashDarkColor, Main.rand.NextFloat(1.8f, 2.2f));
                        trail.noGravity = true;
                    }
                    break;
                }
                case ShieldSlamDash.ElysianAegis:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Dust sacred = Dust.NewDustPerfect(
                            Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                                - Player.velocity * 1.2f
                            , Main.rand.NextBool(8) ? DustID.FireworkFountain_Yellow : 162
                            , -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f))
                                * Main.rand.NextFloat(0.1f, 0.8f)
                            , 0, default, Main.rand.NextFloat(1.8f, 2.8f));
                        sacred.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        sacred.noGravity = sacred.type != DustID.FireworkFountain_Yellow;
                        sacred.fadeIn = 0.5f;
                        if (sacred.type == DustID.FireworkFountain_Yellow)
                        {
                            sacred.scale = Main.rand.NextFloat(0.8f, 1.2f);
                            sacred.velocity += new Vector2(0f, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
                        }
                        Dust gold = Dust.NewDustPerfect(Player.Center
                            + Main.rand.NextVector2Circular(6f, 6f) - Player.velocity * 2f
                            , DustID.GoldFlame);
                        gold.velocity = -Player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
                        gold.scale = Main.rand.NextFloat(0.9f, 1.4f);
                        gold.noGravity = true;
                    }
                    break;
                }
                case ShieldSlamDash.AsgardsValor:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Player.position + Vector2.UnitY * 4f
                            , Player.width, Player.height - 8
                            , Main.rand.NextBool() ? DustID.CrimsonTorch : DustID.OrangeTorch
                            , 0f, 0f, 0, default, 1.2f);
                        dust.velocity = -Player.velocity * Main.rand.NextFloat(0.1f, 0.75f);
                        dust.scale *= Main.rand.NextFloat(1f, 1.2f);
                        dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        dust.noGravity = true;
                        if (Main.rand.NextBool())
                            dust.fadeIn = 0.1f;
                    }
                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustPerfect(Player.Center
                            + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                            - Player.velocity * 1.7f
                            , DustID.FireworkFountain_Yellow
                            , -Player.velocity * Main.rand.NextFloat(0.15f, 0.4f), 0, default, 0.5f);
                        dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                    }
                    break;
                }
                case ShieldSlamDash.OrnateShield:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust ice = Dust.NewDustPerfect(
                            Player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f))
                                - Player.velocity * 1.7f
                            , Main.rand.NextBool(8) ? 223 : 180
                            , -Player.velocity.RotatedByRandom(MathHelper.ToRadians(10f))
                                * Main.rand.NextFloat(0.1f, 0.8f)
                            , 0, default, Main.rand.NextFloat(0.6f, 0.8f));
                        ice.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                        ice.noGravity = true;
                        ice.fadeIn = 0.5f;
                        if (ice.type == 180)
                            ice.scale = Main.rand.NextFloat(1.6f, 2.2f);
                    }
                    break;
                }
            }
        }
        /// <summary>复位原版冲刺状态、写入短冷却并结束广播。</summary>
        private void EndShieldSlamDash(float runSpeed)
        {
            shieldSlamDashCooldown = ShieldSlamCooldown;
            Player.dashDelay = 0;
            if (Player.velocity.X < 0f)
                Player.velocity.X = -runSpeed;
            else if (Player.velocity.X > 0f)
                Player.velocity.X = runSpeed;
            shieldSlamDashElapsed = 0;
            shieldSlamDashTime = 0;
            SendShieldSlamDashPacket(ShieldSlamDash.None);
        }
        /// <summary>在玩家前方构造略向外扩的冲刺命中框。</summary>
        private void ResolveShieldSlamHits()
        {
            ShieldSlamProfile profile = GetShieldSlamProfile(shieldSlamDash);
            if (profile.Damage <= 0)
                return;
            Rectangle hitArea = new(
                (int)(Player.position.X + Player.velocity.X * 0.5f - 4f),
                (int)(Player.position.Y + Player.velocity.Y * 0.5f - 4f),
                Player.width + 8, Player.height + 8);
            int direction = Player.velocity.X == 0f ? Player.direction : Math.Sign(Player.velocity.X);
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[npc.type])
                    continue;
                if (npc.dontTakeDamage || npc.friendly || npc.immune[Player.whoAmI] > 0)
                    continue;
                if (!hitArea.Intersects(npc.getRect()) || !(npc.noTileCollide || Player.CanHit(npc)))
                    continue;
                int damage = (int)Player.GetTotalDamage<MeleeDamageClass>().ApplyTo(profile.Damage);
                float knockback = Player.GetTotalKnockback<MeleeDamageClass>().ApplyTo(profile.Knockback);
                bool critical = Main.rand.Next(100) < Player.GetTotalCritChance<MeleeDamageClass>();
                Player.ApplyDamageToNPC(npc, damage, knockback, direction, critical, DamageClass.Melee, true);
                if (npc.immune[Player.whoAmI] < MinShieldNPCImmunityFrames)
                    npc.immune[Player.whoAmI] = MinShieldNPCImmunityFrames;
                if (Player.immuneTime < ShieldHitImmunityFrames)
                    Player.immuneTime = ShieldHitImmunityFrames;
                ApplyShieldSlamHitEffects(npc, profile);
            }
        }
        /// <summary>触发震屏、爆炸、粒子、减益与撞击广播。</summary>
        private void ApplyShieldSlamHitEffects(NPC npc, ShieldSlamProfile profile)
        {
            if (profile.ScreenShake > 0f && !shieldSlamDashScreenShook)
            {
                GeneralScreenShakePower = profile.ScreenShake;
                shieldSlamDashScreenShook = true;
            }
            if (shieldSlamDash is ShieldSlamDash.AsgardianAegis or ShieldSlamDash.Demonshade)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero
                    , ModContent.ProjectileType<CosmicDashExplosion>(), GetShieldExplosionDamage(profile)
                    , profile.ExplosionKnockback, Main.myPlayer, 3f, 0f);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityMod", "GodSlayerInferno", profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityModClassicPreTrailer", "GodSlayerInferno"
                    , profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalRD", "GodSlayerInferno", profile.DebuffFrames);
            }
            else if (shieldSlamDash == ShieldSlamDash.ElysianAegis)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero
                    , ModContent.ProjectileType<HolyExplosionSupreme>(), GetShieldExplosionDamage(profile)
                    , profile.ExplosionKnockback, Main.myPlayer, 1f, 0f);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityMod", "HolyFlames", profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalamityModClassicPreTrailer", "HolyLight"
                    , profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalRD", "HolyFlames", profile.DebuffFrames);
                CalamityCompatUtil.TryAddBuff(npc, "CalRD", "HolyLight", profile.DebuffFrames);
            }
            else if (shieldSlamDash == ShieldSlamDash.OrnateShield && profile.DebuffFrames > 0)
                npc.AddBuff(BuffID.Frostburn2, profile.DebuffFrames);
            SpawnShieldSlamHitParticles(shieldSlamDash, npc.Center);
            if (shieldSlamDash != ShieldSlamDash.OrnateShield)
                SendShieldSlamHitPacket(shieldSlamDash, npc.whoAmI);
        }
        /// <summary>生成冲刺撞击的圆环、爆炸或三色尘环。</summary>
        private void SpawnShieldSlamHitParticles(ShieldSlamDash dash, Vector2 center)
        {
            switch (dash)
            {
                case ShieldSlamDash.AsgardianAegis:
                    ShieldSlamParticleSystem.SpawnRing(center, new Vector2(2f), 0f, 0.1f, 0.85f, 36);
                    ShieldSlamParticleSystem.SpawnExplosion(center, Color.Magenta
                        , Main.rand.NextFloat(-5f, 5f), 0.65f, 26);
                    break;
                case ShieldSlamDash.Demonshade:
                    ShieldSlamParticleSystem.SpawnRing(center, new Vector2(2f), 0f
                        , 0.1f, 0.92f, 34, DemonshadeDashFlameColor);
                    ShieldSlamParticleSystem.SpawnRing(center, new Vector2(1.7f), 0f
                        , 0.12f, 0.58f, 24, DemonshadeDashCoreColor);
                    ShieldSlamParticleSystem.SpawnExplosion(center, DemonshadeDashFlameColor
                        , Main.rand.NextFloat(-5f, 5f), 0.72f, 30);
                    ShieldSlamParticleSystem.SpawnExplosion(center, DemonshadeDashDarkColor
                        , Main.rand.NextFloat(-5f, 5f), 0.48f, 20);
                    break;
                case ShieldSlamDash.ElysianAegis:
                {
                    float scale = Main.rand.NextFloat(0.45f, 0.55f);
                    ShieldSlamParticleSystem.SpawnExplosion(center, Color.Gray * 0.6f
                        , Main.rand.NextFloat(-5f, 5f), scale + 0.07f, 20);
                    ShieldSlamParticleSystem.SpawnExplosion(center, Color.Orange
                        , Main.rand.NextFloat(-5f, 5f), scale, 20);
                    break;
                }
                case ShieldSlamDash.AsgardsValor:
                {
                    const int dustCount = 12;
                    float step = MathHelper.TwoPi / dustCount;
                    Vector2 start = Vector2.Normalize(new Vector2(-1f, -1f));
                    for (int i = 0; i < dustCount; i++)
                    {
                        Vector2 velocity = start.RotatedBy(step * i);
                        SpawnShieldHitDust(center, velocity, DustID.CrimsonTorch, 3f, 2.5f, default);
                        SpawnShieldHitDust(center, velocity, DustID.OrangeTorch, 5f, 2.2f, Color.Salmon);
                        SpawnShieldHitDust(center, velocity, DustID.IchorTorch, 7f, 1.9f, Color.SandyBrown);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(center, DustID.FireworkFountain_Yellow
                            , new Vector2(0f, -3.5f).RotatedByRandom(0.7f) * Main.rand.NextFloat(0.8f, 1.4f)
                            , 0, default, 1.2f);
                        dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
                    }
                    SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.6f, PitchVariance = 0.3f }, center);
                    break;
                }
            }
        }
        /// <summary>生成一颗护盾染色的撞击尘。</summary>
        private void SpawnShieldHitDust(Vector2 center, Vector2 direction, int type
            , float speed, float scale, Color color)
        {
            Dust dust = Dust.NewDustPerfect(center, type, direction * speed, 0, default, scale);
            dust.noGravity = true;
            dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cShield, Player);
            if (color != default)
                dust.color = color;
        }
        /// <summary>以通用伤害为底，叠加近战、远程、魔法与召唤中的最高职业加成计算爆炸伤害。</summary>
        private int GetShieldExplosionDamage(ShieldSlamProfile profile)
        {
            StatModifier damage = StatModifier.Default;
            StatModifier generic = Player.GetTotalDamage<GenericDamageClass>();
            damage.Base = generic.Base;
            damage *= generic.Multiplicative;
            damage.Flat = generic.Flat;
            float strongest = Math.Max(1f, Player.GetTotalDamage<MeleeDamageClass>().Additive);
            strongest = Math.Max(strongest, Player.GetTotalDamage<RangedDamageClass>().Additive);
            strongest = Math.Max(strongest, Player.GetTotalDamage<MagicDamageClass>().Additive);
            strongest = Math.Max(strongest, Player.GetTotalDamage<SummonDamageClass>().Additive * 0.75f);
            damage += strongest - 1f;
            return (int)damage.ApplyTo(profile.ExplosionDamage);
        }
        /// <summary>阿巴顿在弹幕暴击命中时生成一次短冷却硫磺爆炸。</summary>
        public void TrySpawnAbaddonCrit(Projectile projectile, NPC target, NPC.HitInfo hit)
        {
            if (!abaddon || !hit.Crit || abaddonCritCooldown > 0
                || Player.whoAmI != Main.myPlayer)
                return;
            abaddonCritCooldown = 15;
            int damage = DamageSoftCap(projectile.damage * 0.03f, 25);
            Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, Vector2.Zero
                , ModContent.ProjectileType<AbaddonCrit>(), damage, 0f, Player.whoAmI);
        }
        /// <summary>对超过上限的伤害按平方根曲线折算增长幅度。</summary>
        private static int DamageSoftCap(float damage, int cap)
        {
            if (damage < cap)
                return (int)damage;
            double ratio = Math.Sqrt(damage / cap) / 1.25 + 0.2;
            return (int)(cap * ratio);
        }
        /// <summary>阻止原版冲刺和克苏鲁之盾在盾牌冲刺期间抢占状态。</summary>
        private void BlockVanillaDash()
        {
            Player.dashType = 0;
            Player.eocHit = -1;
            if (Player.eocDash != 0)
                Player.eocDash = 0;
        }
        /// <summary>返回指定盾牌冲刺的完整数值档。</summary>
        private static ShieldSlamProfile GetShieldSlamProfile(ShieldSlamDash dash)
        {
            return dash switch
            {
                ShieldSlamDash.Demonshade => new ShieldSlamProfile(
                    31f, 16.5f, 1400, 18f, 6f, 360, 1400, 24f, 0.94f, 0.74f),
                ShieldSlamDash.AsgardianAegis => new ShieldSlamProfile(
                    23.3f, 16f, 1000, 15f, 5f, 300, 1000, 20f),
                ShieldSlamDash.ElysianAegis => new ShieldSlamProfile(
                    21.5f, 14f, 500, 12f, 3.5f, 300, 500, 15f),
                ShieldSlamDash.AsgardsValor => new ShieldSlamProfile(
                    16.9f, DefaultMidDashSpeed, 200, 9f, 0f, 0, 0, 0f),
                ShieldSlamDash.OrnateShield => new ShieldSlamProfile(
                    16.9f, 12.5f, 50, 3f, 0f, 180, 0, 0f),
                _ => new ShieldSlamProfile(DefaultMidDashSpeed, DefaultMidDashSpeed, 0, 0f, 0f, 0, 0, 0f)
            };
        }
        /// <summary>单个盾牌冲刺的只读数值配置。</summary>
        private readonly struct ShieldSlamProfile
        {
            /// <summary>起手速度。</summary>
            public readonly float StartSpeed;
            /// <summary>过程速度上限。</summary>
            public readonly float MidDashSpeed;
            /// <summary>撞击基础伤害。</summary>
            public readonly int Damage;
            /// <summary>撞击基础击退。</summary>
            public readonly float Knockback;
            /// <summary>首次命中的屏幕震动强度。</summary>
            public readonly float ScreenShake;
            /// <summary>附加减益持续时间。</summary>
            public readonly int DebuffFrames;
            /// <summary>爆炸弹幕基础伤害。</summary>
            public readonly int ExplosionDamage;
            /// <summary>爆炸弹幕基础击退。</summary>
            public readonly float ExplosionKnockback;
            /// <summary>高速阶段每帧速度衰减。</summary>
            public readonly float FastDecay;
            /// <summary>跑速以上阶段每帧速度衰减。</summary>
            public readonly float RunDecay;
            /// <summary>构造一档完整冲刺数值。</summary>
            public ShieldSlamProfile(float startSpeed, float midDashSpeed, int damage, float knockback
                , float screenShake, int debuffFrames, int explosionDamage, float explosionKnockback
                , float fastDecay = ShieldSpeedDeceleration
                , float runDecay = ShieldRunSpeedDeceleration)
            {
                StartSpeed = startSpeed;
                MidDashSpeed = midDashSpeed;
                Damage = damage;
                Knockback = knockback;
                ScreenShake = screenShake;
                DebuffFrames = debuffFrames;
                ExplosionDamage = explosionDamage;
                ExplosionKnockback = explosionKnockback;
                FastDecay = fastDecay;
                RunDecay = runDecay;
            }
        }
        /// <summary>同步始源林海免死窗口状态的网络消息码。</summary>
        public const byte MsgSilvaRevivalState = 4;
        /// <summary>本帧是否穿着任意始源林海三件套。</summary>
        public bool silvaSet;
        /// <summary>本帧是否穿着始源林海近战头。</summary>
        public bool silvaMelee;
        /// <summary>本帧是否穿着始源林海射手脚盔。</summary>
        public bool silvaRanged;
        /// <summary>本帧是否穿着始源林海法师罩帽。</summary>
        public bool silvaMage;
        /// <summary>本帧是否穿着始源林海召唤头盔。</summary>
        public bool silvaSummon;
        /// <summary>本帧是否穿着始源林海盗贼面具。</summary>
        public bool silvaThrowing;
        /// <summary>本帧是否受到始源林海迟滞影响。</summary>
        public bool silvaHysteresis;
        /// <summary>本帧是否检测到远古叶棱晶在场。</summary>
        public bool hasSilvaCrystal;
        /// <summary>免死窗口剩余帧数；只在首次致命伤后开始倒计时。</summary>
        public int silvaCountdown = 600;
        /// <summary>本轮生命内免死窗口触发后再次受致命伤的次数。</summary>
        public int silvaHitCounter;
        /// <summary>本命是否已经触发过始源林海免死窗口。</summary>
        public bool hasSilvaEffect;
        /// <summary>法师魔法爆裂的 300 帧冷却。</summary>
        private int silvaMageBurstCooldown;
        /// <summary>缓存不视为战斗减益的怒气、肾上腺素和暴怒状态。</summary>
        private static HashSet<int> silvaPseudoDebuffs;
        /// <summary>清空始源林海每帧分支标记；跨帧窗口和冷却不在此时复位。</summary>
        private void ResetSilvaEffects()
        {
            silvaSet = false;
            silvaMelee = false;
            silvaRanged = false;
            silvaMage = false;
            silvaSummon = false;
            silvaThrowing = false;
            silvaHysteresis = false;
            hasSilvaCrystal = false;
        }
        /// <summary>死亡后恢复最大生命惩罚、免死窗口和叶棱晶状态。</summary>
        private void ResetSilvaStateOnDeath()
        {
            silvaCountdown = 600;
            silvaHitCounter = 0;
            hasSilvaEffect = false;
            silvaMageBurstCooldown = 0;
            hasSilvaCrystal = false;
            Player.ClearBuff(ModContent.BuffType<SilvaRevival>());
        }
        /// <summary>
        /// 结算近全减益免疫、免死窗口、最大生命惩罚以及各职业分支属性。
        /// 所有定时值只在装备期间继续推进，但最大生命惩罚直到真正死亡才恢复。
        /// </summary>
        private void ApplySilvaEffects()
        {
            if (silvaSet)
                ApplySilvaDebuffImmunity();
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                silvaCountdown--;
                if (silvaCountdown <= 0)
                    SoundEngine.PlaySound(SoundID.Item29, Player.position);
                SpawnSilvaAuraDust();
            }
            if (silvaHitCounter > 0)
            {
                Player.statLifeMax2 -= silvaHitCounter * 100;
                if (Player.statLifeMax2 <= 400)
                {
                    Player.statLifeMax2 = 400;
                    if (silvaCountdown > 0)
                    {
                        Player.ClearBuff(ModContent.BuffType<SilvaRevival>());
                        SoundEngine.PlaySound(SoundID.Item29, Player.position);
                    }
                    silvaCountdown = 0;
                }
            }
            if (silvaSummon && hasSilvaEffect && silvaCountdown <= 0)
            {
                Player.maxMinions += 2;
                Player.GetDamage(DamageClass.Summon) += 0.10f;
            }
            if (silvaRanged && Player.HeldItem.useTime > 3
                && Player.HeldItem.CountsAsClass(DamageClass.Ranged))
                Player.GetAttackSpeed(DamageClass.Ranged) += 0.10f;
            if (silvaThrowing)
            {
                DamageClass rogue = CRUtil.GetRogueDamageClass();
                if (silvaCountdown > 0 && hasSilvaEffect)
                    Player.GetDamage(rogue) += 0.40f;
                if (Player.statLife > Player.statLifeMax2 * 0.5f
                    && Player.HeldItem.useTime > 3
                    && Player.HeldItem.CountsAsClass(rogue))
                    Player.GetAttackSpeed(rogue) += 0.10f;
            }
            if (silvaMage && silvaCountdown > 0 && hasSilvaEffect)
                Player.GetDamage(DamageClass.Magic) += 0.60f;
            if (silvaMageBurstCooldown > 0)
                silvaMageBurstCooldown--;
            ApplySilvaDebuffRegenProtection();
        }
        /// <summary>授予战斗减益免疫；免死窗口内再补齐所有当前减益，怒气类状态除外。</summary>
        private void ApplySilvaDebuffImmunity()
        {
            for (int i = 1; i < Player.buffImmune.Length; i++)
            {
                if (BuffUtil.Contains(i))
                    Player.buffImmune[i] = true;
            }
            if (silvaCountdown <= 0 || !hasSilvaEffect)
                return;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int buffType = Player.buffType[i];
                if (buffType <= 0 || Player.buffTime[i] <= 0 || !Main.debuff[buffType]
                    || IsSilvaPseudoDebuff(buffType))
                    continue;
                Player.buffImmune[buffType] = true;
            }
        }
        /// <summary>免死窗口期间阻止减益把最终生命再生压成负值。</summary>
        private void ApplySilvaDebuffRegenProtection()
        {
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet && Player.lifeRegen < 0)
                Player.lifeRegen = 0;
        }
        /// <summary>在增益结算阶段再次清理负再生，覆盖持续伤害减益的写入时机。</summary>
        public override void PostUpdateBuffs()
        {
            ApplySilvaDebuffRegenProtection();
        }
        /// <summary>始源林海三件套提高 5% 最大跑速和加速度。</summary>
        private void ApplySilvaRunSpeed()
        {
            if (!silvaSet)
                return;
            Player.maxRunSpeed *= 1.05f;
            Player.runAcceleration *= 1.05f;
        }
        /// <summary>真近战命中时有 25% 概率造成五倍最终伤害。</summary>
        private void ApplySilvaMeleeDamage(Item item, ref NPC.HitModifiers modifiers)
        {
            if (!silvaMelee || !Main.rand.NextBool(4))
                return;
            if (item.CountsAsClass(DamageClass.Melee) || item.CountsAsClass(DamageClass.MeleeNoSpeed))
                modifiers.FinalDamage *= 5f;
        }
        /// <summary>免死窗口内远程弹幕增伤 40%，窗口结束后召唤弹幕增伤 10%。</summary>
        private void ApplySilvaProjectileDamage(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (silvaRanged && silvaCountdown > 0 && hasSilvaEffect
                && projectile.CountsAsClass(DamageClass.Ranged))
                modifiers.FinalDamage *= 1.40f;
            if (silvaSummon && silvaCountdown <= 0 && hasSilvaEffect
                && projectile.CountsAsClass(DamageClass.Summon))
                modifiers.FinalDamage *= 1.10f;
        }
        /// <summary>免死结束后的近战接触伤害降低 20%。</summary>
        private void ApplySilvaContactReduction(ref Player.HurtModifiers modifiers)
        {
            if (silvaMelee && silvaCountdown <= 0 && hasSilvaEffect)
                modifiers.FinalDamage *= 0.80f;
        }
        /// <summary>处理近战迟滞、法师巨型爆裂和任意友方弹幕的治疗叶球。</summary>
        private void ApplySilvaProjectileHit(Projectile projectile, NPC target, NPC.HitInfo hit
            , int damageDone)
        {
            if (!silvaSet || projectile.owner != Player.whoAmI || Player.whoAmI != Main.myPlayer)
                return;
            if (silvaMelee && Main.rand.NextBool(4)
                && (projectile.CountsAsClass(DamageClass.Melee)
                    || projectile.CountsAsClass(DamageClass.MeleeNoSpeed)))
                target.AddBuff(ModContent.BuffType<SilvaHysteresis>(), 20);
            if (silvaMage && silvaMageBurstCooldown <= 0
                && projectile.CountsAsClass(DamageClass.Magic)
                && (projectile.penetrate == 1 || projectile.timeLeft <= 5)
                && Player.whoAmI == Main.myPlayer)
            {
                silvaMageBurstCooldown = 300;
                SoundEngine.PlaySound(SoundID.Zombie103, projectile.Center);
                int damage = (int)(800D + projectile.damage * 0.6D);
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center
                    , Vector2.Zero, ModContent.ProjectileType<SilvaBurst>(), damage, 8f
                    , Player.whoAmI);
            }
            if (target.canGhostHeal)
                TrySpawnSilvaHealingOrbs(projectile);
            if (auricSet && silvaThrowing && hit.Crit
                && Player.statLife > Player.statLifeMax2 * 0.5f
                && CRUtil.IsRogueDamageType(projectile.DamageType))
            {
                int bonusDamage = (int)(damageDone * 0.25f);
                if (bonusDamage > 0)
                {
                    Player.ApplyDamageToNPC(target, bonusDamage, 0f, 0, false
                        , CRUtil.GetRogueDamageClass());
                }
            }
        }
        /// <summary>按弹幕伤害比例和固定 5～10 点两条口径生成治疗叶球。</summary>
        private void TrySpawnSilvaHealingOrbs(Projectile projectile)
        {
            if (Player.whoAmI != Main.myPlayer || Player.lifeSteal <= 0f || projectile.damage <= 0)
                return;
            float ratio = 0.03f - projectile.numHits * 0.015f;
            int ratioHeal = (int)(projectile.damage * ratio);
            if (ratio > 0f && ratioHeal > 0 && Player.lifeSteal > 0f)
            {
                Player.lifeSteal -= ratioHeal * 1.5f;
                SpawnSilvaOrb(projectile, FindSilvaHealTarget(projectile, 1200f), ratioHeal);
            }
            int fixedHeal = Main.rand.Next(5, 11);
            if (Player.lifeSteal <= 0f)
                return;
            Player.lifeSteal -= fixedHeal * 2f;
            SpawnSilvaOrb(projectile, FindSilvaHealTarget(projectile, 3000f), fixedHeal);
        }
        /// <summary>生成一枚由服务端最终验证的治疗叶球。</summary>
        private void SpawnSilvaOrb(Projectile source, int targetIndex, int heal)
        {
            if (targetIndex < 0 || targetIndex >= Main.maxPlayers || heal <= 0)
                return;
            Projectile.NewProjectile(source.GetSource_FromThis(), source.Center, Vector2.Zero
                , ModContent.ProjectileType<SilvaOrb>(), 0, 0f, Player.whoAmI
                , targetIndex, heal);
        }
        /// <summary>按曼哈顿距离选择范围内生命缺口最大的友方玩家。</summary>
        private int FindSilvaHealTarget(Projectile projectile, float range)
        {
            int targetIndex = Player.whoAmI;
            int largestMissingLife = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player candidate = Main.player[i];
                if (!candidate.active || candidate.dead)
                    continue;
                bool friendly = (!Player.hostile && !candidate.hostile)
                    || Player.team == candidate.team;
                if (!friendly)
                    continue;
                float distance = System.Math.Abs(candidate.Center.X - projectile.Center.X)
                    + System.Math.Abs(candidate.Center.Y - projectile.Center.Y);
                int missingLife = candidate.statLifeMax2 - candidate.statLife;
                if (distance >= range || missingLife <= largestMissingLife)
                    continue;
                largestMissingLife = missingLife;
                targetIndex = i;
            }
            return targetIndex;
        }
        /// <summary>触发致命伤保护、生命惩罚计数和 10 秒复苏标记。</summary>
        private bool TryActivateSilvaRevival()
        {
            if (!silvaSet || silvaCountdown <= 0)
                return false;
            if (hasSilvaEffect)
                silvaHitCounter++;
            if (Player.FindBuffIndex(ModContent.BuffType<SilvaRevival>()) < 0)
            {
                SoundEngine.PlaySound(SoundID.Item60, Player.position);
                Player.AddBuff(ModContent.BuffType<SilvaRevival>(), 600);
            }
            hasSilvaEffect = true;
            if (Player.statLife < 1)
                Player.statLife = 1;
            if (Main.netMode == NetmodeID.Server)
                SyncSilvaRevivalState(Player);
            return true;
        }
        /// <summary>把免死窗口状态广播给其他客户端，避免最大生命显示和后续伤害判定分叉。</summary>
        private void SyncSilvaRevivalState(Player player)
        {
            if (Main.netMode != NetmodeID.Server)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write(MsgSilvaRevivalState);
            packet.Write((byte)player.whoAmI);
            packet.Write(silvaCountdown);
            packet.Write(hasSilvaEffect);
            packet.Write(silvaHitCounter);
            packet.Send(-1, -1);
        }
        /// <summary>客户端接收服务端确认的始源林海免死窗口状态。</summary>
        internal static void HandleSilvaRevivalState(BinaryReader reader, int whoAmI)
        {
            int playerIndex = reader.ReadByte();
            int countdown = reader.ReadInt32();
            bool effect = reader.ReadBoolean();
            int hitCounter = reader.ReadInt32();
            if (Main.netMode == NetmodeID.Server || !Main.player.IndexInRange(playerIndex))
                return;
            CalamityRebirthPlayer modPlayer = Main.player[playerIndex]
                .GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.silvaCountdown = countdown;
            modPlayer.hasSilvaEffect = effect;
            modPlayer.silvaHitCounter = hitCounter;
        }
        /// <summary>沿玩家身体生成两枚叶绿呼吸尘。</summary>
        private void SpawnSilvaAuraDust()
        {
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height
                    , DustID.ChlorophyteWeapon, 0f, 0f, 100
                    , new Color(Main.DiscoR, 203, 103), 2f);
                Main.dust[dust].position += new Vector2(Main.rand.Next(-20, 21)
                    , Main.rand.Next(-20, 21));
                Main.dust[dust].velocity *= 0.9f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale *= 1f + Main.rand.Next(40) * 0.01f;
                Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
            }
        }
        /// <summary>判断灾厄怒气、肾上腺素或暴怒是否为非伤害型伪减益。</summary>
        private static bool IsSilvaPseudoDebuff(int buffType)
        {
            silvaPseudoDebuffs ??=
            [
                CalamityCompatUtil.GetBuffType("CalamityMod", "AdrenalineMode"),
                CalamityCompatUtil.GetBuffType("CalamityMod", "RageMode"),
                CalamityCompatUtil.GetBuffType("CalamityMod", "Enraged"),
                CalamityCompatUtil.GetBuffType("CalamityModClassicPreTrailer", "AdrenalineMode"),
                CalamityCompatUtil.GetBuffType("CalamityModClassicPreTrailer", "RageMode"),
                CalamityCompatUtil.GetBuffType("CalamityModClassicPreTrailer", "Enraged"),
                CalamityCompatUtil.GetBuffType("CalRD", "AdrenalineMode"),
                CalamityCompatUtil.GetBuffType("CalRD", "RageMode"),
                CalamityCompatUtil.GetBuffType("CalRD", "Enraged")
            ];
            return silvaPseudoDebuffs.Contains(buffType);
        }
        /// <summary>热重载时清空包含外部 Buff ID 的伪减益缓存。</summary>
        internal static void ResetSilvaStaticCache()
        {
            silvaPseudoDebuffs = null;
        }
        /// <summary>是否装备远古化石。</summary>
        public bool ancientFossil;
        /// <summary>是否装备太古粉末。</summary>
        public bool archaicPowder;
        /// <summary>是否装备腐化之瓶。</summary>
        public bool corruptFlask;
        /// <summary>是否装备猩红之瓶。</summary>
        public bool crimsonFlask;
        /// <summary>是否装备辉光软泥。</summary>
        public bool radiantOoze;
        /// <summary>是否装备生命之露。</summary>
        public bool livingDew;
        /// <summary>是否装备蜜露。</summary>
        public bool honeyDew;
        /// <summary>是否装备百草天香瓶。</summary>
        public bool ambrosialAmpoule;
        /// <summary>是否装备法力凝胶。</summary>
        public bool manaJelly;
        /// <summary>是否装备生命凝胶。</summary>
        public bool lifeJelly;
        /// <summary>是否装备活力凝胶。</summary>
        public bool vitalJelly;
        /// <summary>是否装备绚彩凝胶。</summary>
        public bool grandGelatin;
        /// <summary>是否装备海螺壳。</summary>
        public bool seaShell;
        /// <summary>是否装备巨蟹甲壳。</summary>
        public bool crawCarapace;
        /// <summary>是否装备真菌甲壳。</summary>
        public bool fungalCarapace;
        /// <summary>是否装备巨型陆龟壳。</summary>
        public bool giantTortoiseShell;
        /// <summary>是否装备阿米迪亚斯之火花。</summary>
        public bool amidiasSpark;
        /// <summary>是否装备阴阳吸星石。</summary>
        public bool theAbsorber;
        /// <summary>是否装备巨型贝壳。</summary>
        public bool giantShell;
        /// <summary>是否装备涡流虹吸石。</summary>
        public bool sponge;
        /// <summary>是否具有蜂类弹幕减伤。</summary>
        public bool beeResist;
        /// <summary>甲壳爆发增益是否生效。</summary>
        public bool shellBoost;
        /// <summary>清空本帧涡流虹吸石链状态。</summary>
        public void ResetSpongeChainEffects()
        {
            ancientFossil = false;
            archaicPowder = false;
            corruptFlask = false;
            crimsonFlask = false;
            radiantOoze = false;
            livingDew = false;
            honeyDew = false;
            ambrosialAmpoule = false;
            manaJelly = false;
            lifeJelly = false;
            vitalJelly = false;
            grandGelatin = false;
            seaShell = false;
            crawCarapace = false;
            fungalCarapace = false;
            giantTortoiseShell = false;
            amidiasSpark = false;
            theAbsorber = false;
            giantShell = false;
            sponge = false;
            beeResist = false;
            shellBoost = false;
        }
        /// <summary>结算整条链的常驻面板、环境效果与静态再生。</summary>
        public void ApplySpongeChainEffects()
        {
            if (ancientFossil && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight))
                Player.pickSpeed -= 0.35f;
            if (archaicPowder && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight
                || Player.ZoneUnderworldHeight))
            {
                Player.statDefense += 3;
                Player.endurance += 0.07f;
                Player.pickSpeed -= 0.5f;
            }
            if (corruptFlask && Player.ZoneCorrupt)
                AddFlaskBonus();
            if (crimsonFlask && Player.ZoneCrimson)
                AddFlaskBonus();
            if (radiantOoze && !Main.dayTime)
            {
                Lighting.AddLight(Player.Center / 16f, 1f, 1f, 0.6f);
                Player.lifeRegen += 2;
            }
            if (livingDew && Player.ZoneJungle)
                AddJungleDewBonus();
            if (honeyDew)
            {
                if (Player.ZoneJungle)
                    AddJungleDewBonus();
                Player.buffImmune[BuffID.Poisoned] = true;
                Player.buffImmune[BuffID.Venom] = true;
                ApplyHoneyRegen();
                Player.lifeRegenTime += 2;
                Player.lifeRegen += 2;
            }
            if (manaJelly)
            {
                Player.statManaMax2 += 20;
                if (IsStandingStill())
                    Player.manaRegenBonus += 2;
            }
            if (lifeJelly)
            {
                Player.statLifeMax2 += 20;
                if (IsStandingStill())
                    Player.lifeRegen += 2;
            }
            if (vitalJelly)
            {
                Player.moveSpeed += 0.1f;
                Player.jumpSpeedBoost += 1f;
            }
            if (grandGelatin)
            {
                Player.moveSpeed += 0.1f;
                Player.jumpSpeedBoost += 1f;
                Player.statLifeMax2 += 20;
                Player.statManaMax2 += 20;
                if (IsStandingStill())
                {
                    Player.lifeRegen += 2;
                    Player.manaRegenBonus += 2;
                }
            }
            if (seaShell && IsSubmerged())
                AddSubmergedShellBonus();
            if (crawCarapace)
            {
                Player.endurance += 0.05f;
                Player.thorns = Math.Max(Player.thorns, 0.25f);
            }
            if (giantTortoiseShell)
            {
                Player.moveSpeed -= 0.1f;
                Player.thorns = Math.Max(Player.thorns, 0.25f);
            }
            if (theAbsorber)
            {
                Player.statLifeMax2 += 30;
                Player.statManaMax2 += 20;
                Player.moveSpeed += 0.12f;
                Player.jumpSpeedBoost += 1.2f;
                Player.thorns = 1f;
                Player.endurance += 0.06f;
                if (IsStandingStill())
                {
                    Player.lifeRegen += 6;
                    Player.manaRegenBonus += 2;
                }
                if (IsSubmerged())
                {
                    Player.statDefense += 5;
                    Player.endurance += 0.05f;
                    Player.moveSpeed += 0.2f;
                    Player.ignoreWater = true;
                }
            }
            if (giantShell)
                Player.moveSpeed -= 0.15f;
            if (ambrosialAmpoule)
                ApplyAmbrosialAmpouleEffects();
            if (sponge)
                ApplySpongeEffects();
            if (shellBoost)
            {
                Player.panic = false;
                Player.moveSpeed += 0.9f;
            }
        }
        /// <summary>应用百草天香瓶的减伤、挖掘、照明、再生、免疫和蜂蜜恢复。</summary>
        private void ApplyAmbrosialAmpouleEffects()
        {
            Player.endurance += 0.12f;
            Player.pickSpeed -= 0.5f;
            Lighting.AddLight(Player.Center / 16f, 1f, 1f, 0.6f);
            Player.lifeRegen += 4;
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Venom] = true;
            ApplyHoneyRegen();
            Player.lifeRegenTime += 2;
        }
        /// <summary>应用涡流虹吸石的生存、浸水、挖掘、照明与免疫。</summary>
        private void ApplySpongeEffects()
        {
            Player.lifeRegen += 6;
            Player.endurance += 0.18f;
            Player.statLifeMax2 += 30;
            Player.statManaMax2 += 30;
            Player.moveSpeed += 0.12f;
            Player.jumpSpeedBoost += 1.2f;
            Player.thorns = 1f;
            if (IsStandingStill())
            {
                Player.lifeRegen += 6;
                Player.manaRegenBonus += 2;
            }
            if (IsSubmerged())
            {
                Player.statDefense += 5;
                Player.endurance += 0.1f;
                Player.moveSpeed += 0.2f;
                Player.ignoreWater = true;
            }
            Player.pickSpeed -= 0.5f;
            Lighting.AddLight(Player.Center / 16f, 1f, 1f, 0.6f);
            Player.buffImmune[BuffID.Chilled] = true;
            Player.buffImmune[BuffID.Frostburn] = true;
            Player.buffImmune[BuffID.Frostburn2] = true;
            Player.buffImmune[BuffID.Frozen] = true;
            Player.buffImmune[BuffID.Poisoned] = true;
            Player.buffImmune[BuffID.Venom] = true;
            ApplyHoneyRegen();
            Player.lifeRegenTime += 2;
        }
        /// <summary>添加邪恶生态下的三点防御与七百分点减伤。</summary>
        private void AddFlaskBonus()
        {
            Player.statDefense += 3;
            Player.endurance += 0.07f;
        }
        /// <summary>添加丛林露水系列的再生、防御与减伤。</summary>
        private void AddJungleDewBonus()
        {
            Player.lifeRegen += 2;
            Player.statDefense += 5;
            Player.endurance += 0.1f;
        }
        /// <summary>添加浸水海螺壳的防御、减伤、移速与水体忽略。</summary>
        private void AddSubmergedShellBonus()
        {
            Player.statDefense += 3;
            Player.endurance += 0.05f;
            Player.moveSpeed += 0.15f;
            Player.ignoreWater = true;
        }
        /// <summary>在未泡蜂蜜且负再生时抵消四点，并保证结果不超过零。</summary>
        private void ApplyHoneyRegen()
        {
            if (Player.honey || Player.lifeRegen >= 0)
                return;
            Player.lifeRegen += 4;
            if (Player.lifeRegen > 0)
                Player.lifeRegen = 0;
        }
        /// <summary>判断玩家是否完全静止且没有物品挥动。</summary>
        private bool IsStandingStill()
        {
            return Math.Abs(Player.velocity.X) < 0.05f
                && Math.Abs(Player.velocity.Y) < 0.05f && Player.itemAnimation == 0;
        }
        /// <summary>判断玩家是否处于完整溺水碰撞环境。</summary>
        private bool IsSubmerged()
        {
            return Collision.DrownCollision(Player.position, Player.width, Player.height, Player.gravDir);
        }
        /// <summary>蜂类弹幕命中时降低至百分之七十五伤害。</summary>
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (beeResist && IsBeeProjectile(proj.type))
                modifiers.FinalDamage *= 0.75f;
        }
        /// <summary>判断原版与三个分支中的蜂刺类弹幕。</summary>
        private static bool IsBeeProjectile(int type)
        {
            if (type == ProjectileID.Stinger || type == ProjectileID.HornetStinger)
                return true;
            return IsNamedProjectile(type, "CalamityMod", "PlagueStingerGoliath")
                || IsNamedProjectile(type, "CalamityMod", "PlagueStingerGoliathV2")
                || IsNamedProjectile(type, "CalamityModClassicPreTrailer", "PlagueExplosion")
                || IsNamedProjectile(type, "CalamityModClassicPreTrailer", "PlagueStingerGoliath")
                || IsNamedProjectile(type, "CalamityModClassicPreTrailer", "PlagueStingerGoliathV2")
                || IsNamedProjectile(type, "CalRD", "PlagueExplosion")
                || IsNamedProjectile(type, "CalRD", "PlagueStingerGoliath")
                || IsNamedProjectile(type, "CalRD", "PlagueStingerGoliathV2");
        }
        /// <summary>比较指定外部模组弹幕类型。</summary>
        private static bool IsNamedProjectile(int type, string modName, string projectileName)
        {
            return ModContent.TryFind(modName, projectileName, out ModProjectile projectile)
                && projectile.Type == type;
        }
        /// <summary>在受伤结算前按阴阳吸星石或涡流虹吸石比例直接治疗。</summary>
        public void ApplySpongeAbsorbHeal(ref Player.HurtModifiers modifiers)
        {
            if (theAbsorber)
                HealBeforeDamage((int)modifiers.SourceDamage.Base / 20);
            if (sponge)
                HealBeforeDamage((int)modifiers.SourceDamage.Base / 16);
        }
        /// <summary>在受击后触发甲壳爆发。</summary>
        public void ApplySpongeChainOnHurt()
        {
            if ((theAbsorber || giantShell || sponge) && !Player.panic)
                Player.AddBuff(ModContent.BuffType<ShellBoost>(), 300);
        }
        /// <summary>在受击后生成火花与松露孢子反击。</summary>
        public void ApplySpongeChainPostHurt(Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Player.whoAmI != Main.myPlayer)
                return;
            if (amidiasSpark || theAbsorber)
                SpawnSparkBurst(Main.hardMode ? 36 : 6);
            if (sponge)
                SpawnSparkBurst(Main.hardMode ? 144 : 36);
            if (fungalCarapace || theAbsorber)
                SpawnSporeBurst((int)Player.GetDamage(DamageClass.Generic).ApplyTo(70));
            if (sponge)
                SpawnSporeBurst(448);
        }
        /// <summary>从玩家中心向八个方向生成近战火花。</summary>
        private void SpawnSparkBurst(int damage)
        {
            SoundEngine.PlaySound(SoundID.Item93, Player.position);
            SpawnBidirectionalBurst(Player.Center, damage, ModContent.ProjectileType<Spark>());
        }
        /// <summary>从玩家两侧生成八颗松露孢子。</summary>
        private void SpawnSporeBurst(int damage)
        {
            SoundEngine.PlaySound(SoundID.NPCHit45, Player.position);
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            for (int i = 0; i < 4; i++)
            {
                float x = Main.rand.NextBool(2) ? Player.Center.X + 100f : Player.Center.X - 100f;
                Vector2 origin = new(x, Player.Center.Y + Main.rand.Next(-100, 101));
                double angle = startAngle + deltaAngle * (i + i * i) / 2d + 32d * i;
                SpawnPair(origin, angle, damage, ProjectileID.TruffleSpore);
            }
        }
        /// <summary>在指定中心生成八向火花。</summary>
        private void SpawnBidirectionalBurst(Vector2 origin, int damage, int projectileType)
        {
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            for (int i = 0; i < 4; i++)
            {
                double angle = startAngle + deltaAngle * (i + i * i) / 2d + 32d * i;
                SpawnPair(origin, angle, damage, projectileType);
            }
        }
        /// <summary>沿给定角度的正反方向各生成一枚一百二十帧弹幕。</summary>
        private void SpawnPair(Vector2 origin, double angle, int damage, int projectileType)
        {
            float speedX = (float)Math.Sin(angle) * 5f;
            float speedY = (float)Math.Cos(angle) * 5f;
            int first = Projectile.NewProjectile(Entity.GetSource_FromThis(), origin, new Vector2(speedX, speedY)
                , projectileType, damage, 1.25f, Player.whoAmI);
            int second = Projectile.NewProjectile(Entity.GetSource_FromThis(), origin, new Vector2(-speedX, -speedY)
                , projectileType, damage, 1.25f, Player.whoAmI);
            if (Main.projectile.IndexInRange(first))
                Main.projectile[first].timeLeft = 120;
            if (Main.projectile.IndexInRange(second))
                Main.projectile[second].timeLeft = 120;
        }
        /// <summary>在伤害结算前直接治疗并限制到最大生命。</summary>
        private void HealBeforeDamage(int amount)
        {
            if (amount <= 0 || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            Player.Heal(amount);
            Player.HealEffect(amount, true);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null
                    , Player.whoAmI, Player.statLife, Player.statMana);
        }
        /// <summary>本帧是否穿着任意龙蒿三件套。</summary>
        public bool tarraSet;
        /// <summary>本帧是否穿着龙蒿近战头套装。</summary>
        public bool tarraMelee;
        /// <summary>本帧是否穿着龙蒿远程头套装。</summary>
        public bool tarraRanged;
        /// <summary>本帧是否穿着龙蒿法师头套装。</summary>
        public bool tarraMage;
        /// <summary>本帧是否穿着龙蒿召唤头套装。</summary>
        public bool tarraSummon;
        /// <summary>本帧是否穿着龙蒿盗贼头套装。</summary>
        public bool tarraThrowing;
        /// <summary>本帧是否受到龙蒿生命再生增益。</summary>
        public bool tarraLifeRegen;
        /// <summary>龙蒿生命光环的跨帧计时器。</summary>
        private int tarraLifeAuraTimer;
        /// <summary>龙蒿法师法弹治疗冷却。</summary>
        public int tarraMageHealCooldown;
        /// <summary>累计魔法暴击次数；满 5 次后由下一次魔法射击消耗。</summary>
        public int tarraCrits;
        /// <summary>盗贼暴击计数；满 25 次触发免伤。</summary>
        public int tarraThrowingCrits;
        /// <summary>盗贼暴击免伤冷却。</summary>
        public int tarraThrowingCritTimer;
        /// <summary>近战生命护罩剩余可用冷却。</summary>
        private int tarraCooldown;
        /// <summary>近战生命护罩是否正在生效。</summary>
        private bool tarraDefense;
        /// <summary>近战生命护罩剩余持续帧数。</summary>
        private int tarraDefenseTime = 600;
        /// <summary>伪减益缓存；这些 buff 在 Main.debuff 中为真但不应计入盗贼增伤。</summary>
        private static HashSet<int> tarraPseudoDebuffs;
        /// <summary>清空本帧龙蒿套装与增益状态。</summary>
        private void ResetTarragonEffects()
        {
            tarraSet = false;
            tarraMelee = false;
            tarraRanged = false;
            tarraMage = false;
            tarraSummon = false;
            tarraThrowing = false;
            tarraLifeRegen = false;
        }
        /// <summary>死亡时清空护罩和跨帧计数，避免复活继承旧状态。</summary>
        private void ResetTarragonStateOnDeath()
        {
            tarraLifeAuraTimer = 0;
            tarraMageHealCooldown = 0;
            tarraCrits = 0;
            tarraThrowingCrits = 0;
            tarraThrowingCritTimer = 0;
            tarraCooldown = 0;
            tarraDefense = false;
            tarraDefenseTime = 600;
        }
        /// <summary>
        /// 结算龙蒿通用、召唤、近战护罩计时、法师冷却和盗贼套装。
        /// 只在拥有对应职业套装时执行，通用效果只依赖 tarraSet。
        /// </summary>
        private void ApplyTarragonEffects()
        {
            if (tarraSet)
            {
                Player.calmed = !tarraMelee;
                Player.lifeMagnet = true;
            }
            if (tarraSummon)
            {
                Lighting.AddLight(Player.Center, 0f, 3f, 0f);
                if (Player.statLife >= Player.statLifeMax2)
                {
                    Player.GetDamage(DamageClass.Summon) += 0.10f;
                    Player.maxMinions += 2;
                }
                tarraLifeAuraTimer = (tarraLifeAuraTimer + 1) % 80;
                if (tarraLifeAuraTimer == 0 && Player.whoAmI == Main.myPlayer)
                    ApplyTarragonLifeAura();
            }
            if (tarraMelee)
            {
                if (tarraDefense)
                {
                    tarraDefenseTime--;
                    if (tarraDefenseTime <= 0)
                    {
                        tarraDefenseTime = 600;
                        tarraCooldown = 1800;
                        tarraDefense = false;
                    }
                    SpawnTarragonCloakDust();
                }
                if (tarraCooldown > 0)
                    tarraCooldown--;
            }
            if (tarraMageHealCooldown > 0)
                tarraMageHealCooldown--;
            if (tarraThrowing)
            {
                Player.immuneTime = Math.Min(Player.immuneTime, 300);
                if (tarraThrowingCritTimer > 0)
                    tarraThrowingCritTimer--;
                if (tarraThrowingCrits >= 25)
                {
                    tarraThrowingCrits = 0;
                    tarraThrowingCritTimer = 1800;
                    Player.immune = true;
                    Player.immuneTime = 300;
                }
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffType = Player.buffType[i];
                    if (buffType > 0 && Player.buffTime[i] > 0 && Main.debuff[buffType]
                        && !IsTarragonPseudoDebuff(buffType))
                        CRUtil.ApplyRogueStats(Player, 0.10f, 0, 0f, 0f, 0f, 0f);
                }
            }
        }
        /// <summary>以 120 基础召唤伤害结算半径 300 像素的生命光环。</summary>
        private void ApplyTarragonLifeAura()
        {
            int damage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(120f);
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                    continue;
                if (Vector2.Distance(Player.Center, npc.Center) <= 300f)
                    Player.ApplyDamageToNPC(npc, damage, 0f, 0, false, DamageClass.Summon);
            }
        }
        /// <summary>在护罩持续期间生成绿色生命粒子。</summary>
        private void SpawnTarragonCloakDust()
        {
            for (int i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(Player.position, Player.width, Player.height
                    , DustID.ChlorophyteWeapon, 0f, 0f, 100
                    , new Color(Main.DiscoR, 203, 103), 2f);
                Main.dust[dust].position += new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21));
                Main.dust[dust].velocity *= 0.9f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale *= 1f + Main.rand.Next(40) * 0.01f;
                Main.dust[dust].shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
            }
        }
        /// <summary>近战龙蒿套受击时以 25% 概率获得 90～179 帧生命再生。</summary>
        private void ApplyTarragonMeleeOnHurt()
        {
            if (tarraMelee && Main.rand.NextBool(4))
                Player.AddBuff(ModContent.BuffType<TarraLifeRegen>(), Main.rand.Next(90, 180));
        }
        /// <summary>龙蒿近战护罩持续期间将 NPC 接触伤害减半。</summary>
        private void ApplyTarragonContactReduction(ref Player.HurtModifiers modifiers)
        {
            if (tarraDefense && tarraMelee)
                modifiers.FinalDamage *= 0.5f;
        }
        /// <summary>读取龙蒿快捷键并在冷却结束后启动 10 秒生命护罩。</summary>
        private void TryActivateTarragonCloak()
        {
            if (tarraMelee && !tarraDefense && tarraCooldown <= 0)
            {
                tarraDefense = true;
                tarraDefenseTime = 600;
            }
        }
        /// <summary>
        /// 结算龙蒿远程、法师和盗贼弹幕命中：
        /// 远程暴击产生树叶，法师累计暴击并受限治疗，盗贼暴击累计免伤。
        /// </summary>
        private void ApplyTarragonProjectileHit(Projectile projectile, NPC target
            , NPC.HitInfo hit)
        {
            if (projectile.owner != Player.whoAmI || Player.whoAmI != Main.myPlayer)
                return;
            if (tarraRanged && hit.Crit && projectile.CountsAsClass(DamageClass.Ranged))
            {
                int leafCount = Main.rand.Next(2, 4);
                int leafDamage = (int)(projectile.damage * 0.25f);
                for (int i = 0; i < leafCount; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7f, 10f);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), target.Center, velocity
                        , ProjectileID.Leaf, leafDamage, 0f, Player.whoAmI);
                }
            }
            if (tarraMage)
            {
                bool magicProjectile = projectile.CountsAsClass(DamageClass.Magic);
                if (hit.Crit && magicProjectile)
                    tarraCrits++;
                if (magicProjectile && target.canGhostHeal
                    && tarraMageHealCooldown <= 0 && projectile.damage > 0)
                {
                    tarraMageHealCooldown = 90;
                    if (Player.lifeSteal > 0f)
                        RequestHeal(Player, HealRequestTarragon, projectile.damage);
                }
            }
            if (tarraThrowing && hit.Crit && tarraThrowingCritTimer <= 0
                && tarraThrowingCrits < 25 && CRUtil.IsRogueDamageType(projectile.DamageType))
                tarraThrowingCrits++;
        }
        /// <summary>判断 buff 是否为肾上腺素、怒气或其他战斗伪减益。</summary>
        private static bool IsTarragonPseudoDebuff(int buffType)
        {
            tarraPseudoDebuffs ??=
            [
                CalamityCompatUtil.GetBuffType("CalamityMod", "AdrenalineMode"),
                CalamityCompatUtil.GetBuffType("CalamityMod", "RageMode"),
                CalamityCompatUtil.GetBuffType("CalamityMod", "Enraged"),
                CalamityCompatUtil.GetBuffType("CalamityModClassicPreTrailer", "AdrenalineMode"),
                CalamityCompatUtil.GetBuffType("CalamityModClassicPreTrailer", "RageMode"),
                CalamityCompatUtil.GetBuffType("CalamityModClassicPreTrailer", "Enraged"),
                CalamityCompatUtil.GetBuffType("CalRD", "AdrenalineMode"),
                CalamityCompatUtil.GetBuffType("CalRD", "RageMode"),
                CalamityCompatUtil.GetBuffType("CalRD", "Enraged")
            ];
            return tarraPseudoDebuffs.Contains(buffType);
        }
        /// <summary>热重载时清空包含外部 Buff ID 的伪减益缓存。</summary>
        internal static void ResetTarragonStaticCache()
        {
            tarraPseudoDebuffs = null;
        }
		/// <summary>本帧是否获得天使之靴的飞行时间加成。</summary>
		public bool angelTreads;
		/// <summary>本帧是否装备炽天使翼靴。</summary>
		public bool seraphTracers;
		/// <summary>清空翼靴链的逐帧状态。</summary>
		public void ResetWingChainEffects()
		{
			angelTreads = false;
			seraphTracers = false;
		}
		/// <summary>结算天使之靴飞行时间与炽天使翼靴的灾厄兼容标记。</summary>
		public void ApplyWingChainEffects()
		{
			if (angelTreads)
				Player.wingTimeMax = (int)(Player.wingTimeMax * 1.1f);
			if (seraphTracers)
				CalamityCompatUtil.MirrorSeraphTracersToCalamity(Player);
		}
		/// <summary>
		/// 炽天使翼靴在单次受伤超过二百点时追加三十帧无敌。
		/// 只由本地玩家写入，随后通过玩家状态同步到其他端。
		/// </summary>
		public void ApplyWingChainPostHurt(Player.HurtInfo info)
		{
			if (seraphTracers && Player.whoAmI == Main.myPlayer && info.Damage > 200)
				Player.immuneTime += 30;
		}
    }
}
