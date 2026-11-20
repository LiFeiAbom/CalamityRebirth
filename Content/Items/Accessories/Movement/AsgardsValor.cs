using CalamityRebirth.Enums;
using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
    /// <summary>
    /// 阿斯加德之英勇：提供基础盾牌冲撞、高额生存属性与多类减益免疫。
    /// </summary>
    [AutoloadEquip(EquipType.Shield)]
    internal sealed class AsgardsValor : ModItem
    {
        /// <summary>设置判定尺寸、四十五金价格、现代黄色稀有度与十六点防御。</summary>
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 16;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态并注册基础盾牌冲刺。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.asgardsValor = true;
            modPlayer.ProvideShieldSlamDash(ShieldSlamDash.AsgardsValor);
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 三条配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代配方使用本工程前置、灾厄核心与生命果。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("CoreofCalamity", out ModItem coreOfCalamity))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.AnkhShield)
                .AddIngredient<OrnateShield>()
                .AddIngredient<ShieldoftheOcean>()
                .AddIngredient<Abaddon>()
                .AddIngredient(coreOfCalamity.Type, 3)
                .AddIngredient(ItemID.LifeFruit, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>经典 1.4.2.101 配方使用三种原生核心各三枚。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CoreofEleum", out ModItem coreOfEleum)
                || !classic.TryFind<ModItem>("CoreofCinder", out ModItem coreOfCinder)
                || !classic.TryFind<ModItem>("CoreofChaos", out ModItem coreOfChaos))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.AnkhShield)
                .AddIngredient<OrnateShield>()
                .AddIngredient<ShieldoftheOcean>()
                .AddIngredient<Abaddon>()
                .AddIngredient(coreOfEleum.Type, 3)
                .AddIngredient(coreOfCinder.Type, 3)
                .AddIngredient(coreOfChaos.Type, 3)
                .AddIngredient(ItemID.LifeFruit, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>CalRD 配方使用本工程前置与一枚灾厄核心。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("CoreofCalamity", out ModItem coreOfCalamity))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.AnkhShield)
                .AddIngredient<OrnateShield>()
                .AddIngredient<ShieldoftheOcean>()
                .AddIngredient<Abaddon>()
                .AddIngredient(coreOfCalamity.Type)
                .AddIngredient(ItemID.LifeFruit, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
