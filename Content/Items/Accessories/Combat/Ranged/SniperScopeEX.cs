using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Ranged
{
    /// <summary>
    /// 狙击镜EX（SniperScopeEX） - 远程饰品。贴图是把侦察镜（Recon Scope）按 Crossbow_Scope 的复合造型重绘的
    /// `SniperScopeEX.png`（34×36，紫灰镜筒 + 金环 + 绿镜片）。
    /// 属性（用户 2026-10-25 修正）：**+22% 远程伤害 / +22% 远程暴击率 / +22% 射击速度 /
    /// +22% 远程弹幕飞行速度 / 40% 概率不消耗弹药 / -400 仇恨**；额外：
    /// ① **熔岩箭袋 + 潜行者箭袋**的其余效果（魔箭袋底子、着火箭与 -400 仇恨；不保留箭矢伤害）；
    /// ② **饰品可见时**获得步枪瞄准镜（Rifle Scope）的缩放能力。
    /// <para>
    /// ⚠️ 原先的「远程命中发射星璇火箭」（<c>ProjectileID.VortexBeaterRocket</c>）已于 2026-10-17
    /// 按用户要求**整体删除**——火箭爆炸太频繁、太卡；连带的 PvP 封包与全局弹幕钩子也一并撤掉了。
    /// 「射击时额外生成夜明箭 / 夜明弹」也已于 2026-10-18 按用户要求整体删除，不再保留运行时钩子与常量。
    /// </para>
    /// <para>
    /// 落点：三围、魔法箭袋、缩放直接在 <see cref="UpdateAccessory"/>；省弹 / 弹速
    /// 分别落在 <see cref="CalamityRebirthPlayer"/> 的 <c>CanConsumeAmmo</c> / <c>ModifyShootStats</c>，
    /// 靠每帧置位的 <c>sniperScopeEX</c> 标记触发。
    /// </para>
    /// </summary>
    internal class SniperScopeEX : ModItem
    {
        /// <summary>远程伤害加成（按比例加，用户 2026-10-17 指定 22%）</summary>
        public const float RangedBonus = 0.22f;
        /// <summary>远程暴击率加成（按百分点加，22 = +22%）</summary>
        public const float RangedCritBonus = 22f;
        /// <summary>远程射击速度加成（比例，22% = 0.22，走 GetAttackSpeed(Ranged)）</summary>
        public const float RangedAttackSpeedBonus = 0.22f;
        /// <summary>远程弹幕飞行速度加成（比例，初速乘 1.22；**不是攻击速度**）</summary>
        public const float ProjectileVelocityBonus = 0.22f;
        /// <summary>不消耗弹药的概率（0.40 = 40%）</summary>
        public const float AmmoSaveChance = 0.40f;
        /// <summary>
        /// 仇恨降低量（潜行者箭袋那条：原版 `aggro -= 400`）。两个箭袋都带这条，本饰品**只算一次**。
        /// </summary>
        public const int AggroReduction = 400;
        /// <summary>
        /// 物品基础属性：34×36 的饰品贴图（与 Crossbow_Scope 同画布）。
        /// 稀有度沿用远程系 EX 档 <see cref="RangerEXRarity"/>（星旋绿青渐变字色）。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 34;    // 贴图宽（像素）
            Item.height = 36;   // 贴图高（像素）
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 15);   // 沿用 EX 饰品的量级（用户未点名）
            Item.rare = ModContent.RarityType<RangerEXRarity>();
        }
        /// <summary>
        /// 装备时生效：远程三围 + 魔法箭袋 + 可见时的步枪瞄准镜缩放，
        /// 并把「已装备」标记交给 ModPlayer 供省弹与弹速两个运行时钩子读取
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += RangedBonus;
            player.GetCritChance(DamageClass.Ranged) += RangedCritBonus;
            player.GetAttackSpeed(DamageClass.Ranged) += RangedAttackSpeedBonus;
            // ── 熔岩箭袋的效果：魔箭袋底子 + 着火箭；箭矢伤害加成按用户要求删除 ──
            player.magicQuiver = true;
            player.hasMoltenQuiver = true;                  // 把普通箭点成火焰箭（PickAmmo 里读这个）
            // ── 潜行者箭袋的其余效果：-400 仇恨（其箭矢伤害加成已删除） ──
            player.aggro -= AggroReduction;                 // 原版是 aggro -= 400；这里只减一次
            // 步枪瞄准镜效果（原版 Player.scope）：只在饰品可见（未隐藏外观）时启用
            if (!hideVisual)
            {
                player.scope = true;
            }
            player.GetModPlayer<CalamityRebirthPlayer>().sniperScopeEX = true;
        }
        /// <summary>
        /// 配方：潜行者箭袋 + 熔岩箭袋 + 侦察镜 `ReconScope`
        /// + 烈火手套EX 的四味非饰品材料。
        /// 现代灾厄用化神魂精并在星宇砧合成；经典灾厄按完整配方等价换算并在嘉登熔炉合成。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                && modern.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                // 现代灾厄保留原版高阶材料与星宇砧路径。
                Recipe recipe = CreateSniperScopeBaseRecipe();
                recipe.AddIngredient(ascendantSpiritEssence.Type, FireGauntletEX.SpiritMaterialCount);
                recipe.AddIngredient(cosmiliteBar.Type, FireGauntletEX.CosmiliteBarCount);
                recipe.AddTile(cosmicAnvil.Type);
                recipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicCosmiliteBar)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
            {
                // 经典 1.4.2 没有现代版化神魂精，CRUtil 会换成四种等价材料。
                Recipe recipe = CreateSniperScopeBaseRecipe();
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
                // CalRD 仍提供 AscendantSpiritEssence，但高级合成站统一使用嘉登熔炉。
                Recipe recipe = CreateSniperScopeBaseRecipe();
                recipe.AddIngredient(classic1457Essence.Type, FireGauntletEX.SpiritMaterialCount);
                recipe.AddIngredient(classic1457CosmiliteBar.Type, FireGauntletEX.CosmiliteBarCount);
                recipe.AddTile(classic1457DraedonsForge.Type);
                recipe.Register();
            }
        }
        /// <summary>创建三版本共用的狙击镜EX基础升级材料，不包含高阶材料与合成站。</summary>
        private Recipe CreateSniperScopeBaseRecipe()
        {
            return CreateRecipe()
                .AddIngredient(ItemID.StalkersQuiver)
                .AddIngredient(ItemID.MoltenQuiver)
                .AddIngredient(ItemID.ReconScope)
                .AddIngredient(ItemID.FragmentSolar, FireGauntletEX.SolarFragmentCount)
                .AddIngredient(ItemID.LunarBar, FireGauntletEX.LunarBarCount);
        }
    }
}
