using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Content.Items.Potions.PermanentPowerUps;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.Combat.Magic;
using CalamityRebirth.Content.Items.Accessories.Movement;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
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
