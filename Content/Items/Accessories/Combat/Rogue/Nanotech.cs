using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>纳米技术：独立的顶级盗贼饰品，与蚀日魔镜不互相合成。</summary>
    internal sealed class Nanotech : ModItem
    {
        /// <summary>用户指定使用弑神者紫色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.rare = ModContent.RarityType<CosmicPurpleRarity>();
            Item.accessory = true;
        }
        /// <summary>装备时置位纳米技术与命中特效状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().nanotech = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 的纳米技术配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代配方在星宇砧使用火星管道与纳米机器人。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("Necroplasm", out ModItem necroplasm)
                || !calamity.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !calamity.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.MartianConduitPlating, 250)
                .AddIngredient(ItemID.Nanites, 500)
                .AddIngredient(necroplasm.Type, 20)
                .AddIngredient(nightmareFuel.Type, 20)
                .AddIngredient(endothermicEnergy.Type, 20)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>1.4.2.101 使用幻影质、梦魇燃料与吸热能量各20，在嘉登熔炉制作。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("Phantoplasm", out ModItem phantoplasm)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.MartianConduitPlating, 250)
                .AddIngredient(ItemID.Nanites, 500)
                .AddIngredient(phantoplasm.Type, 20)
                .AddIngredient(nightmareFuel.Type, 20)
                .AddIngredient(endothermicEnergy.Type, 20)
                .AddTile(draedonsForge.Type)
                .Register();
        }
        /// <summary>CalRD 使用其原生盗贼饰品融合链，最终在嘉登熔炉制作。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("RogueEmblem", out ModItem rogueEmblem)
                || !calRD.TryFind<ModItem>("RaidersTalisman", out ModItem raidersTalisman)
                || !calRD.TryFind<ModItem>("MoonstoneCrown", out ModItem moonstoneCrown)
                || !calRD.TryFind<ModItem>("ElectriciansGlove", out ModItem electriciansGlove)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendant)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient(rogueEmblem.Type)
                .AddIngredient(raidersTalisman.Type)
                .AddIngredient(moonstoneCrown.Type)
                .AddIngredient(electriciansGlove.Type)
                .AddIngredient(ascendant.Type, 4)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
