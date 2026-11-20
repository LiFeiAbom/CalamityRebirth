using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 烦恼项链：元素之握近战饰品链中的低生命增伤饰品。
    /// 采用现代灾厄 2.2.2 的连续生命比例增伤与毒液命中机制，并使用同版本贴图。
    /// </summary>
    internal sealed class NecklaceofVexation : ModItem
    {
        /// <summary>生命值降至零时达到的最大通用伤害加成。</summary>
        public const float MaximumDamageBonus = 0.30f;
        /// <summary>所有 NPC 命中附加的毒液持续时间，单位为帧。</summary>
        public const int VenomDuration = 120;
        /// <summary>现代 2.2.2 配方需要的永续锭数量。</summary>
        private const int ModernPerennialBarCount = 10;
        /// <summary>两个经典灾厄分支配方需要的 DraedonBar 数量。</summary>
        private const int ClassicDraedonBarCount = 2;
        /// <summary>
        /// 设置与现代 2.2.2 一致的 26×34 判定尺寸、柠檬色稀有度和标准购买价。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.accessory = true;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.Lime);
        }
        /// <summary>
        /// 点亮本帧烦恼项链状态。
        /// 动态伤害在玩家结算阶段统一添加，NPC 命中附加毒液由本工程的物品与弹幕全局钩子处理。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().vexation = true;
        }
        /// <summary>
        /// 分别注册现代 2.2.2、经典 1.4.2.101 与 CalRD 的 ReferenceMod 原配方。
        /// </summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代 2.2.2 原配方：复仇者徽章 + 永续锭 ×10，于秘银砧制作。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod modern)
                || !modern.TryFind<ModItem>("PerennialBar", out ModItem perennialBar))
                return;
            CreateRecipe()
                .AddIngredient(perennialBar.Type, ModernPerennialBarCount)
                .AddIngredient(ItemID.AvengerEmblem)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 原配方：DraedonBar ×2 + 复仇者徽章，于秘银砧制作。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("DraedonBar", out ModItem draedonBar))
                return;
            CreateRecipe()
                .AddIngredient(draedonBar.Type, ClassicDraedonBarCount)
                .AddIngredient(ItemID.AvengerEmblem)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 原配方：DraedonBar ×2 + 复仇者徽章，于秘银砧制作。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("DraedonBar", out ModItem draedonBar))
                return;
            CreateRecipe()
                .AddIngredient(draedonBar.Type, ClassicDraedonBarCount)
                .AddIngredient(ItemID.AvengerEmblem)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
