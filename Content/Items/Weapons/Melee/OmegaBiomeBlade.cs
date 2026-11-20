using CalamityRebirth.Content.Items.Materials;
using CalamityRebirth.Content.Projectiles.Melee;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Weapons.Melee
{
    /// <summary>
    /// 欧米茄环境之刃：每次挥砍散射五枚短程追踪环境球，并读取四柱环境。
    /// </summary>
    internal sealed class OmegaBiomeBlade : ModItem
    {
        /// <summary>高阶唯一武器只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 使用调整后的旧链面板：220 近战伤害、18 帧使用时间、8 击退；
        /// 稀有度和价格挂钩现代欧米茄环境之刃的青色八十金档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 62;
            Item.damage = 220;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.Cyan);
            Item.shoot = ModContent.ProjectileType<OmegaBiomeOrb>();
            Item.shootSpeed = 15f;
        }
        /// <summary>
        /// 每次挥砍生成五枚环境球，每枚在基础速度上叠加正负两个单位的横向与纵向随机偏移。
        /// 返回 false 以阻止默认单弹生成。
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 spreadVelocity = new(
                    velocity.X + Main.rand.Next(-40, 41) * 0.05f,
                    velocity.Y + Main.rand.Next(-40, 41) * 0.05f);
                Projectile.NewProjectile(source, position, spreadVelocity, type, damage, knockback
                    , player.whoAmI);
            }
            return false;
        }
        /// <summary>修正大型武器挥舞位置，并保留少量泥土尘。</summary>
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            CRUtil.BetterSwing(player);
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Dirt);
        }
        /// <summary>分别注册现代、经典 1.4.2.101 和 CalRD 的配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代配方：真·环境之刃、灾厄核心、生命合金、本工程银河奇点和夜明锭，在远古操纵机合成。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("CoreofCalamity", out ModItem core)
                || !calamity.TryFind<ModItem>("LifeAlloy", out ModItem lifeAlloy))
                return;
            CreateRecipe()
                .AddIngredient<TrueBiomeBlade>()
                .AddIngredient(core.Type, 3)
                .AddIngredient(lifeAlloy.Type, 3)
                .AddIngredient<GalacticaSingularity>(3)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 配方：生命锭替换生命合金，并使用该版本原生的银河奇点。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!TryGetClassicMaterials("CalamityModClassicPreTrailer", out ModItem core
                , out ModItem barOfLife, out ModItem singularity))
                return;
            CreateRecipe()
                .AddIngredient<TrueBiomeBlade>()
                .AddIngredient(core.Type, 3)
                .AddIngredient(barOfLife.Type, 3)
                .AddIngredient(singularity.Type, 3)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>CalRD 1.4.5.7 使用相同的灾厄核心、生命锭与原生银河奇点配方。</summary>
        private void AddCalRDRecipe()
        {
            if (!TryGetClassicMaterials("CalRD", out ModItem core
                , out ModItem barOfLife, out ModItem singularity))
                return;
            CreateRecipe()
                .AddIngredient<TrueBiomeBlade>()
                .AddIngredient(core.Type, 3)
                .AddIngredient(barOfLife.Type, 3)
                .AddIngredient(singularity.Type, 3)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>从指定经典模组查找灾厄核心、生命锭和银河奇点。</summary>
        private static bool TryGetClassicMaterials(string modName, out ModItem core
            , out ModItem barOfLife, out ModItem singularity)
        {
            core = barOfLife = singularity = null;
            return ModLoader.TryGetMod(modName, out Mod mod)
                && mod.TryFind("CoreofCalamity", out core)
                && mod.TryFind("BarofLife", out barOfLife)
                && mod.TryFind("GalacticaSingularity", out singularity);
        }
    }
}
