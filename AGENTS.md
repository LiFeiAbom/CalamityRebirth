# CalamityRebirth — 工程记忆（新会话先读）
> 最后整理：2026-11-26
> 当前目标是按用户点名的非武器内容，从 `CalamityExpansion` 与 `CalamityDemutation` 分步移植；
> 任何武器、武器专属弹幕、贴图和音效均不在移植范围内，也不是一次性全量复制。
> 当前版本：`0.1.2.7`
> 当前代码规模：324 个手写 C# 文件，空行数 0
> 最近审计（2026-11-26）：编译 0 警告、0 错误；中英文 Hjson 均为 524 条路径，
> 缺失、多余与类型差异均为 0；全部武器及专属内容已移除。魔影盔甲与紫色魔影灼燃已完成，
> 待进游戏回归项见第 30 节。
> 最近目录整理（2026-11-19）：饰品已按主用途分类，现有 `Combat 44`、`Movement 1`、
> `HealthandMana 4`、`Construction 1`；`Information/Immunity/Fishing/Miscellaneous` 已建为空门类。
## 1. 工程身份与硬口径
- 工程目录：`C:\Users\28155\Documents\My Games\Terraria\tModLoader\ModSources\CalamityRebirth`
- 模组名：`CalamityRebirth`
- 主类：`CalamityRebirth`
- 玩家结算类：`CalamityRebirthPlayer`
- 武器硬口径：不得新增、恢复或引进任何武器，也不得加入武器专属贴图、弹幕、音效和本地化。
  饰品与盔甲自身提供的战斗效果不属于武器本体，继续保留；当前全部武器内容已于 2026-11-26 移除。
- 饰品按主用途分类，根目录固定为：
  `Combat`、`Movement`、`Information`、`Immunity`、`Construction`、`HealthandMana`、`Fishing`、`Miscellaneous`。
  战斗职业饰品放 `Combat/<职业>`，综合战斗核心直接放 `Combat`；物品有多个效果时必须按主要用途归类。
  生命、防御、恢复和生命宝石链固定放 `HealthandMana`。不要恢复 `JobAcc`。
- Buff 统一位于 `Content/ModBuffs`：正面增益与增益基类放 `Buffs`，负面减益、酒精中毒与酒类持久减益放 `Debuffs`。
- 所有 Global 类固定使用模组名前缀：
  - `CalamityRebirthGlobalItem`
  - `CalamityRebirthGlobalProjectile`
  - `CalamityRebirthGlobalNPC`
- 用户要求 C# 文件不留空行；新增或移植文件完成后应清理空行。
- 全部代码开发必须补充详细注释：类型与成员使用 `///` 文档注释，非直观流程、兼容分支、联网/绘制约束和数值公式使用就地注释。
- C# 注释只描述功能、数值和实现方式，不记录内容出处、移植来源、参照工程或原件归属。
- Utilities 通用化原则：
  - 初始的 `WeaponUtil`、`MaterialUtil`、`ProjectileUtil`、`RogueUtil`、`CollisionUtil`
    是稳定拆分的基础工具，不得为了“整合”而合并、改名或迁移，除非用户明确点名。
  - 只整理职责过窄、绑定具体物品链或带版本前缀的工具。
  - 稀有度与价格使用 `RarityUtil`，不再使用 `ModernRarityValueUtil`。
  - 减益查询、环境增益等通用 Buff 逻辑统一放 `BuffUtil`。
  - `CalamityCompatUtil`、`PerPlayerDropRule` 等已有清晰通用职责的类型保持独立。
  - 工具重命名或整合后必须同步全部调用点、清除旧名称并重新编译。
- 配方优先使用对应灾厄版本的原生物品，并通过弱依赖查找；只有该版本确实删除了某个中间物时，
  才把该中间物自身的配方原料直接展开到最终物品配方中，不得在本工程新增同名材料类物品补位。
- `build.txt` 只保存构建字段。当前弱依赖：
  `CalamityMod, CalamityModClassicPreTrailer, CalRD`
- 模组简介在 `description.txt` 与 `description_workshop.txt`，不要在 `build.txt` 写 `description`。
  `description.txt` 保持原有短简介；`description_workshop.txt` 当前为完整中英双语创意工坊文本，且无空行分隔。
- 根目录只保留最终图标：
  `icon.png`、`icon_small.png`、`icon_workshop.png`。
## 2. 参考源码
- 主要移植来源：`D:\Game\Terraria\PrivateMod\CalamityExpansion`
- 历史实现参考：`D:\Game\Terraria\PrivateMod\CalamityDemutation`
- 三版灾厄参考根目录：`D:\Game\Terraria\ReferenceMod`。后续核对原版配方、稀有度和基础属性时直接从该根目录检索，用户无需重复提供完整路径。
- 三版灾厄源码：
  - `D:\Game\Terraria\ReferenceMod\CalamityModPublic-1.4.4-release\CalamityModPublic-1.4.4-release`（实际版本 2.2.2）
  - `D:\Game\Terraria\ReferenceMod\CalamityModClassic-cal-1.4.2.101\CalamityModClassic-cal-1.4.2.101`
  - `D:\Game\Terraria\ReferenceMod\Calamity-Mod-Classic-1.4.5.7-main\Calamity-Mod-Classic-1.4.5.7-main`
- 内部模组名：
  - 现代灾厄：`CalamityMod`
  - 经典 1.4.2.101：`CalamityModClassicPreTrailer`
  - 经典 1.4.5.7：`CalRD`
- 1.4.5.7 没有 `CosmicAnvil`，对应高级配方应使用 `DraedonsForge`。
## 3. 当前已移植内容
### 3.1 药水与永久解锁
- `魔君牌兴奋剂 / Yharim's Stimulants`
- 30 分钟增益，Orange 稀有度，售价 40 银币。
- 增益：
  - 全伤害 +22%
  - 全暴击 +10
  - 近战攻速 +12%
  - 防御 +10
  - 伤害减免 +6%
  - 移动速度 +15%
  - 召唤击退 ×1.5
  - 生命再生 +1 HP/s，对应 `Player.lifeRegen += 2`
- 配方：
  - 六种原版药水 @ 炼药桌
  - 血珠 ×50 + 瓶装水 @ 炼药桌
- 已接现代灾厄、1.4.2 经典和 `CalRD`；1.4.5.7 中 `BloodOrb` 已确认存在。
- `龙舌兰日出 / Tequila Sunrise`
  - 28×18、堆叠 30、浅红稀有度、17 帧饮用动作、5 分钟增益。
  - 价值采用现代 2.2.2 的显式 2 银，不套用浅红标准价。
  - 昼夜通用：伤害 +3%、暴击 +2、减伤 +3%、击退 +3%。
  - 白天额外：伤害 +7%、暴击 +3、防御 +10、减伤 +7%、击退 +7%；
    防御按用户要求从参照版 +15 调整为 +10。
  - 生命再生 `lifeRegen -= 10`，约 -5 HP/s；酒醉等级 +1。
  - 双配方：麦芽酒 + `ScoriaOre/CruptixBar/ChaoticOre`×3 @ 酒桶；
    瓶装水 + 血珠×5 + 同类材料×1 @ 炼药桌。
  - 已加入本地酒醉等级和 `AlcoholPoisoning` 提示框架，供后续酒类共用。
- 参照版剩余药水已完成移植；其中龙魂秘药随后按用户要求彻底删除，当前新增药水为 20 件：
  - Agentia 药剂：渊洋秘药、弹跳药水、韵律药水、钙质药水、无尽饥饿药水、崩裂药水、
    神圣之怒药水、光合作用药水、渎神之怒药水、新生药水、粉碎药水、翱翔药水、泰坦之鳞药水、胜利药水。
  - 酒类：血腥玛丽、加勒比朗姆酒、摩闪酒、莫斯科骡子、朗姆酒、伏特加。
- Agentia 公共基类 `AgentiaPotionItem` 统一 28×18、17 帧进食、消耗品与血珠兜底配方；
  个别现代版保留的药水使用现代价格/稀有度：Bounding/Calcium 为蓝色 2 银，
  Ceaseless Hunger 为 PureGreen 10 银，Photosynthesis/Soaring 为浅红 2 银。
- 酒类公共基类 `AlcoholPotionItem` 统一现代显式 2 银价格、17 帧饮用动作、饮用粒子、酒醉等级与持久减益标记。
- 命中类药水已接入物品、弹幕和 PvP：Holy Wrath 施加 HolyFlames/HolyLight，
  Crumbling/Shattering 施加 ArmorCrunch；所有灾厄 buff 均弱依赖查找。
- 行为类药水已接入原 Hook：Ceaseless Hunger 吸附掉落物、Cadence 商店/磁铁/生命、
  Photosynthesis 静止再生与红心额外治疗、Revivify 受击回复、Triumph 接触减伤、
  Bloody Mary 血月刷怪、Calcium 防摔、Bounding/Soaring/Draconic 飞行或跳跃属性。
- 现代高阶配方直接使用四色月亮碎片各一份替代银河奇点；经典两版继续使用原生银河奇点。
- `天界洋葱 / Celestial Onion`
  - 专家/大师世界月亮领主掉落
  - 使用后永久解锁一个额外饰品槽，不可逆
  - 解锁状态通过 `SaveData` / `LoadData` 持久化
  - 联机状态由 `MsgPermanentUnlock` 消息同步，服务器以连接玩家索引为准
### 3.2 近战饰品
- `战士徽章EX / Warrior Emblem EX`
  - 近战伤害 +18%
  - 近战暴击 +8
  - 近战攻速 +12%
  - 稀有度：`WarriorEXRarity`
  - 售价 3 金
  - 近战命中附加燃烧III
- `烈火手套EX / Fire Gauntlet EX`
  - 近战伤害、暴击、攻速各 +22%
  - 防御 +8、仇恨 +400、生命再生 +1 HP/s、召唤击退 +1
  - 复合烈火手套与天界壳功能
  - 近战命中附加燃烧III与破晓II
  - 完整移植了破晓II的橙红星屑、NPC 橙红染色、死亡爆尘与传染
- 燃烧III：
  - 300 帧
  - 100 HP/s
  - 防御 -10
  - 受到伤害 +5%
- 破晓II：
  - 300 帧
  - 250 HP/s
  - 防御 -25
  - 受到伤害 +10%
  - 死亡时向 100 像素内敌怪传染
### 3.3 远程饰品
- `游侠徽章EX / Ranger Emblem EX`
  - 远程伤害 +18%
  - 远程暴击 +8
  - 射击速度 +12%
  - 弹速 +8%
  - 省弹概率 20%
  - 稀有度：`RangerEXRarity`
- `代达罗斯纹章 / Daedalus Emblem`
  - 直接移植自 `CalamityDemutation`，贴图逐字节一致。
  - 远程伤害 +15%、远程暴击 +10、省弹概率 20%、防御 +5、生命再生 +1 HP/s、
    召唤击退 +0.5、挖掘速度 +15%。
  - 稀有度按用户要求与现代 `Dynamo Stem Cells` 一致，使用原版紫色；价格保留原版 30 金。
  - 现代 2.2.2、经典 1.4.2.101 与 CalRD 均使用天界石 + 原生 `CoreofCalamity` + 游侠徽章 @ 秘银砧。
- `元素箭袋 / Elemental Quiver`
  - 主体直接移植自 `CalamityDemutation`，贴图逐字节一致。
  - 远程伤害 +20%、远程暴击 +20、省弹概率 20%、防御 +5、生命再生 +1 HP/s、
    召唤击退 +0.5、挖掘速度 +15%。
  - 稀有度与元素之握一致，使用 `CosmicPurpleRarity`；价格保留原版 90 金。
  - 友方箭类远程弹幕每个 AI 更新节拍约 0.5% 概率分裂为两枚镜像箭；
    子箭继承 50% 伤害、持续 60 帧、不掉落物品且不能继续分裂。
  - 额外让非箭类远程弹幕获得 5% 最终乘算伤害，NPC 与 PvP 共用同一倍率。
  - 现代与 1.4.2.101 使用 Demutation 配方；CalRD 使用化神魂精×4 @ 嘉登熔炉。
- `狙击镜EX / Sniper Scope EX`
  - 远程伤害、暴击、射击速度、弹速各 +22%
  - 省弹概率 40%
  - 仇恨 -400
  - 熔岩箭袋、潜行者箭袋的非伤害加成
  - 可见时提供步枪瞄准镜缩放
  - 稀有度：`RangerEXRarity`
### 3.4 法师饰品
- `法师徽章EX / Sorcerer Emblem EX`
  - 魔法伤害 +18%、暴击 +8、施法速度 +12%
  - 最大魔力 +100、魔力消耗 -10%
  - 额外回复 2 魔力/秒
- `天界纹章EX / Celestial Emblem EX`
  - 魔法伤害、暴击、施法速度各 +22%
  - 最大魔力 +200、魔力消耗 -40%
  - 额外回复 4 魔力/秒
  - 免疫魔力病
  - 奥术花、天界手铐、魔力斗篷的非数值能力
  - 稀有度：`SorcererEXRarity`
- `空灵护符 / Ethereal Talisman`
  - 直接移植自 `CalamityDemutation`，贴图逐字节一致；贴图原尺寸 52×40，判定尺寸 28×32。
  - 魔法伤害 +20%、魔法暴击 +20、最大魔力 +150、魔力消耗 -20%。
  - 提供探宝、缩短治疗药水病与自动使用魔力药水。
  - 稀有度按用户要求改为弑神者紫色档，即本工程 `CosmicPurpleRarity`；价格保留 90 金。
  - 配方统一消耗本工程 `灾厄符章`，形成完整的本地前置链。
  - 现代：灾厄符章 + `AnyManaFlower` + 化神魂精×4 @ 星宇砧。
  - 经典 1.4.2.101：灾厄符章 + 魔力花 + 幻影质/梦魇燃料/吸热能量各20 @ 嘉登熔炉。
  - CalRD 1.4.5.7：灾厄符章 + 魔力花 + 化神魂精×4 @ 嘉登熔炉。
- `灾厄符章 / Sigil of Calamitas`
  - 直接移植自 `CalamityDemutation`，34×352 八帧纵向灵魂动画贴图逐字节一致。
  - 魔法伤害 +15%、魔法暴击 +10、最大魔力 +100、魔力消耗 -15%、探宝与缩短药水病。
  - 稀有度按现代版灾厄符章修正为原版黄色。
  - 现代：天界徽章 + 焦炭矿锭×8 + 灾厄之烬×15 @ 秘银砧。
  - 经典 1.4.2.101：神话护身符 + 法师徽章 + 水晶碎块×20 + 灾厄尘×5 + 混乱核心×5
    + 法术书 + 本工程混沌护符 + 邪水/血水×10 @ 秘银砧。
  - CalRD 1.4.5.7：法师徽章 + 天界手铐 + 水晶碎块×20 + 灾厄尘×5 + 混乱核心×5
    + Cruptix 锭×2 + 本工程混沌护符 + `AnyEvilWater`×10 @ 秘银砧。
- `混沌护符 / Chaos Amulet`
  - 直接移植自 `CalamityDemutation`，贴图逐字节一致；固定放在
    `Content/Items/Accessories/Construction`，不要移回战斗饰品目录。
  - +1 HP/s 生命再生、探宝、魔法伤害 +2%、魔法暴击 +2、最大魔力 +20。
  - 稀有度按现代后继 `SpelunkersAmulet` 修正为浅红色。
  - 现代：狱石锭×2 + 灾厄之烬×2 + 洞穴探险药水×7 @ 秘银砧。
  - 经典 1.4.2.101：Cruptix 锭×2 + 洞穴探险药水×7 @ 秘银砧。
  - CalRD 1.4.5.7：混乱精华×7 + 洞穴探险药水×7 @ 秘银砧。
### 3.5 召唤饰品
- `召唤师徽章EX / Summoner Emblem EX`
  - 召唤伤害 +18%
  - 召唤栏 +2、哨兵栏 +1
  - 鞭子攻速与范围各 +12%
  - 召唤击退 +24%
  - 召唤伤害无视 10% 防御
- `甲虫莎草纸EX / Papyrus Scarab EX`
  - 召唤伤害、鞭子攻速、鞭子范围各 +22%
  - 召唤栏 +4、哨兵栏 +1
  - 召唤击退 +44%
  - 召唤伤害无视 30% 防御
  - 稀有度：`SummonerEXRarity`
  - 配方按四种塔防饰品分别注册；统一消耗日曜碎片×20、夜明锭×10、宇宙锭×5
  - 现代 2.2.2 使用化神魂精×5 与星宇砧
  - 经典 1.4.2.101 使用 `CRUtil` 等价换算化神魂精，并使用嘉登熔炉
  - CalRD 1.4.5.7 使用化神魂精×5 与嘉登熔炉
- `始源暗影炎 / The First Shadowflame`
  - 直接移植自 `CalamityDemutation`，26×26 贴图逐字节一致。
  - 用户明确要求使用最纯粹的仆从栏加成：装备时直接 `maxMinions += 1`，
    不经过现代版 `shadowMinions` 的组合式间接结算。
  - 实际仆从与哨兵弹幕命中 NPC 时施加 300 帧原版暗影焰；鞭子不触发。
  - 无配方，普通模式哥布林召唤师 1/7、专家/大师 1/5 掉落。
- `斯塔提斯的祝福 / Statis' Blessing`
  - 直接移植自 `CalamityDemutation`，贴图逐字节一致。
  - 现代稀有度为黄色。
  - 召唤伤害 +10%、召唤击退 +2.5、仆从栏 +3。
  - 召唤伤害弹幕命中施加 60 帧 `TemporalSadness`。
  - 现代：日光精华×5；经典 1.4.2.101 与 CalRD：各版原生 CoreofCinder×5；
    均与甲虫莎草纸、俾格米项链、召唤师徽章、圣水×30 在秘银砧制作。
- `斯塔提斯的诅咒 / Statis' Curse`
  - 召唤伤害 +10%、召唤击退 +2.5、仆从栏 +3、鞭子范围/攻速各 +10%。
  - 召唤攻击施加 60 帧 `TemporalSadness` 与 300 帧暗影焰。
  - 现代使用本工程祝福 + 始源暗影炎 + 星尘碎片×6；两个经典版本使用碎片×10。
- `斯塔提斯诅咒腰带 / Statis' Belt of Curses`
  - 召唤伤害 +20%、召唤击退 +2.5、仆从栏 +4、鞭子范围/攻速各 +20%。
  - 盗贼伤害与暴击各 +20%；保留连续跳跃、爬墙、冲刺与闪避。
  - 召唤攻击施加 120 帧 `TemporalSadness` 与 300 帧暗影焰。
  - 现代/1.4.2 使用本工程诅咒 + 各版原生忍者腰带与旧版三高阶材料；
    CalRD 使用本工程诅咒 + 原生忍者腰带 + 化神魂精×4。
  - 祝福、诅咒、腰带同时装备时只结算最高档，避免降级件重复叠加。
### 3.6 盗贼饰品
- `盗贼徽章EX / Rogue Emblem EX`
  - 盗贼伤害 +18%
  - 盗贼暴击 +8
  - 盗贼攻速 +12%
  - 最大潜行 +60
  - 稀有度：`RogueEXRarity`
  - 潜行打击只消耗 75% 最大潜行值
  - 潜行恢复速度 +5%
  - 静止时再额外 +5%，合计 +10%
  - **用户明确要求删除全部回血效果，不要重新加入**
- `晶光头骨 / Crystal Skull`
  - 最大潜行 +120（内部值 `1.20f`，灾厄 `1f = 100`）、盗贼伤害 +22%、暴击 +22%、攻速 +22%
  - 静止与移动潜行恢复各 +10%，潜行攻击只消耗 50% 最大潜行
  - 盗贼攻击 10% 概率召唤闪电，持续弹幕使用 30 帧防刷屏冷却
  - 闪电造成攻击基础伤害的 300%，并施加 `ElectrifiedII`
  - 继承电工手套：潜行打击 +8 穿甲、+8% 伤害、首次命中回复 10 HP
  - 单次潜行打击最多触发 3 轮、每轮 3 枚电火花；每枚基础伤害 95 后再经盗贼总伤害结算
  - 触电II持续 300 帧；NPC 易伤 +5%、防御 -5、125 HP/s，玩家防御 -15、30 HP/s
  - 使用原版触电电弧尘；闪电与火花均为无贴图自定义弹幕
  - 三版本配方底座：盗贼徽章EX + 宏伟蓝图（ID 3619）+ 电线×1000 + 纳米机器人×1000 + 骨头×1000 + 机械头骨
  - 同时采用其他高级 EX 通用材料：日曜碎片×20、夜明锭×10、宇宙锭×5
  - 现代 2.2.2 使用化神魂精×5 与星宇砧
  - 经典 1.4.2.101 使用 `CRUtil` 等价换算化神魂精，并使用嘉登熔炉
  - CalRD 1.4.5.7 使用化神魂精×5 与嘉登熔炉
- 镜片潜行链已完整本地化，全部贴图逐字节取自 `CalamityDemutation`：
  - `欺诈硬币`：现代蓝色；潜行打击仅消耗 75%，盗贼伤害/暴击各 +3。
  - `毁灭徽章`：现代粉色；盗贼伤害/暴击各 +4、最大潜行 +10、潜行打击半价。
  - `静默剑鞘`：现代绿色；盗贼伤害/暴击各 +2、最大潜行 +20、静止/移动恢复各 +15%。
  - `幻影魔镜`：现代橙色；盗贼伤害/暴击各 +2、最大潜行 +15、恢复 +30%/+20%、仇恨 -200。
  - `深渊魔镜`：现代青柠色；盗贼伤害/暴击各 +5、最大潜行 +20、恢复 +30%/+20%、
    仇恨 -450，并提供恢复 50% 潜行的镜子闪避。
  - `暗物质剑鞘`：现代红色；盗贼伤害/暴击各 +6、最大潜行 +20、潜行打击半价且必定暴击、
    移动潜行恢复加速。
  - `蚀日魔镜`：用户指定 `CosmicPurpleRarity`；盗贼伤害/暴击各 +10、最大潜行 +25、
    静止恢复 +20%、移动恢复加速、潜行打击半价且必定暴击、仇恨 -700；
    镜子闪避恢复全部潜行并释放暗日强光。
  - 镜子闪避共享 90 秒冷却，同时装备时蚀日魔镜优先。
- `纳米技术 / Nanotech`
  - 用户指定 `CosmicPurpleRarity`，贴图取自 Demutation。
  - 盗贼伤害/暴击各 +20、弹速 +15%；手持盗贼武器时防御 +30、减伤 +10%。
  - 盗贼弹幕每 30 帧留下纳米刀刃；潜行弹幕 +20 穿甲且命中获得 5% 最终增伤。
  - 潜行打击每次提供一次服务器验证的 1 HP 治疗，并在前三次命中召唤 NanoFlare。
  - 不移植“概率秒杀普通敌怪”。
### 3.7 武器内容移除状态
- 2026-11-26 已按用户要求彻底移除全部武器，不保留武器本体、配方、贴图、专属弹幕或专属音效。
- 原弑神者冲刺仍在使用的粒子表现已独立为 `GodSlayerDashParticleSystem`，只保留冲刺所需路径；
  原武器专用火花、火焰脉冲、贴图和相关名称已删除。
- 共享的 `ElementalMixII`、`BurnishedAuricRarity` 等非武器专属资源继续供饰品、
  盔甲或全局结算使用，不得因为武器移除而误删。
### 3.8 灾厄原版削弱回调
- 配置项：`VanillaNerfRevertConfig.RevertVanillaNerfs`，默认关闭，`ServerSide`，修改后重载。
- 同时处理现代 `CalamityMod 2.2.2`、`CalamityModClassicPreTrailer 1.4.2.101` 与 `CalRD 1.4.5.7`；
  仅恢复各版本实际存在的原版内容削弱，不处理灾厄自身内容。
- 现代 2.2.2：恢复原版武器、弹药、工具、护甲和物品价值字段，包括伤害、使用时间、弹速、防御、
  斧力、镐力、锤力与售价；覆盖魔法帽、丛林套、法袍、武道服、蘑菇矿胸甲、塔防单件、日耀与星旋头等职业属性。
- 现代 2.2.2：恢复侦察镜、狙击镜、野性爪、日月石与天界石/壳，以及 WizardHat、Adamantite、Cobalt、
  Jungle、Molten、SolarFlare 等套装效果。
- 现代 2.2.2：恢复箭术、魔法力量、糖冲刺、挖矿、迅捷、微醺、饱食/熟食、狼人、狂犬病、星云与甲虫增益；
  甲虫耐力按原版每球 15% 乘算关系还原，避免加算超补。
- 现代 2.2.2：恢复蝴蝶翅膀与幽灵翅膀上升倍率，并在 `PostSetupContent` 中恢复火焰、蝙蝠、女皇、
  悬浮类翅膀和天界星盘的静态飞行、水平、悬浮与加速度参数。
- 现代 2.2.2：恢复暗影护甲与腾飞徽章移动属性，包括腾飞徽章跳跃提速和无限飞行时间；
  同时抵消手持其他职业武器时召唤弹幕承受的 0.75 倍惩罚。
- 1.4.2.101：还原 Daedalus Stormbow、Phoenix Blaster、Venus Magnum、Blizzard Staff 的统一 25% 伤害削弱，
  以及 Razorpine、Last Prism 的单独伤害削弱。
- `CalRD 1.4.5.7`：还原 Phantasm、Daedalus Stormbow、Phoenix Blaster、Aerial Bane、Influx Waver、Xenopopper、
  Electrosphere Launcher、Optic Staff、Razorpine、Blizzard Staff、Laser Machinegun、Stardust Dragon Staff 的伤害。
- 1.4.2.101：还原塔防守卫套若干单件的召唤、魔法、近战攻速与暴击属性扣除。
- 两版共通：还原蠕虫围巾 7% 减伤、冰障 10% 减伤、星云伤害与生命回复、三类原版远程弹幕伤害、复仇模式跑鞋速度惩罚；
  CalRD 还额外还原复仇模式对滑冰鞋加速度的 1/3 削弱。
- 两版共通：还原 Precise/Lucky 饰品词缀从近战、远程、魔法职业挪出的 1%/2% 暴击。
- 统一实现位于 `Systems/VanillaNerfRevertSystem.cs`；物品、护甲与翅膀入口为 `CalamityRebirthGlobalItem`，
  增益入口为 `CalamityRebirthGlobalBuff`，弹幕与跑速入口位于 `CalamityRebirthPlayer`，
  翅膀静态表入口位于主类 `CalamityRebirth.PostSetupContent`。
### 3.9 血神核心系列
- 六件饰品：血神核心、血炎晶核、血契、血肉图腾、血腥蠕虫围巾、血腥蠕虫牙。
- 血神核心：最大生命 +10%、通用伤害/暴击 +12%、近战攻速 +12%、减伤 +10%、防御 <100 时通用伤害再 +15%，
  继承血肉图腾接触减伤，并生成基础伤害 40、NPC 本地命中冷却 5 帧的灵魂虹吸光环。
- 血炎晶核：吸血光环与防御 <100 时 +15% 通用伤害；生命 ≤50% 时减伤/通用伤害/暴击 +15%/+10%/+10，
  生命 <15% 时改为 +20%/+20%/+20。
- 血腥蠕虫围巾：近战伤害 +10%、近战攻速 +10%、减伤 +10%；血腥蠕虫牙常态三属性 +5%，生命 <50% 时为 +10%。
- 血契保留旧版最大生命翻倍与 25% 概率承受 2.5 倍最终伤害；血肉图腾保留接触伤害减半与 1200 帧冷却。
- 价格与稀有度采用现代 2.2.2：血神核心 2 铂金/CosmicPurple；血炎晶核 1 铂金 50 金/Turquoise；
  血契与血肉图腾 60 金/黄色；围巾 10 金/浅红；牙齿 5 金/橙色。
- `CosmicPurple` 与 `Turquoise` 已移植为本工程的 `CosmicPurpleRarity`、`TurquoiseRarity`，
  不再借用外部稀有度类型；现代稀有度价格统一由 `RarityUtil.Apply` 设置。
- 2.2.2 将血腥蠕虫牙设为血肉宿主的专家门槛物品：专家/大师固定从 `PerforatorBag` 获得，
  普通模式本体直掉代码在 2.2.2 中处于注释状态，因此普通模式不掉。
- 血契固定、血肉图腾 1/2、血炎晶核在击败亵渎天神后固定进入掠夺者宝藏袋；覆盖现代、1.4.2.101 与 CalRD。
- 血神核心三版本配方均使用四件前置；现代为宇宙锭×5 + 死灵质×5 @ 宇宙砧，
  1.4.2.101 与 CalRD 为宇宙锭×5 + 幻影质×5 @ 嘉登熔炉。
### 3.10 灼辉金源之翼
- `灼辉金源之翼 / Burnished Auric Wings`
- 实际来源：`CalamityExpansion/Content/Items/Accessories/MoveMent/AncientAuricWings.cs`；
  `CalamityDemutation` 中没有此物品。内部类名保留 `AncientAuricWings`，显示名按用户要求调整。
- 基础面板：飞行 400 帧、水平速度 12、水平加速度倍率 3；
  下落上升 1、上升速度 0.175、最大上升倍率 3.25、持续上升 0.15。
- 属性：通用伤害 +8%、通用暴击 +8、近战攻速 +8%、伤害减免 +8%、免疫摔落伤害。
- 稀有度为 `BurnishedAuricRarity`；源物品显式价格 1 铂金，不套用该稀有度通常的 2 铂金 40 金。
- 可见飞行时生成淡金色钻石尘，隐藏外观时关闭粒子。
- 三版本配方：
  - 现代 2.2.2：圣金源锭×12 + 飞翔之魂×20 @ 嘉登熔炉。
  - 经典 1.4.2.101：`CRUtil.TryAddClassicAuricBar(recipe, 12)` 换算为圣金源矿×60 + 龙魂碎片×12，
    再加飞翔之魂×20 @ 嘉登熔炉。
  - CalRD 1.4.5.7：圣金源锭×12 + 飞翔之魂×20 @ 嘉登熔炉。
- 物品贴图 30×26、翅膀贴图 86×248，均与 `CalamityExpansion` 原件逐字节一致。
### 3.11 元素之握链
- 顶点 `元素之握 / ElementalGauntlet` 已移植，整条前置链完成。
- 完整链为两条支流汇合：
  - 烈火手套支流：泰坦手套 → 强力手套 → 机械手套 → 原版烈火手套。
  - 证章支流：勇气勋章 + 烦恼项链 + 战士徽章 + 十字项链 + `CoreofCinder` → 魔君之证章。
  - 顶点：原版烈火手套 + 魔君之证章 + 对应高阶段材料 → 元素之握。
- `勇气勋章 / BadgeofBravery` 已移植：
  - 用户指定属性：近战伤害 +10%、近战暴击 +10、近战盔甲穿透 +10。
  - 不附加参考工程旧版的龙蒿套装条件攻速。
  - 使用现代 2.2.2 贴图（42×30），判定尺寸 30×30，`TurquoiseRarity`，价格 1 铂金 50 金。
  - 现代 2.2.2 配方：战士徽章 + 永恒花锭×12 @ 秘银砧。
  - 经典 1.4.2.101 与 CalRD 配方：`UeliaceBar×2` + 猛爪手套 @ 远古操纵机。
- `烦恼项链 / NecklaceofVexation` 已移植：
  - 采用现代 2.2.2 行为：按已损失生命比例获得通用伤害，满血 0%、空血最高 +30%。
  - 所有武器直击和友方弹幕命中 NPC 时附加 120 帧毒液；PvP 不额外施加毒液，与灾厄现代 NPC 减益入口一致。
  - 使用现代 2.2.2 贴图（26×34），判定尺寸 26×34，柠檬色稀有度，价格 45 金。
  - 现代 2.2.2 配方：复仇者徽章 + 永续锭×10 @ 秘银砧。
  - 经典 1.4.2.101 与 CalRD 配方：复仇者徽章 + `DraedonBar×2` @ 秘银砧。
- `余烬核心 / CoreofCinder` 不再作为本工程物品存在，但版本分支必须优先使用灾厄原生件：
  - 经典 1.4.2.101：从 `CalamityModClassicPreTrailer` 弱依赖查找原生 `CoreofCinder`。
  - CalRD 1.4.5.7：从 `CalRD` 弱依赖查找原生 `CoreofCinder`。
  - 现代 2.2.2：原生物品已删除，魔君之证章分支才直接展开为 `EssenceofSunlight + Ectoplasm`。
  - 不得恢复本工程中间物品、贴图、发光层或本地化条目，也不得让经典版本绕过原生件直接使用原料。
- `魔君之证章 / YharimsInsignia` 已移植：
  - 用户指定基础面板：近战伤害 +15%、近战暴击 +10、近战攻速 +15%。
  - 生命低于 50% 时通用伤害 +10%；获得近战击退、长无敌和额外 240 帧岩浆免疫。
  - 近战武器与近战弹幕直接尝试施加当前灾厄版本的圣火，不建立显式三版本分支。
  - 22×38、30 金、`TurquoiseRarity`；贴图来自 `CalamityDemutation` 原件。
  - 现代 2.2.2：战士徽章 + 烦恼项链 + 日耀精华 + 灵气 + 十字项链 + 勇气勋章 @ 远古操纵机。
  - 经典 1.4.2.101：同一五件链路使用灾厄原生 `CoreofCinder ×5`。
  - CalRD 1.4.5.7：原生 `CoreofCinder ×5` + 神性晶核 ×5，不消耗勇气勋章。
- `元素之握 / ElementalGauntlet` 已移植：
  - 基础面板沿用 `CalamityDemutation`：近战伤害、近战暴击、近战攻速各 +20%；自动挥舞、武器放大、
    烈火手套击退、长无敌、岩浆免疫额外 240 帧。
  - 22×38、2 铂金、`CosmicPurpleRarity`，与血神核心一致；贴图来自 `CalamityDemutation` 原件。
  - 元素之握自身的每次近战物品或近战弹幕命中只施加本工程强化减益 `ElementalMixII`，持续 120 帧；
    不再施加原版六种减益、三版本灾厄元素减益或随机 `GlacialState`。
  - 本工程 `ElementalMixII / 元素混合II`：
    NPC 每秒流失 700 HP、防御 -50、受到伤害 +15%；玩家每秒流失 50 HP、防御 -50、受到伤害 +15%。
  - `ElementalMixII` 的视觉完全沿用现代 `ElementalMix`：四色全息方条、六种帧语言、
    原版三通道色差、原版尺寸/寿命/生成频率与 Dust 66/247，不追加额外核心或回落晶屑。
  - NPC、PvP 玩家、物品直击和近战弹幕共用同一条施加逻辑。
  - 现代 2.2.2：原版烈火手套 + 化神魂精×4 @ 星宇砧。
  - 经典 1.4.2.101：原版烈火手套 + 魔君之证章 + 幻影质×20 + 梦魇燃料×20 + 吸热能量×20 @ 嘉登熔炉。
  - CalRD 1.4.5.7：原版烈火手套 + 魔君之证章 + 化神魂精×4 @ 嘉登熔炉。
### 3.12 生命与减益宝石链
- 古老蜂蜜生命链已完整移植：
  - `蜜露（古）/ HoneyDew2`：20×20、绿色、2 金；最大生命 +30，常驻蜂蜜再生、
    蜂蜜内静止加速再生，并使病症/中毒类减益时长减半。
  - `生命露（古）/ LivingDew2`：34×22、黄绿色、48 金；最大生命 +50，继承蜜露（古），
    并把时长减半扩展到火系/燃烧类减益。
  - `光辉软泥（古）/ RadiantOoze2`：20×20、浅红、12 金；按已损失生命提供约 2～6 HP/s 再生，
    且上位件不在场时发出暖黄光。
  - `甘露安瓿（古）/ AmbrosialAmpoule2`：20×20、青色、80 金；最大生命 +70，
    继承整条古老蜂蜜链，并在静止时提供额外再生。
  - 现代版缺失的 `MurkyPaste` / `TrapperBulb` 不新增同名材料，
    `HoneyDew2` 与 `LivingDew2` 的现代配方分别展开为丛林孢子和藤蔓；经典版继续使用原生材料。
- `王冠宝石 / CrownJewel`
  - 26×26、防御 +4、蓝色稀有度、标准 1 金价格；无配方。
  - 史莱姆王普通与专家均按 10% 掉落。
- `感染宝石 / InfectedJewel`
  - 26×26、防御 +6、柠檬色稀有度、显式 48 金价格。
  - 常态生命再生 +1 HP/s；命中宝石清单中的任一战斗减益时再 +2 HP/s，合计 +3 HP/s，
    并把 `lifeRegenTime` 提升到至少 1800。
  - 首个减益提供 16 防御，同类减益每多一个再 +5；减益消失后按每秒 1 点回落。
  - 现代配方：王冠宝石 + `AureusCell`×10 + `StarblightSoot`×25 @ 秘银砧。
  - 经典 1.4.2.101 与 `CalRD`：王冠宝石 + `AstralJelly`×10 + `Stardust`×25 @ 秘银砧。
- `无暇粹魂晶 / Purity`
  - 18×44、防御 +12、`CosmicPurpleRarity`、1 铂金 50 金。
  - 最大生命 +100、免疫击退；继承甘露安瓿（古）的常驻蜂蜜再生、站桩再生与目标战斗减益时长减半。
  - 按已损失生命从 3 HP/s 线性提高到 7 HP/s。
  - 有目标战斗减益时抵消最多 12 HP/s 的减益掉血；首个减益提供 20 防御，
    每多一个再 +8，减益消失后按每秒 1 点回落。
  - 有目标战斗减益时每 12 帧直接回复 1 HP；持续 300 帧后治疗间隔逐步延长，
    上限 180 帧。多人客户端不直接改写生命，由服务器验证并广播生命值。
  - 现代配方：甘露安瓿（古）+ 感染宝石 + 圣金源锭×5 + 化神魂精×4 @ 星宇砧。
  - 经典 1.4.2.101 使用 `CRUtil` 等价材料；1.4.2.101 与 CalRD 均使用嘉登熔炉，
    且两版都消耗本工程甘露安瓿（古），不再回引灾厄同名基础件。
- 原月后 15 级紫色档已按用户要求映射为现代 God Slayer 使用的 `CosmicPurpleRarity`；
  旧 `PostMoonLordPurpleRarity` 类型已删除，Purity 继续保留 1 铂金 50 金的显式价格。
- `BuffUtil` 固定包含参照工程 2.0.3.9 的原版减益清单、本工程四种战斗减益，
  并按已加载灾厄版本弱依赖查找同名灾厄减益；酒精、药水病等长期状态不触发宝石强化。
- 三件宝石按 `无暇粹魂晶 > 感染宝石 > 王冠宝石` 的 `else-if` 优先级结算；
  动态防御与治疗节拍跨帧保存，死亡时归零，避免复活后继承旧惩罚。
### 3.13 已移除的旧武器链
- 原有多件近战武器及其整条配方、武器贴图、专属弹幕、专属音效、本地化路径均已删除。
- 不得从参考工程重新导入这些武器，也不得以同名材料、隐藏物品或调试物品的方式变相恢复。
- `BurnishedAuricRarity` 仍是独立稀有度资源，可由现有非武器内容继续使用。
### 3.14 综合战斗饰品
- `蚀日尊戒 / Dark Sun Ring`
  - 直接移植自 `CalamityDemutation`，32×192 六帧纵向灵魂动画贴图逐字节一致；判定尺寸 26×26。
  - 基础防御 +10、生命再生 +1（约 +0.5 HP/s）。
  - 通用伤害 +12%、近战攻速 +12%、通用暴击 +5、召唤击退 +1.2、最大仆从 +2、挖掘速度 +15%。
  - 白昼额外 `lifeRegen += 3`，约 +1.5 HP/s；夜晚额外防御 +30；日食期间两项同时生效。
  - 稀有度按现代 2.2.2 统一为 `CosmicPurpleRarity`，购买价由 `RarityUtil` 设为 2 铂金。
  - 现代 2.2.2：UelibloomBar×10 + DarksunFragment×20 @ 星宇砧。
  - 经典 1.4.2.101：UeliaceBar×10 + DarksunFragment×100 @ 秘银砧。
  - CalRD 1.4.5.7：UeliaceBar×10 + DarksunFragment×100 @ 嘉登熔炉。
### 3.15 聚合之脑链
- `腐烂之脑 / Rotten Brain`
  - 直接移植自 `CalamityDemutation`；38×26 贴图逐字节一致，判定尺寸 34×34。
  - 生命低于 75% 时通用伤害 +15%；生命低于 50% 时移动速度 -5%。
  - 受击无敌期间每帧 1/8 概率生成 `AuraRain`，每滴基础伤害 18 并按通用伤害结算。
  - 现代 2.2.2 稀有度为原版橙色、价格 5 金；专家标记保留。
  - 三个灾厄分支均向原生 `HiveMindBag` 固定追加本工程腐烂之脑。
- `聚合之脑 / Amalgamated Brain`
  - 直接移植自 `CalamityDemutation`；44×42 贴图逐字节一致，判定尺寸 34×34。
  - 通用伤害 +10%、通用暴击 +5。
  - 受击无敌期间每帧 1/8 概率生成 `AuraRain`，每滴基础伤害 60。
  - 受击时按本次伤害计算半径与时长，使范围内敌怪陷入混乱，并生成原版 `BrainOfConfusion` 幻象弹幕。
  - `FreeDodge` 提供 1/10 概率完全闪避，优先于现有魔镜共享闪避链。
  - 现代 2.2.2 稀有度为浅红色、价格 10 金；专家标记保留。
  - 现代：本工程腐烂之脑 + 混乱之脑 + 暗影魂×3 @ 铁砧。
  - 经典 1.4.2.101 与 CalRD：本工程腐烂之脑 + 混乱之脑 @ 秘银砧。
- `AuraRain / 暗影雨`
  - 复用工程透明贴图，2×2 碰撞箱、300 帧、原版友好雨云 AI 模板、额外更新 1 次。
  - 使用恶魔粉尘表现下坠轨迹；只在拥有者客户端生成，避免多人重复。
- 现代 `BloodyWormTooth` 的墓园拆解配方已改为消耗本工程腐烂之脑，不再依赖灾厄外部同名物品。
### 3.16 融合之脑顶点链
- Demutation 源码中 `AmalgamatedBrain` 与 `TheAmalgam` 的旧中文名都叫“聚合之脑”；
  本工程将顶点 `TheAmalgam` 定名为“融合之脑”，英文仍为 `The Amalgam`。
- `灾厄之环 / Calamity Ring`
  - Demutation 专家饰品，灾厄克隆体宝藏袋掉落。
  - 通用伤害 +15%；受击无敌期间每帧 1/10 概率降下基础伤害 30 的站火。
  - 现代 2.2.2 已删除该物品，保留 Demutation 的柠檬色稀有度、专家标记与 24 金价格。
- `焦热地狱 / Gehenna`
  - 硫磺火元素宝藏袋掉落，每 600 帧向瞄准方向发射 4 枚火球。
  - 每枚基础伤害 54，命中施加 1000 帧灾厄硫磺火，消亡时生成硫磺火爆炸。
  - 现代对应物为 `Slagsplitter Pauldron`，采用现代浅红色稀有度与 10 金价格。
- `终结虚空 / Void of Extinction`
  - 黑曜石玫瑰 + 焦热地狱 + 灾厄之环 @ 秘银砧。
  - 通用伤害 +15%，额外 240 帧岩浆免疫，身处岩浆再 +25% 伤害。
  - 无敌期间每帧 1/10 概率降下基础伤害 40 的站火；每 600 帧发射一轮基础伤害 70 的火球。
  - 现代版已删除该物品，保留 CalRD 的黄色稀有度与 Demutation 的 30 金价格。
- `真菌团块 / Fungal Clump`
  - 克鲁布龙宝藏袋掉落，装备后维持一只不占召唤栏的真菌团块。
  - 仆从命中按实际伤害 25% 吸血，生成追踪治疗珠；现代稀有度为绿色、价格 2 金。
- `利维坦龙涎香 / Leviathan Ambergris`
  - 利维坦宝藏袋掉落；干燥时伤害/减伤 +5%，浸水时伤害 +10%、防御 +20、移速 +75%。
  - 移动时持续留下毒海水；浸水无敌期间按 1/5 检查节拍毒害 200 像素内敌怪。
  - 现代稀有度为柠檬色、价格 45 金。
- `融合之脑 / The Amalgam`
  - 46×276 六帧灵魂动画贴图取自 Demutation。
  - 通用伤害 +20%、暴击 +10，继承终结虚空的岩浆战斗与火球齐射。
  - 无敌期间随机降下暗影雨或站火，基础伤害 500；每 600 帧齐射 4 枚基础伤害 500 的火球。
  - 继承利维坦式水下战斗与毒海水；浸水光环伤害 80；干燥时伤害/减伤 +10%，浸水时防御 +40。
  - 受击困惑周围敌怪；`FreeDodge` 提供 1/8 概率完全闪避。
  - 可见时维持一只基础伤害 250 的真菌团块；隐藏外观时保留属性并关闭仆从。
  - 与聚合之脑、终结虚空、灾厄之环、真菌团块、利维坦龙涎香双向互斥。
  - 现代：四组件 + 宇宙锭×5 + 死灵质×5 @ 星宇砧。
  - 经典 1.4.2.101：四组件 + 宇宙锭×5 + 幻影质×5 @ 嘉登熔炉。
  - CalRD 1.4.5.7：四组件 + 宇宙锭×5 + 飘渺之灵精华×1 @ 嘉登熔炉。
- 三版本宝藏袋均已追加本工程组件：
  腐巢意志→腐烂之脑、硫磺火元素→焦热地狱、灾厄克隆体→灾厄之环、
  克鲁布龙→真菌团块、利维坦→利维坦龙涎香。
- 新增共用弹幕/召唤：
  `AuraRain`、`StandingFire`、`BrimstoneHellfireballFriendly2`、
  `HellfireExplosionFriendly2`、`PoisonousSeawater`、`FungalClumpMinion`、`FungalHeal`；
  真菌团块增益为 `FungalClumpBuff`。
### 3.17 神之壁垒链
- 完整链为 `寒霜壁垒 + 神圣护符 → 神之壁垒`，三件贴图均与 `CalamityDemutation` SHA-256 一致。
- `寒霜壁垒 / Frigid Bulwark`
  - 38×44 判定尺寸、防御 +8、青色稀有度、30 金；击退免疫。
  - 生命高于 25% 时赋予圣骑士护盾；半血以下维持原版冰屏障；
    15% 生命以下额外 +5% 减伤。
  - 现代 2.2.2：圣骑士盾 + 冰冻海龟壳 + `EssenceofEleum`×2 + 灵气×2 @ 秘银砧；
    原 `CoreofEleum` 三枚一组的换算向上取整，不新增本工程同名材料。
  - 经典 1.4.2.101 与 CalRD 1.4.5.7：圣骑士盾 + 冰冻海龟壳 + 原生 `CoreofEleum`×5 @ 秘银砧。
- `神圣护符 / Deific Amulet`
  - 26×26 判定尺寸、现代青色稀有度、45 金；通用护甲穿透 +25。
  - 提供恐慌加速、魔力星吸附、魔法手铐与浸水照明；受击延长无敌并获得 3 枚神圣星辰。
  - 现代 2.2.2：天界手铐 + 水母项链 + 恐慌项链 + 鲨牙项链 + 星星面纱
    + `StarblightSoot`×25 + 陨石锭×25 @ 秘银砧。
  - 经典 1.4.2.101 同上，仅把现代材料换为原生 `Stardust`×25。
  - CalRD 1.4.5.7 使用神话护身符替代天界手铐，其余组件与原生 `Stardust`×25 不变。
- `神之壁垒 / Rampart of Deities`
  - 38×44 判定尺寸、防御 +18、90 金、`BurnishedAuricRarity`，并注册盾牌装备图层。
  - 完整继承神圣护符并强化为通用护甲穿透 +50、受击 6 枚神圣星辰与更高无敌延长。
  - 完整继承寒霜壁垒的圣骑士护盾、冰屏障、击退免疫和残血减伤。
  - 同时装备前置件时只结算顶点档，避免穿甲、星光与护盾重复叠加。
  - 受击星辰只在受伤玩家的主人客户端生成，并使用 5 帧本地 NPC 命中冷却。
  - 现代 2.2.2：寒霜壁垒 + 神圣护符 + `AuricBar`×5 + `AscendantSpiritEssence`×4 @ 星宇砧。
  - 经典 1.4.2.101 与 CalRD 1.4.5.7：寒霜壁垒 + 神圣护符 + 银河奇点×5
    + 神圣晶核×10 + 宇宙锭×20 @ 嘉登熔炉。
### 3.18 阿斯加德系列
- 完整本地链：`华丽盾 + 海之盾 + 阿巴顿 → 阿斯加德之英勇`；
  `阿斯加德之英勇 + 极乐之庇护 → 阿斯加德之庇护`。
- `华丽盾 / Ornate Shield`
  - 36×32、8 防御、12 金、粉色稀有度；最大生命 +20、生命再生 +1 HP/s、低血防御 +4。
  - 双击方向键发动冰霜盾牌冲撞：起手 16.9、过程上限 12.5、撞击伤害 50、击退 3、180 帧霜燃 II。
  - 现代与经典 1.4.2.101 使用各版寒元系矿锭×5 + 水晶碎片×10 @ 秘银砧。
  - CalRD 使用维斯蒂德锭×5 + 水晶碎片×10 @ 秘银砧。
- `海之盾 / Shield of the Ocean`
  - 24×28、2 防御、3 金、绿色稀有度；浸入液体时额外 +5 防御。
  - 现代使用海之遗骸×5 + 珊瑚×5 @ 铁砧；经典两版使用维斯蒂德锭×5 + 珊瑚×5 @ 铁砧。
- `阿巴顿 / Abaddon`
  - 26×26、24 金、粉色稀有度，并注册面部装备图层。
  - 通用暴击 +8；免疫硫磺火；弹幕暴击命中生成硫磺爆裂，15 帧冷却。
  - 硫磺爆裂基础伤害为触发弹幕伤害的 3%（软上限 25），300 像素圆形判定，命中施加 360 帧硫磺火。
  - 各版本硫磺火元素普通/专家分别按 1/12、1/7 掉落。
- `极乐之庇护 / Elysian Aegis`
  - 48×42、16 防御、60 金、`TurquoiseRarity`、专家标记，并注册盾牌装备图层。
  - 最大生命 +30、生命再生 +1.5 HP/s；免疫击退、岩浆、火焰与多数火焰减益。
  - 神圣盾牌冲撞：起手 21.5、过程上限 14、撞击伤害 500、击退 12；
    撞击生成 90×90 神圣爆炸，基础伤害 500、击退 15，并施加 300 帧圣焰。
  - 默认 N 键切换守护状态；守护期间充能由 5 每秒消耗约 4.8，
    最多转化 +15% 伤害、+10 暴击、+1100 仇恨与 +20 防御，同时移动速度下降。
  - 各版本亵渎天神固定掉落。
- `阿斯加德之英勇 / Asgard's Valor`
  - 38×44 判定尺寸、16 防御、45 金价格、现代黄色稀有度，并注册盾牌装备图层。
  - 最大生命 +50；免疫击退、岩浆与火焰；免疫多数冰霜、烈焰和原版战斗减益。
  - 浸入液体时额外 12% 减伤；双击左右方向键发动基础盾牌冲撞。
  - 冲撞起手速度 16.9、过程速度上限 12、撞击基础伤害 200、击退 9。
  - 现代配方：十字章护盾 + 本工程华丽盾、海之盾、阿巴顿 + 原生灾厄核心×3
    + 生命果×5 @ 秘银砧。
  - 经典 1.4.2.101：同一原版链路 + 三种经典核心各×3 @ 秘银砧。
  - CalRD 1.4.5.7：同一原版链路 + 原生灾厄核心×1 @ 秘银砧。
- `阿斯加德之庇护 / Asgardian Aegis`
  - 60×54 判定尺寸、28 防御、90 金价格、`CosmicPurpleRarity`，并注册盾牌装备图层。
  - 最大生命 +40、生命再生 +2.5 HP/s；残血低于 25% 时防御 +10；浸水时额外 12% 减伤。
  - 免疫击退、岩浆与火焰，并扩展免疫冰霜、火焰、毒液、黑暗、神性烈焰与宇宙级减益。
  - 双击方向键发动宇宙盾牌冲撞；起手速度 23.3、过程速度上限 16、撞击伤害 1000、击退 15。
  - 撞击生成 140×140 宇宙爆炸，基础伤害 1000、击退 20，并施加 300 帧弑神者地狱火。
  - 现代：阿斯加德之英勇 + 本工程极乐之庇护 + 宇宙锭×10 + 飞升星魂×4 @ 星宇砧。
  - 经典 1.4.2.101：同链路 + 宇宙锭×5，并换算四份飞升星魂材料 @ 嘉登熔炉。
  - CalRD 1.4.5.7：同链路 + 原生宇宙锭×5 @ 嘉登熔炉。
- 冲刺状态机由玩家分部类承载，使用 30 帧收尾冷却、12 帧命中碰撞免疫和独立双键窗口；
  顶点优先级高于英勇，同时装备时只注册宇宙冲撞。
- 守卫状态由默认 N 键切换，按键显示名随玩家改键实时更新。
- 开始/结束与撞击表现分别使用两条网络消息广播；位移继续由原版玩家同步负责，碰撞伤害只在主人端结算。
- 新增强化圆环、细节爆炸粒子系统，以及宇宙爆炸不可见判定弹幕。
- 六件物品的十张贴图、两张粒子贴图与两段音效均与原件 SHA-256 一致。
### 3.19 灾劫之尖啸
- `灾劫之尖啸 / Affliction`
  - 20×20 判定尺寸、34×40 贴图、60 金价格、专家标记与 `PureGreenRarity`。
  - 通用伤害 +15%、防御 +45、最大生命 +20%、约 4 HP/s 再生、伤害减免 +8%，并免疫击退。
  - 每 10 帧向非同自己的同队玩家刷新 20 帧 `Afflicted / 侵蚀`；队伍编号为 0 时不传播。
  - `Afflicted` 为正面、不存档、可 PvP 传播且不受专家模式时长延长的增益，和饰品共享同一套属性结算。
  - 没有合成配方；现代、经典 1.4.2.101 与 CalRD 的噬魂幽花宝藏袋均将原生同名物品替换为本工程版本。
### 3.20 涡流虹吸石链
- 完整链共 20 件本地饰品：
  `远古化石 → 太古粉末`，`生命之露 → 蜜露`，`腐化之瓶/猩红之瓶 + 太古粉末 + 辉光软泥 + 蜜露 → 百草天香瓶`；
  `生命凝胶 + 法力凝胶 + 活力凝胶 → 绚彩凝胶`；
  `绚彩凝胶 + 海螺壳 + 巨蟹甲壳/巨型贝壳 + 真菌甲壳 + 巨型陆龟壳 + 阿米迪亚斯之火花 → 阴阳吸星石`；
  `阴阳吸星石 + 百草天香瓶 → 涡流虹吸石`。
- `涡流虹吸石 / Sponge`
  - 20×20、20 防御、90 金、月后深蓝稀有度。
  - 生命/魔力 +30、移速 +12%、跳跃 +120%、减伤 +18%、基础再生 +3 HP/s，静止时再 +3 HP/s 与 +2 魔力再生。
  - 免疫中毒、毒液、冰冻、寒冷和两种霜燃；获得蜂蜜式再生；挖掘速度 +50% 并自身发光。
  - 浸水时防御 +5、减伤 +10%、移速 +20% 并忽略水体；接触荆棘伤害 100%。
  - 受击时按原始伤害的 1/16 先回复生命，并触发甲壳爆发、双圈强化火花与双圈强化松露孢子。
  - 现代：阴阳吸星石 + 百草天香瓶 + 宇宙锭×15 + 死灵质×15 @ 星宇砧。
  - 经典 1.4.2.101：同链 + 宇宙锭×15 + 幻影质×15 @ 嘉登熔炉。
  - CalRD 1.4.5.7：同链 + 宇宙锭×5 + 飞升星魂×1 @ 嘉登熔炉。
- 前置链保留两条甲壳路线：巨蟹甲壳版与巨型贝壳版；两者最终都消耗真菌甲壳、巨型陆龟壳和火花。
- 阴阳吸星石提供生命/魔力、移速、跳跃、荆棘、减伤、浸水、受击加速、火花/孢子反击与原始伤害 1/20 的受伤前治疗。
- 新增 `ShellBoost / 甲壳爆发` 正面 Buff 与 `Spark / 火花` 弹幕，并接入蜂类弹幕 25% 减伤。
- 凝胶、甲壳、海螺壳和火花前置均接入普通/专家掉落；新增现代、经典与 CalRD 配方分支。
- 稀有度按现代同名物品适配：辉光软泥浅红、生命之露黄绿、蜜露橙色、活力凝胶蓝色、
  绚彩凝胶粉色、巨型陆龟壳浅红；现代已删除的组件保留 Demutation/对应经典档位。
### 3.21 星云核心
- `星云核心 / NebulousCore`
  - 16×14 判定尺寸、32×32 贴图、专家物品；稀有度与价格采用现代 `CosmicPurpleRarity` 档位。
  - 通用伤害 +20%、通用暴击 +20；该面板沿用 Demutation 本体口径，不采用现代 2.2.2 的仅 +12% 伤害。
  - 可见时约每 15 帧尝试生成一枚星云之星；已有星星越多成功率越低，上限 20 枚。
  - 每枚星星基础伤害 300，按玩家通用伤害加成结算；隐藏外观时停止生成，并让已有星星立即消散。
  - `NebulaStar` 为 34×34 单穿透追踪弹幕，600 像素索敌，使用六种身份方向呼吸漂移；
    命中 NPC 或 PvP 玩家后，由主人端散射 `NebulaDust`。
  - `NebulaDust` 为 32×32 隐身无限穿透区域，本地 NPC 命中冷却 3 帧，生成后先淡入再淡出。
  - 致命伤时按 1/10 概率阻止死亡并回复 100 生命，播放暗束尘与 `Item67`；
    该保命沿用 Demutation/两个经典版本的无冷却口径，不采用现代 90 秒冷却和 300 治疗。
  - 没有合成配方；现代、经典 1.4.2.101 与 CalRD 的神之使徒宝藏袋均将原生同名物品替换为本工程版本。
  - 物品贴图 32×32、星星与发光层均 34×34、尘埃贴图 26×32；四张资源均与 Demutation 原件逐字节一致。
### 3.22 元灵之心链
- 完整链共 10 件本地饰品：4 枚元素石 + 5 件元素召唤饰品 + 元灵之心顶点。
- 元素石：
  - `疾风石 / AeroStone`：浅红稀有度；移动速度 +10%、跳跃速度 +2、通用伤害 +3%，青色照明。
  - `冰晶石 / CryoStone`：6 防御、减伤 +5%、通用伤害 +3%，蓝色照明。
  - `混沌石 / ChaosStone`：青柠稀有度；最大魔力 +50、魔力消耗 -5%、通用伤害 +3%，红色照明。
  - `繁花石 / BloomStone`：青柠稀有度；通用伤害 +2%、通用暴击 +2，绿色照明；周期性诅咒附近敌怪并造成 10 点伤害，脚边可生长花草。
- 元素召唤饰品：
  - `玫瑰石 / RoseStone`：最大生命 +20、生命再生 +1 HP/s、通用伤害 +3%，召唤硫火元素。
  - `风暴之眼 / EyeoftheStorm`：召唤云元素，近距离会高速冲锋撞击。
  - `魅惑之珠 / LureofEnthrallment`：召唤固定在玩家上方的塞壬，轮流水矛、霜雾与歌声。
  - `瓶装沙元素 / WifeinaBottle`：召唤沙元素，沙之箭落点生成沙之印记与竖直沙暴。
  - `稀有瓶装沙元素 / WifeinaBottlewithBoobs`：召唤治疗型沙元素，周期性生成回复 15 HP 的治疗珠。
- `元灵之心 / HeartoftheElements`
  - 20×20 判定、9 防御、现代红色稀有度、显式 80 金价格。
  - 两档共同提供约 2 HP/s 再生、2 魔力再生、魔力消耗 -5%、通用暴击 +10。
  - 可见档：最大生命 +20、最大魔力 +50、移速 +10%、跳跃 +2、减伤 +5%、通用伤害 +10%。
  - 隐藏档：最大生命 +25、最大魔力 +60、移速 +12%、跳跃 +2.2、减伤 +6%、通用伤害 +12%。
  - 可见时维持五种元素仆从各一只；隐藏时关闭全部元素但保留隐藏档属性。
  - 玩家站立时会在草地、神圣草地、丛林草地或无水泥土空位生长花草；与花绽石共用同类诅咒光环。
  - 元素仆从基础伤害为月前 100、月后 300；击败神明吞噬者后 ×2.5。
  - 九件合成配方统一在远古操纵机注册。
- 元素仆从与辅助弹幕：
  - `BigBustyRose`、`CloudyWaifu`、`SirenLure`、`SandyWaifu`、`DrewsSandyWaifu` 五个常驻仆从。
  - `WaterSpearFriendly`、`FrostMistFriendly`、`SirenSongFriendly`、`SandBolt`、`SandMark`、
    `SandTornado`、`CactusHealOrb` 七个辅助弹幕。
  - 六种召唤增益 `HotE`、`BrimstoneWaifu`、`CloudyWaifu`、`SirenLure`、`SandyWaifu`、`DrewsSandyWaifu`
    均无时间条、不存档，并按佩戴物或仆从在场状态自动移除。
- 掉落：
  - 元素石保留非专家约 1/10 Boss 掉落，并在现代、1.4.2.101、CalRD 对应宝藏袋中按原生概率替换。
  - 魅惑之珠与风暴之眼为普通/专家 1/7、1/5 掉落。
  - 瓶装沙元素来自沙元素普通/专家 1/7、1/5；稀有版本仅专家 1/20。
  - 五个可叠加元素召唤饰品和元灵之心双向互斥，避免重复仆从。
  - 十件物品、九张仆从/攻击弹幕贴图和六张增益图标均与 Demutation 原件 SHA-256 一致。
## 4. 三版本盗贼软依赖
实现入口：`Utilities/RogueUtil.cs`，内部类型为分部类 `CRUtil`
### 4.1 现代 2.2.2
- 使用真实 `RogueDamageClass`。
- 通过 `ModContent.TryFind<DamageClass>` 获取，不写强类型引用。
- 伤害、暴击、攻速直接修改该 DamageClass。
- 潜行上限通过 `AddMaxStealth` 和 `SetWearingRogueArmor` Mod.Call。
- 75% 消耗和恢复速度使用反射字段：
  `stealthStrike75Cost`、`stealthGenStandstill`、`stealthGenMoving`。
- 75% 与 50% 潜行攻击消耗统一由 `CRUtil.SetStealthStrikeCost` 写入单一档位；
  每帧只选最低档，晶光头骨也使用通用 50% 档，不使用 `flatStealthLossReduction`。
  潜行打击命中时还会把当前值校准为最大值的 75%。
### 4.2 经典 1.4.2.101
- 武器使用 `DamageClass.Throwing`。
- 自定义伤害/暴击位于：
  `CalamityPlayerPreTrailer` 模组内的 `CalamityCustomThrowingDamagePlayer`
  - `throwingDamage`
  - `throwingCrit`
- 潜行字段位于 `CalamityPlayerPreTrailer`：
  - `rogueStealthMax`
- 该版本没有 `stealthStrike75Cost` 与 `stealthGen*` 字段，不能等价设置离散的 75% 消耗；保留其原生连续消耗机制。
### 4.3 经典 1.4.5.7（`CalRD`）
- 武器同样使用 `DamageClass.Throwing`。
- `CalRD.CalPlayer.CalamityPlayer` 内同时存在：
  - `throwingDamage`
  - `throwingCrit`
  - `rogueStealthMax`
  - `wearingRogueArmor`
  - `stealthStrike75Cost`
  - `stealthGenStandstill`
  - `stealthGenMoving`
  - `rogueStealth`
  - `stealthStrikeThisFrame`
- 全部通过反射写入；字段缺失时安全跳过。
- CalRD 原生支持 50% 潜行攻击消耗字段，晶光头骨与其他半价件共用同档。
## 5. 关键文件
- `Players/CalamityRebirthPlayer.cs`
- 玩家状态、药水增益、饰品结算、回蓝、召唤穿防、盗贼兼容调用。
- `Content/Items/CalamityRebirthGlobalItem.cs`
  - 物品 PvP 命中入口。
  - 血神核心系列宝藏袋掉落与亵渎天神进度条件。
  - 星云核心的三版本神之使徒宝藏袋替换入口。
- `Content/Items/Accessories/Combat/NebulousCore.cs`
  - 星云核心面板、星星生成与专家标记。
- `Content/Projectiles/Typeless/NebulaStar.cs`
- `Content/Projectiles/Typeless/NebulaDust.cs`
  - 星云之星追踪、S 形漂移、发光叠加、命中尘埃与伤害区域。
- `Players/CalamityRebirthPlayer.NebulousCore.cs`
  - 通用伤害/暴击、外观标记与无冷却致命伤保护。
- `Content/Items/Accessories/Combat/HeartoftheElements.cs`
  - 元灵之心九件配方、五元素召唤、隐藏档属性开关与互斥。
- `Content/Items/Accessories/Combat/AeroStone.cs`
- `Content/Items/Accessories/Combat/CryoStone.cs`
- `Content/Items/Accessories/Combat/BloomStone.cs`
- `Content/Items/Accessories/Combat/Magic/ChaosStone.cs`
  - 四枚元素石入口。
- `Content/Items/Accessories/Combat/Summon/RoseStone.cs`
- `Content/Items/Accessories/Combat/Summon/EyeoftheStorm.cs`
- `Content/Items/Accessories/Combat/Summon/LureofEnthrallment.cs`
- `Content/Items/Accessories/Combat/Summon/WifeinaBottle.cs`
- `Content/Items/Accessories/Combat/Summon/WifeinaBottlewithBoobs.cs`
  - 五件独立元素召唤饰品与单仆从维持逻辑。
- `Content/Projectiles/Summon/ElementalMinionUtil.cs`
  - 元素仆从共用索敌、悬停、动画与进度命中冷却。
- `Players/CalamityRebirthPlayer.ElementalChain.cs`
  - 元素石面板、元灵之心两档属性、诅咒光环与植物生长。
- `Content/Items/Accessories/Combat/CoreOfTheBloodGod.cs`
  - 血神核心及三版本高级配方。
- `Content/Items/Accessories/Combat/BloodflareCore.cs`
- `Content/Items/Accessories/HealthandMana/BloodPact.cs`
- `Content/Items/Accessories/HealthandMana/CrownJewel.cs`
  - 宝石链起点、史莱姆王掉落与基础再生。
- `Content/Items/Accessories/HealthandMana/InfectedJewel.cs`
  - 减益启动强化再生与动态防御，三版本升级配方。
- `Content/Items/Accessories/HealthandMana/Purity.cs`
  - 宝石链顶点、甘露安瓿继承、动态再生/防御/治疗与三版本配方。
- `Utilities/BuffUtil.cs`
  - 通用战斗减益查询、环境/月亮事件增益、星陨环境补充增益与热重载清理。
- `Content/Items/Accessories/Combat/FleshTotem.cs`
- `Content/Items/Accessories/Combat/Melee/BloodyWormScarf.cs`
- `Content/Items/Accessories/Combat/Melee/BloodyWormTooth.cs`
- `Content/Items/Accessories/Movement/AncientAuricWings.cs`
  - 灼辉金源之翼本体、三版本配方与翅膀机动性。
- `Content/Items/Accessories/Movement/AngelTreads.cs`
- `Content/Items/Accessories/Movement/InfinityBoots.cs`
- `Content/Items/Accessories/Movement/ElysianTracers.cs`
- `Content/Items/Accessories/Movement/DrewsWings.cs`
- `Content/Items/Accessories/Movement/SeraphTracers.cs`
  - 完整翼靴链、可切换飞行模式、配方与装备贴图入口。
- `Players/CalamityRebirthPlayer.WingChain.cs`
  - 飞行时间、重伤额外无敌与现代灾厄兼容字段镜像。
- `Content/Items/Accessories/Combat/Melee/BadgeofBravery.cs`
  - 元素之握前置勇气勋章；用户指定的近战三属性。
- `Content/Items/Accessories/Combat/Melee/NecklaceofVexation.cs`
  - 低生命动态通用伤害与全攻击毒液入口常量。
- `Content/Items/Accessories/Combat/Melee/YharimsInsignia.cs`
  - 元素之握证章支流中间件、修正面板、低生命增伤、生存属性与三版本配方。
- `Content/Items/Accessories/Combat/Melee/ElementalGauntlet.cs`
  - 元素之握本体、三版本配方与近战面板状态入口。
- `Content/ModBuffs/Debuffs/ElementalMixII.cs`
  - 强化元素谐鸣减益本体、NPC/玩家数值口径与四色全息视觉生成入口。
- `Content/Particles/TechyHoloysquareParticle.cs`
  - 沿用现代原始类名，承载六帧全息方条、双层色差残影与白色能量核心粒子。
- `Content/Particles/CalamityRebirthParticleSystem.cs`
  - 通用客户端粒子池、加法混合绘制与世界卸载清理。
- `Utilities/CalamityCompatUtil.cs`
  - 三版本亵渎天神进度与外部静态成员反射查询。
- `Utilities/RarityUtil.cs`
  - 稀有度到标准购买价的统一映射；灼辉金源固定映射 RarityViolet 的 2 铂金 40 金，
    并支持药水等物品传入显式价格覆盖。
- `Content/Items/Potions/FoodandDrinkPotions/TequilaSunrise.cs`
  - 龙舌兰日出本体、昼夜属性与三版本双配方。
- `Content/Items/Potions/BuffPotions/AgentiaPotionItem.cs`
  - 15 件 Agentia 药水的公共面板和三版本血珠兜底。
- `Content/Items/Potions/FoodandDrinkPotions/AlcoholPotionItem.cs`
  - 酒类药水公共现代价格、食用面板与持久酒类入口。
- `Content/ModBuffs/Debuffs/TequilaSunriseBuff.cs`
  - 龙舌兰日出增益与酒醉等级累计。
- `Content/ModBuffs/Debuffs/AlcoholPoisoning.cs`
  - 超过三级酒醉时共用的酒精中毒提示。
- `Content/Rarities/CosmicPurpleRarity.cs`
  - CosmicPurple 颜色、重铸映射、水晶名称辉光与星屑绘制。
- `Content/Rarities/TurquoiseRarity.cs`
  - Turquoise 颜色与可用重铸映射。
- `Content/Projectiles/CalamityRebirthGlobalProjectile.cs`
  - 弹幕 PvP 命中入口。
- `NPCs/CalamityRebirthGlobalNPC.cs`
  - 燃烧III、破晓II、触电II 的 NPC 掉血、减防易伤、死亡处理和粒子入口。
  - 专家/大师月亮领主核心的天界洋葱掉落。
- `Utilities/MaterialUtil.cs`
  - 现代材料到经典 1.4.2 材料的等价换算。
- `Utilities/RogueUtil.cs`
  - 2.2.2 / 1.4.2.101 / 1.4.5.7 盗贼软依赖桥。
- `Systems/VanillaNerfRevertConfig.cs`
  - 灾厄原版削弱回调开关；服务器端配置，修改后重载生效。
- `Systems/VanillaNerfRevertSystem.cs`
  - 现代 2.2.2、1.4.2.101 与 `CalRD` 的精确原版数值、护甲、饰品、翅膀、增益、弹幕与跑速回调数据。
- `Content/Particles/GodSlayerDashParticleSystem.cs`
  - 弑神者冲刺火花、龙颚光片与定向脉冲环；不再包含任何武器专用分支。
- `Content/Particles/GodSlayerDashParticle.cs`
  - 弑神者冲刺粒子的位置、寿命、淡出、缩放和绘制实现。
- `Content/Items/Accessories/Combat/Rogue/CrystalSkull.png`
  - 显示名“晶光头骨”；内部资源名保持英文 `CrystalSkull`，采用盗贼徽章EX的暗蓝、暗紫、金属灰与冷白高光色系。
- `Content/Rarities/BurnishedAuricRarity.cs`
  - 灼辉金源完整稀有度绘制。
- `Content/Items/Accessories/Combat/Rogue/CrystalSkull.cs`
  - 晶光头骨本体与可用灾厄分支的融合配方探测。
- `Content/Items/Potions/PermanentPowerUps/CelestialOnion.cs`
  - 天界洋葱消耗品与永久额外饰品槽。
  - `CelestialOnion.png` 已于 2026-11-12 替换为用户提供的 42×52 新贴图。
- `Content/ModBuffs/Debuffs/ElectrifiedII.cs`
  - 强化触电减益与电弧粒子。
- `Content/Projectiles/Rogue/CrystalSkullSpark.cs`
  - 电工手套潜行火花，基础伤害 95。
- `Content/Projectiles/Rogue/CrystalSkullLightning.cs`
  - 10% 盗贼命中闪电与 PvP 触电II入口。
- `Utilities/ProjectileUtil.cs`
  - 原生残影模式、追踪和通用落点生成工具；仅供现有非武器弹幕调用。
- `Utilities/CalamityCompatUtil.cs`
  - 三版本 `ZoneAstral` 反射缓存与外部减益入口。
## 6. 1.4.5.7 配方口径
- 高级饰品使用 `DraedonsForge`。
- `AscendantSpiritEssence`、`CosmiliteBar`、`BloodOrb`、`RogueEmblem` 均已确认存在。
- 不要查询或依赖 `CosmicAnvil`；1.4.5.7 没有星宇砧。
- 涉及高级 EX 套件的通用材料：
  - 化神魂精 ×5
  - 宇宙锭 ×5
  - 日曜碎片 ×20
  - 夜明锭 ×10
## 7. 验证
编译：
```powershell
dotnet build -t:Compile -v q -nologo
```
隐式同名贴图检查：
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File `
  "D:\Game\Terraria\PrivateMod\CalamityExpansion\Tools\CheckImplicitTextures.ps1" `
  -Root "C:\Users\28155\Documents\My Games\Terraria\tModLoader\ModSources\CalamityRebirth"
```
当前最后一次验证：
- C# 编译 0 警告、0 错误。
- 直接隐式贴图 93 个类全部命中，4 个抽象类按规则跳过。
- 当前 C# 文件为 176 个，空行数为 0。
- 两套本地化均通过 tModLoader 自带 Hjson 库严格解析；中英文均为 374 条路径，类型差异为 0。
- 中英文本地化 `DisplayName` 均为 139 条，花括号均为 119/119，三引号均为 126/126。
- 2026-11-18 元素之握改为只施加 `ElementalMixII` 后，旧多减益逻辑与“元素谐鸣II”旧名称均无残留。
  构建会生成 `.vs`、`bin`、`obj` 缓存；2026-11-19 审计发现的 231 字节根目录 `$null`
  错误输出文件已删除。构建缓存不属于发布内容，不应作为工程记忆或交付文件保留。
## 8. 已知限制
- 当前只移植了用户点名的内容，不是完整 `CalamityExpansion`。
- 不要因为看到参考工程的大目录就再次全量复制。
- 现代 2.2.2 的血神核心已改名为 `ChaliceOfTheBloodGod` 且机制不同；本项目按用户指定保留旧版血神核心机制，
  只复用现代价格、稀有度与掉落口径。
- 本工程已独立移植 `CosmicPurpleRarity` 与 `TurquoiseRarity`，独立加载时也保持对应颜色和名称效果。
- `PureGreenRarity` 已补移植，Turquoise 的正向重铸现在按现代版进入 PureGreen。
- 龙魂秘药已按用户要求彻底删除，不恢复其物品、增益、配方、贴图或玩家结算。
- 三种高阶 Agentia 药水的现代配方使用四色月亮碎片各一；
  1.4.2.101 与 CalRD 继续使用各自原生银河奇点。
- 血神核心系列已完成编译与贴图检查，仍需进游戏回归测试三版本配方、宝藏袋条件、光环生成和多人表现。
- 2026-11-12 全量审计修复：晶光头骨、新生药水、光合作用红心治疗改为服务器验证请求；
  天界洋葱改为服务器验证解锁、每玩家独立掉落；血神核心低血/低防阈值延后到最终生命与防御结算；
  经典分支银河奇点与 CalRD 光合作用配方补回退；
  无尽饥渴只在本机所属客户端处理未归属掉落；酒类饮用动作、堆叠、粒子颜色、研究数量和暗影焰本地化已修正。
- 1.4.2.101 没有离散潜行打击消耗字段，75% 消耗仅适用于自带该字段的现代版与 `CalRD`。
- 2026-11-09 已删除无消费方的 `WarpShader`、`NeutronWarp`、`NeutronRingShader`、`HellfireIV`、
  `DRK_HeavenfallStar` 和根目录 `1.png`；除非新增对应功能，不要恢复。
- 破晓II火花已按灾厄 2.2.2 的 `MiracleBlight + GlowSparkParticle` 完整还原，仅把四色彩虹换成
  燃烧橙红色阶。当前实现为 `DaybreakSparkParticle` + `DaybreakParticleSystem` + `GlowSpark.png`。
  `DRKLoader`、`BaseParticle`、`DRK_Spark` 已全部删除，不要恢复；`StarProj.png` 只允许由
  `GodSlayerDashParticleSystem` 使用。
- 2026-11-10 删除破晓II的全屏渲染目标、`On_Main.DrawNPC` 着色 Hook、`EffectLoader`、
  `DaybreakII.fx/.fxc` 与 `Neurons.png`；该路径会造成命中后背景黑屏和严重卡顿，不要恢复。
  破晓II现使用 NPC 绘制颜色橙红染色与安全火花粒子。
- 2026-11-26 已彻底移除全部武器、武器专属配方、贴图、弹幕、音效和本地化，并完成非武器依赖拆分；
  该清理是不可逆口径，后续不得重新移植或变相恢复。
- 2026-11-10 修复灼辉金源中文 tooltip 黑屏：名称效果改为在当前 UI 批次直接绘制，
  不再创建全屏渲染目标或结束/重启 `SpriteBatch`。
- 晶光头骨仍需实测三版本属性、50% 潜行消耗、电工手套火花、闪电触发与触电II的 PvP/NPC 分支。
- 天界洋葱服务器验证、每玩家掉落与多人同步已实现，仍建议在主机/客机、多人同时击杀和重连场景实测。
- 完整打包可能因当前环境无权写入
  `C:\Users\28155\Documents\My Games\Terraria\tModLoader\Mods\CalamityRebirth.tmod`
  而失败；这是权限/文件占用问题，`-t:Compile` 仍可验证代码。
- 2026-11-14 新增的灼辉金源之翼、勇气勋章与烦恼项链均只完成编译/贴图/本地化验证，
  仍需进游戏回归飞行粒子、低生命动态增伤、毒液覆盖范围和三版本配方显示。
- 2026-11-16 新增的魔君之证章已完成编译、隐式贴图与本地化结构验证；
  仍需进游戏回归近战三围、低生命增伤、长无敌、岩浆免疫、圣火施加与三版本配方。
- 2026-11-18 新增的代达罗斯纹章已完成编译、贴图与本地化结构验证；
  仍需进游戏回归远程面板、20% 省弹、召唤击退、挖掘速度与三版本配方。
- 2026-11-18 新增的元素箭袋已完成编译、贴图与本地化结构验证；
  仍需进游戏回归箭矢分裂频率、子箭伤害、非箭远程弹幕 5% 乘算、PvP 与非箭判定。
- 2026-11-18 新增的空灵护符已完成编译、贴图与本地化结构验证；
  仍需进游戏回归魔法面板、魔力消耗、探宝、药水病、自动喝蓝与三版本配方。
- 2026-11-18 补齐混沌护符与灾厄符章后，法师空灵护符链已全部本地化；
  稀有度已按现代版修正，混沌护符已迁入 `Construction`；
  仍需进游戏回归灾厄符章八帧动画、两版邪恶水配方、混沌护符法系面板与完整合成链。
- 2026-11-18 新增的始源暗影炎已完成编译、贴图、掉落与本地化验证；
  仍需进游戏回归纯 +1 仆从栏、仆从与哨兵暗影焰、鞭子排除和哥布林召唤师掉落。
- 2026-11-18 新增的斯塔提斯祝福、诅咒与诅咒腰带已完成编译、贴图与本地化验证；
  仍需进游戏回归召唤栏不叠加、鞭子范围/攻速、时间之殇、暗影焰、
  盗贼桥接、移动作业与三版本配方。
- 2026-11-18 新增的盗贼镜片潜行链与纳米技术已完成编译、贴图和本地化结构验证；
  仍需进游戏回归半价/75% 潜行消耗、必暴字段、90 秒镜子闪避、
  蚀日强光、纳米刀刃追踪、NanoFlare 与多人服务器治疗同步。
- 2026-11-18 全工程审计确认：项目内部潜行单位统一为 `1f = 100`，
  `CRUtil` 已对传入的潜行增量和恢复比例做 `0–5` 安全约束，禁止整数点数直写。
  盗贼徽章EX为 `0.60f`，晶光头骨为 `1.20f`，镜片链各件为 `0.10f–0.25f`。
- 审计同时修复：晶光头骨改为通用 50% 消耗档，不再与 75% 档乘算；
  镜片链潜行上限先于晶光头骨结算，确保最终上限与统一消耗档一致；
  CalRD/1.4.2 多版本桥改为单分支返回；1.4.2 镜子闪避可恢复当前潜行；
  召唤腰带不再误用会自动附赠潜行恢复的旧 `ApplyStats`；
  纳米治疗仅由主人客户端发起服务器请求；新增弹幕索引均增加越界保护。
- 2026-11-18 进一步核对 ReferenceMod 后确认：50%/75% 消耗依赖
  `stealthStrikeHalfCost`、`stealthStrike75Cost` 两个每帧字段。`CRUtil` 现优先通过
  `GetContent<ModPlayer>()` 定位实际 CalamityPlayer，并在初始化缺失时自动重试；
  同时在 `PostUpdateEquips` 与 `PostUpdateMiscEffects` 写入同一消耗档，避免字段写入失败。
  Mod.Call 设置盗贼潜行模式后还会通过反射兜底写入 `wearingRogueArmor`。
- 2026-11-18 元素之握已改为只施加 `ElementalMixII`，其视觉恢复为现代原始元素混合效果，
  饰品稀有度改为与血神核心一致的 `CosmicPurpleRarity`；仍需进游戏回归三版配方、
  NPC 700 HP/s/-50 防御/+15% 易伤、
  玩家 50 HP/s/-50 防御/+15% 易伤与 PvP 路径。
- 2026-11-13 新增的王冠宝石、感染宝石与无暇粹魂晶已完成编译/贴图/本地化验证，
  仍需进游戏回归史莱姆王掉落、三版本配方、减益清单、治疗节拍、动态防御回落与多人同步。
- 判断 C# 文件是否“无消费方”时，不能只做普通字符串引用搜索：`ModItem`、`ModBuff`、
  `ModProjectile`、`ModRarity`、`ModSystem`、`GlobalItem`、`GlobalNPC`、`GlobalProjectile`
  由 tModLoader 自动加载；`[AutoloadEquip]` 还会隐式消费 `_Wings`、`_Neck` 等后缀贴图。
- 勇气勋章按用户明确要求保留近战穿甲 10；现代 2.2.2 原值为 5，不要按现代值“修正”。
- 烦恼项链源码只给 NPC 命中附加毒液，不给 PvP 玩家路径补毒液；不要误扩到 `OnHitPvp`。
- 2026-11-16 按用户要求删除自持的余烬核心中间物品。经典 1.4.2.101 与 CalRD
  仍必须使用各自灾厄模组原生的 `CoreofCinder`；只有原生物品已被删除的现代 2.2.2
  才允许在魔君之证章配方中直接展开为 `EssenceofSunlight + Ectoplasm`。
## 9. 2026-11-19 全量审计与交接快照
- 审计方式：
  - `dotnet build -t:Compile -v q -nologo`。
  - `CheckImplicitTextures.ps1` 与 `CheckResources.ps1`。
  - 使用 tModLoader 自带 `Hjson.dll` 递归比较中英文全部对象路径与叶子类型。
  - 扫描重复本地化键、空字符串、重复显示名、NUL/缺失资源、`TODO/FIXME/NotImplemented`。
  - 对本轮 Demutation 血脑/融合之脑链逐项做源码行为对照。
- 已确认无问题：
  - 176 个手写 C# 文件编译 0 警告、0 错误，空行数 0。
  - 93 个直接隐式贴图全部命中；44 条资源路径全部存在。
  - 中英文 Hjson 均可严格解析，双方均为 374 条路径，没有重复键、空字符串、
    同文件重复物品显示名、缺失键或叶子类型冲突。
  - 83 个 `Items` 本地化键均能对应到同名的本工程物品类。
  - 弱依赖查询仅使用 `CalamityMod`、`CalamityModClassicPreTrailer`、`CalRD`；
    69 个弱依赖物品名与 2 个工作站名都能在参考源码中找到同名类。
- 本轮已修正：
  - 中文补齐 `CrystalSkullLightning`、`CrystalSkullSpark`、`AbyssalMirrorProjectile`、
    `EclipseMirrorBurst`、`NanotechBlade` 五条弹幕显示名，Hjson 路径达到 374/374。
  - `FungalClump` 仆从伤害恢复为 `10f * Summon.Multiplicative`，与 Demutation 一致。
  - 灾厄之环站火恢复固定 30 点伤害；终结虚空与融合之脑仍按源码应用通用伤害倍率。
  - `CalamityRing` 互斥条件补回自身，防止同种饰品多槽重复叠加。
  - `FungalClumpMinion` 移除显式伤害类型，恢复 Demutation 的默认结算口径。
  - `FungalHeal` 恢复两档蓝色妖精尘；`moonLeech` 使用目标玩家判定，
    由于目标必然是弹幕主人，与源码本地玩家检查等价。
  - 聚合/融合之脑受击困惑恢复为只排除友方 NPC，不再额外跳过 `dontTakeDamage`。
  - 宝藏袋改用 `ItemLoot.Get/Remove` 递归替换原生组件规则，再追加本工程组件；
    腐巢意志、硫磺火元素/硫磺火妻、灾厄/克隆体、克鲁布龙、利维坦不再同时掉双版本同名件。
  - 根目录 `$null` 错误输出文件已删除。
- 有意的三版本差异：
  - 现代仍存在的组件按现代稀有度/价格；现代删除的灾厄之环与终结虚空保留 Demutation/CalRD 档位。
  - `TheAmalgam` 使用现代、1.4.2、CalRD 三版本配方；Demutation 本身没有 CalRD 分支。
  - 中文把顶点命名为“融合之脑”，避免与 `Amalgamated Brain / 聚合之脑` 重名；英文仍为 `The Amalgam`。
- 新会话优先顺序：
  1. 进游戏回归三版本宝藏袋，确认 `ItemLoot.Remove` 后只剩本工程组件且条件掉率未受影响。
  2. 回归真菌仆从伤害/吸血、毒海水生成频率、火球/站火伤害和多人同步。
  3. 回归融合之脑互斥、受击困惑范围与 1/8 闪避。
  4. 检查三版本配方显示和 CalRD 顶点配方。
## 10. 饰品目录分类（2026-11-19）
- 固定根目录：
  - `Content/Items/Accessories/Combat`
  - `Content/Items/Accessories/Movement`
  - `Content/Items/Accessories/Information`
  - `Content/Items/Accessories/Immunity`
  - `Content/Items/Accessories/Construction`
  - `Content/Items/Accessories/HealthandMana`
  - `Content/Items/Accessories/Fishing`
  - `Content/Items/Accessories/Miscellaneous`
- 当前归档：
  - `Combat`：全部职业战斗饰品、综合输出/专家战斗核心、召唤战斗饰品。
  - `Movement`：`AncientAuricWings / 灼辉金源之翼`。
  - `Information`：当前无物品，保留给探宝、敌人信息、世界信息类饰品。
  - `Immunity`：当前无物品；主要提供状态免疫、减益抵抗的饰品放这里。
  - `Construction`：`ChaosAmulet / 混沌护符`；挖掘、建造、探矿类放这里。
  - `HealthandMana`：`BloodPact`、`CrownJewel`、`InfectedJewel`、`Purity`；
    生命、防御、恢复、生命/法力资源与宝石链固定放这里。
  - `Fishing`：当前无物品，保留给钓鱼与渔获类饰品。
  - `Miscellaneous`：当前无物品；无法归入其他主用途的综合被动件放这里。
- 多效果物品按主用途归类，不按附带属性拆分类：
  - 元素箭袋虽带挖掘速度，主用途仍是远程战斗，留在 `Combat/Ranged`。
  - 蚀日尊戒虽带挖掘速度与再生，主用途仍是综合战斗，留在 `Combat`。
  - 融合之脑与利维坦龙涎香虽带移动/水下效果，主用途仍是战斗，留在 `Combat`。
- 移动代码与同名贴图必须一起迁移，并同步修改 C# `namespace`、调用方 `using` 和工程记忆路径。
## 11. 2026-11-19 收工快照
- 本轮已完成：
  - 全工程代码与本地化审计，并修正审计发现的问题。
  - 中英文 Hjson 达到 374/374，缺失键 0、类型差异 0。
  - 真菌团块、灾厄之环、宝藏袋替换、互斥和弹幕表现已按 Demutation 口径修正。
  - 饰品目录完成主用途分类，`HealthandMana` 按用户要求保留并恢复。
- 当前饰品目录及 C# 文件数：
  - `Combat`：44
  - `Movement`：1，`AncientAuricWings`
  - `Information`：0
  - `Immunity`：0
  - `Construction`：1，`ChaosAmulet`
  - `HealthandMana`：4，`BloodPact`、`CrownJewel`、`InfectedJewel`、`Purity`
  - `Fishing`：0
  - `Miscellaneous`：0
- 当前验证：
  - `dotnet build -t:Compile`：0 警告、0 错误。
  - 隐式贴图 93/93 命中。
  - 资源路径 44/44 命中。
  - 176 个手写 C# 文件，空行数 0。
- 明日继续时优先做游戏内回归：
  1. 三版本宝藏袋是否只掉本工程组件。
  2. 真菌团块伤害/吸血、毒海水频率、站火与火球伤害。
  3. 融合之脑互斥、1/8 闪避、受击困惑和多人同步。
  4. 确认 `HealthandMana` 分类迁移后物品、配方、掉落与贴图正常。
- 用户尚未指定下一件移植物品；新会话应先询问或等待点名，不要自行扩大移植范围。
## 12. 2026-11-20 中止任务交接快照
- 本轮已完成：
  - `灾劫之尖啸 / Affliction`：防御 +45、通用伤害 +15%、最大生命 +20%、约 4 HP/s、
    减伤 +8%、击退免疫，并向同队玩家传播 `Afflicted / 侵蚀`。
  - 阿斯加德完整本地链六件：华丽盾、海之盾、阿巴顿、极乐之庇护、阿斯加德之英勇、
    阿斯加德之庇护；盾牌冲刺、守护按键、`AbaddonCrit`、`HolyExplosionSupreme` 与联机同步均已接入。
  - 涡流虹吸石完整本地链二十件：远古化石、太古粉末、腐化之瓶、猩红之瓶、辉光软泥、
    生命之露、蜜露、百草天香瓶、法力凝胶、生命凝胶、活力凝胶、绚彩凝胶、海螺壳、
    巨蟹甲壳、巨型陆龟壳、真菌甲壳、巨型贝壳、阿米迪亚斯之火花、阴阳吸星石、涡流虹吸石。
  - `Spark`、`ShellBoost`、受伤前吸收治疗、蜂类弹幕减伤、受击火花/孢子反击和相应掉落均已接入。
- 稀有度修正：
  - 辉光软泥：浅红；生命之露：黄绿；蜜露：橙；活力凝胶：蓝；绚彩凝胶：粉；巨型陆龟壳：浅红。
  - 现代已删除的链组件保留 Demutation 或对应经典档位；涡流虹吸石保留月后深蓝。
- 配方修正：
  - 补齐 CalRD 的腐化/猩红瓶、生命露、蜜露与太古粉末专用原料。
  - 涡流虹吸石链不再依赖外部同名前置饰品，所有本地合成节点均可闭合。
- 当前验证：
  - 完整 `dotnet build`：0 警告、0 错误。
  - 219 个手写 C# 文件，空行数 0。
  - 涡流虹吸石链 20 件物品贴图与 `ShellBoost` 图标 SHA-256 全部命中。
  - 中英文 Hjson 严格解析通过。
  - C# 注释中的来源归属关键词扫描 0 命中。
- 当前饰品目录：
  - `Combat`：60 个物品类。
  - `Movement`：8 个物品类。
  - `Construction`：3 个物品类。
  - `HealthandMana`：9 个物品类。
  - `Information`、`Immunity`、`Fishing`、`Miscellaneous`：0。
- 对照统计：源目录 95 个饰品物品类，本工程已对应 69 个，剩余 26 个：
  - `AeroStone`、`AmbrosialAmpoule2`、`AngelTreads`、`BloomStone`、`ChaosStone`、
    `CryoStone`、`DrewsWings`、`ElysianTracers`、`EyeoftheStorm`、`FrostBarrier`、
    `HeartoftheElements`、`HoneyDew2`、`InfinityBoots`、`LeadCore`、`LivingDew2`、
    `LureofEnthrallment`、`ManaPolarizer`、`NebulousCore`、`PsychoticAmulet`、
    `QuiverofNihility`、`RadiantOoze2`、`RoseStone`、`SeraphTracers`、`StatisNinjaBelt`、
    `WifeinaBottle`、`WifeinaBottlewithBoobs`。
- 用户已要求中止当前任务并新开任务。新会话只读取本快照后等待新指令，不自行继续补链。
## 13. 2026-11-21 GitHub 仓库封装
- 本地目录已经是标准 Git 仓库，Git 元数据位于常规 `.git` 目录。
- GitHub 远端固定为：
  - `origin`
  - `https://github.com/LiFeiAbom/CalamityRebirth.git`
- 当前发布分支为 `master`，本地、`origin/master` 与 GitHub 远端均已对齐。
- 已建立的提交：
  - `5f5834f chore: 初始化 CalamityRebirth 仓库`
  - `4068f78 docs: 补齐仓库说明与协作规范`
- 当前版本库跟踪 413 个文件，工作区应保持干净。
- 仓库维护文件：
  - `README.md`：项目简介、当前内容、兼容环境、构建方式、仓库结构与反馈入口。
  - `CONTRIBUTING.md`：修改原则、问题报告格式、代码提交与素材要求。
  - `LICENSE.md`：使用与再分发条款；本仓库不授予开源许可证。
  - `.editorconfig`：UTF-8、LF、C# 制表符缩进和配置文件空格缩进。
  - `.gitattributes`：统一文本行尾并保护 PNG、OGG、WAV、XNB 二进制资源。
- `.gitignore` 固定排除：
  - `bin/`、`obj/`、`.vs/`、`.idea/`、`.vscode/`。
  - `*.tmod`、`*.tmod.pdb`、用户文件、测试结果与日志。
  - `.git-old-*/`、`tmp-remove-git-*/` 等受限会话产生的临时 Git 清理目录。
- 后续提交前至少检查：
  - `git status -sb`，不要提交构建缓存、临时目录或敏感凭据。
  - 涉及 C# 时执行 `dotnet build`，并继续遵守本文件的无空行、文档注释与兼容分支规则。
  - 涉及本地化时检查中英文 Hjson 路径一致。
- 正常发布命令：
  - `git push origin master`
- 不得把 GitHub 令牌、代理账号、Cookie、Credential Manager 内容或其他认证信息写入仓库、文档或 `AGENTS.md`。
- 本快照只记录稳定仓库状态；临时代理端口、登录账号和本次命令行绕过方式不属于工程记忆。
## 14. 2026-11-21 星云核心移植快照
- 本轮完成 `NebulousCore / 星云核心` 全链：
  - 物品、`NebulaStar`、`NebulaDust`、玩家保命结算和三版本神之使徒宝藏袋替换掉落。
  - 四张贴图均从 Demutation 原件复制并通过 SHA-256 核对。
  - 中英文本地化新增物品名、说明、星云之星与星云尘埃显示名。
- 当前验证：
  - `dotnet build -t:Compile -v q -nologo`：0 警告、0 错误。
  - 223 个手写 C# 文件，空行数 0。
  - 132 个直接隐式贴图与 4 个抽象类检查通过，没有缺失。
  - 资源路径 44/44 存在。
  - 中英文 Hjson 均可严格解析，新增路径类型一致。
- 当前饰品数量：
  - `Combat`：61 个物品类。
  - `Movement`：8 个物品类。
  - `Construction`：3 个物品类。
  - `HealthandMana`：9 个物品类。
  - 全部饰品 C# 文件共 81 个。
- 当前 Demutation 饰品对照：95 个源物品类中已对应 70 个，剩余 25 个；
  `NebulousCore` 已从剩余清单完成。
- 仍需进游戏回归：
  - 三版本神之使徒宝藏袋是否只掉本工程星云核心。
  - 隐藏外观后已有星星是否立即消散。
  - 星星生成上限、600 像素追踪、命中尘埃和 PvP 路径。
  - 1/10 致命伤保护、100 点治疗和多人同步表现。
## 15. 2026-11-21 元灵之心链移植快照
- 本轮完成 `HeartoftheElements / 元灵之心` 全链：
  - 4 枚元素石、5 件元素召唤饰品、元灵之心本体。
  - 5 个元素仆从、7 个辅助攻击/沙暴/治疗弹幕和 6 个常驻召唤增益。
  - 三版本普通 Boss、专家宝藏袋和沙元素掉落路径全部接入。
  - 10 张物品贴图、9 张仆从/攻击弹幕贴图和 6 张增益图标逐字节复制并核对 SHA-256。
- 当前验证：
  - `dotnet build -t:Compile -v q -nologo`：0 警告、0 错误。
  - 255 个手写 C# 文件，空行数 0。
  - 160 个直接隐式贴图与 4 个抽象类检查通过，没有缺失。
  - 资源路径 44/44 存在。
  - 中英文 Hjson 严格解析通过，双方均为 549 条对象路径，缺失 0、多余 0、类型差异 0。
  - 同时修复了中文原有 7 条被注释掉的弹幕显示名。
- 当前饰品数量：
  - `Combat`：71 个物品类。
  - `Movement`：8 个物品类。
  - `Construction`：3 个物品类。
  - `HealthandMana`：9 个物品类。
  - 全部饰品 C# 文件共 91 个。
- 当前 Demutation 饰品对照：95 个源物品类中已对应 84 个，剩余 11 个：
  `FrostBarrier`、`LeadCore`、`ManaPolarizer`、`PsychoticAmulet`、`QuiverofNihility`、
  `AngelTreads`、`StatisNinjaBelt`、`DrewsWings`、`ElysianTracers`、`InfinityBoots`、`SeraphTracers`。
- 仍需进游戏回归：
  - 三版本元素石普通掉落与宝藏袋替换是否各只生成一次。
  - 元灵之心可见/隐藏切换是否立即关闭或重建五种元素。
  - 五种元素仆从的索敌、远程攻击、沙暴、治疗珠和多人同步。
  - 花绽石与元灵之心的诅咒光环伤害、植物生长方块同步。
## 17. 2026-11-21 无暇粹魂晶古老生命链补齐
- 新增 `HoneyDew2`、`LivingDew2`、`RadiantOoze2`、`AmbrosialAmpoule2` 四件本地饰品。
- 玩家结算新增：
  - `honeyTurboRegen`：蜂蜜内静止再生。
  - `aAmpoule`：甘露安瓿（古）静止再生与按缺失生命动态再生。
  - `rOoze`：光辉软泥（古）按缺失生命动态再生。
  - `livingDewHalveDebuffs`：把时长减半从病症类扩展到火系类。
- `BuffUtil` 新增独立的病症/中毒与火系/燃烧分类查询；无暇粹魂晶仍使用完整战斗减益清单。
- `Purity` 三版本配方全部改为消耗本工程 `AmbrosialAmpoule2`。
- 当前验证：
  - 编译 0 警告、0 错误。
  - 255 个手写 C# 文件，空行数 0。
  - 163 个直接隐式贴图与 4 个抽象类检查通过。
  - 资源路径 44/44 存在。
  - 中英文 Hjson 均为 558 条路径，缺失、多余、类型差异均为 0。
- 当前 Demutation 对照：已对应 84 件，剩余 11 件。
## 18. 2026-11-21 删除银河奇点材料
- 已彻底删除本工程 `GalacticaSingularity / 银河奇点`：
  - 删除物品类、贴图、中英文本地化键及空的 `Content/Items/Materials` 目录。
  - 不恢复同名材料或替身物品。
- 仅现代 `CalamityMod` 配方按等价关系替换：
  - 旧现代配方消耗银河奇点 ×N 时，改为太阳碎片、星旋碎片、星云碎片、星尘碎片各 ×N。
  - 涉及元素方舟、欧米茄环境之刃、神之壁垒、神圣之怒、渎神之怒与无尽饥渴。
  - 1.4.2.101 与 CalRD 1.4.5.7 继续使用各版本原生 `GalacticaSingularity`。
- 当前验证：
  - 编译 0 警告、0 错误。
  - 255 个手写 C# 文件，空行数 0。
  - 163 个直接隐式贴图全部命中。
  - 资源路径 44/44 存在。
  - 中英文 Hjson 均为 558/558，缺失、多余与类型差异均为 0。
  - 本工程不再持有银河奇点物品；经典配方只通过弱依赖引用外部原生件。
## 19. 2026-11-21 炽天使翼靴链移植快照
- 已完成五件链：
  - `AngelTreads / 天使之靴`
  - `InfinityBoots / 天界翼靴`
  - `ElysianTracers / 极乐翼靴`
  - `DrewsWings / Drew之翼`
  - `SeraphTracers / 炽天使翼靴`
- 天界、极乐、炽天使三件均支持 Shift+右键切换翅膀/履带模式：
  - 状态保存在物品实例上，写入 `SaveData/LoadData`。
  - 通过 `NetSend/NetReceive` 与 `Item.NetStateChanged()` 同步。
  - 履带模式分别提供火箭靴 4/1/2 级，并继承天使之靴 +10% 飞行时间。
- 炽天使翼靴：
  - 单次伤害超过 200 时追加 30 帧无敌，仅本地玩家写入，PvP 同样适用。
  - 现代灾厄通过反射镜像 `seraphTracers`，获得金源排斥与危险砖块免疫。
  - 反射绑定缓存在卸载阶段清空，避免热重载后保留旧字段。
- 配方已覆盖现代、1.4.2.101 与 CalRD；Drew之翼从三版本犽戎宝藏袋固定替换掉落。
- 当前验证：
  - 编译 0 警告、0 错误。
  - 261 个手写 C# 文件，空行数 0。
  - 168 个直接隐式贴图与 4 个抽象类检查通过。
  - 资源路径 44/44 存在。
  - 中英文 Hjson 均为 576/576，缺失、多余与类型差异均为 0。
- 当前饰品数量：
  - `Movement`：13 个物品类。
  - 全部饰品 C# 文件共 100 个。
- 当前 Demutation 对照：95 个源物品类中已对应 89 个，剩余 6 个：
  `FrostBarrier`、`LeadCore`、`ManaPolarizer`、`PsychoticAmulet`、
  `QuiverofNihility`、`StatisNinjaBelt`。
- `TornadoProj.png` 固定放在 `Content/Projectiles` 根目录，与 `InvisibleProj.png` 同级；
  `SandTornado` 的显式贴图路径必须为 `CalamityRebirth/Content/Projectiles/TornadoProj`。
## 16. 2026-11-21 Utilities 通用化整理
- 保留原有 `WeaponUtil`、`MaterialUtil`、`ProjectileUtil`、`RogueUtil`、`CollisionUtil`
  五个 `CRUtil` 分部和 `CalamityCompatUtil` 不变。
- `ModernRarityValueUtil` 已重命名为 `RarityUtil`，所有调用同步更新为 `RarityUtil.Apply`。
- 旧的减益查询与环境增益工具已合并为 `BuffUtil`：
  - `BuffUtil.Contains`：战斗减益查询。
  - `BuffUtil.Unload`：热重载清理。
  - `BuffUtil.ApplyEnvironmentBuffs`：月亮事件、群系与星陨环境增益。
- `Utilities` 当前文件：
  - `BuffUtil.cs`
  - `CalamityCompatUtil.cs`
  - `CollisionUtil.cs`
  - `MaterialUtil.cs`
  - `PerPlayerDropRule.cs`
  - `ProjectileUtil.cs`
  - `RarityUtil.cs`
  - `RogueUtil.cs`
  - `WeaponUtil.cs`
- 当前版本已提升为 `0.1.2.6`，并同步更新：
  - `build.txt`
  - `description.txt`
  - `description_workshop.txt`
  - `README.md`
- 描述文件已补充 `NebulousCore / 星云核心` 与 `HeartoftheElements / 元灵之心` 全链。
## 20. 2026-11-22 古圣金源前置饰品
- 已完成古圣金源套装的两件直接前置饰品：
  - `FrostBarrier / 寒霜屏障`：归入 `Content/Items/Accessories/Combat`，20×24；
    三版本统一采用 Demutation 的 4 防御、9 金与橙色稀有度；仅免疫寒冷，
    受击冻结按 200～900 像素衰减范围、90～240 帧浮动时长与原生 `GlacialState` 结算，
    缺失时回退原版 `Frozen`。
  - `PsychoticAmulet / 疯狂护符`：归入 `Content/Items/Accessories/Combat/Ranged`，26×26；
    三版本统一采用 Demutation 的 15 金与浅紫稀有度；远程伤害/暴击各 +5%，
    提供蘑菇矿潜行，站定潜行最高再提供远程伤害 +20%、远程暴击 +10、仇恨 -750。
- 获取方式：
  - 寒霜屏障：三版本 `IceClasper` 均为 1/10；旅行商人上架条件沿用 Demutation。
  - 疯狂护符：原版着魔盔甲普通 1/200、专家/大师 1/150，三版本一致，无合成配方。
  - 同名原生 NPC 掉落和商店商品会替换为本工程版本，避免同一来源生成两份。
- 寒霜冻结优先依次查找三个灾厄分支的 `GlacialState`，均缺失时回退原版 `Frozen`。
- 古圣金源套装及其全部下位前置统一以 `CalamityDemutation` 的内容、数值与行为为准；
  `CalamityExpansion` 与其他灾厄版本只用于三版本外部材料、合成站和弱依赖适配。
- 玩家结算集中在 `Players/CalamityRebirthPlayer.AuricTeslaAccessories.cs`；装备标记已接入
  `ResetEffects`、`OnHurt` 与 `PostUpdateMiscEffects`。
- 两张贴图逐字节复制自 Demutation 并通过 SHA-256 核对。
- 当前验证：
  - 编译 0 警告、0 错误；264 个手写 C# 文件，空行数 0。
  - 中英文 Hjson 均可严格解析，双方均为 404 条路径，缺失、多馀与类型差异均为 0。
  - Demutation 饰品剩余 4 件：`LeadCore`、`ManaPolarizer`、`QuiverofNihility`、
    `StatisNinjaBelt`。
## 21. 2026-11-22 龙蒿系列盔甲
- 文件放在 `Content/Items/Armors/Tarragon`，共 7 个物品类与 14 张物品/装备贴图：
  - `TarragonBreastplate`：37 防御、40 生命、约 1 HP/s 再生、通用伤害/暴击各 +10%。
  - `TarragonLeggings`：32 防御、移速 +20%，半血以下再 +15%，通用伤害/暴击各 +10%。
  - `TarragonHelm`：近战伤害/暴击 +10%、攻速 +15%、减伤 +5%。
  - `TarragonVisage`：远程伤害/暴击 +10%、减伤 +5%。
  - `TarragonMask`：魔法伤害/暴击 +10%、减伤 +5%、最大魔力 +100、蓝耗 -15%。
  - `TarragonHornedHelm`：仆从栏 +3、召唤伤害/鞭速/鞭长各 +12%、减伤 +5%。
  - `TarragonHelmet`：盗贼伤害/暴击 +10%、减伤 +5%。
- 五个头部均提供 4 秒岩浆免疫、液体自由移动，并免疫诅咒地狱火、着火了、诅咒与寒冷。
- 配方覆盖三版：
  - 现代：`UelibloomBar` 胸/腿/头 24/18/12、`DivineGeode` 18/12/6，秘银砧。
  - 经典 1.4.2.101 与 CalRD：`UeliaceBar` 胸/腿/头 15/11/7、`DivineGeode`
    18/12/6，远古操纵机。
- 运行依赖：
  - 新增 `TarraLifeRegen` 正面 Buff 与 `TarraEnergy` 隐形生命能量弹幕。
  - 新增 `TarragonKeybinds`，默认 `Y` 启动近战生命护罩。
  - `CalamityRebirthPlayer.Tarragon.cs` 负责通用、五职业、套装计数器与跨帧冷却。
  - `CalamityRebirthGlobalItem.Shoot` 处理法师第 5 次暴击后的叶暴风。
  - `CalamityRebirthGlobalProjectile.OnKill` 处理远程弹幕消失分裂生命能量。
  - `CalamityRebirthGlobalNPC.OnKill` 处理龙蒿红心额外掉落。
- 套装效果：
  - 通用：红心磁吸、除近战头外降低刷怪率、合法敌怪死亡 1/5 额外掉红心。
  - 近战：受击 25% 获得 90～179 帧 `TarraLifeRegen`；Y 键 10 秒接触伤害减半，冷却 30 秒。
  - 远程：暴击产生 2～3 枚树叶；弹幕消失 12% 产生 2～3 枚生命能量，单枚封顶 65。
  - 法师：累计 5 次魔法暴击，下次魔法射击释放 9～11 枚树叶；法弹命中按伤害/50 受限治疗。
  - 召唤：套装已有 +50% 召唤伤害；满血额外 +2 仆从栏与 +10% 召唤伤害；每 80 帧结算一次
    300 像素、120 基础伤害的生命光环。
  - 盗贼：潜行上限 +130；每条有效减益 +10% 盗贼伤害；25 次盗贼暴击触发 5 秒免伤，
    冷却 30 秒。
- 当前验证：
  - 编译 0 警告、0 错误；276 个手写 C# 文件，空行数 0。
  - 14 张龙蒿防具贴图与 Demutation 原件 SHA-256 全部一致。
  - 中英文 Hjson 均为 427 条路径，缺失、多馀与类型差异均为 0。
  - 隐式贴图 172/172 命中，资源路径 44/44 命中。
## 22. 2026-11-22 血炎系列盔甲
- 文件放在 `Content/Items/Armors/Bloodflare`，共 7 个物品类与 14 张物品/装备贴图：
  - `BloodflareBodyArmor`：35 防御、+40 生命、通用伤害/暴击各 +14%；岩浆中 +30 防御与再生。
  - `BloodflareCuisses`：29 防御、移速 +30%、通用伤害/暴击各 +10%。
  - `BloodflareMask`：近战伤害/暴击 +10%、攻速 +18%。
  - `BloodflareHornedHelm`：远程伤害/暴击 +10%。
  - `BloodflareHornedMask`：魔法伤害/暴击 +10%、最大魔力 +100、蓝耗 -17%。
  - `BloodflareHelmet`：仆从栏 +3、召唤伤害/鞭速/鞭长各 +12%。
  - `BloodflareHelm`：盗贼伤害/暴击 +10%。
- 所有头部提供 4 秒岩浆免疫与液体自由移动；套装激活时开启细微残影与猩红再生。
- 配方覆盖三版：
  - 现代：`Bloodstone` 胸/腿/头 50/35/25、`BloodOrb` 20/15/10、
    `RuinousSoul` 4/3/2，秘银砧。
  - 经典 1.4.2.101 与 CalRD：`BloodstoneCore` 16/13/11、`RuinousSoul` 4/3/2，
    远古操纵机。
- 新增五个弹幕：
  - `BloodBomb`、`BloodBombExplosion`：2% 远程追加血弹与 250×250 范围爆炸。
  - `GhostlyBolt`：5% 魔法追加隐形幽灵魔弹。
  - `GhostlyMine`：召唤套装每 15 秒生成三枚 550 像素环绕地雷。
  - `BloodflareSoul`：套装键一次释放 16 枚追踪灵魂，单枚基础伤害 800。
- 套装效果：
  - 通用：红心/魔力星掉落各有 180 帧冷却；血月地表击杀 1/2 掉落当前版本血珠。
  - 近战：真近战命中回血；累计 15 次后 5 秒狂怒，近战伤害/暴击 +25%、接触伤害减半，
    冷却 30 秒。
  - 远程：2% 血液炸弹；套装键释放灵魂爆发，冷却 30 秒。
  - 法师：5% 幽灵魔弹；魔法暴击每 2 秒释放三枚火球。
  - 召唤：生命 ≥90% 时召唤伤害 +10%；≤50% 时 +20 防御与 +2 再生；每 15 秒生成环绕地雷。
  - 盗贼：生命 >80% 时 +30 防御与 +5 盗贼暴击；否则 +10% 盗贼伤害；
    潜行上限 +135；盗贼暴击 50% 回 1 HP。
- 当前验证：
  - 编译 0 警告、0 错误；290 个手写 C# 文件，空行数 0。
  - 14 张防具贴图和 4 张弹幕贴图与 Demutation 原件 SHA-256 全部一致。
  - 中英文 Hjson 均为 451 条路径，缺失、多馀与类型差异均为 0。
  - 隐式贴图 177/177 命中，资源路径 44/44 命中。
## 23. 2026-11-22 始源林海系列盔甲
- 文件放在 `Content/Items/Armors/Silva`，共 7 个物品类与 14 张物品/装备贴图：
  - `SilvaArmor`：44 防御、+80 生命、移速 +20%、通用伤害/暴击各 +18%。
  - `SilvaLeggings`：39 防御、移速 +45%、通用伤害/暴击各 +12%。
  - `SilvaHelm`：52 防御、近战伤害/暴击 +13%、攻速 +19%。
  - `SilvaHornedHelm`：36 防御、远程伤害/暴击 +13%。
  - `SilvaMaskedCap`：21 防御、魔法伤害/暴击 +13%、最大魔力 +100、蓝耗 ×0.81。
  - `SilvaHelmet`：13 防御、仆从栏 +5、召唤伤害/鞭速/鞭长各 +12%。
  - `SilvaMask`：30 防御、盗贼伤害/暴击 +13%、移速 +20%、最大潜行 +150。
- 三版本配方按 Demutation：
  - 现代：胸 90/12/4、腿 60/10/3、头 30/8/2（植物混融块/耀斑羽/至尊精神），
    全部额外消耗本工程铅芯，宇宙砧。
  - 经典 1.4.2.101：胸 10/10/10/12/16/16、腿 7/7/7/9/15/15、
    头 5/5/5/6/14/14（暗日碎片/耀斑羽/宇宙锭/暗影之魂/梦魇燃料/吸热能量），嘉登熔炉；
    只有胸甲消耗本工程铅芯。
  - CalRD：胸 10/10/10/12/3、腿 7/7/7/9/2、头 5/5/5/6/2
    （暗日碎片/耀斑羽/金锭/暗影之魂/至尊精神），嘉登熔炉；只有胸甲消耗本工程铅芯。
- 通用套装：战斗减益近全免疫、任意友方弹幕命中生成治疗叶球、跑速与加速度 +5%。
- 免死：1 HP 时开启 600 帧窗口；窗口内再次致命伤扣 100 最大生命，
  最大生命降至 400 时终止；死亡后全部恢复；网络状态由服务端广播。
- 近战：真近战 25% 五倍伤害；近战弹幕 25% 施加 20 帧始源林海迟滞；
  免死结束后接触伤害 ×0.8。
- 远程：持续远程攻速 +10%；免死窗口内远程最终伤害 ×1.4。
- 法师：免死窗口内魔法伤害 +60%；单穿透或即将消失的魔法弹触发 300 帧冷却的 96×96 爆裂。
- 召唤：套装召唤伤害 +75%；生成零仆从栏占用的远古叶棱晶，基础伤害 1500，
  每 25 帧向 1500 像素内目标发射三枚爆裂；免死结束后 +2 仆从栏与 +10% 召唤伤害。
- 盗贼：生命 >50% 时盗贼攻速 +10%；免死窗口内盗贼伤害 +40%。
- 新增 `LeadCore / 铅芯`，放在 `Accessories/Immunity`；三版本辐照史莱姆 1/10 掉落，
  替换原生同名掉落，装备后免疫当前版本 `Irradiated`。
- 新增 Buff：`SilvaRevival`、`SilvaHysteresis`、`SilvaCrystal`。
- 新增弹幕：`SilvaOrb`、`SilvaBurst`、`SilvaCrystal`、`SilvaCrystalExplosion`。
- 当前验证：
  - 编译 0 警告、0 错误；307 个手写 C# 文件，空行数 0。
  - 14 张始源林海防具贴图与 Demutation 原件 SHA-256 全部一致。
  - 中英文 Hjson 均为 479 条路径，缺失、多余与类型差异均为 0。
  - 隐式贴图 161/161 命中，显式贴图 24 个，抽象类 7 个按规则跳过。
## 24. 2026-11-22 弑神者系列盔甲
- 文件放在 `Content/Items/Armors/GodSlayer`，共 7 个物品类与 14 张物品/装备贴图：
  - `GodSlayerChestplate`：41 防御、60 金、+60 生命、移速与通用伤害/暴击各 +15%，
    反伤 +0.9，并启用 2% 完全闪避与动态低伤保护。
  - `GodSlayerLeggings`：35 防御、45 金、移速 +35%、通用伤害/暴击各 +11%。
  - `GodSlayerHelm`：48 防御、75 金、近战伤害/暴击 +14%、攻速 +20%。
  - `GodSlayerHelmet`：35 防御、远程伤害/暴击 +14%。
  - `GodSlayerVisage`：21 防御、魔法伤害/暴击 +14%、最大魔力 +100、蓝耗 ×0.83。
  - `GodSlayerHornedHelm`：12 防御、仆从栏 +3、召唤伤害/鞭速/鞭长各 +12%。
  - `GodSlayerMask`：29 防御、盗贼伤害/暴击 +14%、移速 +18%、潜行上限 160。
- 所有部件使用现代 `Cosmic Purple` 稀有度；三版本价格按 Demutation，头 75 金、胸 60 金、腿 45 金。
- 配方：
  - 现代：头宇宙锭×10 + 至尊精神×2，胸 ×20/×4，腿 ×15/×3，宇宙砧。
  - 经典 1.4.2.101：头宇宙锭×14 + 梦魇燃料×8 + 吸热能量×8，
    胸 23/11/11，腿 18/9/9，嘉登熔炉。
  - CalRD：头宇宙锭×14 + 至尊精神×1，胸 23/2，腿 18/2，嘉登熔炉。
- 通用套装：致命伤恢复 100 生命，45 秒冷却；冷却期间通用伤害 +10%。
- 近战：受击 >80 释放 8 枚 1500 基础伤害飞镖；命中每 60 帧生成八向挂载飞镖；
  80 及以下基础伤害压到 1；反伤提高。
- 远程：溢出 100% 的暴击率或 5% 固定概率触发再次暴击，最终为四倍暴击伤害；
  每 2.5 秒追加一枚破片弹，软上限 1500，炸出 4～6 枚次级破片。
- 法师：魔法命中每 2 帧生成追踪弑神光球，伤害 `(400 + 手持伤害/2)×5`；
  吸血目标生成 5～10 点治疗球；受击释放 1200 伤害、250×250 的弑神烈焰。
- 召唤：套装召唤伤害 +65%；召唤物/哨兵命中按伤害预算生成弑神幻影；
  召唤四节噬神机械蠕虫，基础伤害按召唤加成与仆从数量计算。
- 盗贼：满血时盗贼伤害/暴击/弹速各 +10%；受击 >80 额外 30 帧无敌。
- 八方向冲刺：速度 80，持续 25 帧，单次命中基础伤害 3000，附加 300 帧神谕狱火，
  45 秒冷却；支持开始与命中的多人同步。
- 新增 Buff：`GodSlayerCooldown`、`GodSlayerDashCooldown`、`Mechworm`。
- 新增弹幕：`GodSlayerDart`、`GodSlayerShrapnelRound`、`GodSlayerShrapnel`、
  `GodSlayerOrb`、`GodSlayerHealOrb`、`GodSlayerBlaze`、`GodSlayerPhantom`、
  `MechwormHead`、`MechwormBody`、`MechwormBody2`、`MechwormTail`。
- 当前验证：
  - 编译 0 警告、0 错误；332 个手写 C# 文件。
  - 中英文 Hjson 均为 516 条路径，缺失、多余与类型差异均为 0。
  - 隐式贴图 175/175 命中，显式贴图 24 个，抽象类 8 个按规则跳过。
## 25. 2026-11-22 古圣金源终阶套装
- 文件放在 `Content/Items/Armors/AuricTesla`，共 7 个物品类、15 张物品/装备/披风贴图：
  - `AuricTeslaBodyArmor`：48 防御、1 铂金 44 金、+100 生命、移速 +25%、
    通用伤害/暴击各 +22%，继承寒霜屏障、弑神闪避和动态低伤保护，并注册披风。
  - `AuricTeslaCuisses`：44 防御、1 铂金 8 金、移速 +50%、通用伤害/暴击各 +14%，提供飞毯。
  - `AuricTeslaHelm`：54 防御、1 铂金 80 金、近战伤害/暴击 +20%、攻速 +28%。
  - `AuricTeslaHoodedFacemask`：40 防御、远程伤害/暴击 +30%。
  - `AuricTeslaWireHemmedVisage`：24 防御、魔法伤害 +30%、魔法暴击 +20%、
    最大魔力 +100、蓝耗 ×0.8。
  - `AuricTeslaSpaceHelmet`：12 防御、仆从栏 +7、召唤伤害/鞭速/鞭长 +12%。
  - `AuricTeslaPlumedHelm`：34 防御、盗贼伤害/暴击 +20%、移速 +25%。
- 三版本终阶配方均真实消耗四套对应下位件：
  - 头部：龙蒿、血炎、始源林海、弑神者对应职业头 + 妄想护符。
  - 胸甲：四套胸甲 + 霜冻屏障。
  - 腿甲：四套护腿 + 飞毯。
  - 现代消耗金源锭 头/胸/腿 10/20/15，宇宙砧。
  - 经典 1.4.2.101 使用金源矿及梦魇燃料、吸热能量、幻影质、暗日碎片、生命锭、
    地狱施法者碎片、灾厄核心、银河奇点，嘉登熔炉。
  - CalRD 使用金源锭 10/18/14，嘉登熔炉。
- 五颗头共用静止增益：静止时逐步获得最多 20% 通用伤害与 10 通用暴击，并放大武器击退。
- 套装共通：棘刺 +3、岩浆免疫延长 240 帧、液体自由移动、血腥再生、岩浆中 +30 防御与再生。
- 近战：复合四套近战效果；仇恨 +1200；按当前生命比例提供最多 20% 近战伤害。
- 远程：复合四套远程效果；血炎血弹伤害倍率提高到 ×2.2。
- 法师：复合四套法师效果；血炎幽灵魔弹倍率提高到 ×4.2；龙蒿法弹回血效率减半。
- 召唤：复合四套召唤效果；额外召唤伤害 +120%、仆从栏 +1；
  四套召唤物均启用，并将古圣叶棱晶基础伤害提高到 3000。
- 盗贼：复合四套盗贼效果；最大潜行 160；生命 >50% 时盗贼暴击额外造成 25% 后续伤害。
- 终阶套装额外提供跑速与加速度 +10%，以及金源飞毯、移动拖影和角色染色。
- 当前验证：
  - 编译 0 警告、0 错误；341 个手写 C# 文件，空行数 0。
  - 中英文 Hjson 均为 535 条路径，缺失、多余与类型差异均为 0。
  - 新增 7 个物品贴图全部命中；全局隐式贴图与显式贴图检查无缺失，抽象类 9 个按规则跳过。
  - 15 张古圣金源防具/披风贴图与 Demutation 原件 SHA-256 全部一致。
## 26. 2026-11-22 全量稀有度、BUG 与多人审计
- 月后五阶防具统一映射已确认：
  - 龙蒿源 12 档 → `TurquoiseRarity`。
  - 血炎源 13 档 → `PureGreenRarity`。
  - 弑神者 → 现代 `CosmicPurpleRarity`。
  - 始源林海 → 现代 `CosmicPurpleRarity`。
  - 古圣金源源 20 档 → `BurnishedAuricRarity`。
- `RarityUtil` 补齐 14 档深蓝的标准购买价 2 铂金；所有新防具仍按源物品显式价格覆盖，
  不因档位价格改变 75/60/45 金或 1 铂金 44/8 金等具体售价。
- 常规物品与 Demutation 稀有度差异逐项复核后，保留 AGENTS 已记录的明确用户定制：
  代达罗斯纹章原版紫色、元素之箭袋/元素之握等 CosmicPurple、无暇粹魂晶 CosmicPurple、
  灼辉金源之翼，以及血神核心系列指定档位。
- 英文旧名与现代后继只用于定位候选；必须同时核对获取时期、配方和物品强度。
  例如 `Purity → Radiance`、`ElysianTracers → Void Striders` 已有不同阶段或明确用户定制，
  不得仅凭 `[LegacyName]` 直接套用现代 rarity。
- 多人逻辑修正：
  - 仙人掌治疗珠、真菌治疗球改由服务端验证治疗，再广播 `PlayerLifeMana`。
  - 阴阳吸星石/涡流虹吸石的受击前治疗只由单机或服务端写入生命。
  - 龙蒿、血炎、始源林海、弑神者的命中生成逻辑统一限定弹幕主人客户端执行，
    避免远端客户端重复生成树叶、血弹、光球、飞镖和幻影。
- 热重载修正：
  - `CRUtil.UnloadRogueBindings` 清空三版盗贼反射类型、字段、方法与 Mod 引用。
  - 龙蒿与始源林海的外部 Buff ID 伪减益缓存会在卸载时清空。
  - 星辉环境反射绑定和其他灾厄兼容绑定均在卸载时清空。
- 保留的多人设计：治疗叶球/弑神治疗球只允许服务器结算；玩家造成的真实伤害、
  debuff 和冲刺命中仍由服务端权威结果同步到其他客户端。
- 本轮版本号保持 `0.1.2.6`。
## 27. 2026-11-22 弹幕目录整理
- `Content/Projectiles` 根目录只保留：
  - `CalamityRebirthGlobalProjectile.cs`：全局弹幕钩子，不属于具体弹幕。
  - `InvisibleProj.png`：多弹幕共用隐形贴图。
  - `TornadoProj.png`：`SandTornado` 显式引用的共用贴图，路径必须保持
    `CalamityRebirth/Content/Projectiles/TornadoProj`。
- 根目录最后六个实际弹幕已按用途迁移：
  - `Typeless`：`BloodBomb`、`BloodBombExplosion`、`BloodflareSoul`、`TarraEnergy`。
  - `Magic`：`GhostlyBolt`。
  - `Summon`：`GhostlyMine`。
- 对应贴图随类型迁移：
  - `BloodBomb.png`、`BloodBombExplosion.png`、`BloodflareSoul.png` 进入 `Typeless`。
  - `GhostlyMine.png` 进入 `Summon`。
- 所有命名空间、`using` 与本地化分类已同步，避免移动后类型解析或资源路径出错。
## 28. 2026-11-22 玩家结算类合并
- 原 12 个 `CalamityRebirthPlayer.*.cs` 分片已全部机械合并进：
  `Players/CalamityRebirthPlayer.cs`
- 合并前逐段校验 12 个分片的类正文均完整进入候选文件，编译后确认无成员缺失。
- 统一后的文件包含全部玩家状态、装备结算、套装逻辑、冲刺、网络消息与受击保护。
- `partial` 修饰符已移除，`Players` 目录现在仅保留一个 `CalamityRebirthPlayer*.cs` 文件。
- 当前版本号保持 `0.1.2.6`。
## 29. 2026-11-22 最终全量 BUG 与错误审计
- 最终构建：0 警告、0 错误；源码 C# 文件 329 个，空行 0。
- 修复防具本地化节点错误：14 件龙蒿、血炎、始源林海、弑神者与古圣金源头部的
  `DisplayName`、`Tooltip` 和 `SetBonus` 已从中英 Hjson 的 `Buffs` 节点迁回 `Items` 节点。
- 修复 `FungalClumpMinion` 的远端客户端重复生成治疗珠问题；治疗珠生成现在只由弹幕所属客户端触发，
  实际治疗仍由服务器验证并广播。
- 恢复 Drew 之翼飞行粒子对云瓶首跳的排除判断，避免普通云瓶跳跃错误触发翅膀尘埃。
- 清理 C# 注释中残余的参考工程、外部绘制类和来源归属措辞；兼容版本行为说明保留在功能性描述中。
- 中英文 Hjson 均为 759 条完整路径，重复键 0、路径差异 0；`Items` 的 170 个物品条目均包含 `DisplayName`。
- 199 个非抽象直接 `ModItem`、`ModProjectile`、`ModBuff` 类所隐式命中的贴图全部存在；
  15 条显式 `CalamityRebirth/...` 贴图路径全部存在；装备贴图后缀检查全部命中。
- 三版灾厄弱依赖的物品、制作站、Buff 名称已逐项核对；现代版使用 `LegacyName` 的少数旧名与
  经典版专有名称均保留对应分支检查。三版盗贼反射类型、字段和读取方法均存在。
- 所有兼容缓存均在 `Unload()` 清理；治疗、掉落、套装命中生成和远程冲刺消息未发现新的重复写入路径。
- 仍需进游戏回归：
  - 单机与双客户端下的治疗请求、血炎/始源林海/弑神者治疗珠和 Purity 治疗节拍。
  - 远端玩家的弑神者、盾牌冲刺表现，以及机械蠕虫四节链在重连、死亡和切套装后的同步。
  - 古圣叶棱晶在多名召唤玩家同时装备时的生成、伤害基线与切换头盔后的 3000 基础伤害更新。
  - 三版本最终古圣金源九件配方、稀有度颜色和价格在配方浏览器与多人客户端中的显示。
- 本轮未修改版本号，仍为 `0.1.2.6`。
## 30. 2026-11-25 魔影盔甲移植
- `Content/Items/Armors/Demonshade` 已加入通用魔影头盔、胸甲与护腿，三件防御均为 50，
  价格依次为 5、4、3 铂金，名称使用对应现代 `Hot Pink` 自定义色的橙红动态
  `DemonshadeRarity`。
- 魔影头盔提供通用伤害 +50%、通用暴击 +25%、近战攻速 +25%、仆从栏 +10、哨兵栏 +5；
  不提供通用攻击速度，也不读取或染色盗贼潜行。
- 魔影胸甲提供通用伤害 +40%、通用暴击 +20%、生命与魔力上限各 +300、反伤 100、
  远程省弹 50%、魔力消耗 ×0.5、鞭子攻速与范围各 +30%，以及静止暗影再生。
- 魔影护腿提供通用伤害 +30%、通用暴击 +15%，并把最终跑动速度与加速度各提高 50%。
- 三件配方分别消耗对应灾厄分支的原生 `ShadowspecBar` 40、50、45，在 `DraedonsForge`
  制作；现代、1.4.2.101 与 `CalRD` 均使用弱依赖分支。
- 套装额外提供通用伤害 +100%、常驻红魔、受击暗影束/恶魔镰刀、按 Y 的 3000 像素狂怒技能，
  以及最高优先级的魔影盾牌冲刺。狂怒持续 600 帧，提高玩家攻击输出并强化被标记敌怪。
- 红魔使用 `ProjectileID.UnholyTridentFriendly`（114）发射邪秽三叉戟，伤害类型覆盖为通用，
  并对同一 NPC 使用 10 帧本地命中冷却。
- `DemonshadeBurn / 魔影灼燃` 不再使用多档随机时长。所有魔影套直接攻击和友方弹幕命中统一施加
  300 帧，NPC 与 PvP 玩家均为 2500 HP/s。
- 魔影灼燃的视觉效果按现代 `VulnerabilityHex` 结构实现：NPC 底部持续紫色火簇、向上漂移的
  黑芯紫色火花与少量紫焰尘；玩家和 NPC 共用同一紫色色带，不再使用原橙红火焰。
- 魔影套可使用弑神者冲刺，颜色改为黑红，拥有独立 `DemonshadeDashCooldown` 15 秒冷却；
  冷却图标直接使用根目录 `1.png` 的副本。
- 魔影盾牌冲刺属于短促强化宇宙冲撞档：起手 31、过程上限 16.5、命中与爆炸均为 1400；
  高速阶段速度衰减 0.94、跑速以上阶段 0.74，使位移集中在约 20 帧内完成。
- 魔影盾冲的爆发环不再使用默认青色，外层改为黑红主火色、内层改为橙红亮核；
  拖尾、命中爆炸与宇宙爆炸尘同步使用魔影配色。
- 新增 `HatExtensionLayer` 与 `FrontArmOverShoulderpadLayer`，用于完整绘制魔影头盔兜帽/角
  和胸甲前臂；两者仅在客户端绘制，不参与服务端属性结算。
- 前臂绘制必须使用本工程独占接口 `IDrawFrontArmOverShoulderpad`，不得改回
  `IDrawArmOverShoulderpad`；后者与灾厄同名全局接口冲突，会使 tModLoader 编译出的胸甲
  硬绑定 `CalamityMod`，导致未启用现代灾厄时整个模组无法加载。
- 头部附加层必须使用本工程独占接口 `IHatExtensionDrawer`，不得改回 `IExtendedHat`；
  后者同样与灾厄全局接口冲突，并会把魔影头盔硬绑定到 `CalamityMod`。
- 现代版稀有度复核后修正：气之石浅红、花绽石与混沌石青柠、寒霜屏障浅红、
  斯塔提斯的祝福黄色、极乐之庇护绿松石。
- 2026-11-26 封装验证：源码 C# 文件 345 个、空行 0；`dotnet build` 为 0 警告、0 错误；
  中英文 Hjson 均为 555 条路径，缺失、多余与类型差异均为 0；`git diff --check` 未发现空白错误。
- 仍需进游戏回归：红魔在多人重连与死亡后的续存、狂怒网络请求、黑红冲刺远端表现、
  自定义兜帽/前臂层与两套冲刺的切换优先级。
- 本次封装提交不修改版本号，仍为 `0.1.2.6`。
