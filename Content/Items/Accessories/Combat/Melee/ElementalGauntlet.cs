using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 元素之握：元素之握前置链的顶点饰品。
    /// 提供强化近战面板，并为三版灾厄分别注册对应配方。
    /// </summary>
    internal sealed class ElementalGauntlet : ModItem
    {
        /// <summary>近战伤害、暴击率与攻击速度的统一加成比例。</summary>
        public const float MeleeBonus = 0.20f;
        /// <summary>近战暴击率加成，单位为百分点。</summary>
        public const float MeleeCritBonus = 20f;
        /// <summary>额外提供的岩浆免疫帧数。</summary>
        public const int LavaImmunityFrames = 240;
        /// <summary>元素之握每次近战命中施加元素混合II的持续时间。</summary>
        public const int HitDebuffDuration = 120;
        /// <summary>现代与 CalRD 配方消耗的化神魂精数量。</summary>
        private const int AscendantSpiritCount = 4;
        /// <summary>经典 1.4.2 配方中三种终局材料各自的数量。</summary>
        private const int ClassicMaterialCount = 20;
        /// <summary>
        /// 使用 22×38 的灾厄原生判定尺寸，并与血神核心保持 CosmicPurple 稀有度和标准价格。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 38;
            Item.accessory = true;
            RarityUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>());
        }
        /// <summary>
        /// 仅点亮本帧装备状态；完整面板和命中减益由玩家结算入口统一处理。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().elementalGauntlet = true;
        }
        /// <summary>注册现代、经典 1.4.2.101 与 CalRD 三版灾厄配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代 2.2.2：原版烈火手套 + 化神魂精×4，于星宇砧制作。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod modern)
                || !modern.TryFind<ModItem>("AscendantSpiritEssence", out ModItem spiritEssence)
                || !modern.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.FireGauntlet)
                .AddIngredient(spiritEssence.Type, AscendantSpiritCount)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101：保留魔君之证章与三类终局材料，于嘉登熔炉制作。
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
                .AddIngredient(ItemID.FireGauntlet)
                .AddIngredient<YharimsInsignia>()
                .AddIngredient(phantoplasm.Type, ClassicMaterialCount)
                .AddIngredient(nightmareFuel.Type, ClassicMaterialCount)
                .AddIngredient(endothermicEnergy.Type, ClassicMaterialCount)
                .AddTile(draedonsForge.Type)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7：原版烈火手套 + 魔君之证章 + 化神魂精×4，于嘉登熔炉制作。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem spiritEssence)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.FireGauntlet)
                .AddIngredient<YharimsInsignia>()
                .AddIngredient(spiritEssence.Type, AscendantSpiritCount)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
