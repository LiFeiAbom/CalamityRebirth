using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 蚀日尊戒：同时强化通用输出、近战挥速、召唤栏与生存能力的综合饰品。
    /// 面板为复合昼夜战斗属性，稀有度和价格按现代 2.2.2 的 CosmicPurple 档位统一处理。
    /// </summary>
    internal sealed class DarkSunRing : ModItem
    {
        /// <summary>通用伤害提高比例。</summary>
        public const float DamageBonus = 0.12f;
        /// <summary>近战攻速提高比例。</summary>
        public const float MeleeSpeedBonus = 0.12f;
        /// <summary>通用暴击率提高值。</summary>
        public const int CritBonus = 5;
        /// <summary>召唤击退提高值。</summary>
        public const float MinionKnockbackBonus = 1.2f;
        /// <summary>额外提供的最大仆从栏。</summary>
        public const int MaxMinionBonus = 2;
        /// <summary>挖掘速度提升比例；Terraria 中通过降低 pickSpeed 实现。</summary>
        public const float MiningSpeedBonus = 0.15f;
        /// <summary>白昼与日食期间额外获得的生命再生内部值；2 点约等于 1 HP/s。</summary>
        public const int DaylightLifeRegenBonus = 3;
        /// <summary>夜晚与日食期间额外获得的防御。</summary>
        public const int NightDefenseBonus = 30;
        /// <summary>
        /// 注册六帧纵向灵魂动画。贴图为 32×192，每帧 32×32。
        /// </summary>
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        /// <summary>
        /// 设置原版物品判定尺寸、基础防御和生命再生，并将稀有度及购买价绑定到现代 2.2.2 的紫色档位。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.defense = 10;
            Item.lifeRegen = 1;
            RarityUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>());
            Item.accessory = true;
        }
        /// <summary>
        /// 仅置位本帧装备标记；具体数值统一在玩家结算类中应用，避免物品 Hook 与其他综合饰品互相覆盖。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().darkSunRing = true;
        }
        /// <summary>分别注册现代 2.2.2、经典 1.4.2.101 和 CalRD 1.4.5.7 的原生配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代 2.2.2：永续锭×10 + 暗日碎片×20，在星宇砧制作。
        /// 任一模组物品或工作站缺失时整体跳过，避免留下半成品配方。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("UelibloomBar", out ModItem uelibloomBar)
                || !calamity.TryFind<ModItem>("DarksunFragment", out ModItem darksunFragment)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient(uelibloomBar.Type, 10)
                .AddIngredient(darksunFragment.Type, 20)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101：UeliaceBar×10 + 暗日碎片×100，在秘银砧制作。
        /// 该版本仍保留原生的旧阶段材料数量，因此不套用现代配方比例。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("UeliaceBar", out ModItem ueliaceBar)
                || !classic.TryFind<ModItem>("DarksunFragment", out ModItem darksunFragment))
                return;
            CreateRecipe()
                .AddIngredient(ueliaceBar.Type, 10)
                .AddIngredient(darksunFragment.Type, 100)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7：UeliaceBar×10 + 暗日碎片×100，在嘉登熔炉制作。
        /// 1.4.5.7 没有星宇砧，不能用现代工作站替代。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("UeliaceBar", out ModItem ueliaceBar)
                || !calRD.TryFind<ModItem>("DarksunFragment", out ModItem darksunFragment)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile draedonsForge))
                return;
            CreateRecipe()
                .AddIngredient(ueliaceBar.Type, 10)
                .AddIngredient(darksunFragment.Type, 100)
                .AddTile(draedonsForge.Type)
                .Register();
        }
    }
}
