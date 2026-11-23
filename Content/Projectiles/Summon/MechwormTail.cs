using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Projectiles.Summon
{
    /// <summary>噬神机械蠕虫尾节，不占仆从栏。</summary>
    public sealed class MechwormTail : ModProjectile
    {
        /// <summary>创建时仆从栏上限。</summary>
        private int initialMaxMinions;
        /// <summary>首帧初始化标记。</summary>
        private bool initialized;
        /// <summary>注册为可牺牲召唤物。</summary>
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }
        /// <summary>设置尾节基础属性。</summary>
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }
        /// <summary>跟随父节点，父节点消失时尾节自毁。</summary>
        public override void AI()
        {
            MechwormSegmentUtil.Update(Projectile, this, ref initialized, ref initialMaxMinions, true);
        }
        /// <summary>绘制尾节。</summary>
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null
                , Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f
                , Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
