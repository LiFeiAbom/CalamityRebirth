using Terraria.Audio;
namespace CalamityRebirth.Sounds
{
    /// <summary>
    /// 本模组集中注册的自定义音效。
    /// 通过静态 SoundStyle 复用同一资源，避免每次播放时重复创建配置对象。
    /// </summary>
    internal static class CalamityRebirthSounds
    {
        /// <summary>宇宙冲刺爆炸使用的体节碎裂低频冲击。</summary>
        public static readonly SoundStyle DevourerSegmentBreak =
            new("CalamityRebirth/Sounds/Custom/DevourerSegmentBreak1") { Volume = 0.3f };
        /// <summary>神圣爆破撞击使用的高能冲击音。</summary>
        public static readonly SoundStyle ProvidenceHolyBlastImpact =
            new("CalamityRebirth/Sounds/Custom/ProvidenceHolyBlastImpact") { Volume = 0.6f };
    }
}
