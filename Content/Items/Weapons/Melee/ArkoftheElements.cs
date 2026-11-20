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
    /// 元素方舟：发射永恒光束与四枚追踪元素球，命中附加多类元素减益。
    /// </summary>
    internal sealed class ArkoftheElements : ModItem
    {
        /// <summary>高阶唯一武器只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 使用调整后的旧链面板：235 近战伤害、18 帧使用时间、8.5 击退；
        /// 稀有度和价格挂钩现代元素方舟的紫色一铂金二十金档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 84;
            Item.height = 84;
            Item.damage = 235;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item60;
            Item.autoReuse = true;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.Purple);
            Item.shoot = ModContent.ProjectileType<EonBeam>();
            Item.shootSpeed = 10f;
        }
        /// <summary>
        /// 先发射一条持续 160 帧的穿墙永恒光束，再从玩家上方生成四枚半伤害追踪元素球。
        /// 元素球逐层抬高生成位置，并朝鼠标方向下坠。
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile beam = Projectile.NewProjectileDirect(source, position, velocity, type, damage
                , knockback, player.whoAmI);
            beam.timeLeft = 160;
            beam.tileCollide = false;
            beam.ai[1] = Main.rand.Next(1, 5);
            float orbSpeed = Main.rand.Next(22, 30);
            for (int i = 0; i < 4; i++)
            {
                Vector2 spawn = CRUtil.GetFallingProjectileSpawn(player, i, false);
                Vector2 direction = Main.MouseWorld - spawn;
                if (direction.Y < 0f)
                    direction.Y *= -1f;
                if (direction.Y < 20f)
                    direction.Y = 20f;
                direction.Normalize();
                direction *= orbSpeed;
                direction += Main.rand.NextVector2Circular(7.2f, 7.2f);
                Projectile.NewProjectile(source, spawn, direction, ModContent.ProjectileType<ElementBall>()
                    , damage / 2, knockback, player.whoAmI, 0f, Main.rand.Next(3));
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
        /// <summary>近战命中 NPC 时施加霜火、圣火、硫磺火和瘟疫各两秒。</summary>
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyElementDebuffs(target);
        }
        /// <summary>PvP 命中使用完全相同的元素减益表。</summary>
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            ApplyElementDebuffs(target);
        }
        /// <summary>元素方舟给所有近战命中额外增加十点暴击。</summary>
        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit += 10f;
        }
        /// <summary>分别注册现代、经典 1.4.2.101 和 CalRD 配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代配方：真·远古方舟、本工程银河奇点五、灾厄核心五、生命合金五和夜明锭五，
        /// 在远古操纵机合成。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("CoreofCalamity", out ModItem core)
                || !calamity.TryFind<ModItem>("LifeAlloy", out ModItem lifeAlloy))
                return;
            CreateRecipe()
                .AddIngredient<TrueArkoftheAncients>()
                .AddIngredient<GalacticaSingularity>(5)
                .AddIngredient(core.Type, 5)
                .AddIngredient(lifeAlloy.Type, 5)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 配方：使用该版本银河奇点、灾厄核心、生命锭十和夜明锭十五。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!TryGetClassicMaterials("CalamityModClassicPreTrailer", out ModItem singularity
                , out ModItem core, out ModItem barOfLife))
                return;
            CreateRecipe()
                .AddIngredient<TrueArkoftheAncients>()
                .AddIngredient(singularity.Type, 5)
                .AddIngredient(core.Type, 5)
                .AddIngredient(barOfLife.Type, 10)
                .AddIngredient(ItemID.LunarBar, 15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 配方：银河奇点五、灾厄核心五、生命锭五和夜明锭五。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!TryGetClassicMaterials("CalRD", out ModItem singularity
                , out ModItem core, out ModItem barOfLife))
                return;
            CreateRecipe()
                .AddIngredient<TrueArkoftheAncients>()
                .AddIngredient(singularity.Type, 5)
                .AddIngredient(core.Type, 5)
                .AddIngredient(barOfLife.Type, 5)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        /// <summary>从指定经典模组查找银河奇点、灾厄核心和生命锭。</summary>
        private static bool TryGetClassicMaterials(string modName, out ModItem singularity
            , out ModItem core, out ModItem barOfLife)
        {
            singularity = core = barOfLife = null;
            return ModLoader.TryGetMod(modName, out Mod mod)
                && mod.TryFind("GalacticaSingularity", out singularity)
                && mod.TryFind("CoreofCalamity", out core)
                && mod.TryFind("BarofLife", out barOfLife);
        }
        /// <summary>
        /// 施加霜火并按当前灾厄版本追加圣火、硫磺火和瘟疫。
        /// 经典 1.4.2 的圣火对应 HolyLight。
        /// </summary>
        private static void ApplyElementDebuffs<T>(T target)
        {
            switch (target)
            {
                case NPC npc:
                    npc.AddBuff(BuffID.Frostburn, 120);
                    break;
                case Player player:
                    player.AddBuff(BuffID.Frostburn, 120);
                    break;
            }
            if (ModLoader.HasMod("CalamityMod"))
            {
                TryAddExternalBuff(target, "CalamityMod", "HolyFlames");
                TryAddExternalBuff(target, "CalamityMod", "BrimstoneFlames");
                TryAddExternalBuff(target, "CalamityMod", "Plague");
            }
            else if (ModLoader.HasMod("CalRD"))
            {
                TryAddExternalBuff(target, "CalRD", "HolyFlames");
                TryAddExternalBuff(target, "CalRD", "BrimstoneFlames");
                TryAddExternalBuff(target, "CalRD", "Plague");
            }
            else if (ModLoader.HasMod("CalamityModClassicPreTrailer"))
            {
                TryAddExternalBuff(target, "CalamityModClassicPreTrailer", "HolyLight");
                TryAddExternalBuff(target, "CalamityModClassicPreTrailer", "BrimstoneFlames");
                TryAddExternalBuff(target, "CalamityModClassicPreTrailer", "Plague");
            }
        }
        /// <summary>把两秒外部元素减益施加到 NPC 或 PvP 玩家。</summary>
        private static bool TryAddExternalBuff<T>(T target, string modName, string buffName)
        {
            return target switch
            {
                NPC npc => CalamityCompatUtil.TryAddBuff(npc, modName, buffName, 120),
                Player player => CalamityCompatUtil.TryAddBuff(player, modName, buffName, 120),
                _ => false,
            };
        }
    }
}
