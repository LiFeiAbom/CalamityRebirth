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
    /// 远古方舟：随机发射永恒光束或附魔光束，并从天空召唤圣星。
    /// </summary>
    internal sealed class ArkoftheAncients : ModItem
    {
        /// <summary>高阶唯一武器只需研究一次。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 使用旧链面板：92 近战伤害、22 帧使用时间、6.25 击退；
        /// 现代对应物 Fractured Ark 使用浅红十金档。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 92;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.LightRed);
            Item.shoot = ModContent.ProjectileType<EonBeam>();
            Item.shootSpeed = 10f;
        }
        /// <summary>
        /// 随机选择永恒光束或附魔光束，并从玩家上方 600 像素处召唤两组圣星。
        /// 圣星使用三分之一面板伤害并强制转为近战伤害。
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            int beamType = Main.rand.NextBool()
                ? ModContent.ProjectileType<EonBeam>()
                : ProjectileID.EnchantedBeam;
            int beamIndex = Projectile.NewProjectile(source, position, velocity, beamType, damage
                , knockback, player.whoAmI);
            Projectile beam = Main.projectile[beamIndex];
            if (beam.type == ModContent.ProjectileType<EonBeam>())
                beam.penetrate = 2;
            else
                beam.extraUpdates = 1;
            float starSpeed = Main.rand.Next(18, 25);
            SpawnFallingStars(player, source, starSpeed, damage, knockback);
            return false;
        }
        /// <summary>修正大剑挥舞位置，并随机生成金色、荧光或蓝色神圣粉尘。</summary>
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
        /// <summary>近战命中 NPC 时有一半概率施加五秒霜火。</summary>
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Frostburn, 300);
        }
        /// <summary>PvP 命中使用相同的霜火概率和时长。</summary>
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Frostburn, 300);
        }
        /// <summary>分别注册现代、经典 1.4.2.101 和 CalRD 三条版本配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddLegacyRecipes();
        }
        /// <summary>现代配方使用日光精华、极寒精华、落星与附魔剑，在铁砧合成。</summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("EssenceofSunlight", out ModItem sunlight)
                || !calamity.TryFind<ModItem>("EssenceofEleum", out ModItem eleum))
                return;
            CreateRecipe()
                .AddIngredient(sunlight.Type, 3)
                .AddIngredient(eleum.Type, 3)
                .AddIngredient(ItemID.Starfury)
                .AddIngredient(ItemID.EnchantedSword)
                .AddTile(TileID.Anvils)
                .Register();
        }
        /// <summary>
        /// 两个经典灾厄版本使用余烬精华、极寒精华、落星和断钢剑，
        /// 并分别提供附魔剑与 Arkhalis 两条平行配方。
        /// </summary>
        private void AddLegacyRecipes()
        {
            AddLegacyRecipe("CalamityModClassicPreTrailer");
            AddLegacyRecipe("CalRD");
        }
        /// <summary>为指定经典模组注册附魔剑与 Arkhalis 两条配方。</summary>
        private void AddLegacyRecipe(string modName)
        {
            if (!ModLoader.TryGetMod(modName, out Mod mod)
                || !mod.TryFind<ModItem>("EssenceofCinder", out ModItem cinder)
                || !mod.TryFind<ModItem>("EssenceofEleum", out ModItem eleum))
                return;
            CreateRecipe()
                .AddIngredient(cinder.Type, 3)
                .AddIngredient(eleum.Type, 3)
                .AddIngredient(ItemID.Starfury)
                .AddIngredient(ItemID.EnchantedSword)
                .AddIngredient(ItemID.Excalibur)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(cinder.Type, 3)
                .AddIngredient(eleum.Type, 3)
                .AddIngredient(ItemID.Starfury)
                .AddIngredient(ItemID.Arkhalis)
                .AddIngredient(ItemID.Excalibur)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        /// <summary>
        /// 从玩家上方生成两组圣星，朝鼠标位置下落，并附加固定水平散布以避免完全重叠。
        /// </summary>
        private static void SpawnFallingStars(Player player, EntitySource_ItemUse_WithAmmo source
            , float speed, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 spawn = CRUtil.GetFallingProjectileSpawn(player, i);
                Vector2 direction = Main.MouseWorld - spawn;
                direction.Y = System.Math.Abs(direction.Y);
                if (direction.Y < 20f)
                    direction.Y = 20f;
                direction.Normalize();
                direction *= speed;
                direction += Main.rand.NextVector2Circular(2.4f, 2.4f);
                int index = Projectile.NewProjectile(source, spawn, direction, ProjectileID.HallowStar
                    , damage / 3, knockback, player.whoAmI, 0f, Main.rand.Next(5));
                Main.projectile[index].DamageType = DamageClass.Melee;
            }
        }
    }
}
