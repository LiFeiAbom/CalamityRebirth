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
    /// 真·远古方舟：发射减弱速度与伤害的永恒光束，并召唤圣星和大地弹混合轰击。
    /// </summary>
    internal sealed class TrueArkoftheAncients : ModItem
    {
        /// <summary>高阶唯一武器只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 使用调整后的旧链面板：100 近战伤害、22 帧使用时间、6.5 击退；
        /// 稀有度和价格挂钩现代真·远古方舟的黄色六十金档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.Yellow);
            Item.shoot = ModContent.ProjectileType<EonBeam>();
            Item.shootSpeed = 10f;
        }
        /// <summary>
        /// 主光束使用 75% 伤害与速度、两次穿透和十四帧本地冷却；
        /// 每次再生成 2 至 3 组圣星和大地弹，圣星为半伤害，大地弹为满伤害。
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile beam = Projectile.NewProjectileDirect(source, position, velocity * 0.75f, type
                , (int)(damage * 0.75f), knockback, player.whoAmI);
            beam.localNPCHitCooldown = 14;
            beam.penetrate = 2;
            beam.ai[0] = 1f;
            beam.ai[1] = Main.rand.Next(1, 3);
            int groups = Main.rand.Next(2, 4);
            for (int i = 0; i < groups; i++)
            {
                Vector2 spawn = CRUtil.GetFallingProjectileSpawn(player, i);
                Vector2 direction = Main.MouseWorld - spawn;
                direction.Y = System.Math.Abs(direction.Y);
                if (direction.Y < 20f)
                    direction.Y = 20f;
                direction.Normalize();
                direction *= Main.rand.Next(18, 27);
                Vector2 starVelocity = direction + Main.rand.NextVector2Circular(3.2f, 3.2f);
                int star = Projectile.NewProjectile(source, spawn, starVelocity
                    , ProjectileID.HallowStar, damage / 2, knockback, player.whoAmI
                    , 0f, Main.rand.Next(10));
                Main.projectile[star].DamageType = DamageClass.Melee;
                Vector2 terraVelocity = direction + Main.rand.NextVector2Circular(1.6f, 1.6f);
                Projectile.NewProjectile(source, spawn, terraVelocity, ModContent.ProjectileType<TerraBall>()
                    , damage, knockback, player.whoAmI, 0f, Main.rand.Next(5));
            }
            return false;
        }
        /// <summary>修正挥舞位置，并随机生成金色、荧光或蓝色神圣粉尘。</summary>
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            CRUtil.BetterSwing(player);
            if (!Main.rand.NextBool(5))
                return;
            int dustType = Main.rand.Next(3) switch
            {
                0 => 15,
                1 => 57,
                _ => 58,
            };
            int dustIndex = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height
                , dustType, player.direction * 2f, 0f, 150, default, 1.3f);
            Dust dust = Main.dust[dustIndex];
            dust.velocity *= 0.2f;
        }
        /// <summary>近战命中 NPC 时有一半概率施加五秒深海重压。</summary>
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                ApplyCrushDepth(target);
        }
        /// <summary>PvP 命中使用相同的深海重压概率和时长。</summary>
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(2))
                ApplyCrushDepth(target);
        }
        /// <summary>分别注册现代、经典 1.4.2.101 和 CalRD 配方。</summary>
        public override void AddRecipes()
        {
            AddRecipeForMod("CalamityMod", false);
            AddRecipeForMod("CalamityModClassicPreTrailer", true);
            AddRecipeForMod("CalRD", true);
        }
        /// <summary>
        /// 按模组注册升级配方。现代版只消耗灾厄核心和断裂英雄剑；
        /// 两个经典版按各自原版额外消耗三份生命碎片。
        /// </summary>
        private void AddRecipeForMod(string modName, bool usesLivingShard)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("CoreofCalamity", out ModItem core))
                return;
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<ArkoftheAncients>();
            recipe.AddIngredient(core.Type);
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            if (usesLivingShard)
            {
                if (!mod.TryFind<ModItem>("LivingShard", out ModItem livingShard))
                    return;
                recipe.AddIngredient(livingShard.Type, 3);
            }
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
        /// <summary>按当前灾厄版本施加深海重压；无对应模组时退回霜火。</summary>
        private static void ApplyCrushDepth<T>(T target)
        {
            bool applied = target switch
            {
                NPC npc => CalamityCompatUtil.TryAddBuff(npc, "CalamityMod", "CrushDepth", 300)
                    || CalamityCompatUtil.TryAddBuff(npc, "CalRD", "CrushDepth", 300)
                    || CalamityCompatUtil.TryAddBuff(npc, "CalamityModClassicPreTrailer"
                        , "CrushDepth", 300),
                Player player => CalamityCompatUtil.TryAddBuff(player, "CalamityMod"
                        , "CrushDepth", 300)
                    || CalamityCompatUtil.TryAddBuff(player, "CalRD", "CrushDepth", 300)
                    || CalamityCompatUtil.TryAddBuff(player, "CalamityModClassicPreTrailer"
                        , "CrushDepth", 300),
                _ => false,
            };
            if (applied)
                return;
            switch (target)
            {
                case NPC npc:
                    npc.AddBuff(BuffID.Frostburn, 300);
                    break;
                case Player player:
                    player.AddBuff(BuffID.Frostburn, 300);
                    break;
            }
        }
    }
}
