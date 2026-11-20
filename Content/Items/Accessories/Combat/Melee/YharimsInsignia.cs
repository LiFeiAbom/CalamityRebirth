using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 魔君之证章：元素之握证章支流的中间饰品。
    /// 提供 15% 近战伤害、10% 近战暴击和 15% 近战攻速。
    /// </summary>
    internal sealed class YharimsInsignia : ModItem
    {
        /// <summary>常驻近战伤害加成。</summary>
        public const float MeleeDamageBonus = 0.15f;
        /// <summary>常驻近战暴击加成，单位为百分点。</summary>
        public const float MeleeCritBonus = 10f;
        /// <summary>常驻近战攻击速度加成。</summary>
        public const float MeleeSpeedBonus = 0.15f;
        /// <summary>生命低于阈值时追加的通用伤害加成。</summary>
        public const float LowLifeDamageBonus = 0.10f;
        /// <summary>低生命增伤的生命比例阈值。</summary>
        public const float LowLifeThreshold = 0.50f;
        /// <summary>岩浆免疫额外持续帧数。</summary>
        public const int LavaImmunityFrames = 240;
        /// <summary>圣火减益的短持续时间。</summary>
        public const int HolyShortDuration = 120;
        /// <summary>圣火减益的中等持续时间。</summary>
        public const int HolyMediumDuration = 240;
        /// <summary>圣火减益的长时间持续时间。</summary>
        public const int HolyLongDuration = 360;
        /// <summary>经典版本证章配方消耗的原生余烬核心数量。</summary>
        private const int NativeCoreCount = 5;
        /// <summary>CalRD 证章配方消耗的神性晶核数量。</summary>
        private const int DivineGeodeCount = 5;
        /// <summary>
        /// 使用 22×38 判定尺寸、30 金价格与青绿色稀有度。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 38;
            Item.accessory = true;
            ModernRarityValueUtil.Apply(
                Item,
                ModContent.RarityType<TurquoiseRarity>(),
                Item.buyPrice(0, 30, 0, 0));
        }
        /// <summary>
        /// 只点亮本帧装备状态；面板、低生命增伤和生存属性由玩家结算入口统一处理。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().yharimsInsignia = true;
        }
        /// <summary>注册现代展开配方和两个经典灾厄的原生材料配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代 2.2.2 已删除原生余烬核心，因此把日耀精华与灵气直接放进证章配方。
        /// 其余四件旧近战链材料保持不变。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("EssenceofSunlight", out ModItem sunlightEssence))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient<NecklaceofVexation>()
                .AddIngredient(sunlightEssence.Type)
                .AddIngredient(ItemID.Ectoplasm)
                .AddIngredient(ItemID.CrossNecklace)
                .AddIngredient<BadgeofBravery>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 保留原五件链路，并直接使用该版本自带的原生余烬核心。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod calamity)
                || !calamity.TryFind<ModItem>("CoreofCinder", out ModItem coreOfCinder))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient<NecklaceofVexation>()
                .AddIngredient(coreOfCinder.Type, NativeCoreCount)
                .AddIngredient(ItemID.CrossNecklace)
                .AddIngredient<BadgeofBravery>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 的原配方使用原生余烬核心与神性晶核，不消耗勇气勋章。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calamity)
                || !calamity.TryFind<ModItem>("CoreofCinder", out ModItem coreOfCinder)
                || !calamity.TryFind<ModItem>("DivineGeode", out ModItem divineGeode))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.WarriorEmblem)
                .AddIngredient<NecklaceofVexation>()
                .AddIngredient(coreOfCinder.Type, NativeCoreCount)
                .AddIngredient(divineGeode.Type, DivineGeodeCount)
                .AddIngredient(ItemID.CrossNecklace)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
