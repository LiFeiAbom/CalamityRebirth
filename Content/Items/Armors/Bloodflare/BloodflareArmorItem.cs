using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>
    /// 血炎防具公共基类，统一尺寸、纯绿稀有度和三版本配方。
    /// </summary>
    internal abstract class BloodflareArmorItem : ModItem
    {
        /// <summary>设置 18×18 判定尺寸、防御和显式购买价。</summary>
        protected void SetArmorDefaults(int defense, int goldPrice)
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = defense;
            RarityUtil.Apply(Item, ModContent.RarityType<PureGreenRarity>()
                , Item.buyPrice(0, goldPrice, 0, 0));
        }
        /// <summary>应用血炎职业头共有的岩浆与液体移动效果。</summary>
        protected static void ApplyHeadProtection(Player player)
        {
            player.lavaMax += 240;
            player.ignoreWater = true;
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 三条配方。
        /// 现代使用 Bloodstone/BloodOrb/RuinousSoul 与秘银砧；
        /// 两款经典版使用 BloodstoneCore/RuinousSoul 与远古操纵机。
        /// </summary>
        protected void AddBloodflareRecipes(int modernBloodstoneCount, int modernBloodOrbCount
            , int classicBloodstoneCount, int soulCount)
        {
            AddModernRecipe(modernBloodstoneCount, modernBloodOrbCount, soulCount);
            AddClassicRecipe("CalamityModClassicPreTrailer", classicBloodstoneCount, soulCount);
            AddClassicRecipe("CalRD", classicBloodstoneCount, soulCount);
        }
        /// <summary>注册现代配方。</summary>
        private void AddModernRecipe(int bloodstoneCount, int bloodOrbCount, int soulCount)
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod mod)
                || !mod.TryFind<ModItem>("Bloodstone", out ModItem bloodstone)
                || !mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb)
                || !mod.TryFind<ModItem>("RuinousSoul", out ModItem soul))
                return;
            CreateRecipe()
                .AddIngredient(bloodstone.Type, bloodstoneCount)
                .AddIngredient(bloodOrb.Type, bloodOrbCount)
                .AddIngredient(soul.Type, soulCount)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>注册指定经典分支的配方。</summary>
        private void AddClassicRecipe(string modName, int bloodstoneCount, int soulCount)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("BloodstoneCore", out ModItem bloodstoneCore)
                || !mod.TryFind<ModItem>("RuinousSoul", out ModItem soul))
                return;
            CreateRecipe()
                .AddIngredient(bloodstoneCore.Type, bloodstoneCount)
                .AddIngredient(soul.Type, soulCount)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
