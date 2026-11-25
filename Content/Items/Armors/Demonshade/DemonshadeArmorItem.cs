using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Demonshade
{
    /// <summary>魔影三件套公共默认值与三版本魔影锭配方。</summary>
    internal abstract class DemonshadeArmorItem : ModItem
    {
        /// <summary>设置十八像素判定、五十防御、指定价格与品红稀有度。</summary>
        protected void SetArmorDefaults(int goldPrice)
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 50;
            RarityUtil.Apply(Item, ModContent.RarityType<DemonshadeRarity>()
                , Item.buyPrice(0, goldPrice, 0, 0));
        }
        /// <summary>为三个已加载的灾厄分支分别注册魔影锭与嘉登熔炉配方。</summary>
        protected void AddShadowspecRecipe(int barCount)
        {
            AddShadowspecRecipe("CalamityMod", barCount);
            AddShadowspecRecipe("CalamityModClassicPreTrailer", barCount);
            AddShadowspecRecipe("CalRD", barCount);
        }
        /// <summary>弱依赖查找指定分支的魔影锭和嘉登熔炉，缺件时只跳过该分支。</summary>
        private void AddShadowspecRecipe(string modName, int barCount)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("ShadowspecBar", out ModItem shadowspecBar)
                || !mod.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient(shadowspecBar.Type, barCount)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
