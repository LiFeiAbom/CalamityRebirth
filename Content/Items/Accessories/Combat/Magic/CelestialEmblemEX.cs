using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Magic
{
    /// <summary>
    /// 天界纹章EX（CelestialEmblemEX） - 月后魔法饰品。贴图是原版天界徽章按星云盔甲色系染成的
    /// 「灰盘 + 蓝紫环 + 品红星」（2026-10-18 完成重染色）。
    /// 属性（用户 2026-10-26 修正）：+22% 魔法伤害、+22% 魔法暴击率、+22% 施法速度、
    /// +200 最大魔力、+4 魔力/秒、降低 40% 魔力消耗、免疫魔力病；
    /// 并获得奥术花、天界手铐、魔力斗篷的效果（自动喝蓝、魔力星拾取、受伤回蓝）。
    /// <para>
    /// 数值口径：+40% 蓝耗与 +200 最大魔力是**总数值**，不再额外叠加奥术花/魔力斗篷各自的 8% 蓝耗，
    /// 也不额外叠加天界手铐的 +20 最大魔力；三件饰品的非数值能力照原版字段全部给到。
    /// </para>
    /// </summary>
    internal class CelestialEmblemEX : ModItem
    {
        /// <summary>魔法伤害加成（比例，22% = 0.22）</summary>
        public const float MagicBonus = 0.22f;
        /// <summary>魔法暴击率加成（百分点，22 = +22%）</summary>
        public const float MagicCritBonus = 22f;
        /// <summary>施法速度加成（比例，22% = 0.22）</summary>
        public const float MagicCastSpeedBonus = 0.22f;
        /// <summary>最大魔力值加成（点）</summary>
        public const int MaxManaBonus = 200;
        /// <summary>每秒额外回复的魔力（点）</summary>
        public const int ManaPerSecond = 4;
        /// <summary>魔力消耗降低比例（0.40 = 降低 40%）</summary>
        public const float ManaCostReduction = 0.40f;
        /// <summary>
        /// 物品基础属性：30×32 的饰品贴图（与天界徽章原图同口径）。
        /// 稀有度沿用魔法系 EX 档 <see cref="SorcererEXRarity"/>（星云紫/品红/冰蓝渐变）。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ModContent.RarityType<SorcererEXRarity>();
        }
        /// <summary>
        /// 装备时生效：魔法三围、最大魔力、蓝耗、魔力病免疫，以及三件原版饰品的非数值能力。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += MagicBonus;
            player.GetCritChance(DamageClass.Magic) += MagicCritBonus;
            player.GetAttackSpeed(DamageClass.Magic) += MagicCastSpeedBonus;
            player.statManaMax2 += MaxManaBonus;
            player.manaCost -= ManaCostReduction;
            player.buffImmune[BuffID.ManaSickness] = true;             // 免疫魔力病
            // 奥术花 / 魔力斗篷：自动使用魔力药水
            player.manaFlower = true;
            // 奥术花 / 天界手铐：扩大魔力星拾取范围
            player.manaMagnet = true;
            // 天界手铐：受伤时恢复魔力
            player.magicCuffs = true;
            // 魔力斗篷：受伤时恢复魔力（原版用 starCloakItem 的 override 项实现）
            player.starCloakItem = Item;
            player.starCloakItem_manaCloakOverrideItem = Item;
            player.GetModPlayer<CalamityRebirthPlayer>().celestialEmblemEX = true;
        }
        /// <summary>
        /// 配方（2026-10-26 用户修正）：奥术花 + 天界手铐 + 魔力斗篷 + 法师徽章EX
        /// + 烈火手套EX 的四味非饰品材料。
        /// 现代灾厄用化神魂精并在星宇砧合成；经典灾厄按完整配方等价换算并在嘉登熔炉合成。
        /// 已删去天界徽章与星云碎片 ×99，与狙击镜EX 的改法保持一致。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                && modern.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                // 现代灾厄分支使用真实化神魂精、宇宙锭和星宇砧。
                Recipe recipe = CreateCelestialEmblemBaseRecipe();
                recipe.AddIngredient(ascendantSpiritEssence.Type, FireGauntletEX.SpiritMaterialCount);
                recipe.AddIngredient(cosmiliteBar.Type, FireGauntletEX.CosmiliteBarCount);
                recipe.AddTile(cosmicAnvil.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicCosmiliteBar)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
            {
                // 经典 1.4.2 分支通过 CRUtil 将缺失的化神魂精换算为等价材料。
                Recipe recipe = CreateCelestialEmblemBaseRecipe();
                if (!CRUtil.TryAddClassicAscendantSpiritEssence(
                    recipe, FireGauntletEX.SpiritMaterialCount))
                {
                    return;
                }
                recipe.AddIngredient(classicCosmiliteBar.Type, FireGauntletEX.CosmiliteBarCount);
                recipe.AddTile(draedonsForge.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod classic1457)
                && classic1457.TryFind<ModItem>("AscendantSpiritEssence", out ModItem classic1457Essence)
                && classic1457.TryFind<ModItem>("CosmiliteBar", out ModItem classic1457CosmiliteBar)
                && classic1457.TryFind<ModTile>("DraedonsForge", out ModTile classic1457DraedonsForge))
            {
                // CalRD 分支直接使用其同名高级材料，但统一在嘉登熔炉制作。
                Recipe recipe = CreateCelestialEmblemBaseRecipe();
                recipe.AddIngredient(classic1457Essence.Type, FireGauntletEX.SpiritMaterialCount);
                recipe.AddIngredient(classic1457CosmiliteBar.Type, FireGauntletEX.CosmiliteBarCount);
                recipe.AddTile(classic1457DraedonsForge.Type);
                recipe.Register();
            }
        }
        /// <summary>创建三版本共享的饰品底座，返回未注册配方供各版本补充高阶材料。</summary>
        private Recipe CreateCelestialEmblemBaseRecipe()
        {
            return CreateRecipe()
                .AddIngredient(ItemID.ArcaneFlower)
                .AddIngredient(ItemID.CelestialCuffs)
                .AddIngredient(ItemID.ManaCloak)
                .AddIngredient<SorcererEmblemEX>()
                .AddIngredient(ItemID.FragmentSolar, FireGauntletEX.SolarFragmentCount)
                .AddIngredient(ItemID.LunarBar, FireGauntletEX.LunarBarCount);
        }
    }
}
