using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
    /// <summary>始源林海爆裂：停留 60 帧后形成一次 60×60 召唤伤害爆炸。</summary>
    public sealed class SilvaCrystalExplosion : ModProjectile
    {
        /// <summary>标记为召唤射击。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }
        /// <summary>设置 14×14 隐形判定框、穿透地形、水面和召唤伤害。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.DamageType = DamageClass.Summon;
        }
        /// <summary>淡入、旋转并绘制与母体连接的生命光束，60 帧后自毁。</summary>
        public override void AI()
        {
            Color color = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            int crystalIndex = (int)Projectile.ai[1];
            if (crystalIndex < 0 || crystalIndex >= Main.maxProjectiles
                || !Main.projectile[crystalIndex].active
                || Main.projectile[crystalIndex].type != ModContent.ProjectileType<SilvaCrystal>())
                Projectile.ai[1] = -1f;
            else
            {
                DelegateMethods.v3_1 = color.ToVector3() * 0.5f;
                Utils.PlotTileLine(Projectile.Center, Main.projectile[crystalIndex].Center
                    , 8f, DelegateMethods.CastLight);
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Main.rand.NextFloat() * 0.8f + 0.8f;
                Projectile.direction = Main.rand.Next(2) > 0 ? 1 : -1;
            }
            Projectile.rotation = Projectile.localAI[1] / 40f * MathHelper.TwoPi * Projectile.direction;
            if (Projectile.alpha > 0)
                Projectile.alpha = System.Math.Max(0, Projectile.alpha - 8);
            if (Projectile.alpha == 0)
                Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.5f);
            SpawnCoreDust(color);
            SpawnLeafDust(color);
            Projectile.scale = Projectile.Opacity / 2f * Projectile.localAI[0];
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] >= 60f)
                Projectile.Kill();
        }
        /// <summary>在核心周围按两组垂直方向生成少量彩虹尘。</summary>
        private void SpawnCoreDust(Color color)
        {
            for (int group = 0; group < 2; group++)
            {
                if (Main.rand.Next(10) != 0)
                    continue;
                Vector2 direction = Vector2.UnitY.RotatedBy(group * MathHelper.Pi)
                    .RotatedBy(Projectile.rotation);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2
                    , direction * 2.5f, 225, color, 1.5f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.scale = Projectile.Opacity * Projectile.localAI[0];
            }
            for (int group = 0; group < 2; group++)
            {
                if (Main.rand.Next(10) != 0)
                    continue;
                Vector2 direction = Vector2.UnitY.RotatedBy(group * MathHelper.Pi);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2
                    , direction * 2.5f, 225, color, 1.5f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.scale = Projectile.Opacity * Projectile.localAI[0];
            }
        }
        /// <summary>在核心外围空腔内生成上浮绿光叶尘。</summary>
        private void SpawnLeafDust(Color color)
        {
            if (Main.rand.Next(10) != 0)
                return;
            Vector2 offset = Utils.RandomVector2(Main.rand, -1f, 1f);
            if (offset != Vector2.Zero)
                offset.Normalize();
            offset *= 20f + Main.rand.NextFloat() * 100f;
            Vector2 position = Projectile.Center + offset;
            Point tile = position.ToTileCoordinates();
            if (!WorldGen.InWorld(tile.X, tile.Y, 0) || WorldGen.SolidTile(tile.X, tile.Y))
                return;
            Dust dust = Dust.NewDustPerfect(position, DustID.RainbowMk2
                , -Vector2.UnitY * (1f + Main.rand.NextFloat() * 2f)
                * (Main.rand.NextFloat() * 0.9f + 1.6f), 127, color, 1f + Main.rand.NextFloat());
            dust.noGravity = true;
            dust.fadeIn = 1f + Main.rand.NextFloat();
            dust.noLight = true;
            Dust clone = Dust.CloneDust(dust);
            clone.scale *= 0.65f;
            clone.fadeIn *= 0.65f;
            clone.color = Color.White;
        }
        /// <summary>绘制旋转与正向两层彩色棱晶，以及朝叶棱晶延伸的光束。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Size() / 2f;
            Color color = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f)
                .MultiplyRGBA(new Color(255, 255, 255, 0));
            Main.EntitySpriteDraw(texture, drawPosition, frame, color, Projectile.rotation
                , origin, Projectile.scale * 2f, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, drawPosition, frame, color, 0f
                , origin, Projectile.scale * 2f, SpriteEffects.None);
            int crystalIndex = (int)Projectile.ai[1];
            if (crystalIndex < 0 || crystalIndex >= Main.maxProjectiles
                || !Main.projectile[crystalIndex].active
                || Main.projectile[crystalIndex].type != ModContent.ProjectileType<SilvaCrystal>()
                || Projectile.Opacity <= 0.3f)
                return false;
            Vector2 toCrystal = Main.projectile[crystalIndex].Center - Projectile.Center;
            Vector2 beamScale = new(1f, toCrystal.Length() / texture.Height);
            float rotation = toCrystal.ToRotation() + MathHelper.PiOver2;
            float alpha = MathHelper.Clamp(MathHelper.Distance(30f, Projectile.localAI[1]) / 20f
                , 0f, 1f);
            if (alpha <= 0f)
                return false;
            Vector2 beamPosition = drawPosition + toCrystal / 2f;
            Main.EntitySpriteDraw(texture, beamPosition, frame, color * alpha, rotation
                , origin, beamScale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, beamPosition, frame, Projectile.GetAlpha(lightColor)
                * alpha, rotation, origin, beamScale / 2f, SpriteEffects.None);
            return false;
        }
        /// <summary>本体仅通过 PreDraw 显示彩色光效。</summary>
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha
                , 255 - Projectile.alpha, 0);
        }
        /// <summary>消散时先喷出两圈彩虹尘，再以 60×60 判定框结算一次召唤伤害。</summary>
        public override void OnKill(int timeLeft)
        {
            Color color = Main.hslToRgb(Projectile.ai[0], 1f, 0.5f);
            color.A = 255;
            SpawnBurstDust(color, Projectile.Center);
            if (Projectile.owner != Main.myPlayer)
                return;
            int width = Projectile.width;
            int height = Projectile.height;
            int penetrate = Projectile.penetrate;
            Vector2 center = Projectile.Center;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 60;
            Projectile.Center = center;
            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            Projectile.penetrate = penetrate;
            Projectile.width = width;
            Projectile.height = height;
            Projectile.Center = center;
        }
        /// <summary>生成两圈方向、速度和尺寸略有差异的彩虹爆尘。</summary>
        private static void SpawnBurstDust(Color color, Vector2 center)
        {
            Vector2 start = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
            int count = Main.rand.Next(7, 13);
            Vector2 speedScale = new(2.1f, 2f);
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = start.RotatedBy(MathHelper.TwoPi * i / count) * speedScale
                    * (0.8f + Main.rand.NextFloat() * 0.4f);
                Dust dust = Dust.NewDustPerfect(center, DustID.RainbowMk2
                    , velocity, 0, color, 2f);
                dust.noGravity = true;
                dust.fadeIn = Main.rand.NextFloat() * 2f;
                Dust clone = Dust.CloneDust(dust);
                clone.scale /= 2f;
                clone.fadeIn /= 2f;
                clone.color = Color.White;
            }
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = start.RotatedBy(MathHelper.TwoPi * i / count) * speedScale
                    * (0.8f + Main.rand.NextFloat() * 0.4f) * (Main.rand.NextFloat() * 0.8f);
                Dust dust = Dust.NewDustPerfect(center, DustID.RainbowMk2
                    , velocity, 0, color, Main.rand.NextFloat());
                dust.noGravity = true;
                dust.fadeIn = Main.rand.NextFloat() * 2f;
                Dust clone = Dust.CloneDust(dust);
                clone.scale /= 2f;
                clone.fadeIn /= 2f;
                clone.color = Color.White;
            }
        }
    }
}
