using Terraria;
using Terraria.ID;
namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 欧米茄环境之刃发射的终阶环境球。
    /// 除普通群系外还会读取四柱环境，并在命中时按当前月亮事件为发射者提供增益。
    /// </summary>
    internal sealed class OmegaBiomeOrb : BiomeOrb
    {
        /// <summary>使用六格历史位置形成完整拖尾。</summary>
        protected override int TrailLength => 6;
        /// <summary>欧米茄环境球沿用原生的 120 帧短寿命。</summary>
        protected override int InitialLifeTime => 120;
        /// <summary>欧米茄环境球只追踪 200 像素内的近距离目标。</summary>
        protected override float TrackingDistance => 200f;
        /// <summary>终阶环境球使用更亮的 1.2 倍粉尘。</summary>
        protected override float DustScale => 1.2f;
        /// <summary>启用星云、星尘、日耀和星旋四柱配色与粉尘。</summary>
        protected override bool IncludesLunarTowers => true;
        /// <summary>NPC 与 PvP 环境减益统一持续十秒。</summary>
        protected override int NpcDebuffDuration => 600;
        /// <summary>PvP 环境减益持续十秒。</summary>
        protected override int PvpDebuffDuration => 600;
        /// <summary>经典灾厄专属减益持续十秒。</summary>
        protected override int ClassicModDebuffDuration => 600;
        /// <summary>
        /// 命中时按月亮事件追加对应增益：血月给予战斗，霜月给予快速治疗，南瓜月给予饱食。
        /// 三个事件使用独立判断。
        /// </summary>
        protected override void ApplyOwnerOnHitBuff(Player owner)
        {
            if (Main.bloodMoon)
                owner.AddBuff(BuffID.Battle, 600);
            if (Main.snowMoon)
                owner.AddBuff(BuffID.RapidHealing, 600);
            if (Main.pumpkinMoon)
                owner.AddBuff(BuffID.WellFed, 600);
        }
    }
}
