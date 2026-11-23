using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.Combat.Ranged;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.AuricTesla
{
    /// <summary>古圣金源防具公共基类，统一面板与三版本升阶配方。</summary>
    internal abstract class AuricTeslaArmorItem : ModItem
    {
        /// <summary>设置 18×18 判定、防御、金币价格和灼辉金源稀有度。</summary>
        protected void SetArmorDefaults(int defense, int goldPrice)
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = defense;
            RarityUtil.Apply(Item, ModContent.RarityType<BurnishedAuricRarity>()
                , Item.buyPrice(0, goldPrice, 0, 0));
        }
        /// <summary>应用五职业共用的古圣金源套装防御、环境与岩浆属性。</summary>
        protected static void ApplyCommonSetBonuses(Player player, bool increasedAggro)
        {
            player.thorns += 3f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.crimsonRegen = true;
            if (increasedAggro)
                player.aggro += 1200;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }
        /// <summary>注册五职业头部升阶配方。</summary>
        protected void AddHeadRecipes<TTarragon, TBloodflare, TSilva, TGodSlayer>()
            where TTarragon : ModItem
            where TBloodflare : ModItem
            where TSilva : ModItem
            where TGodSlayer : ModItem
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AuricBar", out ModItem modernBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient(modernBar.Type, 10)
                    .AddIngredient<PsychoticAmulet>()
                    .AddTile(cosmicAnvil.Type)
                    .Register();
            }
            AddClassicHeadRecipe<TTarragon, TBloodflare, TSilva, TGodSlayer>();
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("AuricBar", out ModItem calRDBar)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDForge))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient(calRDBar.Type, 10)
                    .AddIngredient<PsychoticAmulet>()
                    .AddTile(calRDForge.Type)
                    .Register();
            }
        }
        /// <summary>注册经典 1.4.2.101 的五职业头部升阶配方。</summary>
        private void AddClassicHeadRecipe<TTarragon, TBloodflare, TSilva, TGodSlayer>()
            where TTarragon : ModItem
            where TBloodflare : ModItem
            where TSilva : ModItem
            where TGodSlayer : ModItem
        {
            if (!TryGetClassicMaterials(out ClassicAuricMaterials materials, out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient<TTarragon>()
                .AddIngredient<TBloodflare>()
                .AddIngredient<TSilva>()
                .AddIngredient<TGodSlayer>()
                .AddIngredient(materials.AuricOre.Type, 60)
                .AddIngredient(materials.EndothermicEnergy.Type, 10)
                .AddIngredient(materials.NightmareFuel.Type, 10)
                .AddIngredient(materials.Phantoplasm.Type, 8)
                .AddIngredient(materials.DarksunFragment.Type, 6)
                .AddIngredient(materials.BarofLife.Type, 5)
                .AddIngredient(materials.HellcasterFragment.Type, 5)
                .AddIngredient(materials.CoreofCalamity.Type, 2)
                .AddIngredient(materials.GalacticaSingularity.Type)
                .AddIngredient<PsychoticAmulet>()
                .AddTile(forge.Type)
                .Register();
        }
        /// <summary>注册四套胸甲汇合的古圣金源胸甲配方。</summary>
        protected void AddBodyRecipes<TTarragon, TBloodflare, TSilva, TGodSlayer>()
            where TTarragon : ModItem
            where TBloodflare : ModItem
            where TSilva : ModItem
            where TGodSlayer : ModItem
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AuricBar", out ModItem modernBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient<FrostBarrier>()
                    .AddIngredient(modernBar.Type, 20)
                    .AddTile(cosmicAnvil.Type)
                    .Register();
            }
            if (TryGetClassicMaterials(out ClassicAuricMaterials materials, out ModTile forge))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient(materials.AuricOre.Type, 100)
                    .AddIngredient(materials.EndothermicEnergy.Type, 30)
                    .AddIngredient(materials.NightmareFuel.Type, 30)
                    .AddIngredient(materials.Phantoplasm.Type, 20)
                    .AddIngredient(materials.DarksunFragment.Type, 15)
                    .AddIngredient(materials.BarofLife.Type, 10)
                    .AddIngredient(materials.HellcasterFragment.Type, 7)
                    .AddIngredient(materials.CoreofCalamity.Type, 5)
                    .AddIngredient(materials.GalacticaSingularity.Type, 3)
                    .AddIngredient<FrostBarrier>()
                    .AddTile(forge.Type)
                    .Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("AuricBar", out ModItem calRDBar)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDForge))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient<FrostBarrier>()
                    .AddIngredient(calRDBar.Type, 18)
                    .AddTile(calRDForge.Type)
                    .Register();
            }
        }
        /// <summary>注册四套护腿汇合的古圣金源腿甲配方。</summary>
        protected void AddLegRecipes<TTarragon, TBloodflare, TSilva, TGodSlayer>()
            where TTarragon : ModItem
            where TBloodflare : ModItem
            where TSilva : ModItem
            where TGodSlayer : ModItem
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("AuricBar", out ModItem modernBar)
                && modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient(ItemID.FlyingCarpet)
                    .AddIngredient(modernBar.Type, 15)
                    .AddTile(cosmicAnvil.Type)
                    .Register();
            }
            if (TryGetClassicMaterials(out ClassicAuricMaterials materials, out ModTile forge))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient(materials.AuricOre.Type, 80)
                    .AddIngredient(materials.EndothermicEnergy.Type, 20)
                    .AddIngredient(materials.NightmareFuel.Type, 20)
                    .AddIngredient(materials.Phantoplasm.Type, 15)
                    .AddIngredient(materials.DarksunFragment.Type, 10)
                    .AddIngredient(materials.BarofLife.Type, 8)
                    .AddIngredient(materials.HellcasterFragment.Type, 6)
                    .AddIngredient(materials.CoreofCalamity.Type, 3)
                    .AddIngredient(materials.GalacticaSingularity.Type, 2)
                    .AddIngredient(ItemID.FlyingCarpet)
                    .AddTile(forge.Type)
                    .Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("AuricBar", out ModItem calRDBar)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDForge))
            {
                CreateRecipe()
                    .AddIngredient<TTarragon>()
                    .AddIngredient<TBloodflare>()
                    .AddIngredient<TSilva>()
                    .AddIngredient<TGodSlayer>()
                    .AddIngredient(ItemID.FlyingCarpet)
                    .AddIngredient(calRDBar.Type, 14)
                    .AddTile(calRDForge.Type)
                    .Register();
            }
        }
        /// <summary>弱依赖读取经典 1.4.2.101 的整套后期材料。</summary>
        private static bool TryGetClassicMaterials(out ClassicAuricMaterials materials, out ModTile forge)
        {
            materials = default;
            forge = null;
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("AuricOre", out materials.AuricOre)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out materials.EndothermicEnergy)
                || !classic.TryFind<ModItem>("NightmareFuel", out materials.NightmareFuel)
                || !classic.TryFind<ModItem>("Phantoplasm", out materials.Phantoplasm)
                || !classic.TryFind<ModItem>("DarksunFragment", out materials.DarksunFragment)
                || !classic.TryFind<ModItem>("BarofLife", out materials.BarofLife)
                || !classic.TryFind<ModItem>("HellcasterFragment", out materials.HellcasterFragment)
                || !classic.TryFind<ModItem>("CoreofCalamity", out materials.CoreofCalamity)
                || !classic.TryFind<ModItem>("GalacticaSingularity", out materials.GalacticaSingularity)
                || !classic.TryFind<ModTile>("DraedonsForge", out forge))
                return false;
            return true;
        }
        /// <summary>经典配方所需的九个灾厄材料引用。</summary>
        private struct ClassicAuricMaterials
        {
            internal ModItem AuricOre;
            internal ModItem EndothermicEnergy;
            internal ModItem NightmareFuel;
            internal ModItem Phantoplasm;
            internal ModItem DarksunFragment;
            internal ModItem BarofLife;
            internal ModItem HellcasterFragment;
            internal ModItem CoreofCalamity;
            internal ModItem GalacticaSingularity;
        }
    }
}
