using CalamityRebirth.Players;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Content.Projectiles.Magic;
using CalamityRebirth.Content.Projectiles.Typeless;
using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Content.Items.Accessories.Combat.Magic;
using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Items.Accessories.Movement;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items
{
    /// <summary>
    /// 物品侧全局钩子，集中处理 PvP 近战命中与自定义稀有度名称绘制。
    /// </summary>
    internal class CalamityRebirthGlobalItem : GlobalItem
    {
        /// <summary>
        /// 血炎晶核进入掠夺者宝藏袋所需的世界进度条件。
        /// 条件在开袋时求值，因此不依赖模组加载时的世界状态。
        /// </summary>
        private static readonly IItemDropRuleCondition ProvidenceDownedCondition = new Condition(
            "Mods.CalamityRebirth.Conditions.ProvidenceDowned",
            CalamityCompatUtil.IsProvidenceDowned)
            .ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied);
        /// <summary>
        /// 在原版物品默认值建立后撤销当前灾厄分支直接写入的装备数值削减。
        /// 弱依赖使灾厄先加载，本回调随后把受影响武器恢复为原版数值。
        /// </summary>
        public override void SetDefaults(Item item)
        {
            VanillaNerfRevertSystem.RestoreItemDefaults(item);
        }
        /// <summary>
        /// 还原当前灾厄分支对原版护甲单件造成的职业属性扣除。
        /// </summary>
        public override void UpdateEquip(Item item, Player player)
        {
            VanillaNerfRevertSystem.RestoreArmorPieceStats(item, player);
        }
        /// <summary>
        /// 还原现代灾厄对原版套装效果施加的攻速、伤害、法力消耗和减伤削减。
        /// </summary>
        public override void UpdateArmorSet(Player player, string set)
        {
            VanillaNerfRevertSystem.RestoreArmorSetStats(player, set);
        }
        /// <summary>
        /// 还原当前灾厄分支对原版饰品属性与词缀施加的削减。
        /// </summary>
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            VanillaNerfRevertSystem.RestoreAccessoryStats(item, player);
        }
        /// <summary>
        /// 龙蒿法师套装在累计 5 次魔法暴击后，让下一次魔法射击额外发射 9～11 枚树叶。
        /// 计数只由本机拥有者消耗，避免多人环境重复生成。
        /// </summary>
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source
            , Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (modPlayer.tarraMage && modPlayer.tarraCrits >= 5
                && item.CountsAsClass(DamageClass.Magic)
                && player.whoAmI == Main.myPlayer)
            {
                modPlayer.tarraCrits = 0;
                int leafCount = 9 + Main.rand.Next(3);
                for (int i = 0; i < leafCount; i++)
                {
                    float spreadMultiplier = 0.025f * i;
                    Vector2 leafVelocity = velocity + new Vector2(
                        Main.rand.Next(-25, 26) * spreadMultiplier,
                        Main.rand.Next(-25, 26) * spreadMultiplier);
                    float speed = leafVelocity.Length();
                    if (speed > 0f)
                        leafVelocity *= item.shootSpeed / speed;
                    Projectile.NewProjectile(source, position, leafVelocity
                        , ProjectileID.Leaf, (int)(damage * 0.20f), knockback, player.whoAmI);
                }
            }
            modPlayer.TryShootGodSlayerRound(item, position, velocity, damage);
            if (modPlayer.bloodflareRanged && item.CountsAsClass(DamageClass.Ranged)
                && player.whoAmI == Main.myPlayer && Main.rand.Next(100) < 2)
                Projectile.NewProjectile(source, position, velocity
                    , ModContent.ProjectileType<BloodBomb>()
                    , (int)(damage * (modPlayer.auricSet ? 2.2f : 1.6f))
                    , 2f, player.whoAmI);
            if (modPlayer.bloodflareMage && item.CountsAsClass(DamageClass.Magic)
                && player.whoAmI == Main.myPlayer && Main.rand.Next(100) < 5)
                Projectile.NewProjectile(source, position, velocity
                    , ModContent.ProjectileType<GhostlyBolt>()
                    , (int)(damage * (modPlayer.auricSet ? 4.2f : 2.6f))
                    , 1f, player.whoAmI);
            return true;
        }
        /// <summary>
        /// 还原现代灾厄对蝴蝶翅膀与幽灵翅膀上升倍率的削减。
        /// </summary>
        public override void VerticalWingSpeeds(Item item, Player player
            , ref float ascentWhenFalling, ref float ascentWhenRising
            , ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            VanillaNerfRevertSystem.RestoreVerticalWingSpeeds(item, ref maxAscentMultiplier);
        }
        /// <summary>
        /// 向三个灾厄分支的血肉宿主与掠夺者宝藏袋追加血神核心系列掉落。
        /// 2.2.2 把血腥蠕虫牙设为血肉宿主的专家门槛物品，因此只在它的宝藏袋中固定掉落。
        /// </summary>
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            AddBloodGodBagDrops("CalamityMod", item, itemLoot);
            AddBloodGodBagDrops("CalamityModClassicPreTrailer", item, itemLoot);
            AddBloodGodBagDrops("CalRD", item, itemLoot);
            AddRottenBrainBagDrop("CalamityMod", item, itemLoot);
            AddRottenBrainBagDrop("CalamityModClassicPreTrailer", item, itemLoot);
            AddRottenBrainBagDrop("CalRD", item, itemLoot);
            AddAmalgamChainBagDrops("CalamityMod", item, itemLoot);
            AddAmalgamChainBagDrops("CalamityModClassicPreTrailer", item, itemLoot);
            AddAmalgamChainBagDrops("CalRD", item, itemLoot);
            AddAfflictionBagDrop("CalamityMod", item, itemLoot);
            AddAfflictionBagDrop("CalamityModClassicPreTrailer", item, itemLoot);
            AddAfflictionBagDrop("CalRD", item, itemLoot);
			AddNebulousCoreBagDrop("CalamityMod", item, itemLoot);
			AddNebulousCoreBagDrop("CalamityModClassicPreTrailer", item, itemLoot);
			AddNebulousCoreBagDrop("CalRD", item, itemLoot);
			AddElementalChainBagDrops("CalamityMod", item, itemLoot);
			AddElementalChainBagDrops("CalamityModClassicPreTrailer", item, itemLoot);
			AddElementalChainBagDrops("CalRD", item, itemLoot);
			AddDrewsWingsBagDrop("CalamityMod", item, itemLoot);
			AddDrewsWingsBagDrop("CalamityModClassicPreTrailer", item, itemLoot);
			AddDrewsWingsBagDrop("CalRD", item, itemLoot);
        }
		/// <summary>把三版本犽戎宝藏袋中的 Drew之翼替换为本工程版本。</summary>
		private static void AddDrewsWingsBagDrop(string modName, Item item, ItemLoot itemLoot)
		{
			if (!ModLoader.TryGetMod(modName, out Mod mod)
				|| !mod.TryFind<ModItem>("YharonBag", out ModItem bag)
				|| item.type != bag.Type)
				return;
			int nativeType = mod.TryFind<ModItem>("DrewsWings", out ModItem nativeItem)
				? nativeItem.Type : -1;
			ReplaceOrAddItemDrop(itemLoot, nativeType, ModContent.ItemType<DrewsWings>());
		}
		/// <summary>
		/// 处理三版本元素石与元素召唤饰品的宝藏袋掉落。
		/// 原生同名饰品存在时先移除其规则，再按原概率加入本工程版本。
		/// </summary>
		private static void AddElementalChainBagDrops(string modName, Item item, ItemLoot itemLoot)
		{
			if (!ModLoader.TryGetMod(modName, out Mod calamity))
				return;
			string chaosBag = modName == "CalamityMod" ? "CalamitasCloneBag" : "CalamitasBag";
			string brimstoneBag = modName == "CalamityMod"
				? "BrimstoneElementalBag" : "BrimstoneWaifuBag";
			ReplaceBagDrop(calamity, item, itemLoot, brimstoneBag, "RoseStone"
				, ModContent.ItemType<RoseStone>(), 10);
			ReplaceBagDrop(calamity, item, itemLoot, "DesertScourgeBag", "AeroStone"
				, ModContent.ItemType<AeroStone>(), 10);
			ReplaceBagDrop(calamity, item, itemLoot, "AquaticScourgeBag", "AeroStone"
				, ModContent.ItemType<AeroStone>(), 10);
			ReplaceBagDrop(calamity, item, itemLoot, "CryogenBag", "CryoStone"
				, ModContent.ItemType<CryoStone>(), 10);
			ReplaceBagDrop(calamity, item, itemLoot, chaosBag, "ChaosStone"
				, ModContent.ItemType<ChaosStone>(), 10);
			ReplaceBagDrop(calamity, item, itemLoot, "PlaguebringerGoliathBag", "BloomStone"
				, ModContent.ItemType<BloomStone>(), 10);
			if (!calamity.TryFind<ModItem>("LeviathanBag", out ModItem leviathanBag)
				|| item.type != leviathanBag.Type)
				return;
			RemoveNativeDrop(calamity, itemLoot, "LureofEnthrallment");
			RemoveNativeDrop(calamity, itemLoot, "PearlofEnthrallment");
			itemLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode()
				, ModContent.ItemType<LureofEnthrallment>(), 3));
		}
		/// <summary>按指定宝藏袋与原生物品名称替换一条固定概率掉落。</summary>
		private static void ReplaceBagDrop(Mod mod, Item item, ItemLoot itemLoot, string bagName
			, string nativeName, int replacementType, int chanceDenominator)
		{
			if (!mod.TryFind<ModItem>(bagName, out ModItem bag) || item.type != bag.Type)
				return;
			int nativeType = mod.TryFind<ModItem>(nativeName, out ModItem nativeItem)
				? nativeItem.Type : -1;
			ReplaceOrAddItemDrop(itemLoot, nativeType, replacementType, chanceDenominator);
		}
		/// <summary>从宝藏袋移除指定原生物品的顶层规则，不存在时安全跳过。</summary>
		private static void RemoveNativeDrop(Mod mod, ItemLoot itemLoot, string nativeName)
		{
			if (!mod.TryFind<ModItem>(nativeName, out ModItem nativeItem))
				return;
			RemoveItemDrop(itemLoot, nativeItem.Type);
		}
		/// <summary>
		/// 将指定灾厄分支神之使徒宝藏袋中的原生星云核心替换为本工程版本。
		/// 三个分支都保留固定掉落，因此直接替换同名规则而不追加重复物品。
		/// </summary>
		private static void AddNebulousCoreBagDrop(string modName, Item item, ItemLoot itemLoot)
		{
			if (!ModLoader.TryGetMod(modName, out Mod calamity)
				|| !calamity.TryFind<ModItem>("DevourerofGodsBag", out ModItem bag)
				|| item.type != bag.Type)
				return;
			int nativeItemType = calamity.TryFind<ModItem>("NebulousCore", out ModItem nativeItem)
				? nativeItem.Type : -1;
			ReplaceOrAddItemDrop(itemLoot, nativeItemType, ModContent.ItemType<NebulousCore>());
		}
        /// <summary>将指定分支噬魂幽花宝藏袋中的原生同名饰品替换为本工程版本。</summary>
        private static void AddAfflictionBagDrop(string modName, Item item, ItemLoot itemLoot)
        {
            if (!ModLoader.TryGetMod(modName, out Mod calamity)
                || !calamity.TryFind("PolterghastBag", out ModItem bag))
                return;
            if (item.type != bag.Type)
                return;
            int nativeItemType = calamity.TryFind("Affliction", out ModItem nativeItem)
                ? nativeItem.Type : -1;
            ReplaceOrAddItemDrop(itemLoot, nativeItemType, ModContent.ItemType<Affliction>());
        }
        /// <summary>
        /// 将三个灾厄分支对应专家宝藏袋中的原生组件替换为本工程聚合之脑链组件。
        /// 现代分支使用 BrimstoneElementalBag/CalamitasCloneBag，两个经典分支使用 BrimstoneWaifuBag/CalamitasBag。
        /// </summary>
        private static void AddAmalgamChainBagDrops(string modName, Item item, ItemLoot itemLoot)
        {
            if (!ModLoader.TryGetMod(modName, out Mod calamity))
                return;
            string brimstoneBagName = modName == "CalamityMod"
                ? "BrimstoneElementalBag" : "BrimstoneWaifuBag";
            string calamitasBagName = modName == "CalamityMod"
                ? "CalamitasCloneBag" : "CalamitasBag";
            if (calamity.TryFind<ModItem>(brimstoneBagName, out ModItem brimstoneBag)
                && item.type == brimstoneBag.Type)
            {
                int nativeItemType = calamity.TryFind<ModItem>("Gehenna", out ModItem nativeItem)
                    ? nativeItem.Type : -1;
                ReplaceOrAddItemDrop(itemLoot, nativeItemType, ModContent.ItemType<Gehenna>());
            }
            if (calamity.TryFind<ModItem>(calamitasBagName, out ModItem calamitasBag)
                && item.type == calamitasBag.Type)
            {
                int nativeItemType = calamity.TryFind<ModItem>("CalamityRing", out ModItem nativeItem)
                    ? nativeItem.Type : -1;
                ReplaceOrAddItemDrop(itemLoot, nativeItemType, ModContent.ItemType<CalamityRing>());
            }
            if (calamity.TryFind<ModItem>("CrabulonBag", out ModItem crabulonBag)
                && item.type == crabulonBag.Type)
            {
                int nativeItemType = calamity.TryFind<ModItem>("FungalClump", out ModItem nativeItem)
                    ? nativeItem.Type : -1;
                ReplaceOrAddItemDrop(itemLoot, nativeItemType, ModContent.ItemType<FungalClump>());
            }
            if (calamity.TryFind<ModItem>("LeviathanBag", out ModItem leviathanBag)
                && item.type == leviathanBag.Type)
            {
                int nativeItemType = calamity.TryFind<ModItem>("LeviathanAmbergris", out ModItem nativeItem)
                    ? nativeItem.Type : -1;
                ReplaceOrAddItemDrop(itemLoot, nativeItemType, ModContent.ItemType<LeviathanAmbergris>());
            }
        }
        /// <summary>
        /// 将指定灾厄分支腐巢意志专家宝藏袋中的原生腐烂之脑替换为本工程版本。
        /// 本工程物品与灾厄原生物品是不同注册项，因此必须单独加入，才能继续合成聚合之脑。
        /// </summary>
        private static void AddRottenBrainBagDrop(string modName, Item item, ItemLoot itemLoot)
        {
            if (!ModLoader.TryGetMod(modName, out Mod calamity)
                || !calamity.TryFind<ModItem>("HiveMindBag", out ModItem hiveMindBag)
                || item.type != hiveMindBag.Type)
                return;
            int nativeItemType = calamity.TryFind<ModItem>("RottenBrain", out ModItem nativeItem)
                ? nativeItem.Type : -1;
            ReplaceOrAddItemDrop(itemLoot, nativeItemType, ModContent.ItemType<RottenBrain>());
        }
        /// <summary>
        /// 从外部宝藏袋移除指定原生物品的顶层规则，再替换为本工程同名组件。
        /// 若原生版本不存在该物品，则只追加本工程组件；条件规则内部含目标物品时整体替换，
        /// 避免同一宝藏袋同时生成重复物品。
        /// </summary>
        private static void ReplaceOrAddItemDrop(ItemLoot itemLoot, int nativeItemType, int replacementItemType)
        {
			RemoveItemDrop(itemLoot, nativeItemType);
            itemLoot.Add(ItemDropRule.Common(replacementItemType));
        }
		/// <summary>按指定概率替换原生物品，用于保留宝藏袋原本的抽取权重。</summary>
		private static void ReplaceOrAddItemDrop(ItemLoot itemLoot, int nativeItemType
			, int replacementItemType, int chanceDenominator)
		{
			RemoveItemDrop(itemLoot, nativeItemType);
			itemLoot.Add(ItemDropRule.Common(replacementItemType, chanceDenominator));
		}
		/// <summary>移除所有顶层及链式规则中包含指定物品的规则。</summary>
		private static void RemoveItemDrop(ItemLoot itemLoot, int itemType)
		{
			if (itemType < 0)
				return;
			foreach (IItemDropRule rule in itemLoot.Get().ToArray())
			{
				if (ContainsItemDrop(rule, itemType))
					itemLoot.Remove(rule);
			}
		}
        /// <summary>递归检查一条掉落规则及其后继规则是否包含指定物品。</summary>
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
        /// 从指定灾厄模组定位两个宝藏袋并追加对应饰品。
        /// 任一宝藏袋或饰品缺失时仅跳过该分支，不影响其他模组版本。
        /// </summary>
        private static void AddBloodGodBagDrops(string modName, Item item, ItemLoot itemLoot)
        {
            if (!ModLoader.TryGetMod(modName, out Mod calamity))
                return;
            if (calamity.TryFind<ModItem>("PerforatorBag", out ModItem perforatorBag)
                && item.type == perforatorBag.Type)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyWormTooth>()));
            }
            if (!calamity.TryFind<ModItem>("RavagerBag", out ModItem ravagerBag)
                || item.type != ravagerBag.Type)
                return;
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodPact>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FleshTotem>(), 2));
            itemLoot.Add(ItemDropRule.ByCondition(
                ProvidenceDownedCondition,
                ModContent.ItemType<BloodflareCore>()));
        }
        /// <summary>
        /// 物品直接命中 PvP 玩家时补挂近战减益。
        /// PvP 不会经过 NPC 的 OnHitNPC，因此必须单独覆盖这条命中路径。
        /// </summary>
        public override void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo)
        {
            CalamityRebirthPlayer.TryApplyMeleeHitDebuffsOnPvP(player, target, item.DamageType);
            CalamityRebirthPlayer.TryApplyPotionHitDebuffsOnPvP(player, target);
            CalamityRebirthPlayer.TryApplyCrystalSkullLightningOnPvP(player, target, item.DamageType
                , hurtInfo.Damage);
        }
        /// <summary>PvP 真近战命中复用始源林海近战 25% 五倍伤害判定。</summary>
        public override void ModifyHitPvp(Item item, Player player, Player target
            , ref Player.HurtModifiers modifiers)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (!modPlayer.silvaMelee || !Main.rand.NextBool(4))
                return;
            if (item.CountsAsClass(DamageClass.Melee) || item.CountsAsClass(DamageClass.MeleeNoSpeed))
                modifiers.FinalDamage *= 5f;
        }
        /// <summary>
        /// 烦恼项链使任意武器直接命中 NPC 时附加毒液。
        /// 弹幕、召唤物与盗贼弹幕的对应路径由全局弹幕命中钩子处理。
        /// </summary>
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.GetModPlayer<CalamityRebirthPlayer>().vexation)
                target.AddBuff(BuffID.Venom, NecklaceofVexation.VenomDuration);
        }
        /// <summary>
        /// 光合作用药水让红心及其节日变体额外治疗 5 点生命。
        /// 治疗只由拾取者本地发送视觉效果，最终治疗量仍受最大生命钳制。
        /// </summary>
        public override bool OnPickup(Item item, Player player)
        {
            if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
            {
                CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
                if (modPlayer.photosynthesis)
                    CalamityRebirthPlayer.RequestHeal(player
                        , CalamityRebirthPlayer.HealRequestPhotosynthesis);
            }
            return true;
        }
        /// <summary>
        /// 对灼辉金源稀有度接管物品名称行绘制。
        /// 对 CosmicPurple 使用本地水晶文字效果。
        /// 只拦截原版物品名称行，其余词缀、价格和描述行继续使用默认 tooltip 渲染。
        /// </summary>
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (item.rare == ModContent.RarityType<CosmicPurpleRarity>()
                && line.Mod == "Terraria" && line.Name == "ItemName")
            {
                CosmicPurpleRarity.Draw(item, line);
                return false;
            }
            if (item.rare != ModContent.RarityType<BurnishedAuricRarity>()
                || line.Mod != "Terraria" || line.Name != "ItemName")
                return true;
            BurnishedAuricRarity.Draw(item, line);
            return false;
        }
    }
}
