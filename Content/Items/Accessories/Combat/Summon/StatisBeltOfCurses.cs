using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
    /// <summary>
    /// 斯塔提斯诅咒腰带：召唤与盗贼复合最终饰品。
    /// 提供高阶召唤与盗贼面板、鞭子强化、移动能力和召唤命中暗影焰。
    /// </summary>
    internal sealed class StatisBeltOfCurses : ModItem
    {
        /// <summary>使用 28×32 判定尺寸、90 金价值与月后紫色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.rare = ModContent.RarityType<PostMoonLordPurpleRarity>();
            Item.accessory = true;
        }
        /// <summary>装备时置位最终档状态，并立即赋予不依赖职业系统的移动能力。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().statisBeltOfCurses = true;
            player.autoJump = true;
            player.jumpSpeedBoost += 1.2f;
            player.extraFall += 50;
            player.blackBelt = true;
            player.dash = 1;
            player.dashType = 1;
            player.spikedBoots = 2;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 的诅咒腰带配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代使用本工程诅咒与原生忍者腰带，加三种高阶材料各 20，在星宇砧制作。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("StatisNinjaBelt", out ModItem ninjaBelt)
                || !calamity.TryFind<ModItem>("Necroplasm", out ModItem necroplasm)
                || !calamity.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !calamity.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient<StatisCurse>()
                .AddIngredient(ninjaBelt.Type)
                .AddIngredient(necroplasm.Type, 20)
                .AddIngredient(nightmareFuel.Type, 20)
                .AddIngredient(endothermicEnergy.Type, 20)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>经典 1.4.2.101 使用原生忍者腰带与三种高阶材料各 20，在嘉登熔炉制作。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("StatisNinjaBelt", out ModItem ninjaBelt)
                || !classic.TryFind<ModItem>("Phantoplasm", out ModItem phantoplasm)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<StatisCurse>()
                .AddIngredient(ninjaBelt.Type)
                .AddIngredient(phantoplasm.Type, 20)
                .AddIngredient(nightmareFuel.Type, 20)
                .AddIngredient(endothermicEnergy.Type, 20)
                .AddTile(draedonsForge.Type)
                .Register();
        }
        /// <summary>CalRD 使用原生忍者腰带与化神魂精×4，在嘉登熔炉保留召唤链路。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("StatisNinjaBelt", out ModItem ninjaBelt)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<StatisCurse>()
                .AddIngredient(ninjaBelt.Type)
                .AddIngredient(ascendantSpiritEssence.Type, 4)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
