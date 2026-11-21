using CalamityRebirth.Content.Items.Accessories.Combat;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 血腥蠕虫牙：常态提供 5% 近战伤害、攻速和减伤，生命低于 50% 时提高到 10%。
    /// </summary>
    public class BloodyWormTooth : ModItem
    {
        /// <summary>常态三属性提高比例。</summary>
        public const float NormalBonus = 0.05f;
        /// <summary>生命低于 50% 时的三属性提高比例。</summary>
        public const float LowLifeBonus = 0.10f;
        /// <summary>设置贴图尺寸、现代版 5 金售价、橙色稀有度和专家标记。</summary>
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 15;
            RarityUtil.Apply(Item, ItemRarityID.Orange);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>点亮本帧的牙齿状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().bloodyWormTooth = true;
        }
        /// <summary>现代版额外允许在墓园环境用腐化之脑在工匠作坊升级。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                CreateRecipe()
                    .AddIngredient<RottenBrain>()
                    .AddTile(TileID.TinkerersWorkbench)
                    .AddCondition(Condition.InGraveyard)
                    .Register()
                    .DisableDecraft();
            }
        }
    }
}
