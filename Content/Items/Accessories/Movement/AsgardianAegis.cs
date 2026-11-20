using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Enums;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
    /// <summary>
    /// 阿斯加德之庇护：盾牌冲刺链顶点，提供宇宙冲撞、高额防御与广泛减益免疫。
    /// </summary>
    [AutoloadEquip(EquipType.Shield)]
    internal sealed class AsgardianAegis : ModItem
    {
        /// <summary>设置判定尺寸、九十金价格、宇宙暗紫稀有度与二十八点防御。</summary>
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 54;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.defense = 28;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<CosmicPurpleRarity>();
        }
        /// <summary>置位本帧装备状态并注册最高优先级的盾牌冲刺。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.asgardianAegis = true;
            modPlayer.ProvideShieldSlamDash(ShieldSlamDash.AsgardianAegis);
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 三条顶点配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代顶点配方使用本工程极乐之庇护、宇宙锭与飞升星魂。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !calamity.TryFind<ModItem>("AscendantSpiritEssence", out ModItem spiritEssence)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient<AsgardsValor>()
                .AddIngredient<ElysianAegis>()
                .AddIngredient(cosmiliteBar.Type, 10)
                .AddIngredient(spiritEssence.Type, 4)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>经典 1.4.2.101 顶点配方使用等价飞升材料与嘉登熔炉。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            Recipe recipe = CreateRecipe()
                .AddIngredient<AsgardsValor>()
                .AddIngredient<ElysianAegis>()
                .AddIngredient(cosmiliteBar.Type, 5);
            if (!CRUtil.TryAddClassicAscendantSpiritEssence(recipe, 4))
                return;
            recipe.AddTile(draedonsForge.Type).Register();
        }
        /// <summary>CalRD 顶点配方使用本工程极乐之庇护、宇宙锭与嘉登熔炉。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<AsgardsValor>()
                .AddIngredient<ElysianAegis>()
                .AddIngredient(cosmiliteBar.Type, 5)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
