using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 寒霜壁垒：神之壁垒链条的防御支流，由圣骑士护盾与冰冻海龟壳合成。
    /// 提供八点防御、击退免疫、队伍伤害分担、低生命冰屏障与残血额外减伤。
    /// </summary>
    internal sealed class FrigidBulwark : ModItem
    {
        /// <summary>现代灾厄已删除 CoreofEleum 时，单次合成所需的核心数。</summary>
        private const int CoreOfEleumCount = 5;
        /// <summary>三枚极寒核心消耗一份极寒精华与一份灵气。</summary>
        private const int CoreCraftYield = 3;
        /// <summary>
        /// 设置判定尺寸、八点防御、三十金价格、青色稀有度与饰品类型。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 8;
            Item.accessory = true;
        }
        /// <summary>
        /// 只置位本帧装备状态；防御与低生命效果由玩家结算类统一处理。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().frigidBulwark = true;
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 三条原生物品分支。
        /// </summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代 2.2.2 已删除 CoreofEleum，因此把五个核心按三枚一组的原配方向上取整展开。
        /// 最终使用两份极寒精华与两份灵气，对应最多六枚核心，避免因整数配方少给材料。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("EssenceofEleum", out ModItem essenceOfEleum))
                return;
            int conversionCount = (CoreOfEleumCount + CoreCraftYield - 1) / CoreCraftYield;
            CreateRecipe()
                .AddIngredient(ItemID.PaladinsShield)
                .AddIngredient(ItemID.FrozenTurtleShell)
                .AddIngredient(essenceOfEleum.Type, conversionCount)
                .AddIngredient(ItemID.Ectoplasm, conversionCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 保留原生 CoreofEleum 中间件，不展开为其原料。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CoreofEleum", out ModItem coreOfEleum))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.PaladinsShield)
                .AddIngredient(ItemID.FrozenTurtleShell)
                .AddIngredient(coreOfEleum.Type, CoreOfEleumCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 同样保留原生 CoreofEleum，并在秘银砧制作。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("CoreofEleum", out ModItem coreOfEleum))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.PaladinsShield)
                .AddIngredient(ItemID.FrozenTurtleShell)
                .AddIngredient(coreOfEleum.Type, CoreOfEleumCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
