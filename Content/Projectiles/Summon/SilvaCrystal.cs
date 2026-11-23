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
    /// <summary>远古叶棱晶：悬浮于玩家头顶，每 25 帧向目标发射三枚生命爆裂。</summary>
    public sealed class SilvaCrystal : ModProjectile
    {
        /// <summary>注册为可牺牲且接受玩家锁定目标的召唤物。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        /// <summary>设置 32×32 碰撞箱、零仆从栏占用、无限穿透和召唤伤害。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 90000;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
        }
        /// <summary>跟随玩家头顶、筛选 1500 像素内目标并按状态机发射三枚爆裂。</summary>
        public override void AI()
        {
            if (!Main.player.IndexInRange(Projectile.owner))
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            if (!modPlayer.silvaSummon)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
                modPlayer.hasSilvaCrystal = false;
            if (modPlayer.hasSilvaCrystal)
                Projectile.timeLeft = 2;
            Projectile.Center = new Vector2(player.Center.X
                , player.Center.Y + player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                Projectile.Center += new Vector2(0f, 120f);
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.velocity = Vector2.Zero;
            if (Projectile.alpha > 0)
                Projectile.alpha = Math.Max(0, Projectile.alpha - 5);
            if (Projectile.direction == 0)
                Projectile.direction = player.direction;
            if (Projectile.alpha == 0 && Main.rand.NextBool(15))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center
                    + Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi)
                    * (4f * Main.rand.NextFloat() + 26f), DustID.RainbowMk2
                    , Vector2.Zero, 100, new Color(Main.DiscoR, 203, 103), 0.5f);
                dust.noGravity = true;
                dust.fadeIn = 1f;
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 60f)
                Projectile.localAI[0] = 0f;
            if (Projectile.ai[0] < 0f)
                Projectile.ai[0]++;
            if (Projectile.ai[0] == 0f)
            {
                int targetIndex = FindTarget(player);
                if (targetIndex >= 0)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = targetIndex;
                    Projectile.netUpdate = true;
                }
                return;
            }
            int trackedIndex = (int)Projectile.ai[1];
            if (!Main.npc.IndexInRange(trackedIndex)
                || !Main.npc[trackedIndex].CanBeChasedBy(Projectile))
            {
                Projectile.ai[0] = 0f;
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
                return;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 5f)
                return;
            NPC target = Main.npc[trackedIndex];
            Vector2 toTarget = Projectile.DirectionTo(target.Center);
            if (float.IsNaN(toTarget.X) || float.IsNaN(toTarget.Y))
                toTarget = Vector2.UnitY;
            Projectile.direction = toTarget.X > 0f ? 1 : -1;
            Projectile.ai[0] = -20f;
            Projectile.netUpdate = true;
            if (Projectile.owner != Main.myPlayer)
                return;
            Vector2 randomOffset = target.position + target.Size
                * Utils.RandomVector2(Main.rand, 0f, 1f) - Projectile.Center;
            float hue = Main.rgbToHsl(new Color(Main.DiscoR, 203, 103)).X;
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawn = Projectile.Center + randomOffset;
                if (i > 0)
                {
                    spawn = Projectile.Center + randomOffset.RotatedByRandom(MathHelper.PiOver4)
                        * (Main.rand.NextFloat() * 0.5f + 0.75f);
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, Vector2.Zero
                    , ModContent.ProjectileType<SilvaCrystalExplosion>(), Projectile.damage
                    , Projectile.knockBack, Projectile.owner, hue, Projectile.whoAmI);
            }
        }
        /// <summary>优先使用玩家锁定目标，否则选取视线可达的最近敌怪。</summary>
        private int FindTarget(Player player)
        {
            int targetIndex = -1;
            float range = 1500f;
            if (player.HasMinionAttackTargetNPC && Main.npc.IndexInRange(player.MinionAttackTargetNPC))
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float distance = Projectile.Distance(npc.Center);
                if (npc.CanBeChasedBy(Projectile) && distance < range
                    && Collision.CanHitLine(Projectile.Center, 0, 0, npc.Center, 0, 0))
                {
                    range = distance;
                    targetIndex = npc.whoAmI;
                }
            }
            if (targetIndex >= 0)
                return targetIndex;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float distance = Projectile.Distance(npc.Center);
                if (!npc.CanBeChasedBy(Projectile) || distance >= range
                    || !Collision.CanHitLine(Projectile.Center, 0, 0, npc.Center, 0, 0))
                    continue;
                range = distance;
                targetIndex = npc.whoAmI;
            }
            return targetIndex;
        }
        /// <summary>随淡入进度降低透明度，最透明时 alpha 仍保留 127。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha
                , 255 - Projectile.alpha, 127 - Projectile.alpha / 2);
        }
        /// <summary>将同一贴图按 90 度叠绘四次，形成呼吸式十字棱晶光晕。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);
            float offset = (float)Math.Cos(MathHelper.TwoPi * Projectile.localAI[0] / 60f) + 6f;
            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(texture, drawPosition
                    + Vector2.UnitY.RotatedBy(i * MathHelper.PiOver2) * offset, frame
                    , color * 0.2f, Projectile.rotation, origin, Projectile.scale
                    , SpriteEffects.None);
            }
            return false;
        }
        /// <summary>棱晶本体只负责发射爆裂，不造成接触伤害。</summary>
        public override bool? CanDamage()
        {
            return false;
        }
    }
}
