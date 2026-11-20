namespace CalamityRebirth.Content.Projectiles.Melee
{
    /// <summary>
    /// 真·环境之刃发射的强化环境球。
    /// 沿用环境之刃的全部环境映射，并增加四格残影、400 像素追踪和更长的基础减益。
    /// </summary>
    internal sealed class TrueBiomeOrb : BiomeOrb
    {
        /// <summary>使用四格历史位置绘制强化拖尾。</summary>
        protected override int TrailLength => 4;
        /// <summary>只追踪 400 像素内的敌人，保持真·环境之刃的中近距离定位。</summary>
        protected override float TrackingDistance => 400f;
        /// <summary>强化层使用更醒目的 1.2 倍环境粉尘。</summary>
        protected override float DustScale => 1.2f;
        /// <summary>现代与 NPC 基础减益延长到六秒。</summary>
        protected override int NpcDebuffDuration => 360;
        /// <summary>PvP 同样维持六秒。</summary>
        protected override int PvpDebuffDuration => 360;
        /// <summary>经典灾厄专属减益也延长到六秒。</summary>
        protected override int ClassicModDebuffDuration => 360;
    }
}
