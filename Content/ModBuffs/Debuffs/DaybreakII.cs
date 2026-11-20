using CalamityRebirth.NPCs;
using Microsoft.Xna.Framework;
using CalamityRebirth.Content.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.ModBuffs.Debuffs
{
    /// <summary>
    /// 破晓II（DaybreakII） debuff：由「烈火手套EX」的近战命中（物品挥砍与近战弹幕都算）附加，
    /// **敌怪与玩家都生效**（玩家侧由 PvP 命中施加，见 <see cref="CalamityRebirth.Players.CalamityRebirthPlayer"/>）。
    /// 结算分工：置位在本类；敌怪侧数值与表现放在 <see cref="CalamityRebirthGlobalNPC"/>
    /// （掉血、-25 防御、×1.10 最终伤害、灼烧粒子、死亡爆尘与传染）；
    /// 玩家侧数值放在 <c>CalamityRebirthPlayer</c>（UpdateBadLifeRegen / PostUpdateEquips / ModifyHurt），
    /// 玩家侧的橙红火花在 CalamityRebirthPlayer.DrawEffects 里出。
    /// 视觉沿用原燃烧IV：橙红火花 + MiracleBlight 式噪声溶解；减益图标仍使用破晓图标。
    /// 多人：增益本身靠 AddBuff 的同步（非 quiet）走服务器，数值各自在本地算（玩家生命以本人客户端为准），
    /// 死亡传染只在服务端做一次（照原版 <c>Main.netMode != MultiplayerClient</c> 的判据）。
    /// </summary>
    internal class DaybreakII:ModBuff
    {
        // ── 数值口径（单一来源：GlobalNPC 与 ModPlayer 都读这里） ──
        /// <summary>
        /// 掉血档：<c>lifeRegen -= 500</c>；敌怪与玩家同一套换算（lifeRegen ÷ 2 = 每秒），即每秒 250
        /// </summary>
        public const int BurnLifeRegen = 500;
        /// <summary>
        /// 防御降低量（Flat）：敌怪走 <c>HitModifiers.Defense</c>，玩家走 <c>Player.statDefense</c>
        /// </summary>
        public const float DefenseLoss = 25f;
        /// <summary>
        /// 受到伤害的倍率（1.1 = 额外 +10%）
        /// </summary>
        public const float DamageTakenMultiplier = 1.1f;
        /// <summary>
        /// 死亡传染半径（像素，原版 100）
        /// </summary>
        public const float SpreadRadius = 100f;
        /// <summary>
        /// 死亡传染给附近敌怪的时长（帧，原版 300）
        /// </summary>
        public const int SpreadDuration = 300;
        /// <summary>破晓II 溶解时使用的橙红发光色。</summary>
        public static readonly Color GlowColor = new Color(255, 84, 16);
        /// <summary>
        /// 注册为减益：参与 PvP、不随存档保存，且专家/大师模式下延长持续时间
        /// </summary>
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }
        /// <summary>
        /// NPC 版 Update：每帧置位 GlobalNPC 标记，供掉血、减防与粒子结算读取
        /// </summary>
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CalamityRebirthGlobalNPC>().daybreakII = true;
        }
        /// <summary>
        /// 玩家侧视觉：完整对齐 MiracleBlight.DrawEffects(PlayerDrawSet)——
        /// 1/2 概率 RainbowTorch 彩尘，并且每轮绘制生成一枚 GlowSpark；
        /// 原版四色随机替换为燃烧橙红色阶。
        /// </summary>
        public static void DrawEffects(PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            Color sparkColor = RandomSparkColor();
            // 玩家侧按原版 MiracleBlight：彩尘 1/2 概率，火花每轮绘制固定一枚。
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(RandomDebuffVisualSpot(player), DustID.RainbowTorch, RandomSpawnVelocity());
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 0.85f);
                dust.color = sparkColor;
            }
            Vector2 velOffset = RandomSpawnVelocity() * Main.rand.NextFloat(2f, 13f);
            DaybreakParticleSystem.SpawnParticle(player.Center + player.velocity * 3f + velOffset * 1.5f
                , -velOffset * 0.25f, sparkColor);
        }
        /// <summary>
        /// 敌怪侧视觉：完整对齐 MiracleBlight.DrawEffects(NPC)——1/4 概率彩尘、1/3 概率 GlowSpark；
        /// 速度、体积补偿、寿命和绘制参数均沿用原版，仅替换为燃烧橙红色阶。
        /// </summary>
        public static void DrawEffects(NPC npc)
        {
            // 敌怪侧按原版 MiracleBlight：彩尘 1/4、火花 1/3，并增加按体积计算的缩放和速度补偿。
            Vector2 spawn = npc.Center + new Vector2(
                Main.rand.NextFloat(-npc.width / 2f, npc.width / 2f),
                Main.rand.NextFloat(-npc.height / 2f, npc.height / 2f));
            Color sparkColor = RandomSparkColor();
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(spawn, DustID.RainbowTorch, RandomSpawnVelocity());
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.7f, 0.85f) + 0.0000007f * npc.width * npc.height;
                dust.color = sparkColor;
            }
            if (!Main.rand.NextBool(3))
                return;
            Vector2 velOffset = RandomSpawnVelocity() * (Main.rand.NextFloat(5f, 9f) + 0.0002f * npc.width * npc.height);
            DaybreakParticleSystem.SpawnParticle(npc.Center + npc.velocity * 3f + velOffset * 1.5f
                , -velOffset * 0.25f, sparkColor);
        }
        /// <summary>从燃烧橙红四阶色带中随机取色，替代原版 MiracleBlight 的四色彩虹。</summary>
        private static Color RandomSparkColor() => Main.rand.Next(4) switch
        {
            0 => new Color(255, 46, 0),
            1 => new Color(255, 96, 10),
            2 => new Color(255, 140, 28),
            _ => new Color(255, 190, 62),
        };
        /// <summary>在玩家身体附近生成用于减益尘的随机坐标。</summary>
        private static Vector2 RandomDebuffVisualSpot(Player player) => player.Center
            + new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-20f, 20f));
        /// <summary>
        /// 复刻灾厄 RandomVelocity 的归一化随机方向，并把长度限制在约 2.8 到 6.0。
        /// 循环仅处理理论上的零向量，避免 Normalize 产生 NaN。
        /// </summary>
        // 复刻 CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f)：随机方向归一化后取 2.8~6.0 速度。
        private static Vector2 RandomSpawnVelocity()
        {
            Vector2 velocity = new Vector2(Main.rand.NextFloat(-100f, 100f), Main.rand.NextFloat(-100f, 100f));
            while (velocity.X == 0f && velocity.Y == 0f)
                velocity = new Vector2(Main.rand.NextFloat(-100f, 100f), Main.rand.NextFloat(-100f, 100f));
            velocity.Normalize();
            return velocity * Main.rand.NextFloat(70f, 150f) * 0.04f;
        }
        /// <summary>
        /// 原版破晓的死亡表现：照抄 NPC.VanillaHitEffect 里 daybreak 那一支。
        /// 先在死亡位置喷 20 枚随机尘（Torch 6 / SolarFlare 259 / OrangeTorch 158 三选一，
        /// alpha 200、速度 ×1.4、scale 再加一枚 0~1 的随机数），
        /// 再由服务端把破晓传染给 100 像素内、未免疫的敌怪（300 帧）——判据与原版逐条对齐
        /// （active、未免疫、距离 < 100、非 dontTakeDamage、lifeMax > 5、非 friendly、非 townNPC）。
        /// </summary>
        public static void DeathEffect(NPC npc)
        {
            // 原版用 npc.Hitbox.TopLeft() 作为喷尘原点，宽高取 NPC 本体；速度固定 (0, -2.5)
            Rectangle hitbox = npc.Hitbox;
            for (int i = 0; i < 20; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, DustID.Torch, DustID.SolarFlare, DustID.OrangeTorch);
                Dust dust = Main.dust[Dust.NewDust(hitbox.TopLeft(), npc.width, npc.height, dustType, 0f, -2.5f, 0, default, 1f)];
                dust.alpha = 200;
                dust.velocity *= 1.4f;
                dust.scale += Main.rand.NextFloat();
            }
            // 传染只在服务端（或单机）做一次，客户端不做，避免离屏 NPC 的增益不同步
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            int buffType = ModContent.BuffType<DaybreakII>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];
                if (other.active && !other.buffImmune[buffType] && npc.Distance(other.Center) < SpreadRadius
                    && !other.dontTakeDamage && other.lifeMax > 5 && !other.friendly && !other.townNPC)
                {
                    other.AddBuff(buffType, SpreadDuration);
                }
            }
        }
    }
}
