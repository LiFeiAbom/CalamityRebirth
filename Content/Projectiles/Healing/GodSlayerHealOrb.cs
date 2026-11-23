using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Healing
{
    /// <summary>弑神治疗球：追踪指定队友并由服务端结算治疗。</summary>
    public sealed class GodSlayerHealOrb : ModProjectile
    {
        /// <summary>设置 4×4 判定、240 帧寿命和额外三次更新。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 3;
        }
        /// <summary>朝 ai[0] 玩家追踪，接触后回复 ai[1] 点生命。</summary>
        public override void AI()
        {
            int targetIndex = (int)Projectile.ai[0];
            if (!Main.player.IndexInRange(targetIndex) || !Main.player[targetIndex].active
                || Main.player[targetIndex].dead)
            {
                Projectile.Kill();
                return;
            }
            Player target = Main.player[targetIndex];
            if (Vector2.Distance(Projectile.Center, target.Center) < 50f
                && Projectile.Hitbox.Intersects(target.Hitbox))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && !target.moonLeech)
                {
                    int heal = (int)Projectile.ai[1];
                    target.Heal(heal);
                    target.HealEffect(heal, true);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.PlayerLifeMana, -1, -1, null
                            , targetIndex, target.statLife, target.statMana);
                    }
                }
                Projectile.Kill();
                return;
            }
            Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 6.5f;
            Projectile.velocity = (Projectile.velocity * 15f + desired) / 16f;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
        }
    }
}
