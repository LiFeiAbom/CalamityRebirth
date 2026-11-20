using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 烈火手套EX：复合战士徽章EX、烈火手套、狂战士手套和天界壳的核心能力，
    /// 并在近战命中时同时附加燃烧III与破晓II。
    /// </summary>
    internal class FireGauntletEX : ModItem
    {
        /// <summary>近战伤害与攻击速度的共用加成比例。</summary>
        public const float MeleeBonus = 0.22f;
        /// <summary>近战暴击率加成，单位为百分点。</summary>
        public const float MeleeCritBonus = 22f;
        /// <summary>直接增加的面板防御。</summary>
        public const int DefenseBonus = 8;
        /// <summary>增加的仇恨值，使敌怪更倾向攻击佩戴者。</summary>
        public const int AggroBonus = 400;
        /// <summary>高级 EX 配方固定的日曜碎片数量。</summary>
        internal const int SolarFragmentCount = 20;
        /// <summary>高级 EX 配方固定的夜明锭数量。</summary>
        internal const int LunarBarCount = 10;
        /// <summary>高级 EX 配方固定的化神魂精数量。</summary>
        internal const int SpiritMaterialCount = 5;
        /// <summary>高级 EX 配方固定的宇宙锭数量。</summary>
        internal const int CosmiliteBarCount = 5;
        /// <summary>设置饰品防御、尺寸、售价和战士系 EX 稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.defense = DefenseBonus;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ModContent.RarityType<WarriorEXRarity>();
        }
        /// <summary>
        /// 应用近战三围、防御、仇恨、烈火手套攻击字段和天界壳生存字段。
        /// 可见性开关只隐藏狼人与海神外观，不关闭被动功能。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += MeleeBonus;
            player.GetCritChance(DamageClass.Melee) += MeleeCritBonus;
            player.GetAttackSpeed(DamageClass.Melee) += MeleeBonus;
            player.aggro += AggroBonus;
            player.magmaStone = true;
            player.kbGlove = true;
            player.autoReuseGlove = true;
            player.meleeScaleGlove = true;
            player.wolfAcc = true;
            player.accMerman = true;
            if (hideVisual)
            {
                player.hideWolf = true;
                player.hideMerman = true;
            }
            player.lifeRegen += 2;
            player.pickSpeed -= 0.15f;
            player.GetKnockback(DamageClass.Summon) += 1f;
            player.GetModPlayer<CalamityRebirthPlayer>().fireGauntletEX = true;
        }
        /// <summary>
        /// 分别为现代灾厄、经典 1.4.2.101 和 CalRD 1.4.5.7 注册配方。
        /// 三种版本对化神魂精和合成站的内置名称不同，因此必须独立探测并安全回退。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                && modern.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                Recipe recipe = CreateFireGauntletBaseRecipe();
                recipe.AddIngredient(ascendantSpiritEssence.Type, SpiritMaterialCount);
                recipe.AddIngredient(cosmiliteBar.Type, CosmiliteBarCount);
                recipe.AddTile(cosmicAnvil.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicCosmiliteBar)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
            {
                Recipe recipe = CreateFireGauntletBaseRecipe();
                if (!CRUtil.TryAddClassicAscendantSpiritEssence(recipe, SpiritMaterialCount))
                    return;
                recipe.AddIngredient(classicCosmiliteBar.Type, CosmiliteBarCount);
                recipe.AddTile(draedonsForge.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod classic1457)
                && classic1457.TryFind<ModItem>("AscendantSpiritEssence", out ModItem classic1457Essence)
                && classic1457.TryFind<ModItem>("CosmiliteBar", out ModItem classic1457CosmiliteBar)
                && classic1457.TryFind<ModTile>("DraedonsForge", out ModTile classic1457DraedonsForge))
            {
                Recipe recipe = CreateFireGauntletBaseRecipe();
                recipe.AddIngredient(classic1457Essence.Type, SpiritMaterialCount);
                recipe.AddIngredient(classic1457CosmiliteBar.Type, CosmiliteBarCount);
                recipe.AddTile(classic1457DraedonsForge.Type);
                recipe.Register();
            }
        }
        /// <summary>
        /// 创建与灾厄版本无关的公共升级底座。
        /// 返回未注册的 Recipe，由各版本分支追加高阶材料与合成站后统一 Register。
        /// </summary>
        private Recipe CreateFireGauntletBaseRecipe()
        {
            return CreateRecipe()
                .AddIngredient<WarriorEmblemEX>()
                .AddIngredient(ItemID.FireGauntlet)
                .AddIngredient(ItemID.BerserkerGlove)
                .AddIngredient(ItemID.CelestialShell)
                .AddIngredient(ItemID.FragmentSolar, SolarFragmentCount)
                .AddIngredient(ItemID.LunarBar, LunarBarCount);
        }
    }
}
