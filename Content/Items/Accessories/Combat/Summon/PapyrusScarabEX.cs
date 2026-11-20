using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
    /// <summary>
    /// 甲虫莎草纸EX（PapyrusScarabEX） - 月后召唤饰品。贴图是甲虫莎草纸按星尘盔甲复合色系染成
    /// 的「金/棕纸面 + 蓝/青甲虫」（2026-10-18 完成重染色）。
    /// 属性（用户 2026-10-26 修正）：+22% 召唤伤害、+22% 鞭子攻击速度、+22% 鞭子攻击范围、
    /// +4 召唤栏、+1 哨兵栏、+44% 召唤击退。
    /// 召唤伤害无视目标 30% 防御（命中结算时缩放 DefenseEffectiveness）。
    /// 配方：召唤师徽章EX + 甲虫莎草纸 + 矮人项链 + 四件哨兵饰品任选其一 + 星尘碎片×99；
    /// 现代灾厄用星宇砧，经典灾厄用嘉登熔炉。
    /// </summary>
    internal class PapyrusScarabEX : ModItem
    {
        /// <summary>召唤伤害加成比例。</summary>
        public const float SummonDamageBonus = 0.22f;
        /// <summary>鞭子攻击速度加成比例。</summary>
        public const float WhipSpeedBonus = 0.22f;
        /// <summary>鞭子攻击范围加成比例。</summary>
        public const float WhipRangeBonus = 0.22f;
        /// <summary>增加的仆从栏位。</summary>
        public const int MinionSlotBonus = 4;
        /// <summary>增加的哨兵栏位。</summary>
        public const int SentrySlotBonus = 1;
        /// <summary>召唤伤害击退加成比例。</summary>
        public const float SummonKnockbackBonus = 0.44f;
        /// <summary>召唤伤害忽略目标防御的比例。</summary>
        public const float DefenseIgnore = 0.30f;
        /// <summary>高级 EX 配方使用的化神魂精数量。</summary>
        internal const int SpiritMaterialCount = 5;
        /// <summary>高级 EX 配方使用的宇宙锭数量。</summary>
        internal const int CosmiliteBarCount = 5;
        /// <summary>高级 EX 配方使用的日曜碎片数量。</summary>
        internal const int SolarFragmentCount = 20;
        /// <summary>高级 EX 配方使用的夜明锭数量。</summary>
        internal const int LunarBarCount = 10;
        /// <summary>可作为甲虫莎草纸EX塔防分支材料的四件原版饰品。</summary>
        private static readonly int[] SentryAccessories =
        [
            ItemID.ApprenticeScarf,
            ItemID.SquireShield,
            ItemID.HuntressBuckler,
            ItemID.MonkBelt,
        ];
        /// <summary>设置饰品尺寸、售价和召唤系 EX 稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ModContent.RarityType<SummonerEXRarity>();
        }
        /// <summary>应用全部召唤数值，并写入穿防标记供统一命中结算。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += SummonDamageBonus;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += WhipSpeedBonus;
            player.whipRangeMultiplier += WhipRangeBonus;
            player.maxMinions += MinionSlotBonus;
            player.maxTurrets += SentrySlotBonus;
            player.GetKnockback(DamageClass.Summon) += SummonKnockbackBonus;
            player.GetModPlayer<CalamityRebirthPlayer>().papyrusScarabEX = true;
        }
        /// <summary>
        /// 按三版本灾厄实际提供的合成站与材料注册四套塔防饰品替换配方。
        /// 现代版使用真实化神魂精与宇宙锭并在星宇砧制作；
        /// 经典 1.4.2.101 通过 CRUtil 换算化神魂精，并在嘉登熔炉制作；
        /// CalRD 1.4.5.7 直接使用其同名高级材料与嘉登熔炉。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                && modern.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                foreach (int sentryAccessory in SentryAccessories)
                {
                    Recipe recipe = CreateSentryRecipeBase(sentryAccessory);
                    recipe.AddIngredient(ascendantSpiritEssence.Type, SpiritMaterialCount);
                    recipe.AddIngredient(cosmiliteBar.Type, CosmiliteBarCount);
                    recipe.AddTile(cosmicAnvil.Type);
                    recipe.Register();
                }
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicCosmiliteBar)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
            {
                foreach (int sentryAccessory in SentryAccessories)
                {
                    Recipe recipe = CreateSentryRecipeBase(sentryAccessory);
                    if (!CRUtil.TryAddClassicAscendantSpiritEssence(recipe, SpiritMaterialCount))
                        continue;
                    recipe.AddIngredient(classicCosmiliteBar.Type, CosmiliteBarCount);
                    recipe.AddTile(draedonsForge.Type);
                    recipe.Register();
                }
            }
            if (ModLoader.TryGetMod("CalRD", out Mod classic1457)
                && classic1457.TryFind<ModItem>("AscendantSpiritEssence", out ModItem classic1457Essence)
                && classic1457.TryFind<ModItem>("CosmiliteBar", out ModItem classic1457CosmiliteBar)
                && classic1457.TryFind<ModTile>("DraedonsForge", out ModTile classic1457DraedonsForge))
            {
                foreach (int sentryAccessory in SentryAccessories)
                {
                    Recipe recipe = CreateSentryRecipeBase(sentryAccessory);
                    recipe.AddIngredient(classic1457Essence.Type, SpiritMaterialCount);
                    recipe.AddIngredient(classic1457CosmiliteBar.Type, CosmiliteBarCount);
                    recipe.AddTile(classic1457DraedonsForge.Type);
                    recipe.Register();
                }
            }
        }
        /// <summary>
        /// 创建三版本共享的塔防饰品升级底座。
        /// 四种塔防饰品互斥，因此每条配方只消耗其中一件；
        /// 日曜碎片与夜明锭在此统一加入，灾厄专属材料由调用方按版本追加。
        /// </summary>
        private Recipe CreateSentryRecipeBase(int sentryAccessory)
        {
            return CreateRecipe()
                .AddIngredient<SummonerEmblemEX>()
                .AddIngredient(ItemID.PapyrusScarab)
                .AddIngredient(ItemID.PygmyNecklace)
                .AddIngredient(sentryAccessory)
                .AddIngredient(ItemID.FragmentStardust, 99)
                .AddIngredient(ItemID.FragmentSolar, SolarFragmentCount)
                .AddIngredient(ItemID.LunarBar, LunarBarCount);
        }
    }
}
