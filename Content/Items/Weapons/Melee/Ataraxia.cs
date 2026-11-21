using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Content.Projectiles.Melee;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Sounds;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Weapons.Melee
{
    /// <summary>
    /// 禅心剑（Ataraxia）：月后近战巨剑。挥砍生成一枚主弹和两枚侧弹，
    /// 近战命中生成大型花瓣爆炸，弹幕命中附加暗影焰。
    /// </summary>
    internal sealed class Ataraxia : ModItem
    {
        /// <summary>作为终局武器，只需研究一次即可解锁完整复制图鉴信息。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        /// <summary>
        /// 设置禅心剑的面板：3651 近战伤害、14 帧使用时间、现代灼辉金源标准价格，
        /// 并指定主弹类型与 10 的初始弹速。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 92;
            Item.damage = 3651;
            // 明确使用普通近战职业，不接入灾厄的 TrueMeleeDamageClass。
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.knockBack = 2.5f;
            RarityUtil.Apply(Item, ModContent.RarityType<BurnishedAuricRarity>());
            Item.shoot = ModContent.ProjectileType<AtaraxiaMain>();
            Item.shootSpeed = 10f;
        }
        /// <summary>
        /// 一次挥砍生成三枚弹幕：中央主弹保持 100% 伤害，左右侧弹各继承 50%。
        /// 侧弹位置沿速度方向的垂直分量外张，并略向玩家后方偏移，形成月牙形站位。
        /// 返回 false 阻止默认单弹生成，避免额外第四枚弹幕。
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item60, position);
            int centerID = ModContent.ProjectileType<AtaraxiaMain>();
            Projectile.NewProjectile(source, position, velocity, centerID, damage, knockback, player.whoAmI, 0f, 0f);
            int sideID = ModContent.ProjectileType<AtaraxiaSide>();
            int sideDamage = (int)(0.5f * damage);
            Vector2 originalVelocity = velocity;
            // 仅在计算侧弹站位时临时放大并归一化速度，生成弹幕仍使用原始初速。
            velocity.Normalize();
            velocity *= 22f;
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 leftOffset = velocity.RotatedBy(MathHelper.PiOver4, default) - 1.4f * velocity;
            Vector2 rightOffset = velocity.RotatedBy(-MathHelper.PiOver4, default) - 1.4f * velocity;
            Projectile.NewProjectile(source, rrp + leftOffset, originalVelocity, sideID, sideDamage, knockback
                , player.whoAmI, 0f, 1f);
            Projectile.NewProjectile(source, rrp + rightOffset, originalVelocity, sideID, sideDamage, knockback
                , player.whoAmI, 0f, 2f);
            player.GetModPlayer<CalamityRebirthPlayer>().ataraxiaHitSound = true;
            return false;
        }
        /// <summary>
        /// 每次近战判定更新武器握持位置，并在挥砍区域内生成少量前代颜色尘埃。
        /// 尘埃 ID 固定保留原始数值，避免新版 DustID 名称变化改变视觉颜色。
        /// </summary>
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            CRUtil.BetterSwing(player);
            int dustCount = Main.rand.Next(3, 6);
            Vector2 corner = new(hitbox.X + hitbox.Width / 4, hitbox.Y + hitbox.Height / 4);
            for (int i = 0; i < dustCount; i++)
            {
                // 固定使用 DustID 70 / 71 / 86；保留数字以免误映射成其他尘。
                int dustID = Main.rand.Next(5) switch
                {
                    0 or 1 => 70,
                    2 => 71,
                    _ => 86,
                };
                int index = Dust.NewDust(corner, hitbox.Width / 2, hitbox.Height / 2, dustID);
                Main.dust[index].noGravity = true;
            }
        }
        /// <summary>近战命中 NPC 时施加 8 秒暗影焰，并触发生命值 70% 的大爆炸。</summary>
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShadowflameII.ApplyToNPC(target, 480);
            OnHitEffects(player, target.Center);
        }
        /// <summary>近战命中 PvP 玩家时使用本模组暗影焰，并触发同样的大爆炸。</summary>
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            ShadowflameII.ApplyToPlayer(target, 480);
            OnHitEffects(player, target.Center);
        }
        /// <summary>
        /// 处理每次挥砍首次近战命中的音效与爆炸生成。
        /// 爆炸伤害基于物品原始伤害的 70%，再经玩家当前近战总伤害倍率结算。
        /// </summary>
        private void OnHitEffects(Player player, Vector2 targetPos)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (modPlayer.ataraxiaHitSound)
            {
                SoundEngine.PlaySound(CalamityRebirthSounds.CursedDaggerThrow, player.Center);
                modPlayer.ataraxiaHitSound = false;
            }
            int boomID = ModContent.ProjectileType<AtaraxiaBoom>();
            int explosionDamage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(0.7f * Item.damage);
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), targetPos, Vector2.Zero, boomID
                , explosionDamage, Item.knockBack, player.whoAmI, 0f, 0f);
        }
        /// <summary>分别注册现代 2.2.2、经典 1.4.2.101 和 CalRD 1.4.5.7 的配方。</summary>
        public override void AddRecipes()
        {
            AddModernRecipe();
            AddClassic142Recipe();
            AddCalRDRecipe();
        }
        /// <summary>
        /// 现代灾厄 2.2.2 配方：断英雄剑、金源锭、宇宙锭、化神魂精和梦魇燃料，在星宇砧制作。
        /// 任一 ModItem 或 ModTile 缺失时整条配方跳过，避免注册半成品。
        /// </summary>
        private void AddModernRecipe()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                || !calamity.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                || !calamity.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !calamity.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
                || !calamity.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
                return;
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(auricBar.Type, 5);
            recipe.AddIngredient(cosmiliteBar.Type, 8);
            recipe.AddIngredient(essence.Type, 2);
            recipe.AddIngredient(nightmareFuel.Type, 20);
            recipe.AddTile(cosmicAnvil.Type);
            recipe.Register();
        }
        /// <summary>
        /// 经典 1.4.2.101 配方：沿用该版本的多种月亮领主后材料，并在嘉登熔炉制作。
        /// 材料数量按经典版原始进度曲线单独配置，不与现代版比例复用。
        /// </summary>
        private void AddClassic142Recipe()
        {
            if (!ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                || !classic.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !classic.TryFind<ModItem>("Phantoplasm", out ModItem phantoplasm)
                || !classic.TryFind<ModItem>("NightmareFuel", out ModItem nightmareFuel)
                || !classic.TryFind<ModItem>("EndothermicEnergy", out ModItem endothermicEnergy)
                || !classic.TryFind<ModItem>("DarksunFragment", out ModItem darksunFragment)
                || !classic.TryFind<ModItem>("BarofLife", out ModItem barofLife)
                || !classic.TryFind<ModItem>("CoreofCalamity", out ModItem coreOfCalamity)
                || !classic.TryFind<ModItem>("HellcasterFragment", out ModItem hellcasterFragment)
                || !classic.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(cosmiliteBar.Type, 25);
            recipe.AddIngredient(phantoplasm.Type, 35);
            recipe.AddIngredient(nightmareFuel.Type, 90);
            recipe.AddIngredient(endothermicEnergy.Type, 90);
            recipe.AddIngredient(darksunFragment.Type, 65);
            recipe.AddIngredient(barofLife.Type, 15);
            recipe.AddIngredient(coreOfCalamity.Type, 5);
            recipe.AddIngredient(hellcasterFragment.Type, 10);
            recipe.AddTile(forge.Type);
            recipe.Register();
        }
        /// <summary>
        /// CalRD 1.4.5.7 配方：使用该版本存在的金源锭、宇宙锭、化神魂精与暗日碎片，
        /// 并明确使用嘉登熔炉，不依赖该版本不存在的星宇砧。
        /// </summary>
        private void AddCalRDRecipe()
        {
            if (!ModLoader.TryGetMod("CalRD", out Mod calRD)
                || !calRD.TryFind<ModItem>("AuricBar", out ModItem auricBar)
                || !calRD.TryFind<ModItem>("CosmiliteBar", out ModItem cosmiliteBar)
                || !calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem essence)
                || !calRD.TryFind<ModItem>("DarksunFragment", out ModItem darksunFragment)
                || !calRD.TryFind<ModTile>("DraedonsForge", out ModTile forge))
                return;
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(auricBar.Type, 4);
            recipe.AddIngredient(cosmiliteBar.Type, 12);
            recipe.AddIngredient(essence.Type, 4);
            recipe.AddIngredient(darksunFragment.Type, 15);
            recipe.AddTile(forge.Type);
            recipe.Register();
        }
    }
}
