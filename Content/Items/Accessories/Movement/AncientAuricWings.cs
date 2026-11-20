using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Movement
{
    /// <summary>
    /// 灼辉金源之翼：高阶段飞行饰品。
    /// 本体提供月后档翅膀机动性，并以通用伤害、暴击、近战攻速和伤害减免构成综合战斗面板。
    /// </summary>
    [AutoloadEquip(EquipType.Wings)]
    internal sealed class AncientAuricWings : ModItem
    {
        /// <summary>飞行时使用的淡金色钻石尘颜色。</summary>
        public static readonly Color WingDustColor = new(236, 214, 147);
        /// <summary>通用伤害加成比例。</summary>
        public const float GenericDamageBonus = 0.08f;
        /// <summary>通用暴击率加成，单位为百分点。</summary>
        public const float GenericCritBonus = 8f;
        /// <summary>近战攻击速度加成比例。</summary>
        public const float MeleeSpeedBonus = 0.08f;
        /// <summary>最终伤害减免加成比例。</summary>
        public const float DamageReductionBonus = 0.08f;
        /// <summary>每份配方需要的圣金源锭数量。</summary>
        private const int AuricBarCount = 12;
        /// <summary>每份配方需要的飞翔之魂数量。</summary>
        private const int FlightSoulCount = 20;
        /// <summary>
        /// 注册翅膀基础统计：400 帧飞行时间、12 水平速度、3 倍水平加速度。
        /// 这些值必须写入自动分配的 <see cref="Item.wingSlot"/>，否则物品只会被视为普通饰品。
        /// </summary>
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(400, 12f, 3f);
        }
        /// <summary>
        /// 设置 22×20 判定尺寸、饰品类型和 1 铂金价格。
        /// 稀有度采用本工程的灼辉金源稀有度；价格显式保留源工程的 1 铂金，不套用同名稀有度的常见价格。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.accessory = true;
            ModernRarityValueUtil.Apply(
                Item, ModContent.RarityType<BurnishedAuricRarity>(), Item.buyPrice(1, 0, 0, 0));
        }
        /// <summary>
        /// 应用全部被动属性，并在可见飞行状态下生成仿照源实现的淡金色钻石尘。
        /// 云朵瓶等附加跳跃正在启动时跳过粒子，避免同一帧把额外跳跃误显示为本翅膀推进。
        /// </summary>
        /// <param name="player">当前正在结算饰品效果的玩家。</param>
        /// <param name="hideVisual">是否隐藏翅膀装备外观与飞行粒子。</param>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += GenericDamageBonus;
            player.GetCritChance(DamageClass.Generic) += GenericCritBonus;
            player.GetAttackSpeed(DamageClass.Melee) += MeleeSpeedBonus;
            player.endurance += DamageReductionBonus;
            player.noFallDmg = true;
            if (player.controlJump
                && player.wingTime > 0f
                && !VanillaExtraJump.CloudInABottle.CanStart(player)
                && player.jump == 0
                && player.velocity.Y != 0f
                && !hideVisual)
            {
                // 粒子生成点位于玩家身体后侧，并随朝向镜像，模拟双翼向外卸除的星屑。
                int dustXOffset = player.direction == 1 ? -40 : 4;
                Vector2 dustPosition = new(
                    player.position.X + player.width / 2f + dustXOffset,
                    player.position.Y + player.height / 2f - 15f);
                int flightDust = Dust.NewDust(
                    dustPosition, 30, 30, DustID.GemDiamond, 0f, 0f, 100, WingDustColor, 2.4f);
                Main.dust[flightDust].noGravity = true;
                Main.dust[flightDust].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                    Main.dust[flightDust].fadeIn = 2f;
                Main.dust[flightDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
        }
        /// <summary>
        /// 设置翅膀的垂直推进曲线：下落时拥有 1 点补偿，持续上升 0.15，最大上升倍率 3.25。
        /// 该组参数与 <see cref="WingStats"/> 的水平参数共同决定完整飞行手感。
        /// </summary>
        public override void VerticalWingSpeeds(
            Player player,
            ref float ascentWhenFalling,
            ref float ascentWhenRising,
            ref float maxCanAscendMultiplier,
            ref float maxAscentMultiplier,
            ref float constantAscend)
        {
            ascentWhenFalling = 1f;
            ascentWhenRising = 0.175f;
            maxCanAscendMultiplier = 1.2f;
            maxAscentMultiplier = 3.25f;
            constantAscend = 0.15f;
        }
        /// <summary>
        /// 分别注册现代 2.2.2、经典 1.4.2.101 与 CalRD 1.4.5.7 的配方。
        /// 三条分支独立探测材料和合成站，任一前置缺失时只跳过对应版本，不影响其他版本加载。
        /// </summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代灾厄配方：圣金源锭×12 + 飞翔之魂×20，在嘉登熔炉制作。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod modern)
                || !modern.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                || !modern.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient(auricBar.Type, AuricBarCount)
                .AddIngredient(ItemID.SoulofFlight, FlightSoulCount)
                .AddTile(forge.Type)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 配方：该版本没有圣金源锭物品，按每锭五矿一碎片换算，
        /// 即圣金源矿×60 + 龙魂碎片×12，再加飞翔之魂×20，在嘉登熔炉制作。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            Recipe recipe = CreateRecipe();
            if (!CRUtil.TryAddClassicAuricBar(recipe, AuricBarCount))
                return;
            recipe.AddIngredient(ItemID.SoulofFlight, FlightSoulCount);
            recipe.AddTile(forge.Type);
            recipe.Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 配方：该版本直接提供圣金源锭，使用圣金源锭×12 + 飞翔之魂×20，
        /// 并统一放到该版本存在的嘉登熔炉制作；不探测其不存在的星宇砧。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient(auricBar.Type, AuricBarCount)
                .AddIngredient(ItemID.SoulofFlight, FlightSoulCount)
                .AddTile(forge.Type)
                .Register();
        }
    }
}
