using CalamityRebirth.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Magic
{
    /// <summary>
    /// 空灵护符：顶级魔法职业复合饰品。
    /// 提供魔法伤害、暴击、最大魔力、魔力减耗、探宝、缩短药水病和自动喝蓝。
    /// 稀有度按用户要求改为现代弑神者套装使用的 CosmicPurple。
    /// </summary>
    internal sealed class EtherealTalisman : ModItem
    {
        /// <summary>魔法伤害加成比例。</summary>
        public const float MagicDamageBonus = 0.20f;
        /// <summary>魔法暴击率加成百分点。</summary>
        public const float MagicCritBonus = 20f;
        /// <summary>最大魔力加成。</summary>
        public const int MaxManaBonus = 150;
        /// <summary>魔力消耗最终乘数，0.8 表示降低 20%。</summary>
        public const float ManaCostMultiplier = 0.8f;
        /// <summary>
        /// 使用 28×32 判定尺寸和 90 金价值，贴图尺寸为 52×40。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.rare = ModContent.RarityType<CosmicPurpleRarity>();
            Item.accessory = true;
        }
        /// <summary>
        /// 结算全部面板与功能属性。
        /// pStone 缩短治疗药水病时间，manaFlower 提供自动使用魔力药水。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += MagicDamageBonus;
            player.GetCritChance(DamageClass.Magic) += MagicCritBonus;
            player.statManaMax2 += MaxManaBonus;
            player.manaCost *= ManaCostMultiplier;
            player.findTreasure = true;
            player.pStone = true;
            player.manaFlower = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 的空灵护符配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代使用本工程灾厄符章、任意魔力花类饰品与化神魂精×4，在星宇砧制作。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil)
                || !RecipeGroup.recipeGroupIDs.ContainsKey("AnyManaFlower"))
                return;
            CreateRecipe()
                .AddIngredient<SigilofCalamitas>()
                .AddRecipeGroup("AnyManaFlower")
                .AddIngredient(ascendantSpiritEssence.Type, 4)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 使用本工程灾厄符章、魔力花与三种高阶材料各 20，在嘉登熔炉制作。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("Phantoplasm", out ModItem phantoplasm)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<SigilofCalamitas>()
                .AddIngredient(ItemID.ManaFlower)
                .AddIngredient(phantoplasm.Type, 20)
                .AddIngredient(nightmareFuel.Type, 20)
                .AddIngredient(endothermicEnergy.Type, 20)
                .AddTile(draedonsForge.Type)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 使用本工程灾厄符章、魔力花与化神魂精×4，在嘉登熔炉制作。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<SigilofCalamitas>()
                .AddIngredient(ItemID.ManaFlower)
                .AddIngredient(ascendantSpiritEssence.Type, 4)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
