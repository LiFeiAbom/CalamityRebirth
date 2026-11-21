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
    /// 鸿蒙方舟：环境之刃与远古方舟两条支流汇合后的终局武器。
    /// 每次挥砍发射永恒光束和四枚满伤害群星，并按环境提供增益。
    /// </summary>
    internal sealed class ArkoftheCosmos : ModItem
    {
        /// <summary>终局唯一武器只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 使用调整后的旧链面板：680 近战伤害、14 帧使用时间、9.5 击退；
        /// 稀有度挂钩现代鸿蒙方舟的灼辉金源，价格使用现代紫色二铂金四十金档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 102;
            Item.damage = 680;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.useTurn = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9.5f;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            RarityUtil.Apply(Item, ModContent.RarityType<BurnishedAuricRarity>());
            Item.shoot = ModContent.ProjectileType<EonBeam>();
            Item.shootSpeed = 14f;
        }
        /// <summary>
        /// 主光束持续 160 帧并穿墙，随后从玩家与鼠标之间向鼠标方向发射四枚群星。
        /// 每枚群星带正负十五度随机偏转，持续 160 帧并继承完整面板伤害。
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile beam = Projectile.NewProjectileDirect(source, position, velocity, type, damage
                , knockback, player.whoAmI);
            beam.timeLeft = 160;
            beam.tileCollide = false;
            beam.ai[1] = Main.rand.Next(1, 5);
            Vector2 mouse = Main.MouseWorld;
            int spawnX = (int)(mouse.X + player.Center.X) / 2;
            for (int i = 0; i < 4; i++)
            {
                Vector2 spawn = new(spawnX, player.Center.Y - 100f * i);
                Vector2 direction = (mouse - spawn).SafeNormalize(Vector2.UnitY)
                    .RotatedByRandom(MathHelper.ToRadians(15f));
                int index = Projectile.NewProjectile(source, spawn, direction * velocity.Length()
                    , ModContent.ProjectileType<Galaxia>(), damage, knockback, player.whoAmI
                    , 0f, Main.rand.Next(3));
                Main.projectile[index].timeLeft = 160;
            }
            return false;
        }
        /// <summary>修正挥舞位置，并以五分之一概率生成彩虹色无重力粉尘。</summary>
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            CRUtil.BetterSwing(player);
            if (!Main.rand.NextBool(5))
                return;
            int dustIndex = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height
                , DustID.RainbowTorch, player.direction * 2f, 0f, 150
                , new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            Dust dust = Main.dust[dustIndex];
            dust.velocity *= 0.2f;
            dust.noGravity = true;
        }
        /// <summary>近战命中 NPC 时按环境给予玩家增益，南瓜月使用三级饱食。</summary>
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            BuffUtil.ApplyEnvironmentBuffs(player, BuffID.WellFed3);
        }
        /// <summary>PvP 本体命中沿用相同的环境增益。</summary>
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            BuffUtil.ApplyEnvironmentBuffs(player, BuffID.WellFed3);
        }
        /// <summary>鸿蒙方舟额外提供十六点近战暴击。</summary>
        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit += 16f;
        }
        /// <summary>分别注册现代、经典 1.4.2.101 和 CalRD 配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>现代配方：银河、元素方舟与五份圣金源锭，在星宇砧制作。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            CreateRecipe()
                .AddIngredient<FourSeasonsGalaxia>()
                .AddIngredient<ArkoftheElements>()
                .AddIngredient(auricBar.Type, 5)
                .AddTile(cosmicAnvil.Type)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 原版配方：银河、元素方舟和五种终局暗黑材料，在嘉登熔炉制作。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermic)
                || !classic.TryFind<ModItem>("HellcasterFragment", out ModItem hellcaster)
                || !classic.TryFind<ModItem>("DarksunFragment", out ModItem darksun)
                || !classic.TryFind<ModItem>("AuricOre", out ModItem auricOre)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient<FourSeasonsGalaxia>()
                .AddIngredient<ArkoftheElements>()
                .AddIngredient(nightmareFuel.Type, 5)
                .AddIngredient(endothermic.Type, 5)
                .AddIngredient(hellcaster.Type, 3)
                .AddIngredient(darksun.Type, 5)
                .AddIngredient(auricOre.Type, 25)
                .AddTile(forge.Type)
                .Register();
        }
        /// <summary>CalRD 1.4.5.7 配方：银河、元素方舟与四份金源锭，在嘉登熔炉制作。</summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            CreateRecipe()
                .AddIngredient<FourSeasonsGalaxia>()
                .AddIngredient<ArkoftheElements>()
                .AddIngredient(auricBar.Type, 4)
                .AddTile(forge.Type)
                .Register();
        }
    }
}
