using CalamityRebirth.Players;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Systems;
using CalamityRebirth.Utilities;
using System.Linq;
using Terraria;
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
        /// 在原版物品默认值建立后撤销经典灾厄直接写入基础伤害的削弱。
        /// 弱依赖使灾厄先加载，本回调随后把受影响武器恢复为原版数值。
        /// </summary>
        public override void SetDefaults(Item item)
        {
            VanillaNerfRevertSystem.RestoreItemDefaults(item);
        }
        /// <summary>
        /// 还原 1.4.2.101 对原版塔防护甲单件造成的召唤、魔法和近战属性扣除。
        /// </summary>
        public override void UpdateEquip(Item item, Player player)
        {
            VanillaNerfRevertSystem.RestoreArmorPieceStats(item, player);
        }
        /// <summary>
        /// 还原经典灾厄对蠕虫围巾减伤与 Precise/Lucky 饰品词缀施加的削弱。
        /// </summary>
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            VanillaNerfRevertSystem.RestoreAccessoryStats(item, player);
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
            foreach (IItemDropRule rule in itemLoot.Get().ToArray())
            {
                if (nativeItemType >= 0 && ContainsItemDrop(rule, nativeItemType))
                    itemLoot.Remove(rule);
            }
            itemLoot.Add(ItemDropRule.Common(replacementItemType));
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
