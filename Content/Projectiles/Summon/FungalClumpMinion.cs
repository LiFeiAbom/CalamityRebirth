using CalamityRebirth.Content.Projectiles.Healing;
using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
    /// <summary>真菌团块召唤物：不占召唤栏，接触命中后按 25% 伤害生成吸血珠。</summary>
    internal sealed class FungalClumpMinion : ModProjectile
    {
        /// <summary>使用真菌团块自己的召唤物贴图。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/Summon/FungalClump";
        /// <summary>标记可牺牲与可锁定目标。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        /// <summary>设置乌鸦 AI 模板、无召唤栏消耗、本地命中冷却和常驻寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.minionSlots = 0f;
            Projectile.aiStyle = ProjAIStyleID.Raven;
            Projectile.timeLeft = 18000 * 5;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            AIType = ProjectileID.Raven;
            Projectile.tileCollide = false;
        }
        /// <summary>未持有召唤源时消散，主人存活时持续续命，并播放蓝妖精出生与孢子尘。</summary>
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.GetModPlayer<CalamityRebirthPlayer>().fungalClump)
            {
                Projectile.active = false;
                return;
            }
            if (!owner.dead)
                Projectile.timeLeft = 2;
            if (Projectile.localAI[0] == 0f)
            {
                const int dustCount = 36;
                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 radial = (Projectile.velocity.SafeNormalize(Vector2.UnitY)
                        * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f)
                        .RotatedBy((i - (dustCount / 2f - 1f)) * MathHelper.TwoPi / dustCount);
                    Vector2 position = Projectile.Center + radial;
                    int dustIndex = Dust.NewDust(position + radial, 0, 0, DustID.BlueFairy
                        , radial.X * 1.5f, radial.Y * 1.5f, 100, default, 1.4f);
                    Dust dust = Main.dust[dustIndex];
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.velocity = radial;
                }
                Projectile.localAI[0] = 1f;
            }
            if (Main.rand.NextBool(16))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height
                    , DustID.BlueFairy, Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f);
            }
        }
        /// <summary>命中 NPC 时按实际伤害的 25% 创建治疗珠。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnHeal(target.type == NPCID.TargetDummy || !target.canGhostHeal ? 0 : hit.Damage * 0.25f
                , target.position);
        }
        /// <summary>PvP 命中同样按 25% 伤害吸血。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SpawnHeal(info.Damage * 0.25f, target.position);
        }
        /// <summary>扣除主人剩余吸血额度并生成追踪治疗珠。</summary>
        private void SpawnHeal(float amount, Vector2 position)
        {
            if ((int)amount == 0)
                return;
            Player owner = Main.player[Projectile.owner];
            if (owner.lifeSteal <= 0f)
                return;
            owner.lifeSteal -= amount;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, Vector2.Zero
                , ModContent.ProjectileType<FungalHeal>(), 0, 0f, Projectile.owner
                , Projectile.owner, amount);
        }
    }
}
