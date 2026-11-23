using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Healing
{
    /// <summary>真菌治疗珠：真菌团块吸血后飞向归属玩家的透明治疗弹幕。</summary>
    internal sealed class FungalHeal : ModProjectile
    {
        /// <summary>视觉由蓝色妖精尘承担。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置微小判定、十次额外更新与 300 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 300;
        }
        /// <summary>朝 ai[0] 指定玩家平滑追踪，接触后回复 ai[1] 点生命。</summary>
        public override void AI()
        {
            int playerIndex = (int)Projectile.ai[0];
            if (!Main.player.IndexInRange(playerIndex) || !Main.player[playerIndex].active || Main.player[playerIndex].dead)
            {
                Projectile.Kill();
                return;
            }
            Player target = Main.player[playerIndex];
            Vector2 center = Projectile.Center;
            float distance = Vector2.Distance(center, target.Center);
            if (distance < 50f && Projectile.Hitbox.Intersects(target.Hitbox))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && !target.moonLeech)
                {
                    int heal = (int)Projectile.ai[1];
                    target.Heal(heal);
                    target.HealEffect(heal, true);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null
                            , playerIndex, target.statLife, target.statMana);
                }
                Projectile.Kill();
                return;
            }
            Vector2 desiredVelocity = (target.Center - center).SafeNormalize(Vector2.UnitY) * 5f;
            Projectile.velocity = (Projectile.velocity * 9f + desiredVelocity) / 16f;
            int largeDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.BlueFairy, 0f, 0f, 100, default, 0.5f);
            Main.dust[largeDust].noGravity = true;
            Main.dust[largeDust].velocity = Vector2.Zero;
            Main.dust[largeDust].position -= Projectile.velocity * 0.334f;
            int smallDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.BlueFairy, 0f, 0f, 100, default, 0.7f);
            Main.dust[smallDust].noGravity = true;
            Main.dust[smallDust].velocity = Vector2.Zero;
            Main.dust[smallDust].position -= Projectile.velocity * 0.2f;
        }
    }
}
