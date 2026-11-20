using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Magic
{
    /// <summary>
    /// 法师徽章EX（SorcererEmblemEX） - 月后魔法饰品。贴图是原版法师徽章按**星云盔甲**色系染成的
    /// 「灰盘 + 靛蓝环 + 紫/品红法杖」（2026-10-18 完成重染色）。
    /// 属性（用户 2026-10-26 修正）：+18% 魔法伤害、+8% 魔法暴击率、+12% 施法速度、
    /// +100 最大魔力、+2 魔力/秒、降低 10% 魔力消耗。
    /// <para>
    /// 落点：三围、最大魔力与蓝耗直接在 <see cref="UpdateAccessory"/>；
    /// +2 魔力/秒由 <see cref="CalamityRebirthPlayer"/> 的 <c>PostUpdateEquips</c> 每 60 帧结算一次。
    /// </para>
    /// <para>
    /// 配方与战士/游侠徽章EX 同构：法师徽章 + 神圣锭 ×15 + 五种魂各 25 @ 秘银砧。
    /// 特殊效果尚未点名。
    /// </para>
    /// </summary>
    internal class SorcererEmblemEX : ModItem
    {
        /// <summary>魔法伤害加成（比例，18% = 0.18）</summary>
        public const float MagicBonus = 0.18f;
        /// <summary>魔法暴击率加成（百分点，8 = +8%）</summary>
        public const float MagicCritBonus = 8f;
        /// <summary>施法速度加成（比例，12% = 0.12）</summary>
        public const float MagicCastSpeedBonus = 0.12f;
        /// <summary>最大魔力值加成（点）</summary>
        public const int MaxManaBonus = 100;
        /// <summary>每秒额外回复的魔力（点）</summary>
        public const int ManaPerSecond = 2;
        /// <summary>魔力消耗降低比例（0.10 = 降低 10%）</summary>
        public const float ManaCostReduction = 0.10f;
        /// <summary>
        /// 物品基础属性：28×28 的饰品贴图（与战士/游侠徽章同口径）。
        /// 稀有度走本工程自带的 <see cref="SorcererEXRarity"/>（星云靛蓝→亮紫→品红→冰蓝渐变字色）。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ModContent.RarityType<SorcererEXRarity>();
        }
        /// <summary>
        /// 装备时生效：魔法三围、最大魔力与蓝耗，并把「已装备」标记交给 ModPlayer 供每秒回蓝读取。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += MagicBonus;
            player.GetCritChance(DamageClass.Magic) += MagicCritBonus;
            player.GetAttackSpeed(DamageClass.Magic) += MagicCastSpeedBonus;
            player.statManaMax2 += MaxManaBonus;
            player.manaCost -= ManaCostReduction;
            player.GetModPlayer<CalamityRebirthPlayer>().sorcererEmblemEX = true;
        }
        /// <summary>
        /// 配方：法师徽章 + 神圣锭 ×15 + 光明/暗影/恐惧/力量/视域之魂各 25 @ 秘银砧。
        /// </summary>
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SorcererEmblem)
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
