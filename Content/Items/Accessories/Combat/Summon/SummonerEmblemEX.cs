using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
    /// <summary>
    /// 召唤师徽章EX（SummonerEmblemEX） - 月后召唤饰品。贴图是召唤师徽章按星尘盔甲色系做的
    /// 冷暖复合染色（2026-10-18 完成重染色）。
    /// 属性（用户 2026-10-26 修正）：+18% 召唤伤害、+2 召唤栏、+1 哨兵栏、
    /// +12% 鞭子攻击速度、+12% 鞭子攻击范围、+24% 召唤击退。
    /// 召唤伤害无视目标 10% 防御（命中结算时缩放 DefenseEffectiveness）。
    /// 配方与战士/游侠/法师徽章EX 同构，在秘银砧合成；特殊效果尚未点名。
    /// </summary>
    internal class SummonerEmblemEX : ModItem
    {
        /// <summary>召唤伤害加成比例。</summary>
        public const float SummonDamageBonus = 0.18f;
        /// <summary>增加的仆从栏位。</summary>
        public const int MinionSlotBonus = 2;
        /// <summary>增加的哨兵栏位。</summary>
        public const int SentrySlotBonus = 1;
        /// <summary>鞭子攻击速度加成比例。</summary>
        public const float WhipSpeedBonus = 0.12f;
        /// <summary>鞭子攻击范围加成比例。</summary>
        public const float WhipRangeBonus = 0.12f;
        /// <summary>召唤伤害击退加成比例。</summary>
        public const float SummonKnockbackBonus = 0.24f;
        /// <summary>召唤伤害忽略目标防御的比例。</summary>
        public const float DefenseIgnore = 0.10f;
        /// <summary>设置饰品尺寸、售价和召唤系 EX 稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ModContent.RarityType<SummonerEXRarity>();
        }
        /// <summary>
        /// 应用召唤伤害、栏位、鞭子与击退属性。
        /// 穿防不能在此直接修改目标，因此只点亮标记，由 ModPlayer 的命中修饰器统一结算。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += SummonDamageBonus;
            player.maxMinions += MinionSlotBonus;
            player.maxTurrets += SentrySlotBonus;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += WhipSpeedBonus;
            player.whipRangeMultiplier += WhipRangeBonus;
            player.GetKnockback(DamageClass.Summon) += SummonKnockbackBonus;
            player.GetModPlayer<CalamityRebirthPlayer>().summonerEmblemEX = true;
        }
        /// <summary>使用召唤师徽章、神圣锭与五种魂在秘银砧升级。</summary>
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SummonerEmblem)
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
