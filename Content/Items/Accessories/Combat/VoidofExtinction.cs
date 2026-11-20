using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 终结虚空：由黑曜石玫瑰、焦热地狱与灾厄之环合成的专家饰品。
    /// 提供通用增伤、岩浆战斗能力和双套硫磺火反击，并使用黄色稀有度与对应价格。
    /// </summary>
    internal sealed class VoidofExtinction : ModItem
    {
        /// <summary>通用伤害提高比例。</summary>
        public const float DamageBonus = 0.15f;
        /// <summary>身处岩浆时追加的通用伤害。</summary>
        public const float LavaDamageBonus = 0.25f;
        /// <summary>额外岩浆免疫帧数。</summary>
        public const int LavaImmunityFrames = 240;
        /// <summary>无敌期间站火触发概率分母。</summary>
        public const int StandingFireChanceDenominator = 10;
        /// <summary>站火基础伤害。</summary>
        public const int StandingFireBaseDamage = 40;
        /// <summary>每轮齐射间隔。</summary>
        public const int BarrageCooldownFrames = 600;
        /// <summary>每枚火球基础伤害。</summary>
        public const int FireballBaseDamage = 70;
        /// <summary>设置尺寸、专家标记、经典黄色稀有度和 30 金价格。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>置位本帧装备状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().voidofExtinction = true;
        }
        /// <summary>灾厄之环与顶点融合之脑已包含本饰品效果，禁止叠加装备。</summary>
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => incomingItem.type != ModContent.ItemType<CalamityRing>()
            && incomingItem.type != ModContent.ItemType<TheAmalgam>();
        /// <summary>三个灾厄分支均使用同一套本地组件配方。</summary>
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ObsidianRose)
                .AddIngredient<Gehenna>()
                .AddIngredient<CalamityRing>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
