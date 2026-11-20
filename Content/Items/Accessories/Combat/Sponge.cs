using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>涡流虹吸石：吸收、再生、荆棘、免疫、挖掘与受击反击的终局综合饰品。</summary>
    internal sealed class Sponge : ModItem
    {
        /// <summary>设置 20×20 判定、二十点防御、九十金价格、月后深蓝稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.defense = 20;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.rare = ModContent.RarityType<PostMoonLordBlueRarity>();
            Item.accessory = true;
        }
        /// <summary>置位吸收与蜂类减伤状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.sponge = true;
            modPlayer.beeResist = true;
        }
        /// <summary>注册三个版本顶点配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("CosmiliteBar", out ModItem modernBar)
                && modern.TryFind<ModItem>("Necroplasm", out ModItem modernNecroplasm)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile modernAnvil))
                CreateRecipe().AddIngredient<TheAbsorber>().AddIngredient<AmbrosialAmpoule>()
                    .AddIngredient(modernBar.Type, 15).AddIngredient(modernNecroplasm.Type, 15)
                    .AddTile(modernAnvil.Type).Register();
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicBar)
                && classic.TryFind<ModItem>("Phantoplasm", out ModItem classicPhantoplasm)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile classicForge))
                CreateRecipe().AddIngredient<TheAbsorber>().AddIngredient<AmbrosialAmpoule>()
                    .AddIngredient(classicBar.Type, 15).AddIngredient(classicPhantoplasm.Type, 15)
                    .AddTile(classicForge.Type).Register();
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("CosmiliteBar", out ModItem calRDBar)
                && calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem calRDEssence)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDForge))
                CreateRecipe().AddIngredient<TheAbsorber>().AddIngredient<AmbrosialAmpoule>()
                    .AddIngredient(calRDBar.Type, 5).AddIngredient(calRDEssence.Type)
                    .AddTile(calRDForge.Type).Register();
        }
    }
}
