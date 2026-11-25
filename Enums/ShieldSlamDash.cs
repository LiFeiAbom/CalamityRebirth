namespace CalamityRebirth.Enums
{
    /// <summary>
    /// 当前生效的盾牌冲刺类型。
    /// 数值顺序同时作为装备优先级，数字越大表示上位效果越强。
    /// </summary>
    public enum ShieldSlamDash
    {
        /// <summary>没有盾牌冲刺。</summary>
        None = 0,
        /// <summary>华丽盾的冰霜冲撞。</summary>
        OrnateShield,
        /// <summary>阿斯加德之英勇的基础冲撞。</summary>
        AsgardsValor,
        /// <summary>极乐之庇护的神圣冲撞。</summary>
        ElysianAegis,
        /// <summary>阿斯加德之庇护的宇宙冲撞。</summary>
        AsgardianAegis,
        /// <summary>魔影套装授予的黑红宇宙冲撞。</summary>
        Demonshade
    }
}
