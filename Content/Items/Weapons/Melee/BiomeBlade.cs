using CalamityRebirth.Content.Projectiles.Melee;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Weapons.Melee
{
    /// <summary>
    /// 环境之刃：鸿蒙方舟链条的最低层武器。
    /// 每次挥砍发射一个随玩家所在环境改变颜色、粉尘和命中减益的环境球。
    /// </summary>
    internal sealed class BiomeBlade : ModItem
    {
        /// <summary>武器作为唯一物品只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 采用旧链面板：70 近战伤害、24 帧使用时间、5 击退，
        /// 发射 12 速度的环境球；稀有度与价格挂钩现代 Broken Biome Blade 的橙色档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 70;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            RarityUtil.Apply(Item, ItemRarityID.Orange);
            Item.shoot = ModContent.ProjectileType<BiomeOrb>();
            Item.shootSpeed = 12f;
        }
        /// <summary>
        /// 修正大型武器的握持位置，并以五分之一概率扬起泥土尘。
        /// </summary>
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            CRUtil.BetterSwing(player);
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Dirt);
        }
        /// <summary>分别注册现代灾厄、经典 1.4.2.101 和 CalRD 1.4.5.7 的配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代配方使用十份群系材料、五份珊瑚和十份珍珠石，
        /// 在铁砧处制作。腐化与猩红世界分别注册以保持原版世界隔离。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.HasMod("CalamityMod"))
                return;
            RegisterLegacyBiomeRecipes(TileID.Anvils, 10, 5, 10);
        }
        /// <summary>
        /// 经典 1.4.2.101 使用该版本原始配方的秘银砧和十份群系材料；
        /// 武器面板使用已确认数值。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.HasMod("CalamityModClassicPreTrailer"))
                return;
            RegisterLegacyBiomeRecipes(TileID.MythrilAnvil, 10, 5, 10);
        }
        /// <summary>
        /// CalRD 1.4.5.7 原始配方使用每种群系材料二十份，并通过 AnyEvilBlock
        /// 同时兼容腐化与猩红方块；该版本配方不消耗珊瑚或珍珠石，在铁砧制作。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.HasMod("CalRD"))
                return;
            CreateRecipe()
                .AddIngredient(ItemID.WoodenSword)
                .AddIngredient(ItemID.DirtBlock, 20)
                .AddIngredient(ItemID.SandBlock, 20)
                .AddIngredient(ItemID.IceBlock, 20)
                .AddRecipeGroup("AnyEvilBlock", 20)
                .AddIngredient(ItemID.GlowingMushroom, 20)
                .AddIngredient(ItemID.Marble, 20)
                .AddIngredient(ItemID.Granite, 20)
                .AddIngredient(ItemID.Hellstone, 20)
                .AddIngredient(ItemID.Coral, 20)
                .AddTile(TileID.Anvils)
                .Register();
        }
        /// <summary>
        /// 注册旧链常见的腐化、猩红双配方。
        /// 两项配方仅邪恶石不同，并限定对应世界，避免同一世界出现无效材料路线。
        /// </summary>
        private void RegisterLegacyBiomeRecipes(int tileType, int blockCount, int coralCount, int pearlstoneCount)
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenSword)
                .AddIngredient(ItemID.DirtBlock, blockCount)
                .AddIngredient(ItemID.SandBlock, blockCount)
                .AddIngredient(ItemID.IceBlock, blockCount)
                .AddIngredient(ItemID.EbonstoneBlock, blockCount)
                .AddIngredient(ItemID.GlowingMushroom, blockCount)
                .AddIngredient(ItemID.Marble, blockCount)
                .AddIngredient(ItemID.Granite, blockCount)
                .AddIngredient(ItemID.Hellstone, blockCount)
                .AddIngredient(ItemID.Coral, coralCount)
                .AddIngredient(ItemID.PearlstoneBlock, pearlstoneCount)
                .AddTile(tileType)
                .AddCondition(Condition.CorruptWorld)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.WoodenSword)
                .AddIngredient(ItemID.DirtBlock, blockCount)
                .AddIngredient(ItemID.SandBlock, blockCount)
                .AddIngredient(ItemID.IceBlock, blockCount)
                .AddIngredient(ItemID.CrimstoneBlock, blockCount)
                .AddIngredient(ItemID.GlowingMushroom, blockCount)
                .AddIngredient(ItemID.Marble, blockCount)
                .AddIngredient(ItemID.Granite, blockCount)
                .AddIngredient(ItemID.Hellstone, blockCount)
                .AddIngredient(ItemID.Coral, coralCount)
                .AddIngredient(ItemID.PearlstoneBlock, pearlstoneCount)
                .AddTile(tileType)
                .AddCondition(Condition.CrimsonWorld)
                .Register();
        }
    }
}
