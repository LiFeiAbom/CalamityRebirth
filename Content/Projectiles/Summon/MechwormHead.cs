using CalamityRebirth.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
    /// <summary>噬神机械蠕虫头部：追击玩家锁定的目标并带动整条蠕虫移动。</summary>
    public sealed class MechwormHead : ModProjectile
    {
        /// <summary>入场尘埃剩余批次。</summary>
        private int dustBursts = 3;
        /// <summary>召唤时记录的仆从栏上限。</summary>
        private int initialMaxMinions;
        /// <summary>首帧记录标记。</summary>
        private bool initialized;
        /// <summary>注册为可牺牲且可接收锁定目标的召唤物。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        /// <summary>设置 20×20 判定、90 秒寿命、召唤类型和四帧局部命中冷却。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }
        /// <summary>追击目标或回到玩家身边，并向后续身体节传递缩放层级。</summary>
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.15f, 0.01f, 0.15f);
            if (!Main.player.IndexInRange(Projectile.owner))
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (!initialized)
            {
                initialized = true;
                initialMaxMinions = player.maxMinions;
            }
            if (dustBursts > 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width
                        , Projectile.height, DustID.BoneTorch, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].scale *= 1.15f;
                }
                dustBursts--;
            }
            player.AddBuff(ModContent.BuffType<Content.ModBuffs.Buffs.Mechworm>(), 3600);
            if (!player.active || player.dead || player.maxMinions < initialMaxMinions)
            {
                modPlayer.mWorm = false;
                Projectile.active = false;
                return;
            }
            initialMaxMinions = Math.Max(initialMaxMinions, player.maxMinions);
            if (modPlayer.mWorm)
                Projectile.timeLeft = 2;
            if ((int)Main.time % 120 == 0)
                Projectile.netUpdate = true;
            if (Vector2.Distance(Projectile.Center, player.Center) > 3000f)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
            }
            int targetIndex = FindTarget(player);
            if (targetIndex >= 0)
                ChaseTarget(Main.npc[targetIndex]);
            else
                FollowOwner(player);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;
            float scaleBonus = MathHelper.Clamp(Projectile.localAI[0], 0f, 50f);
            Projectile.scale = 1f + scaleBonus * 0.01f;
            Projectile.width = Projectile.height = (int)(30f * Projectile.scale);
            if (Projectile.alpha > 0)
                Projectile.alpha = Math.Max(0, Projectile.alpha - 42);
        }
        /// <summary>寻找玩家锁定目标或 1800 像素内任意可追击目标。</summary>
        private int FindTarget(Player player)
        {
            if (player.HasMinionAttackTargetNPC && Main.npc.IndexInRange(player.MinionAttackTargetNPC))
            {
                NPC marked = Main.npc[player.MinionAttackTargetNPC];
                if (marked.CanBeChasedBy(Projectile)
                    && Vector2.Distance(marked.Center, player.Center) < 2200f
                    && Vector2.Distance(marked.Center, Projectile.Center) < 1800f)
                    return marked.whoAmI;
            }
            int target = -1;
            float bestDistance = 1800f;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile)
                    || Vector2.Distance(npc.Center, player.Center) >= 2200f)
                    continue;
                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                if (distance >= bestDistance)
                    continue;
                bestDistance = distance;
                target = npc.whoAmI;
            }
            return target;
        }
        /// <summary>按距离提升转向加速度并把速度限制在 50。</summary>
        private void ChaseTarget(NPC target)
        {
            Vector2 toTarget = target.Center - Projectile.Center;
            float acceleration = toTarget.Length() switch
            {
                < 300f => 0.8f,
                < 600f => 0.6f,
                < 900f => 0.45f,
                _ => 0.3f
            };
            if (toTarget.Length() > target.Size.Length() * 0.75f)
                Projectile.velocity += toTarget.SafeNormalize(Vector2.UnitY) * acceleration * 1.5f;
            if (Projectile.velocity.Length() > 50f)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 50f;
        }
        /// <summary>无目标时缓慢靠近玩家并把速度限制在 25。</summary>
        private void FollowOwner(Player player)
        {
            Vector2 toPlayer = player.Center - Projectile.Center;
            float distance = toPlayer.Length();
            if (distance > 100f)
            {
                float acceleration = distance < 140f ? 0.06f : distance < 200f ? 0.12f : 0.2f;
                Projectile.velocity += toPlayer.SafeNormalize(Vector2.UnitY) * acceleration;
            }
            else
            {
                Projectile.velocity *= 0.96f;
            }
            if (Math.Abs(Projectile.velocity.Y) < 1f)
                Projectile.velocity.Y -= 0.1f;
            if (Projectile.velocity.Length() > 25f)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 25f;
        }
        /// <summary>绘制本体和独立发光层。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition
                , null, Projectile.GetAlpha(lightColor), Projectile.rotation
                , texture.Size() / 2f, Projectile.scale, SpriteEffects.None);
            Texture2D glow = ModContent.Request<Texture2D>(
                "CalamityRebirth/Content/Projectiles/Summon/MechwormHeadGlow").Value;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null
                , Color.White, Projectile.rotation, glow.Size() / 2f, Projectile.scale
                , SpriteEffects.None);
            return false;
        }
    }
}
