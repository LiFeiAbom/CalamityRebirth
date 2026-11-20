using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 神之壁垒：由寒霜壁垒与神圣护符合成的综合生存饰品。
    /// 汇总十八点防御、击退免疫、五十点穿甲、圣骑士护盾、冰屏障与受击神圣星辰。
    /// </summary>
    [AutoloadEquip(EquipType.Shield)]
    internal sealed class RampartofDeities : ModItem
    {
        /// <summary>进入圣骑士护盾状态所需的最低生命比例。</summary>
        public const float PaladinShieldLifeThreshold = 0.25f;
        /// <summary>低于半血时维持原版冰屏障的生命比例。</summary>
        public const float IceBarrierLifeThreshold = 0.50f;
        /// <summary>获得额外伤害减免的残血比例。</summary>
        public const float EmergencyEnduranceLifeThreshold = 0.15f;
        /// <summary>残血时追加的伤害减免比例。</summary>
        public const float EmergencyEnduranceBonus = 0.05f;
        /// <summary>为附近队友补圣骑士护盾的最大距离，单位为像素。</summary>
        public const float PaladinShieldShareDistance = 800f;
        /// <summary>
        /// 设置判定尺寸、十八点防御、九十金价格、灼辉金源稀有度与盾牌饰品类型。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.value = Item.buyPrice(0, 90, 0, 0);
            Item.defense = 18;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<BurnishedAuricRarity>();
        }
        /// <summary>
        /// 只置位本帧装备状态；顶点会在玩家结算类中优先覆盖两件前置件的重复效果。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().rampartOfDeities = true;
        }
        /// <summary>
        /// 注册现代、经典 1.4.2.101 与 CalRD 1.4.5.7 三条顶点配方。
        /// </summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代灾厄使用金源锭、飞升星魂与星宇砧。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                || !calamity.TryFind<ModItem>("AscendantSpiritEssence", out ModItem spiritEssence)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient<FrigidBulwark>()
                .AddIngredient<DeificAmulet>()
                .AddIngredient(auricBar.Type, 5)
                .AddIngredient(spiritEssence.Type, 4)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 使用银河奇点、神圣晶核与化神魂精锭，在嘉登熔炉合成。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("GalacticaSingularity", out ModItem singularity)
                || !classic.TryFind<ModItem>("DivineGeode", out ModItem divineGeode)
                || !classic.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<FrigidBulwark>()
                .AddIngredient<DeificAmulet>()
                .AddIngredient(singularity.Type, 5)
                .AddIngredient(divineGeode.Type, 10)
                .AddIngredient(cosmiliteBar.Type, 20)
                .AddTile(draedonsForge.Type)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 的顶点配方与经典 1.4.2.101 同构，同样使用嘉登熔炉。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("GalacticaSingularity", out ModItem singularity)
                || !calRD.TryFind<ModItem>("DivineGeode", out ModItem divineGeode)
                || !calRD.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient<FrigidBulwark>()
                .AddIngredient<DeificAmulet>()
                .AddIngredient(singularity.Type, 5)
                .AddIngredient(divineGeode.Type, 10)
                .AddIngredient(cosmiliteBar.Type, 20)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
