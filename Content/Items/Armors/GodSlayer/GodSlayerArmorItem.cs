using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.GodSlayer
{
    /// <summary>弑神者防具公共基类，统一尺寸、稀有度、价格和三版本配方。</summary>
    internal abstract class GodSlayerArmorItem : ModItem
    {
        /// <summary>设置 18×18 判定尺寸、防御、金币价格和现代 Cosmic Purple 稀有度。</summary>
        protected void SetArmorDefaults(int defense, int goldPrice)
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = defense;
            RarityUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>()
                , Item.buyPrice(0, goldPrice, 0, 0));
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 配方。
        /// 头部现代需要 10 宇宙锭，两款经典需要 14；身体与腿部同理使用各自数量。
        /// </summary>
        protected void AddGodSlayerRecipes(int modernBarCount, int modernEssenceCount
            , int classicBarCount, int nightmareFuelCount, int endothermicEnergyCount
            , int calRDEssenceCount)
        {
            AddModernRecipe(modernBarCount, modernEssenceCount);
            AddClassic142Recipe(classicBarCount, nightmareFuelCount, endothermicEnergyCount);
            AddCalRDRecipe(classicBarCount, calRDEssenceCount);
        }
        /// <summary>注册现代灾厄宇宙锭、至尊精神与宇宙砧配方。</summary>
        private void AddModernRecipe(int barCount, int essenceCount)
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("CosmiliteBar", out ModItem bar)
                || !calamity.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile anvil))
                return;
            CreateRecipe()
                .AddIngredient(bar.Type, barCount)
                .AddIngredient(essence.Type, essenceCount)
                .AddTile(anvil.Type)
                .Register();
        }
        /// <summary>注册经典 1.4.2.101 宇宙锭、梦魇燃料与吸热能量配方。</summary>
        private void AddClassic142Recipe(int barCount, int nightmareFuelCount, int endothermicEnergyCount)
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CosmiliteBar", out ModItem bar)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient(bar.Type, barCount)
                .AddIngredient(nightmareFuel.Type, nightmareFuelCount)
                .AddIngredient(endothermicEnergy.Type, endothermicEnergyCount)
                .AddTile(forge.Type)
                .Register();
        }
        /// <summary>注册 CalRD 宇宙锭、至尊精神与嘉登熔炉配方。</summary>
        private void AddCalRDRecipe(int barCount, int essenceCount)
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("CosmiliteBar", out ModItem bar)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient(bar.Type, barCount)
                .AddIngredient(essence.Type, essenceCount)
                .AddTile(forge.Type)
                .Register();
        }
    }
}
