using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Content.Items.Accessories.Construction;
using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.HealthandMana
{
    /// <summary>百草天香瓶：提供减伤、挖掘、再生、毒素与冰霜免疫以及蜂类减伤。</summary>
    internal sealed class AmbrosialAmpoule : ModItem
    {
        /// <summary>设置 20×20 判定、六点防御、四十五金价格、红色稀有度与饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.defense = 6;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态与蜂类减伤。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.ambrosialAmpoule = true;
            modPlayer.beeResist = true;
            if (!hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1f, 1f, 0.6f));
        }
        /// <summary>注册现代、经典与 CalRD 的双邪恶路线配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipes();
            AddClassicRecipes("CalamityModClassicPreTrailer");
            AddClassicRecipes("CalRD");
        }
        /// <summary>现代分支使用星辰耀斑与寒元锭。</summary>
        private void AddModernRecipes()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod modern)
                || !modern.TryFind<ModItem>("StarblightSoot", out ModItem soot)
                || !modern.TryFind<ModItem>("CryonicBar", out ModItem bar))
                return;
            CreateAmbrosialRecipe<CorruptFlask>(soot.Type, bar.Type);
            CreateAmbrosialRecipe<CrimsonFlask>(soot.Type, bar.Type);
        }
        /// <summary>经典分支使用星尘与寒元锭。</summary>
        private void AddClassicRecipes(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("Stardust", out ModItem stardust)
                || !mod.TryFind<ModItem>("CryoBar", out ModItem bar))
                return;
            CreateAmbrosialRecipe<CorruptFlask>(stardust.Type, bar.Type);
            CreateAmbrosialRecipe<CrimsonFlask>(stardust.Type, bar.Type);
        }
        /// <summary>为指定邪恶瓶生成共用材料数量的配方。</summary>
        private void CreateAmbrosialRecipe<TFlask>(int dustType, int barType) where TFlask : ModItem
        {
            CreateRecipe()
                .AddIngredient<TFlask>()
                .AddIngredient<ArchaicPowder>()
                .AddIngredient<RadiantOoze>()
                .AddIngredient<HoneyDew>()
                .AddIngredient(dustType, 15)
                .AddIngredient(barType, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
