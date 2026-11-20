using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Ranged
{
    /// <summary>
    /// 元素箭袋：顶级远程职业复合饰品。
    /// 提供远程伤害、暴击、防御、生命再生、召唤击退、挖掘速度、省弹与箭矢分裂。
    /// 额外让非箭类远程弹幕获得 5% 最终乘算伤害，稀有度与元素之握保持一致。
    /// </summary>
    internal sealed class ElementalQuiver : ModItem
    {
        /// <summary>远程伤害加成比例。</summary>
        public const float RangedDamageBonus = 0.20f;
        /// <summary>远程暴击率加成百分点。</summary>
        public const float RangedCritBonus = 20f;
        /// <summary>不消耗弹药的触发概率。</summary>
        public const float AmmoSaveChance = 0.20f;
        /// <summary>召唤击退加成。</summary>
        public const float SummonKnockbackBonus = 0.5f;
        /// <summary>挖掘速度加成比例。</summary>
        public const float MiningSpeedBonus = 0.15f;
        /// <summary>箭矢每个 AI 更新节拍的镜像分裂概率。</summary>
        public const float ArrowSplitChance = 0.005f;
        /// <summary>分裂子箭继承的伤害比例。</summary>
        public const float ArrowSplitDamageMultiplier = 0.5f;
        /// <summary>分裂子箭的存活帧数。</summary>
        public const int ArrowSplitLifetime = 60;
        /// <summary>非箭类远程弹幕的最终伤害乘数。</summary>
        public const float NonArrowDamageMultiplier = 1.05f;
        /// <summary>
        /// 使用原始 28×32 判定尺寸、90 金价值与 5 点防御。
        /// 稀有度按用户要求与元素之握保持一致。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.rare = ModContent.RarityType<CosmicPurpleRarity>();
            Item.defense = 5;
            Item.accessory = true;
        }
        /// <summary>
        /// 结算静态面板属性，并置位省弹、分裂与非箭增伤读取标记。
        /// lifeRegen 每 2 点约等于 1 HP/s，因此这里增加 2 点。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += RangedDamageBonus;
            player.GetCritChance(DamageClass.Ranged) += RangedCritBonus;
            player.lifeRegen += 2;
            player.GetKnockback(DamageClass.Summon).Base += SummonKnockbackBonus;
            player.pickSpeed -= MiningSpeedBonus;
            player.GetModPlayer<CalamityRebirthPlayer>().elementalQuiver = true;
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 对应的元素箭袋配方。
        /// </summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代配方：魔法箭袋 + 代达罗斯纹章 + 三种高阶材料各 20。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("Necroplasm", out ModItem necroplasm)
                || !calamity.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !calamity.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.MagicQuiver)
                .AddIngredient<DaedalusEmblem>()
                .AddIngredient(necroplasm.Type, 20)
                .AddIngredient(nightmareFuel.Type, 20)
                .AddIngredient(endothermicEnergy.Type, 20)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 使用幻影质、梦魇燃料与吸热能量各 20，在嘉登熔炉制作。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("Phantoplasm", out ModItem phantoplasm)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.MagicQuiver)
                .AddIngredient<DaedalusEmblem>()
                .AddIngredient(phantoplasm.Type, 20)
                .AddIngredient(nightmareFuel.Type, 20)
                .AddIngredient(endothermicEnergy.Type, 20)
                .AddTile(draedonsForge.Type)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 使用化神魂精×4，与本地代达罗斯纹章在嘉登熔炉汇合。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem ascendantSpiritEssence)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.MagicQuiver)
                .AddIngredient<DaedalusEmblem>()
                .AddIngredient(ascendantSpiritEssence.Type, 4)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
