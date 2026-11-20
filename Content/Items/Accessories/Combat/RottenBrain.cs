using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 腐烂之脑：腐巢意志宝藏袋中的专家饰品，也是聚合之脑链条的起点。
    /// 低生命时提高通用伤害，生命进一步降低后牺牲少量移动速度，并在受击无敌期间降下暗影雨。
    /// </summary>
    internal sealed class RottenBrain : ModItem
    {
        /// <summary>生命低于 75% 时提供的通用伤害比例。</summary>
        public const float LowLifeDamageBonus = 0.15f;
        /// <summary>触发低血增伤的生命比例。</summary>
        public const float DamageLifeThreshold = 0.75f;
        /// <summary>生命低于 50% 时损失的移动速度比例。</summary>
        public const float LowLifeMoveSpeedPenalty = 0.05f;
        /// <summary>触发移动速度惩罚的生命比例。</summary>
        public const float MoveSpeedLifeThreshold = 0.50f;
        /// <summary>每帧处于无敌状态时触发一轮暗影雨的概率分母。</summary>
        public const int AuraRainChanceDenominator = 8;
        /// <summary>腐烂之脑生成的每滴暗影雨基础伤害。</summary>
        public const int AuraRainBaseDamage = 18;
        /// <summary>
        /// 设置判定尺寸与专家标记；稀有度和售价按现代 2.2.2 的橙色档位统一处理。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.Orange);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>仅置位本帧装备状态，具体面板与受击反击由玩家结算类统一处理。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().rottenBrain = true;
        }
        /// <summary>现代版允许在墓园环境的工匠作坊中消耗血腥蠕虫牙转换为腐烂之脑。</summary>
        public override void AddRecipes()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out _))
                return;
            CreateRecipe()
                .AddIngredient<BloodyWormTooth>()
                .AddTile(TileID.TinkerersWorkbench)
                .AddCondition(Condition.InGraveyard)
                .Register()
                .DisableDecraft();
        }
    }
}
