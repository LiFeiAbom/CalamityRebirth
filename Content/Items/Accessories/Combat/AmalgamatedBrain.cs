using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 聚合之脑：消耗腐烂之脑与原版混乱之脑合成，是专家反伤与闪避链条的进阶件。
    /// 提供常驻伤害和暴击，保留暗影雨，并在受击时困惑周围敌人；另有 1/10 概率完全闪避。
    /// </summary>
    internal sealed class AmalgamatedBrain : ModItem
    {
        /// <summary>常驻通用伤害提高比例。</summary>
        public const float DamageBonus = 0.10f;
        /// <summary>常驻通用暴击率提高值。</summary>
        public const int CritBonus = 5;
        /// <summary>每帧处于无敌状态时触发一轮暗影雨的概率分母。</summary>
        public const int AuraRainChanceDenominator = 8;
        /// <summary>聚合之脑生成的每滴暗影雨基础伤害。</summary>
        public const int AuraRainBaseDamage = 60;
        /// <summary>完全闪避攻击的概率分母。</summary>
        public const int DodgeChanceDenominator = 10;
        /// <summary>
        /// 设置判定尺寸与专家标记；稀有度和售价按现代 2.2.2 的浅红色档位统一处理。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            RarityUtil.Apply(Item, ItemRarityID.LightRed);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>仅置位本帧装备状态，具体面板、暗影雨、困惑与闪避由玩家结算类统一处理。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().amalgamatedBrain = true;
        }
        /// <summary>顶点融合之脑已包含聚合之脑效果，禁止重复装备。</summary>
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => incomingItem.type != ModContent.ItemType<TheAmalgam>();
        /// <summary>分别注册现代 2.2.2、经典 1.4.2.101 和 CalRD 1.4.5.7 的合成分支。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                CreateRecipe()
                    .AddIngredient<RottenBrain>()
                    .AddIngredient(ItemID.BrainOfConfusion)
                    .AddIngredient(ItemID.SoulofNight, 3)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out _))
            {
                CreateRecipe()
                    .AddIngredient<RottenBrain>()
                    .AddIngredient(ItemID.BrainOfConfusion)
                    .AddTile(TileID.MythrilAnvil)
                    .Register();
            }
            if (ModLoader.TryGetMod("CalRD", out _))
            {
                CreateRecipe()
                    .AddIngredient<RottenBrain>()
                    .AddIngredient(ItemID.BrainOfConfusion)
                    .AddTile(TileID.MythrilAnvil)
                    .Register();
            }
        }
    }
}
