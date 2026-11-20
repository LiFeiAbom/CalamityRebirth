using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
    /// <summary>海之盾：提供常驻两点防御与浸水时额外五点防御。</summary>
    internal sealed class ShieldoftheOcean : ModItem
    {
        /// <summary>设置判定尺寸、三金价格、绿色稀有度与两点防御。</summary>
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 2;
            Item.accessory = true;
        }
        /// <summary>置位浸水防御读取标记。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().shieldOfTheOcean = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 的海洋材料配方。</summary>
        public override void AddRecipes()
        {
            AddRecipe("CalamityMod", "SeaRemains");
            AddRecipe("CalamityModClassicPreTrailer", "VictideBar");
            AddRecipe("CalRD", "VictideBar");
        }
        /// <summary>使用指定分支的海洋材料五枚与珊瑚五枚在铁砧制作。</summary>
        private void AddRecipe(string modName, string materialName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>(materialName, out ModItem material))
                return;
            CreateRecipe()
                .AddIngredient(material.Type, 5)
                .AddIngredient(ItemID.Coral, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
