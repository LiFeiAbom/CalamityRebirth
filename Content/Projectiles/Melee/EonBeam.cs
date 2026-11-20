using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 永恒光束：远古方舟、元素方舟与鸿蒙方舟共用的高速穿透光束。
    /// ai[0] 为一时光束不附加元素减益，ai[1] 决定紫、青、天蓝或橙色绘制。
    /// </summary>
    internal sealed class EonBeam : ModProjectile
    {
        /// <summary>ai[1] 的颜色索引、贴图选择和绘制选色共用同一字段。</summary>
        private ref float UseTexture => ref Projectile.ai[1];
        /// <summary>
        /// 光束具有 20×20 碰撞箱、三次穿透、500 帧寿命、每帧额外更新一次，
        /// 并对同一 NPC 使用十帧本地冷却。
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 500;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.extraUpdates = 1;
        }
        /// <summary>
        /// 弹体随速度旋转，弹头生成神圣系粉尘；飞行七帧后追加彩虹拖尾与蓝色照明。
        /// </summary>
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.localAI[1]++;
            int dustType = Main.rand.Next(3) switch
            {
                0 => 15,
                1 => 57,
                _ => 58,
            };
            Vector2 headPosition = new(
                Projectile.position.X - Projectile.velocity.X * 4f + 2f,
                Projectile.position.Y + 2f - Projectile.velocity.Y * 4f);
            int headIndex = Dust.NewDust(headPosition, 8, 8, dustType, 0f, 0f, 100, default, 1.25f);
            Dust headDust = Main.dust[headIndex];
            headDust.velocity *= 0.1f;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.4f, 1f);
            if (Projectile.localAI[1] > 7f)
            {
                int trailIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.RainbowTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f
                    , 150, new Color(53, Main.DiscoG, 255), 1.2f);
                Dust trail = Main.dust[trailIndex];
                trail.velocity *= 0.1f;
                trail.noGravity = true;
            }
        }
        /// <summary>
        /// 普通方舟光束命中 NPC 时施加霜火，并按当前灾厄版本追加硫磺火、圣火/圣光与瘟疫。
        /// 真·远古方舟通过 ai[0] 标记跳过该组效果。
        /// </summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ApplyBeamDebuffs(target);
        }
        /// <summary>PvP 命中使用与 NPC 相同的元素减益规则。</summary>
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            ApplyBeamDebuffs(target);
        }
        /// <summary>寿命结束后生成一圈青色消散尘。</summary>
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 7; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height
                    , DustID.RainbowTorch, 0f, 0f, 150
                    , new Color(53, Main.DiscoG, 255), 1.2f);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true;
            }
        }
        /// <summary>
        /// 出生十帧内隐藏以避免光照和贴图跳变，之后按 ai[1] 选择颜色并沿用近战弹幕握持端绘制信息。
        /// </summary>
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 490)
                return false;
            UseTexture = MathHelper.Clamp(UseTexture, 1f, 4f);
            Color beamColor = UseTexture switch
            {
                1f => Color.Violet,
                2f => Color.Turquoise,
                3f => Color.SkyBlue,
                _ => Color.Orange,
            };
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation
                + (Projectile.spriteDirection == -1 ? MathHelper.PiOver2 + MathHelper.PiOver4 : MathHelper.PiOver4);
            Vector2 origin = Projectile.spriteDirection == -1
                ? new Vector2(texture.Width, texture.Height)
                : new Vector2(0f, texture.Height);
            SpriteEffects effects = Projectile.spriteDirection == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(beamColor)
                , drawRotation - MathHelper.PiOver4, texture.Size() / 2f, Projectile.scale, effects, 0f);
            return false;
        }
        /// <summary>保留外部传入的颜色透明度，不做额外光照染色。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor;
        }
        /// <summary>
        /// 根据当前加载版本施加光束元素减益；没有灾厄时仅保留霜火。
        /// </summary>
        private void ApplyBeamDebuffs<T>(T target)
        {
            if (Projectile.ai[0] == 1f)
                return;
            switch (target)
            {
                case NPC npc:
                    npc.AddBuff(BuffID.Frostburn, 120);
                    break;
                case Player player:
                    player.AddBuff(BuffID.Frostburn, 120);
                    break;
            }
            if (ModLoader.HasMod("CalamityMod"))
            {
                TryAddExternalBuff(target, "CalamityMod", "BrimstoneFlames", 120);
                TryAddExternalBuff(target, "CalamityMod", "HolyFlames", 120);
                TryAddExternalBuff(target, "CalamityMod", "Plague", 120);
            }
            else if (ModLoader.HasMod("CalRD"))
            {
                TryAddExternalBuff(target, "CalRD", "BrimstoneFlames", 120);
                TryAddExternalBuff(target, "CalRD", "HolyFlames", 120);
                TryAddExternalBuff(target, "CalRD", "Plague", 120);
            }
            else if (ModLoader.HasMod("CalamityModClassicPreTrailer"))
            {
                TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                    , "BrimstoneFlames", 120);
                TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                    , "HolyLight", 120);
                TryAddExternalBuff(target, "CalamityModClassicPreTrailer"
                    , "Plague", 120);
            }
        }
        /// <summary>把光束元素减益施加到 NPC 或 PvP 玩家目标。</summary>
        private static bool TryAddExternalBuff<T>(T target, string modName, string buffName, int duration)
        {
            return target switch
            {
                NPC npc => CalamityCompatUtil.TryAddBuff(npc, modName, buffName, duration),
                Player player => CalamityCompatUtil.TryAddBuff(player, modName, buffName, duration),
                _ => false,
            };
        }
    }
}
