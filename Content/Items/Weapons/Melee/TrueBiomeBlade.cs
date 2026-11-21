using CalamityRebirth.Content.Projectiles.Melee;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Weapons.Melee
{
    /// <summary>
    /// 真·环境之刃：环境之刃的月后前进阶形态，发射具有中距离追踪能力的环境球。
    /// </summary>
    internal sealed class TrueBiomeBlade : ModItem
    {
        /// <summary>武器作为高阶唯一物品只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 使用 160 近战伤害、21 帧使用时间、7.5 击退；
        /// 稀有度和价格挂钩现代真·环境之刃的浅紫色三十五金档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.damage = 160;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 21;
            Item.useTime = 21;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            RarityUtil.Apply(Item, ItemRarityID.LightPurple);
            Item.shoot = ModContent.ProjectileType<TrueBiomeOrb>();
            Item.shootSpeed = 12f;
        }
        /// <summary>修正挥舞位置，并保留五分之一概率的泥土尘表现。</summary>
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            CRUtil.BetterSwing(player);
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Dirt);
        }
        /// <summary>注册三个灾厄版本各自可用的升级配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代 2.2.2 前移配方：环境之刃、断裂英雄剑、灵气、深渊细胞、流明晶和虚空石，秘银砧。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("DepthCells", out ModItem depthCells)
                || !calamity.TryFind<ModItem>("Lumenyl", out ModItem lumenyl)
                || !calamity.TryFind<ModItem>("Voidstone", out ModItem voidstone))
                return;
            CreateRecipe()
                .AddIngredient<BiomeBlade>()
                .AddIngredient(ItemID.BrokenHeroSword)
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddIngredient(depthCells.Type, 10)
                .AddIngredient(lumenyl.Type, 10)
                .AddIngredient(voidstone.Type, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 原生配方：环境之刃、生命碎片、灵气、深渊细胞、流明矿和 Tenebris。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!TryGetClassicMaterials("CalamityModClassicPreTrailer", out ModItem livingShard
                , out ModItem depthCells, out ModItem lumenite, out ModItem tenebris))
                return;
            CreateRecipe()
                .AddIngredient<BiomeBlade>()
                .AddIngredient(livingShard.Type, 5)
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddIngredient(depthCells.Type, 10)
                .AddIngredient(lumenite.Type, 10)
                .AddIngredient(tenebris.Type, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>CalRD 1.4.5.7 使用同名经典材料和秘银砧。</summary>
        private void AddCalRDRecipe()
        {
            if (!TryGetClassicMaterials("CalRD", out ModItem livingShard
                , out ModItem depthCells, out ModItem lumenite, out ModItem tenebris))
                return;
            CreateRecipe()
                .AddIngredient<BiomeBlade>()
                .AddIngredient(livingShard.Type, 5)
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddIngredient(depthCells.Type, 10)
                .AddIngredient(lumenite.Type, 10)
                .AddIngredient(tenebris.Type, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// 从指定的经典灾厄模组一次性查找四种同名材料，任一缺失时放弃整条配方。
        /// </summary>
        private static bool TryGetClassicMaterials(string modName, out ModItem livingShard
            , out ModItem depthCells, out ModItem lumenite, out ModItem tenebris)
        {
            livingShard = depthCells = lumenite = tenebris = null;
            return ModLoader.TryGetMod(modName, out Mod mod)
                && mod.TryFind("LivingShard", out livingShard)
                && mod.TryFind("DepthCells", out depthCells)
                && mod.TryFind("Lumenite", out lumenite)
                && mod.TryFind("Tenebris", out tenebris);
        }
    }
}
