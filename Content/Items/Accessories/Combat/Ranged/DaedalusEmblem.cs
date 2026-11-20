using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Ranged
{
    /// <summary>
    /// 代达罗斯纹章：远程职业复合饰品。
    /// 提供远程伤害、暴击、生命再生、召唤击退、防御与挖掘速度，并有概率不消耗弹药。
    /// 稀有度按用户要求与现代版 Dynamo Stem Cells 保持一致，使用原版紫色稀有度。
    /// </summary>
    internal sealed class DaedalusEmblem : ModItem
    {
        /// <summary>远程伤害加成比例。</summary>
        public const float RangedDamageBonus = 0.15f;
        /// <summary>远程暴击率加成百分点。</summary>
        public const float RangedCritBonus = 10f;
        /// <summary>不消耗弹药的触发概率。</summary>
        public const float AmmoSaveChance = 0.20f;
        /// <summary>召唤击退加成。</summary>
        public const float SummonKnockbackBonus = 0.5f;
        /// <summary>挖掘速度加成比例。</summary>
        public const float MiningSpeedBonus = 0.15f;
        /// <summary>
        /// 设置原始贴图判定尺寸、30 金价值、5 点防御与现代 Dynamo Stem Cells 的紫色稀有度。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.defense = 5;
            Item.accessory = true;
        }
        /// <summary>
        /// 直接结算静态面板属性，并置位省弹标记供 CanConsumeAmmo 使用。
        /// lifeRegen 每 2 点约等于 1 HP/s，因此这里增加 2 点。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += RangedDamageBonus;
            player.GetCritChance(DamageClass.Ranged) += RangedCritBonus;
            player.lifeRegen += 2;
            player.GetKnockback(DamageClass.Summon).Base += SummonKnockbackBonus;
            player.pickSpeed -= MiningSpeedBonus;
            player.GetModPlayer<CalamityRebirthPlayer>().daedalusEmblem = true;
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 和 CalRD 1.4.5.7 各自的灾厄核心配方。
        /// </summary>
        public override void AddRecipes()
        {
            AddVersionRecipe("CalamityMod");
            AddVersionRecipe("CalamityModClassicPreTrailer");
            AddVersionRecipe("CalRD");
        }
        /// <summary>
        /// 从指定灾厄版本弱依赖查找原生 Core of Calamity，并注册原始徽章配方。
        /// </summary>
        private void AddVersionRecipe(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod calamity)
                || !calamity.TryFind<ModItem>("CoreofCalamity", out ModItem coreOfCalamity))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.CelestialStone)
                .AddIngredient(coreOfCalamity.Type)
                .AddIngredient(ItemID.RangerEmblem)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
