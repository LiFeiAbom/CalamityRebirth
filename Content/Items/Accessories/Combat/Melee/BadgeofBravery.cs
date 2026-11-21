using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 勇气勋章：元素之握近战饰品链的前置饰品。
    /// 按用户指定提供三项固定近战属性，并采用现代灾厄 2.2.2 的物品贴图与基础稀有度。
    /// </summary>
    internal sealed class BadgeofBravery : ModItem
    {
        /// <summary>近战伤害加成比例。</summary>
        public const float MeleeDamageBonus = 0.10f;
        /// <summary>近战暴击率加成，单位为百分点。</summary>
        public const float MeleeCritBonus = 10f;
        /// <summary>近战护甲穿透加成；用户要求使用 10 点，而非现代 2.2.2 的 5 点。</summary>
        public const float MeleeArmorPenetrationBonus = 10f;
        /// <summary>现代 2.2.2 配方需要的永恒花锭数量。</summary>
        private const int ModernBloomBarCount = 12;
        /// <summary>两个经典灾厄分支配方需要的 UeliaceBar 数量。</summary>
        private const int ClassicBloomBarCount = 2;
        /// <summary>
        /// 设置物品尺寸、饰品类型、现代稀有度与购买价。
        /// 判定尺寸沿用现代物品的 30×30；实际 2.2.2 贴图尺寸为 42×30。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.accessory = true;
            RarityUtil.Apply(Item, ModContent.RarityType<TurquoiseRarity>());
        }
        /// <summary>
        /// 应用用户指定的勇气勋章面板：
        /// 近战伤害 +10%、近战暴击率 +10、近战护甲穿透 +10。
        /// 本实现不附加参考工程旧版的龙蒿套装条件攻速，保持属性与点名规格一致。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += MeleeDamageBonus;
            player.GetCritChance(DamageClass.Melee) += MeleeCritBonus;
            player.GetArmorPenetration(DamageClass.Melee) += MeleeArmorPenetrationBonus;
        }
        /// <summary>
        /// 分别注册现代、经典 1.4.2.101 与 CalRD 的配方。
        /// 三个分支均严格采用 ReferenceMod 中对应灾厄版本的原始配方，不混用旧工程口径。
        /// </summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代 2.2.2 原配方：战士徽章 + 永恒花锭 ×12，于秘银砧制作。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod modern)
                || !modern.TryFind<ModItem>("UelibloomBar", out ModItem bloomBar))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient(bloomBar.Type, ModernBloomBarCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 原配方：UeliaceBar ×2 + 猛爪手套，于远古操纵机制作。
        /// 该版本不消耗战士徽章。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("UeliaceBar", out ModItem bloomBar))
                return;
            CreateRecipe()
                .AddIngredient(bloomBar.Type, ClassicBloomBarCount)
                .AddIngredient(ItemID.FeralClaws)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 原配方：UeliaceBar ×2 + 猛爪手套，于远古操纵机制作。
        /// 与 1.4.2.101 保持相同的材料构成和制作站。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("UeliaceBar", out ModItem bloomBar))
                return;
            CreateRecipe()
                .AddIngredient(bloomBar.Type, ClassicBloomBarCount)
                .AddIngredient(ItemID.FeralClaws)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
