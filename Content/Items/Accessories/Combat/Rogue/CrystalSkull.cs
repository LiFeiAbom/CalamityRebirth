using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>
    /// 晶光头骨：融合电工手套、精准手套与电弧闪戒的终局盗贼饰品。
    /// 物品本身只点亮状态标记，三版本职业数值与潜行桥由 ModPlayer 在晚装阶段统一结算。
    /// </summary>
    internal sealed class CrystalSkull : ModItem
    {
        /// <summary>盗贼伤害加成比例。</summary>
        public const float RogueDamageBonus = 0.22f;
        /// <summary>盗贼暴击率加成，单位为百分点。</summary>
        public const int RogueCritBonus = 22;
        /// <summary>盗贼武器攻击速度加成比例。</summary>
        public const float RogueSpeedBonus = 0.22f;
        /// <summary>增加的最大潜行内部值；1f 对应显示值 100，因此 1.20f 等于 +120。</summary>
        public const float MaxStealthBonus = 1.20f;
        /// <summary>静止和移动状态下同时增加的潜行恢复比例。</summary>
        public const float StealthRegenBonus = 0.10f;
        /// <summary>盗贼攻击召唤闪电的概率。</summary>
        public const float LightningChance = 0.10f;
        /// <summary>闪电伤害相对触发攻击基础伤害的倍率。</summary>
        public const float LightningDamageMultiplier = 3f;
        /// <summary>连续弹幕触发闪电的最短间隔，避免高频攻击刷屏。</summary>
        public const int LightningCooldown = 30;
        /// <summary>电工手套提供的潜行打击护甲穿透。</summary>
        public const float StealthArmorPenetration = 8f;
        /// <summary>电工手套提供的潜行打击伤害倍率。</summary>
        public const float StealthDamageBonus = 0.08f;
        /// <summary>电工手套潜行命中时的治疗量。</summary>
        public const int StealthHeal = 10;
        /// <summary>单次潜行打击最多触发电火花的命中次数。</summary>
        public const int MaxSparkHitTriggers = 3;
        /// <summary>每层潜行打击火花触发生成的电火花数量。</summary>
        public const int SparksPerTrigger = 3;
        /// <summary>单枚电火花的基础伤害。</summary>
        public const int SparkBaseDamage = 95;
        /// <summary>基础配方要求的大量通用材料数量。</summary>
        private const int BulkMaterialCount = 1000;
        /// <summary>原版宏伟蓝图的稳定物品 ID；当前 ItemID 字段表未提供命名常量。</summary>
        private const int GrandDesignItemId = 3619;
        /// <summary>高级 EX 配方使用的化神魂精数量。</summary>
        private const int SpiritMaterialCount = 5;
        /// <summary>高级 EX 配方使用的宇宙锭数量。</summary>
        private const int CosmiliteBarCount = 5;
        /// <summary>高级 EX 配方使用的日曜碎片数量。</summary>
        private const int SolarFragmentCount = 20;
        /// <summary>高级 EX 配方使用的夜明锭数量。</summary>
        private const int LunarBarCount = 10;
        /// <summary>设置饰品尺寸、售价与盗贼系 EX 稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 58;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ModContent.RarityType<RogueEXRarity>();
        }
        /// <summary>点亮本帧装备标记，供命中、潜行和闪电结算读取。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().crystalSkull = true;
        }
        /// <summary>
        /// 注册现代灾厄、经典 1.4.2.101 与 CalRD 的晶光头骨高级 EX 配方。
        /// 三版本共享盗贼徽章EX、宏伟蓝图、电线、纳米机器人、骨头与机械头骨底座，
        /// 再分别追加对应版本存在的化神魂精、宇宙锭、日曜碎片、夜明锭和合成站。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                && modern.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                Recipe recipe = CreateCrystalSkullBaseRecipe();
                recipe.AddIngredient(ascendantSpiritEssence.Type, SpiritMaterialCount);
                recipe.AddIngredient(cosmiliteBar.Type, CosmiliteBarCount);
                recipe.AddTile(cosmicAnvil.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicCosmiliteBar)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
            {
                Recipe recipe = CreateCrystalSkullBaseRecipe();
                if (!CRUtil.TryAddClassicAscendantSpiritEssence(recipe, SpiritMaterialCount))
                    return;
                recipe.AddIngredient(classicCosmiliteBar.Type, CosmiliteBarCount);
                recipe.AddTile(draedonsForge.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem classicAscendantSpiritEssence)
                && calRD.TryFind<ModItem>("CosmiliteBar", out ModItem calRDCosmiliteBar)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDDraedonsForge))
            {
                Recipe recipe = CreateCrystalSkullBaseRecipe();
                recipe.AddIngredient(classicAscendantSpiritEssence.Type, SpiritMaterialCount);
                recipe.AddIngredient(calRDCosmiliteBar.Type, CosmiliteBarCount);
                recipe.AddTile(calRDDraedonsForge.Type);
                recipe.Register();
            }
        }
        /// <summary>
        /// 创建三版本共享的基础配方。
        /// 返回未注册配方，由各灾厄分支追加专属材料与合成站。
        /// </summary>
        private Recipe CreateCrystalSkullBaseRecipe()
        {
            return CreateRecipe()
                .AddIngredient<RogueEmblemEX>()
                .AddIngredient(GrandDesignItemId)
                .AddIngredient(ItemID.Wire, BulkMaterialCount)
                .AddIngredient(ItemID.Nanites, BulkMaterialCount)
                .AddIngredient(ItemID.Bone, BulkMaterialCount)
                .AddIngredient(ItemID.MechanicalSkull)
                .AddIngredient(ItemID.FragmentSolar, SolarFragmentCount)
                .AddIngredient(ItemID.LunarBar, LunarBarCount);
        }
    }
}
