using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Content.Items.Potions.PermanentPowerUps;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Items.Accessories.Immunity;
using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.Combat.Magic;
using CalamityRebirth.Content.Items.Accessories.Combat.Ranged;
using CalamityRebirth.Content.Items.Accessories.Movement;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.NPCs
{
    /// <summary>
    /// 本模组 NPC 全局状态容器。
    /// 每个 NPC 实例独立保存本工程的战斗减益标记，并在每个 AI 帧开始时清除，由对应 Buff 重新写入。
    /// </summary>
    internal class CalamityRebirthGlobalNPC : GlobalNPC
    {
        /// <summary>当前 NPC 是否处于燃烧III状态。</summary>
        public bool burningIII;
        /// <summary>当前 NPC 是否处于破晓II状态。</summary>
        public bool daybreakII;
        /// <summary>当前 NPC 是否处于触电II状态。</summary>
        public bool electrifiedII;
        /// <summary>当前 NPC 是否处于元素混合II状态。</summary>
        public bool elementalMixII;
        /// <summary>当前 NPC 是否处于始源林海迟滞状态。</summary>
        public bool silvaHysteresis;
        /// <summary>
        /// 启用按实体实例保存状态。若为 false，同一 ID 的所有 NPC 会共享标记并互相污染。
        /// </summary>
        public override bool InstancePerEntity => true;
        /// <summary>
        /// 每个更新周期先清空状态，再由 Buff.Update 根据当前 buff 列表重新标记。
        /// </summary>
        public override void ResetEffects(NPC npc)
        {
            burningIII = false;
            daybreakII = false;
            electrifiedII = false;
            elementalMixII = false;
            silvaHysteresis = false;
        }
        /// <summary>
        /// 血腥玛丽在血月期间把玩家刷怪率提高十倍。
        /// spawnRate 越小生成越快，因此先除十并限制最低为 1；最大同时刷怪量直接乘十。
        /// </summary>
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (!Main.bloodMoon || !player.GetModPlayer<CalamityRebirthPlayer>().bloodyMary)
                return;
            spawnRate /= 10;
            if (spawnRate < 1)
                spawnRate = 1;
            maxSpawns *= 10;
        }
        /// <summary>
        /// 汇总持续伤害。先压制正再生，再叠加两种 DoT，并为游戏内置伤害数字设置最小显示值。
        /// 两个减益同时存在时伤害会叠加，符合原版独立 debuff 的结算习惯。
        /// </summary>
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (burningIII)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= BurningIII.BurnLifeRegen;
                if (damage < BurningIII.BurnLifeRegen / 2)
                    damage = BurningIII.BurnLifeRegen / 2;
            }
            if (daybreakII)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= DaybreakII.BurnLifeRegen;
                if (damage < DaybreakII.BurnLifeRegen / 2)
                    damage = DaybreakII.BurnLifeRegen / 2;
            }
            if (electrifiedII)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= ElectrifiedII.NpcLifeRegenLoss;
                if (damage < ElectrifiedII.NpcLifeRegenLoss / 2)
                    damage = ElectrifiedII.NpcLifeRegenLoss / 2;
            }
            if (elementalMixII)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= ElementalMixII.NpcLifeRegenLoss;
                if (damage < ElementalMixII.NpcLifeRegenLoss / 2)
                    damage = ElementalMixII.NpcLifeRegenLoss / 2;
            }
            if (silvaHysteresis)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= Content.ModBuffs.Debuffs.SilvaHysteresis.NpcLifeRegenLoss;
                if (damage < Content.ModBuffs.Debuffs.SilvaHysteresis.MinimumDisplayedDamage)
                    damage = Content.ModBuffs.Debuffs.SilvaHysteresis.MinimumDisplayedDamage;
            }
        }
        /// <summary>
        /// 在受击最终结算阶段降低防御并提高承伤倍率。
        /// 两种减益都采用乘算承伤，因此同时存在时会保留彼此独立的增伤效果。
        /// </summary>
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (burningIII)
            {
                modifiers.Defense.Flat -= BurningIII.DefenseLoss;
                modifiers.FinalDamage *= BurningIII.DamageTakenMultiplier;
            }
            if (daybreakII)
            {
                modifiers.Defense.Flat -= DaybreakII.DefenseLoss;
                modifiers.FinalDamage *= DaybreakII.DamageTakenMultiplier;
            }
            if (electrifiedII)
            {
                modifiers.Defense.Flat -= ElectrifiedII.NpcDefenseLoss;
                modifiers.FinalDamage *= ElectrifiedII.NpcDamageTakenMultiplier;
            }
            if (elementalMixII)
            {
                modifiers.Defense.Flat -= ElementalMixII.DefenseLoss;
                modifiers.FinalDamage *= ElementalMixII.DamageTakenMultiplier;
            }
        }
        /// <summary>
        /// 龙蒿套装持有者附近的合法敌怪死亡时，有 1/5 概率额外掉落红心。
        /// 掉落只在服务端或单机生成，避免多人客户端重复创建物品。
        /// </summary>
        public override void OnKill(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || npc.SpawnedFromStatue)
                return;
            TrySpawnTarragonHeart(npc);
            TrySpawnBloodflareBloodOrb(npc);
        }
        /// <summary>血炎套装的最近玩家附近敌怪死亡时生成额外红心。</summary>
        private static void TrySpawnTarragonHeart(NPC npc)
        {
            int closest = (int)Player.FindClosest(npc.position, npc.width, npc.height);
            if (!Main.player.IndexInRange(closest)
                || !Main.player[closest].active
                || !Main.player[closest].GetModPlayer<CalamityRebirthPlayer>().tarraSet
                || (npc.damage <= 5 && !npc.boss) || npc.lifeMax <= 100
                || !Main.rand.NextBool(5))
                return;
            Item.NewItem(npc.GetSource_FromThis(), npc.position, npc.Size, ItemID.Heart);
        }
        /// <summary>
        /// 血炎套装持有者附近的地表敌怪在血月死亡时，有 1/2 概率额外掉落当前灾厄分支的血珠。
        /// </summary>
        private static void TrySpawnBloodflareBloodOrb(NPC npc)
        {
            if (!Main.bloodMoon || !npc.HasPlayerTarget || npc.position.Y / 16f >= Main.worldSurface
                || (npc.damage <= 5 && !npc.boss))
                return;
            int closest = (int)Player.FindClosest(npc.position, npc.width, npc.height);
            if (!Main.player.IndexInRange(closest)
                || !Main.player[closest].active
                || !Main.player[closest].GetModPlayer<CalamityRebirthPlayer>().bloodflareSet
                || !Main.rand.NextBool(2))
                return;
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (ModLoader.TryGetMod(modName, out Mod mod)
                    && mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    Item.NewItem(npc.GetSource_FromThis(), npc.position, npc.Size, bloodOrb.Type);
                    return;
                }
            }
        }
        /// <summary>玩家物品命中时按目标生命阈值和 3 秒冷却生成红心或魔力星。</summary>
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit
            , int damageDone)
        {
            TrySpawnBloodflareResource(player, npc);
        }
        /// <summary>玩家弹幕命中时按目标生命阈值和 3 秒冷却生成红心或魔力星。</summary>
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit
            , int damageDone)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
                return;
            TrySpawnBloodflareResource(Main.player[projectile.owner], npc);
        }
        /// <summary>以攻击者的血炎套装计时器限制红心/魔力星生成。</summary>
        private static void TrySpawnBloodflareResource(Player player, NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient || npc.SpawnedFromStatue
                || npc.damage <= 0)
                return;
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (!modPlayer.bloodflareSet)
                return;
            float lifeRatio = npc.life / (float)npc.lifeMax;
            if (lifeRatio < 0.5f && modPlayer.bloodflareHeartTimer <= 0)
            {
                modPlayer.bloodflareHeartTimer = 180;
                Item.NewItem(npc.GetSource_FromThis(), npc.position, npc.Size, ItemID.Heart);
            }
            else if (lifeRatio > 0.5f && modPlayer.bloodflareManaTimer <= 0)
            {
                modPlayer.bloodflareManaTimer = 180;
                Item.NewItem(npc.GetSource_FromThis(), npc.position, npc.Size, ItemID.Star);
            }
        }
        /// <summary>
        /// NPC 死亡时触发一次破晓II传播；是否允许联网传播由 DaybreakII 内部按服务端规则判断。
        /// </summary>
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (daybreakII && npc.life <= 0)
                DaybreakII.DeathEffect(npc);
        }
        /// <summary>
        /// 把减益粒子挂到 NPC 的正常绘制流程中，确保粒子与实体位置、屏幕变换保持一致。
        /// </summary>
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (burningIII)
                BurningIII.SpawnBurnVisuals(npc);
            if (daybreakII)
            {
                drawColor = Color.Lerp(drawColor, DaybreakII.GlowColor, 0.18f);
                DaybreakII.DrawEffects(npc);
            }
            if (electrifiedII)
                ElectrifiedII.SpawnElectricVisuals(npc);
            if (elementalMixII)
                ElementalMixII.DrawEffects(npc);
        }
        /// <summary>
        /// 月亮领主核心额外掉落一个天界洋葱。
        /// 与其他月亮领主战利品共用原版单次掉落流程。
        /// </summary>
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
			if (ModLoader.HasMod("CalamityMod")
				|| ModLoader.HasMod("CalamityModClassicPreTrailer")
				|| ModLoader.HasMod("CalRD"))
				AddElementalChainVanillaDrops(npc, npcLoot);
            if (npc.type == NPCID.KingSlime)
            {
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CrownJewel>(), 10, 10));
            }
            if (npc.type == NPCID.GoblinSummoner)
            {
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<TheFirstShadowflame>(), 7, 5));
            }
            if (npc.type == NPCID.PossessedArmor)
            {
                AddPsychoticAmuletDrops(npcLoot);
            }
            if (npc.type == NPCID.MoonLordCore)
            {
                LeadingConditionRule expertOnly = new(new Conditions.IsExpert());
                expertOnly.OnSuccess(new PerPlayerDropRule(ModContent.ItemType<CelestialOnion>()));
                npcLoot.Add(expertOnly);
            }
            AddBossLootForMod(npc, npcLoot, "CalamityMod");
            AddBossLootForMod(npc, npcLoot, "CalamityModClassicPreTrailer");
            AddBossLootForMod(npc, npcLoot, "CalRD");
            AddSpongeChainDrops(npc, npcLoot);
            AddFrostBarrierDrops(npc, npcLoot);
            AddLeadCoreDrops(npc, npcLoot);
        }
        /// <summary>
        /// 用本工程疯狂护符替换两款经典灾厄原版的同名规则。
        /// 三版本统一采用普通 1/200、专家/大师 1/150。
        /// </summary>
        private static void AddPsychoticAmuletDrops(NPCLoot npcLoot)
        {
            RemoveNativeItemDrop(npcLoot, "CalamityModClassicPreTrailer", "PsychoticAmulet");
            RemoveNativeItemDrop(npcLoot, "CalRD", "PsychoticAmulet");
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<PsychoticAmulet>(), 200, 150));
        }
        /// <summary>
        /// 为三个灾厄分支的冰 clasp 敌怪添加本工程寒霜屏障掉落，统一为 1/10。
        /// 各分支内部类名一致但运行时类型不同，因此必须逐版本弱依赖匹配。
        /// </summary>
        private static void AddFrostBarrierDrops(NPC npc, NPCLoot npcLoot)
        {
            AddFrostBarrierDropForMod(npc, npcLoot, "CalamityMod");
            AddFrostBarrierDropForMod(npc, npcLoot, "CalamityModClassicPreTrailer");
            AddFrostBarrierDropForMod(npc, npcLoot, "CalRD");
        }
        /// <summary>从指定灾厄版本查找 IceClasper 并追加寒霜屏障掉落。</summary>
        private static void AddFrostBarrierDropForMod(NPC npc, NPCLoot npcLoot, string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModNPC>("IceClasper", out ModNPC iceClasper)
                || npc.type != iceClasper.Type)
                return;
            RemoveNativeItemDrop(npcLoot, modName, "FrostBarrier");
            npcLoot.Add(new CommonDrop(ModContent.ItemType<FrostBarrier>(), 10));
        }
        /// <summary>为三版本辐射史莱姆替换并由本工程版本提供 1/10 的铅芯掉落。</summary>
        private static void AddLeadCoreDrops(NPC npc, NPCLoot npcLoot)
        {
            AddLeadCoreDropForMod(npc, npcLoot, "CalamityMod");
            AddLeadCoreDropForMod(npc, npcLoot, "CalamityModClassicPreTrailer");
            AddLeadCoreDropForMod(npc, npcLoot, "CalRD");
        }
        /// <summary>从指定灾厄版本查找辐射史莱姆并登记本工程铅芯。</summary>
        private static void AddLeadCoreDropForMod(NPC npc, NPCLoot npcLoot, string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModNPC>("IrradiatedSlime", out ModNPC irradiatedSlime)
                || npc.type != irradiatedSlime.Type)
                return;
            RemoveNativeItemDrop(npcLoot, modName, "LeadCore");
            npcLoot.Add(new CommonDrop(ModContent.ItemType<LeadCore>(), 10));
        }
        /// <summary>
        /// 移除指定灾厄分支的原生同名掉落，避免本工程物品与原生物品同时掉落。
        /// </summary>
        private static void RemoveNativeItemDrop(NPCLoot npcLoot, string modName, string itemName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>(itemName, out ModItem nativeItem))
                return;
            foreach (IItemDropRule rule in npcLoot.Get().ToArray())
            {
                if (ContainsItemDrop(rule, nativeItem.Type))
                    npcLoot.Remove(rule);
            }
        }
        /// <summary>递归检查掉落规则链是否包含指定物品。</summary>
        private static bool ContainsItemDrop(IItemDropRule rule, int itemType)
        {
            if (rule is CommonDrop commonDrop && commonDrop.itemId == itemType)
                return true;
            foreach (IItemDropRuleChainAttempt chain in rule.ChainedRules)
            {
                if (ContainsItemDrop(chain.RuleToChain, itemType))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 将旅行商人原生的寒霜屏障商品替换为本工程版本，并在没有原生商品时补入本工程版本。
        /// </summary>
        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            ReplaceNativeShopItem(items, "CalamityMod", "FrostBarrier");
            ReplaceNativeShopItem(items, "CalamityModClassicPreTrailer", "FrostBarrier");
            ReplaceNativeShopItem(items, "CalRD", "FrostBarrier");
            if (npc.type != NPCID.TravellingMerchant || Main.moonPhase != 0)
                return;
            int localType = ModContent.ItemType<FrostBarrier>();
            foreach (Item item in items)
            {
                if (item is not null && item.type == localType)
                    return;
            }
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is not null && items[i].type != ItemID.None)
                    continue;
                items[i] = new Item();
                items[i].SetDefaults(localType);
                return;
            }
        }
        /// <summary>在活动商店数组中替换指定原生物品；原物品不存在时安全跳过。</summary>
        private static void ReplaceNativeShopItem(Item[] items, string modName, string itemName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>(itemName, out ModItem nativeItem))
                return;
            int replacementType = ModContent.ItemType<FrostBarrier>();
            foreach (Item item in items)
            {
                if (item is not null && item.type == nativeItem.Type)
                    item.SetDefaults(replacementType);
            }
        }
		/// <summary>
		/// 沙元素在普通和专家模式分别掉落瓶装沙元素，专家模式另有二十分之一概率掉落稀有版本。
		/// </summary>
		private static void AddElementalChainVanillaDrops(NPC npc, NPCLoot npcLoot)
		{
			if (npc.type != NPCID.SandElemental)
				return;
			npcLoot.Add(ItemDropRule.NormalvsExpert(
				ModContent.ItemType<WifeinaBottle>(), 7, 5));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert()
				, ModContent.ItemType<WifeinaBottlewithBoobs>(), 20));
		}
        /// <summary>为涡流虹吸石链的凝胶、甲壳、海贝与火花前置添加普通/专家掉落。</summary>
        private static void AddSpongeChainDrops(NPC npc, NPCLoot npcLoot)
        {
            if (!ModLoader.HasMod("CalamityMod")
                && !ModLoader.HasMod("CalamityModClassicPreTrailer")
                && !ModLoader.HasMod("CalRD"))
                return;
            switch (npc.type)
            {
                case NPCID.BlueJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<ManaJelly>(), 7, 5));
                    break;
                case NPCID.PinkJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<LifeJelly>(), 7, 5));
                    break;
                case NPCID.GreenJellyfish:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<VitalJelly>(), 7, 5));
                    break;
                case NPCID.SeaSnail:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<SeaShell>(), 3, 2));
                    break;
                case NPCID.Crawdad:
                case NPCID.Crawdad2:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<CrawCarapace>(), 7, 5));
                    break;
                case NPCID.AnomuraFungus:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<FungalCarapace>(), 7, 5));
                    break;
                case NPCID.GiantTortoise:
                {
                    LeadingConditionRule expertTortoise = new(new Conditions.IsExpert());
                    expertTortoise.OnSuccess(new OneFromOptionsDropRule(200, 2,
                        [ModContent.ItemType<GiantTortoiseShell>()]));
                    LeadingConditionRule normalTortoise = new(new Conditions.NotExpert());
                    normalTortoise.OnSuccess(new OneFromOptionsDropRule(203, 2,
                        [ModContent.ItemType<GiantTortoiseShell>()]));
                    npcLoot.Add(expertTortoise);
                    npcLoot.Add(normalTortoise);
                    break;
                }
                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
                    npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<GiantShell>(), 7, 5));
                    break;
            }
            AddCnidrionDrop(npc, npcLoot, "CalamityMod");
            AddCnidrionDrop(npc, npcLoot, "CalamityModClassicPreTrailer");
            AddCnidrionDrop(npc, npcLoot, "CalRD");
        }
        /// <summary>从指定分支查找水母类敌怪并添加火花掉落。</summary>
        private static void AddCnidrionDrop(NPC npc, NPCLoot npcLoot, string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind("Cnidrion", out ModNPC cnidrion)
                || npc.type != cnidrion.Type)
                return;
            npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<AmidiasSpark>(), 6, 4));
        }
        /// <summary>向指定灾厄分支的硫磺火元素与亵渎天神追加本地链物品。</summary>
        private static void AddBossLootForMod(NPC npc, NPCLoot npcLoot, string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod))
                return;
			AddElementalChainBossDrops(npc, npcLoot, mod);
            if (mod.TryFind("BrimstoneElemental", out ModNPC brimstoneElemental)
                && npc.type == brimstoneElemental.Type)
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<Abaddon>(), 12, 7));
            if (mod.TryFind("Providence", out ModNPC providence)
                && npc.type == providence.Type)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ElysianAegis>()));
        }
		/// <summary>
		/// 为指定灾厄分支的塞壬、云元素和五类元素来源 Boss 添加普通模式掉落。
		/// Lure 与风暴之眼使用普通/专家七分之一和五分之一，元素石只在非专家模式以十分之一掉落。
		/// </summary>
		private static void AddElementalChainBossDrops(NPC npc, NPCLoot npcLoot, Mod mod)
		{
			string lureName = mod.Name == "CalamityMod" ? "Anahita" : "Siren";
			string eyeName = mod.Name == "CalamityMod" ? "CloudElemental" : "ThiccWaifu";
			string chaosName = mod.Name == "CalamityMod" ? "CalamitasClone"
				: mod.Name == "CalRD" ? "CalamitasRun3" : "Calamitas";
			if (NpcMatchesAny(mod, npc, lureName))
			{
				npcLoot.Add(ItemDropRule.NormalvsExpert(
					ModContent.ItemType<LureofEnthrallment>(), 7, 5));
				return;
			}
			if (NpcMatchesAny(mod, npc, eyeName))
			{
				npcLoot.Add(ItemDropRule.NormalvsExpert(
					ModContent.ItemType<EyeoftheStorm>(), 7, 5));
				return;
			}
			if (NpcMatchesAny(mod, npc, "BrimstoneElemental"))
			{
				AddElementStoneDrop(npcLoot, ModContent.ItemType<RoseStone>());
				return;
			}
			if (NpcMatchesAny(mod, npc, "DesertScourgeHead", "AquaticScourgeHead"))
			{
				AddElementStoneDrop(npcLoot, ModContent.ItemType<AeroStone>());
				return;
			}
			if (NpcMatchesAny(mod, npc, "Cryogen"))
				AddElementStoneDrop(npcLoot, ModContent.ItemType<CryoStone>());
			else if (NpcMatchesAny(mod, npc, chaosName))
				AddElementStoneDrop(npcLoot, ModContent.ItemType<ChaosStone>());
			else if (NpcMatchesAny(mod, npc, "PlaguebringerGoliath"))
				AddElementStoneDrop(npcLoot, ModContent.ItemType<BloomStone>());
		}
		/// <summary>判断当前 Boss 是否匹配指定模组中的任一候选内部类名。</summary>
		private static bool NpcMatchesAny(Mod mod, NPC npc, params string[] names)
		{
			foreach (string name in names)
			{
				if (mod.TryFind(name, out ModNPC modNpc) && npc.type == modNpc.Type)
					return true;
			}
			return false;
		}
		/// <summary>以非专家十分之一概率加入一枚元素石。</summary>
		private static void AddElementStoneDrop(NPCLoot npcLoot, int itemType)
		{
			LeadingConditionRule notExpert = new(new Conditions.NotExpert());
			notExpert.OnSuccess(new CommonDrop(itemType, 10));
			npcLoot.Add(notExpert);
		}
    }
}
