using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Typeless
{
    /// <summary>
    /// 站火：灾厄之环、终结虚空与融合之脑在无敌期间降下的隐形火苗。
    /// 弹幕受重力影响并在地面持续燃烧，命中时施加当前灾厄版本的硫磺火。
    /// </summary>
    internal sealed class StandingFire : ModProjectile
    {
        /// <summary>隐形表现，视觉由生命汲取粉尘承担。</summary>
        public override string Texture => "CalamityRebirth/Content/Projectiles/InvisibleProj";
        /// <summary>设置微小碰撞箱、无限穿透与 240 帧寿命。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
        }
        /// <summary>前五帧缓慢点火，随后施加重力、贴地减速并生成红色上升尘。</summary>
        public override void AI()
        {
            Projectile.ai[0] = MathHelper.Min(Projectile.ai[0] + 1f, 5f);
            if (Projectile.ai[0] >= 5f)
            {
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if (MathHelper.Distance(Projectile.velocity.X, 0f) < 0.01f)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity.Y = MathHelper.Min(Projectile.velocity.Y + 0.2f, 16f);
            }
            if (Projectile.velocity.Y > 0.15f && Projectile.velocity.Y < 0.25f)
                Projectile.velocity.X *= 0.8f;
            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.LifeDrain, 0f, 0f, 100, default, 1f);
            Dust dust = Main.dust[dustIndex];
            dust.position += new Vector2(-2f, 2f);
            dust.scale += Main.rand.Next(50) * 0.01f;
            dust.noGravity = true;
            dust.velocity.Y -= 2f;
            if (!Main.rand.NextBool(2))
                return;
            int secondDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                , DustID.LifeDrain, 0f, 0f, 100, default, 1f);
            Dust ember = Main.dust[secondDust];
            ember.position += new Vector2(-2f, 2f);
            ember.scale += 0.3f + Main.rand.Next(50) * 0.01f;
            ember.noGravity = true;
            ember.velocity *= 0.1f;
        }
        /// <summary>命中 NPC 时限制本地免疫窗口并施加 160 帧硫磺火。</summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 7;
            ApplyBrimstoneFlames(target, 160);
        }
        /// <summary>PvP 命中也施加与 NPC 等长的硫磺火。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ApplyBrimstoneFlames(target, 160);
        }
        /// <summary>站火保持穿透，不因地形碰撞销毁。</summary>
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        /// <summary>按已启用灾厄分支弱依赖查找并施加同名硫磺火减益。</summary>
        internal static void ApplyBrimstoneFlames(Entity target, int duration)
        {
            string[] branchNames = ["CalamityMod", "CalamityModClassicPreTrailer", "CalRD"];
            foreach (string branchName in branchNames)
            {
                if (!ModLoader.TryGetMod(branchName, out Mod calamity)
                    || !calamity.TryFind<ModBuff>("BrimstoneFlames", out ModBuff brimstoneFlames))
                    continue;
                if (target is NPC npc)
                    npc.AddBuff(brimstoneFlames.Type, duration);
                else if (target is Player player)
                    player.AddBuff(brimstoneFlames.Type, duration);
                return;
            }
        }
    }
}
