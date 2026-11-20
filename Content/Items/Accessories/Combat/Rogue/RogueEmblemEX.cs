using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Rogue
{
    /// <summary>
    /// 盗贼徽章EX（RogueEmblemEX） - 月后盗贼饰品。贴图是盗贼徽章按皇天套装色系染成，
    /// 灰盘与黑边保持原样，只染金棕色匕首（2026-10-18）。
    /// 属性（用户 2026-10-26 修正）：+18% 盗贼伤害、+8% 盗贼暴击率、
    /// +12% 盗贼武器攻击速度、+60 最大潜行值。
    /// 配方：对应版本灾厄的盗贼徽章 + 神圣锭×15 + 五种魂各25 @ 秘银砧。
    /// </summary>
    internal class RogueEmblemEX : ModItem
    {
        /// <summary>盗贼伤害加成比例。</summary>
        public const float RogueDamageBonus = 0.18f;
        /// <summary>盗贼暴击率加成，单位为百分点。</summary>
        public const int RogueCritBonus = 8;
        /// <summary>盗贼武器攻击速度加成比例。</summary>
        public const float RogueSpeedBonus = 0.12f;
        /// <summary>最大潜行加成，单位为 100% 倍数所需的浮点比例。</summary>
        public const float StealthBonus = 0.60f;
        /// <summary>设置饰品尺寸、售价和盗贼系 EX 稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ModContent.RarityType<RogueEXRarity>();
        }
        /// <summary>
        /// 仅点亮装备标记。盗贼伤害、暴击、攻速和潜行由 CRUtil 根据当前灾厄版本在晚装阶段应用。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().rogueEmblemEX = true;
        }
        /// <summary>分别从现代灾厄、经典 1.4.2.101 和 CalRD 查找同名盗贼徽章。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod modern)
                && modern.TryFind<ModItem>("RogueEmblem", out ModItem modernEmblem))
            {
                AddRogueRecipe(modernEmblem.Type);
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("RogueEmblem", out ModItem classicEmblem))
            {
                AddRogueRecipe(classicEmblem.Type);
            }
            if (ModLoader.TryGetMod("CalRD", out Mod classic1457)
                && classic1457.TryFind<ModItem>("RogueEmblem", out ModItem classic1457Emblem))
            {
                AddRogueRecipe(classic1457Emblem.Type);
            }
        }
        /// <summary>用对应版本盗贼徽章、神圣锭和五种魂创建统一升级配方。</summary>
        private void AddRogueRecipe(int rogueEmblemType)
        {
            CreateRecipe()
                .AddIngredient(rogueEmblemType)
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddIngredient(ItemID.SoulofLight, 25)
                .AddIngredient(ItemID.SoulofNight, 25)
                .AddIngredient(ItemID.SoulofFright, 25)
                .AddIngredient(ItemID.SoulofMight, 25)
                .AddIngredient(ItemID.SoulofSight, 25)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
