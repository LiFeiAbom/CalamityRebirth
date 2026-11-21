using CalamityRebirth.Content.Projectiles.Melee;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Weapons.Melee
{
    /// <summary>
    /// 银河：欧米茄环境之刃的月后进阶，发射一枚高追踪银河弹。
    /// </summary>
    internal sealed class FourSeasonsGalaxia : ModItem
    {
        /// <summary>高阶唯一武器只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 使用调整后的旧链面板：425 近战伤害、16 帧使用时间、9 击退；
        /// 稀有度和价格挂钩现代银河的绿松石一铂金五十金档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 70;
            Item.damage = 425;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            RarityUtil.Apply(Item, ModContent.RarityType<TurquoiseRarity>());
            Item.shoot = ModContent.ProjectileType<Galaxia>();
            Item.shootSpeed = 24f;
        }
        /// <summary>每次生成一枚银河弹，并在速度两个分量上叠加正负 0.4 的抖动。</summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 randomizedVelocity = new(
                velocity.X + Main.rand.Next(-40, 41) * 0.01f,
                velocity.Y + Main.rand.Next(-40, 41) * 0.01f);
            Projectile.NewProjectile(source, position, randomizedVelocity, type, damage, knockback
                , player.whoAmI);
            return false;
        }
        /// <summary>修正挥舞位置，并以五分之一概率生成泥土尘。</summary>
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            CRUtil.BetterSwing(player);
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Dirt);
        }
        /// <summary>近战本体命中 NPC 时按环境给予玩家增益。</summary>
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            BuffUtil.ApplyEnvironmentBuffs(player, BuffID.WellFed);
        }
        /// <summary>PvP 本体命中沿用相同的环境增益。</summary>
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            BuffUtil.ApplyEnvironmentBuffs(player, BuffID.WellFed);
        }
        /// <summary>分别注册现代、经典 1.4.2.101 和 CalRD 配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代配方：欧米茄环境之刃、星辉锭八和暗日碎片八，在星宇砧制作。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("CosmiliteBar", out ModItem cosmilite)
                || !calamity.TryFind<ModItem>("DarksunFragment", out ModItem darksun)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient<OmegaBiomeBlade>()
                .AddIngredient(cosmilite.Type, 8)
                .AddIngredient(darksun.Type, 8)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>经典 1.4.2.101 配方：星辉锭十和幻影质五，在嘉登熔炉制作。</summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CosmiliteBar", out ModItem cosmilite)
                || !classic.TryFind<ModItem>("Phantoplasm", out ModItem phantoplasm)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient<OmegaBiomeBlade>()
                .AddIngredient(cosmilite.Type, 10)
                .AddIngredient(phantoplasm.Type, 5)
                .AddTile(forge.Type)
                .Register();
        }
        /// <summary>CalRD 1.4.5.7 配方：星辉锭十和一份化神魂精，在嘉登熔炉制作。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("CosmiliteBar", out ModItem cosmilite)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient<OmegaBiomeBlade>()
                .AddIngredient(cosmilite.Type, 10)
                .AddIngredient(essence.Type)
                .AddTile(forge.Type)
                .Register();
        }
    }
}
