using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 战士徽章EX：提供高阶近战三围，并通过 ModPlayer 为任意近战命中附加燃烧III。
    /// </summary>
    internal class WarriorEmblemEX : ModItem
    {
        /// <summary>近战伤害加成比例。</summary>
        public const float MeleeDamageBonus = 0.18f;
        /// <summary>近战暴击率加成，单位为百分点。</summary>
        public const float MeleeCritBonus = 8f;
        /// <summary>近战攻击速度加成比例。</summary>
        public const float MeleeSpeedBonus = 0.12f;
        /// <summary>设置饰品尺寸、售价和战士系 EX 稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ModContent.RarityType<WarriorEXRarity>();
        }
        /// <summary>
        /// 应用近战三围，并点亮本帧装备标记供命中钩子读取。
        /// 命中附加效果不在这里施加，因为装备更新阶段没有具体命中目标。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += MeleeDamageBonus;
            player.GetCritChance(DamageClass.Melee) += MeleeCritBonus;
            player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeedBonus;
            player.GetModPlayer<CalamityRebirthPlayer>().warriorEmblemEX = true;
        }
        /// <summary>使用战士徽章、神圣锭与五种魂在秘银砧合成为 EX 版本。</summary>
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WarriorEmblem)
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
