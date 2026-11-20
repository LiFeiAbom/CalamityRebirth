using CalamityRebirth.Content.Items.Accessories.Combat.Magic;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Items.Accessories.Movement;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>绚彩凝胶：融合三种凝胶的移动、跳跃、生命、魔力和静止再生。</summary>
    internal sealed class GrandGelatin : ModItem
    {
        /// <summary>设置 20×24 判定、十五金价格、粉色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().grandGelatin = true;
        }
        /// <summary>任一分支加载时注册三种本地凝胶配方。</summary>
        public override void AddRecipes()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out _)
                && !ModLoader.TryGetMod("CalamityModClassicPreTrailer", out _)
                && !ModLoader.TryGetMod("CalRD", out _))
                return;
            CreateRecipe()
                .AddIngredient<ManaJelly>()
                .AddIngredient<LifeJelly>()
                .AddIngredient<VitalJelly>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
