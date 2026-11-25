using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
    /// <summary>魔影套装召唤的常驻友方红魔，本体不造成接触伤害。</summary>
    internal sealed class RedDevil : ModProjectile
    {
        /// <summary>登场粒子的剩余发射帧计数。</summary>
        private int dust = 3;
        /// <summary>注册五帧动画并允许召唤锁定与替换。</summary>
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        /// <summary>设置四十八像素判定、友方召唤定位与近乎永久的基础寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000 * 5;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        /// <summary>执行跟随、索敌、召回与邪秽三叉戟射击。</summary>
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (!modPlayer.redDevil)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
                modPlayer.redDevil2 = false;
            if (modPlayer.redDevil2)
                Projectile.timeLeft = 2;
            AnimateAndLight();
            SeparateFromOtherRedDevils();
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1]++;
                Projectile.extraUpdates = 1;
                if (Projectile.ai[1] > 40f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 0;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    return;
                }
            }
            bool hasTarget = FindTarget(player, out Vector2 targetCenter);
            ApplyFollowOrAttackMotion(player, hasTarget, targetCenter);
            UpdateAttackCooldown();
            TryShoot(player, modPlayer, hasTarget, targetCenter);
        }
        /// <summary>本体没有接触伤害，所有输出都来自邪秽三叉戟。</summary>
        public override bool? CanDamage() => false;
        /// <summary>推进登场粒子、动画、光照与朝向。</summary>
        private void AnimateAndLight()
        {
            dust--;
            if (dust >= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int index = Dust.NewDust(new Vector2(
                        Projectile.position.X, Projectile.position.Y + 16f),
                        Projectile.width, Projectile.height - 16,
                        DustID.LifeDrain, 0f, 0f, 0, default, 1f);
                    Main.dust[index].velocity *= 2f;
                    Main.dust[index].scale *= 1.15f;
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
                Projectile.frame = 0;
            float brightness = Main.rand.Next(90, 111) * 0.01f * Main.essScale;
            Lighting.AddLight(Projectile.Center, brightness, 0f, brightness * 0.15f);
            Projectile.rotation = Projectile.velocity.X * 0.04f;
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
                Projectile.spriteDirection = -Projectile.direction;
        }
        /// <summary>同主人的红魔距离过近时互相推开。</summary>
        private void SeparateFromOtherRedDevils()
        {
            int type = ModContent.ProjectileType<RedDevil>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (i == Projectile.whoAmI || !other.active || other.owner != Projectile.owner
                    || other.type != type
                    || Math.Abs(Projectile.position.X - other.position.X)
                        + Math.Abs(Projectile.position.Y - other.position.Y) >= Projectile.width)
                    continue;
                Projectile.velocity.X += Projectile.position.X < other.position.X ? -0.05f : 0.05f;
                Projectile.velocity.Y += Projectile.position.Y < other.position.Y ? -0.05f : 0.05f;
            }
        }
        /// <summary>优先锁定玩家指定的目标，否则寻找七百像素内视线可达的最近敌人。</summary>
        private bool FindTarget(Player player, out Vector2 targetCenter)
        {
            targetCenter = Projectile.position;
            float nearest = 700f;
            bool found = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                if (npc.CanBeChasedBy(Projectile) && distance < nearest)
                {
                    targetCenter = npc.Center;
                    found = true;
                }
                return found;
            }
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile))
                    continue;
                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                if (distance >= nearest
                    || !Collision.CanHitLine(Projectile.position, Projectile.width
                        , Projectile.height, npc.position, npc.width, npc.height))
                    continue;
                nearest = distance;
                targetCenter = npc.Center;
                found = true;
            }
            return found;
        }
        /// <summary>有目标时保持约两百像素距离，无目标时悬停在玩家上方三十像素。</summary>
        private void ApplyFollowOrAttackMotion(Player player, bool hasTarget, Vector2 targetCenter)
        {
            float distanceToPlayer = Vector2.Distance(player.Center, Projectile.Center);
            float recallLimit = hasTarget ? 3000f : 2000f;
            if (distanceToPlayer > recallLimit)
            {
                Projectile.ai[0] = 1f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            if (hasTarget && Projectile.ai[0] == 0f)
            {
                Vector2 direction = targetCenter - Projectile.Center;
                float distance = direction.Length();
                direction = direction.SafeNormalize(Vector2.UnitY);
                direction *= distance > 200f ? 16f : -4f;
                Projectile.velocity = (Projectile.velocity * 40f + direction) / 41f;
                return;
            }
            bool recalling = Projectile.ai[0] == 1f;
            float speed = recalling ? 12f : 5f;
            Vector2 hoverOffset = player.Center - Projectile.Center + new Vector2(0f, -30f);
            float hoverDistance = hoverOffset.Length();
            if (hoverDistance > 200f && speed < 6.5f)
                speed = 6.5f;
            if (hoverDistance < 150f && recalling
                && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (hoverDistance > 2000f)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
                return;
            }
            if (hoverDistance > 70f)
            {
                hoverOffset = hoverOffset.SafeNormalize(Vector2.UnitY) * speed;
                Projectile.velocity = (Projectile.velocity * 40f + hoverOffset) / 41f;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = new Vector2(-0.2f, -0.1f);
            }
        }
        /// <summary>攻击计时以随机一到三的步长推进，越过八十后复位。</summary>
        private void UpdateAttackCooldown()
        {
            if (Projectile.ai[1] > 0f)
                Projectile.ai[1] += Main.rand.Next(1, 4);
            if (Projectile.ai[1] > 80f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
        }
        /// <summary>巡游状态且冷却归零时朝目标发射邪秽三叉戟。</summary>
        private void TryShoot(Player player, CalamityRebirthPlayer modPlayer
            , bool hasTarget, Vector2 targetCenter)
        {
            if (Projectile.ai[0] != 0f || !hasTarget || Projectile.ai[1] != 0f)
                return;
            Projectile.ai[1] += 1f;
            if (Main.myPlayer != Projectile.owner
                || !Collision.CanHitLine(Projectile.position, Projectile.width
                    , Projectile.height, targetCenter, 0, 0))
                return;
            Vector2 velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX) * 24f;
            int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center
                , velocity, ProjectileID.UnholyTridentFriendly, Projectile.damage, 0f, Main.myPlayer);
            if (!Main.projectile.IndexInRange(index))
                return;
            Projectile trident = Main.projectile[index];
            trident.timeLeft = 300;
            trident.usesLocalNPCImmunity = true;
            trident.localNPCHitCooldown = 10;
            trident.DamageType = DamageClass.Generic;
            trident.CritChance = (int)player.GetTotalCritChance(DamageClass.Generic);
            Projectile.netUpdate = true;
        }
    }
}
