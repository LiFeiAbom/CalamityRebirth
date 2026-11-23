using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Healing
{
    /// <summary>始源林海治疗叶球：追踪指定队友并在接触后由服务端验证治疗。</summary>
    public sealed class SilvaOrb : ModProjectile
    {
        /// <summary>使用通用隐形贴图，视觉由叶绿粉尘承担。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置 14×14 碰撞箱、单次吸收、300 帧寿命和额外三次更新。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
            Projectile.light = 0.2f;
            Projectile.DamageType = DamageClass.Generic;
        }
        /// <summary>淡入、脉动、追踪 ai[0] 指定玩家，并在接触后结算 ai[1] 点治疗。</summary>
        public override void AI()
        {
            Projectile.alpha -= 2;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale += 0.05f;
                if (Projectile.scale > 1.2f)
                    Projectile.localAI[0] = 1f;
            }
            else
            {
                Projectile.scale -= 0.05f;
                if (Projectile.scale < 0.8f)
                    Projectile.localAI[0] = 0f;
            }
            int playerIndex = (int)Projectile.ai[0];
            if (!Main.player.IndexInRange(playerIndex) || !Main.player[playerIndex].active
                || Main.player[playerIndex].dead)
            {
                Projectile.Kill();
                return;
            }
            Player target = Main.player[playerIndex];
            float distance = Vector2.Distance(Projectile.Center, target.Center);
            if (distance < 50f && Projectile.Hitbox.Intersects(target.Hitbox))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && !target.moonLeech)
                {
                    int heal = (int)Projectile.ai[1];
                    if (heal > 0)
                    {
                        target.Heal(heal);
                        target.HealEffect(heal, true);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null
                                , playerIndex, target.statLife, target.statMana);
                        }
                    }
                }
                Projectile.Kill();
                return;
            }
            Vector2 desiredVelocity = (target.Center - Projectile.Center)
                .SafeNormalize(Vector2.UnitY) * 6f;
            Projectile.velocity = (Projectile.velocity * 15f + desiredVelocity) / 16f;
        }
        /// <summary>以循环彩虹红通道与固定叶绿色绘制隐形弹幕。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 203, 103, Projectile.alpha);
        }
        /// <summary>消散时喷出五枚叶绿粉尘。</summary>
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.ChlorophyteWeapon, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.5f;
                Main.dust[dust].scale = 1.5f;
            }
        }
    }
}
