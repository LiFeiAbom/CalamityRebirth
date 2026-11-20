using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Construction
{
    /// <summary>
    /// 混沌护符：空灵护符链条中的功能型中间饰品。
    /// 提供 1 HP/s 生命再生、探宝以及少量魔法面板属性。
    /// </summary>
    internal sealed class ChaosAmulet : ModItem
    {
        /// <summary>魔法伤害加成比例。</summary>
        public const float MagicDamageBonus = 0.02f;
        /// <summary>魔法暴击率加成百分点。</summary>
        public const float MagicCritBonus = 2f;
        /// <summary>最大魔力加成。</summary>
        public const int MaxManaBonus = 20;
        /// <summary>
        /// 使用 20×24 判定尺寸与 15 金价值。
        /// 稀有度按现代后继 Spelunkers Amulet 修正为浅红色。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.lifeRegen = 2;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }
        /// <summary>
        /// 装备时开启探宝并增加魔法面板；生命再生由 Item.lifeRegen 自动结算。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.findTreasure = true;
            player.GetDamage(DamageClass.Magic) += MagicDamageBonus;
            player.GetCritChance(DamageClass.Magic) += MagicCritBonus;
            player.statManaMax2 += MaxManaBonus;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 的混沌护符配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代使用狱石锭×2、灾厄之烬×2 与洞穴探险药水×7，在秘银砧制作。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("AshesofCalamity", out ModItem ashesOfCalamity))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 2)
                .AddIngredient(ashesOfCalamity.Type, 2)
                .AddIngredient(ItemID.SpelunkerPotion, 7)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>经典 1.4.2.101 使用 Cruptix 锭×2 与洞穴探险药水×7，在秘银砧制作。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CruptixBar", out ModItem cruptixBar))
                return;
            CreateRecipe()
                .AddIngredient(cruptixBar.Type, 2)
                .AddIngredient(ItemID.SpelunkerPotion, 7)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>CalRD 1.4.5.7 使用混乱精华×7 与洞穴探险药水×7，在秘银砧制作。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("EssenceofChaos", out ModItem essenceOfChaos))
                return;
            CreateRecipe()
                .AddIngredient(essenceOfChaos.Type, 7)
                .AddIngredient(ItemID.SpelunkerPotion, 7)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
