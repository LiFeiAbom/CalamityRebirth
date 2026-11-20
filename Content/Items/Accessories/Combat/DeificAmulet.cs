using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 神圣护符：神之壁垒链条的护甲穿透与反击支流。
    /// 融合恐慌项链、魔力磁铁、魔法手铐、水母项链与星光面纱的关键能力。
    /// </summary>
    internal sealed class DeificAmulet : ModItem
    {
        /// <summary>每份灾厄星辰材料的需求量。</summary>
        private const int CalamityMaterialCount = 25;
        /// <summary>原版陨石锭需求量。</summary>
        private const int MeteoriteBarCount = 25;
        /// <summary>
        /// 设置 26×26 判定尺寸、四十五金价格、现代青色稀有度与饰品类型。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.accessory = true;
        }
        /// <summary>
        /// 只置位本帧装备状态；二十五点穿甲、受击星辰与延长无敌由玩家结算类处理。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().deificAmulet = true;
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 三条配方。
        /// </summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代分支使用五件原版饰品、星辰耀斑与陨石锭配方。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("StarblightSoot", out ModItem starblightSoot))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.CelestialCuffs)
                .AddIngredient(ItemID.JellyfishNecklace)
                .AddIngredient(ItemID.PanicNecklace)
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddIngredient(ItemID.StarVeil)
                .AddIngredient(starblightSoot.Type, CalamityMaterialCount)
                .AddIngredient(ItemID.MeteoriteBar, MeteoriteBarCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 使用其原生 Stardust 与天界手铐。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("Stardust", out ModItem stardust))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.CelestialCuffs)
                .AddIngredient(ItemID.JellyfishNecklace)
                .AddIngredient(ItemID.PanicNecklace)
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddIngredient(ItemID.StarVeil)
                .AddIngredient(stardust.Type, CalamityMaterialCount)
                .AddIngredient(ItemID.MeteoriteBar, MeteoriteBarCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 的原配方以神话护身符替代天界手铐，其余原版组件保持不变。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("Stardust", out ModItem stardust))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.CharmofMyths)
                .AddIngredient(ItemID.JellyfishNecklace)
                .AddIngredient(ItemID.PanicNecklace)
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddIngredient(ItemID.StarVeil)
                .AddIngredient(stardust.Type, CalamityMaterialCount)
                .AddIngredient(ItemID.MeteoriteBar, MeteoriteBarCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
