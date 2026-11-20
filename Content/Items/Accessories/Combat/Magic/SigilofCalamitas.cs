using CalamityRebirth.Content.Items.Accessories.Construction;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Magic
{
    /// <summary>
    /// 灾厄符章：魔法职业中间饰品。
    /// 提供魔法伤害、暴击、最大魔力、魔力减耗、探宝与缩短药水病。
    /// 贴图使用 8 帧纵向灵魂动画。
    /// </summary>
    internal sealed class SigilofCalamitas : ModItem
    {
        /// <summary>魔法伤害加成比例。</summary>
        public const float MagicDamageBonus = 0.15f;
        /// <summary>魔法暴击率加成百分点。</summary>
        public const float MagicCritBonus = 10f;
        /// <summary>最大魔力加成。</summary>
        public const int MaxManaBonus = 100;
        /// <summary>魔力消耗最终乘数，0.85 表示降低 15%。</summary>
        public const float ManaCostMultiplier = 0.85f;
        /// <summary>注册 8 帧、每帧 6 tick 的纵向位图动画。</summary>
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        /// <summary>
        /// 使用 28×32 判定尺寸与 30 金价值。
        /// 稀有度按现代版灾厄符章修正为黄色。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }
        /// <summary>结算魔法面板、探宝与缩短药水病效果。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += MagicDamageBonus;
            player.GetCritChance(DamageClass.Magic) += MagicCritBonus;
            player.statManaMax2 += MaxManaBonus;
            player.manaCost *= ManaCostMultiplier;
            player.findTreasure = true;
            player.pStone = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 的灾厄符章配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipes();
            AddCalRDRecipe();
        }
        /// <summary>现代使用天界徽章、焦炭矿锭×8 与灾厄之烬×15，在秘银砧制作。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("ScoriaBar", out ModItem scoriaBar)
                || !calamity.TryFind<ModItem>("AshesofCalamity", out ModItem ashesOfCalamity))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.CelestialEmblem)
                .AddIngredient(scoriaBar.Type, 8)
                .AddIngredient(ashesOfCalamity.Type, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>经典 1.4.2.101 分别为腐化邪水与猩红血水注册两条等价配方。</summary>
        private void AddClassic142Recipes()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CalamityDust", out ModItem calamityDust)
                || !classic.TryFind<ModItem>("CoreofChaos", out ModItem coreOfChaos))
                return;
            AddClassic142Recipe(calamityDust, coreOfChaos, ItemID.UnholyWater);
            AddClassic142Recipe(calamityDust, coreOfChaos, ItemID.BloodWater);
        }
        /// <summary>添加一条经典 1.4.2.101 灾厄符章配方，仅替换邪恶水类型。</summary>
        private void AddClassic142Recipe(ModItem calamityDust, ModItem coreOfChaos, int evilWaterType)
        {
            CreateRecipe()
                .AddIngredient(ItemID.CharmofMyths)
                .AddIngredient(ItemID.SorcererEmblem)
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddIngredient(calamityDust.Type, 5)
                .AddIngredient(coreOfChaos.Type, 5)
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient<ChaosAmulet>()
                .AddIngredient(evilWaterType, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 使用原生材料与本工程混沌护符；
        /// 原版邪恶水优先使用 CalRD 的 AnyEvilWater 组，缺失时拆成腐化与猩红两条配方。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("CalamityDust", out ModItem calamityDust)
                || !calRD.TryFind<ModItem>("CoreofChaos", out ModItem coreOfChaos)
                || !calRD.TryFind<ModItem>("CruptixBar", out ModItem cruptixBar))
                return;
            if (RecipeGroup.recipeGroupIDs.ContainsKey("AnyEvilWater"))
                AddCalRDRecipe(calamityDust, coreOfChaos, cruptixBar, null);
            else
            {
                AddCalRDRecipe(calamityDust, coreOfChaos, cruptixBar, ItemID.UnholyWater);
                AddCalRDRecipe(calamityDust, coreOfChaos, cruptixBar, ItemID.BloodWater);
            }
        }
        /// <summary>添加一条 CalRD 灾厄符章配方，null 邪恶水类型表示使用配方组。</summary>
        private void AddCalRDRecipe(ModItem calamityDust, ModItem coreOfChaos, ModItem cruptixBar, int? evilWaterType)
        {
            Recipe recipe = CreateRecipe()
                .AddIngredient(ItemID.SorcererEmblem)
                .AddIngredient(ItemID.CelestialCuffs)
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddIngredient(calamityDust.Type, 5)
                .AddIngredient(coreOfChaos.Type, 5)
                .AddIngredient(cruptixBar.Type, 2)
                .AddIngredient<ChaosAmulet>()
                .AddTile(TileID.MythrilAnvil);
            if (evilWaterType.HasValue)
                recipe.AddIngredient(evilWaterType.Value, 10);
            else
                recipe.AddRecipeGroup("AnyEvilWater", 10);
            recipe.Register();
        }
    }
}
