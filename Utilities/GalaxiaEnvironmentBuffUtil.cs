using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Utilities
{
    /// <summary>
    /// 银河与鸿蒙方舟本体命中时的环境增益表。
    /// 弹幕命中在各自类中额外处理目标减益和二次弹幕，这里只负责攻击者增益。
    /// </summary>
    internal static class GalaxiaEnvironmentBuffUtil
    {
        /// <summary>
        /// 按月亮事件、主群系和星陨环境提供对应增益。
        /// pumpkinBuff 允许银河使用普通饱食、鸿蒙方舟使用三级饱食。
        /// </summary>
        /// <param name="player">需要获得环境增益的攻击者。</param>
        /// <param name="pumpkinBuff">南瓜月分支使用的饱食增益类型。</param>
        internal static void Apply(Player player, int pumpkinBuff)
        {
            if (Main.bloodMoon)
                player.AddBuff(BuffID.Battle, 600);
            if (Main.snowMoon)
                player.AddBuff(BuffID.RapidHealing, 600);
            if (Main.pumpkinMoon)
                player.AddBuff(pumpkinBuff, 600);
            else if (player.ZoneJungle)
                player.AddBuff(BuffID.Thorns, 600);
            else if (player.ZoneSnow)
                player.AddBuff(BuffID.Warmth, 600);
            else if (player.ZoneBeach)
                player.AddBuff(BuffID.Wet, 600);
            else if (player.ZoneCorrupt)
                player.AddBuff(BuffID.Wrath, 600);
            else if (player.ZoneCrimson)
                player.AddBuff(BuffID.Rage, 600);
            else if (player.ZoneDungeon)
                player.AddBuff(BuffID.Dangersense, 600);
            else if (player.ZoneDesert)
                player.AddBuff(BuffID.Endurance, 600);
            else if (player.ZoneGlowshroom)
                player.AddBuff(BuffID.Spelunker, 600);
            else if (player.ZoneUnderworldHeight)
                player.AddBuff(BuffID.Inferno, 600);
            else if (player.ZoneHallow)
                player.AddBuff(BuffID.Heartreach, 600);
            else if (player.ZoneTowerNebula)
                player.AddBuff(BuffID.MagicPower, 600);
            else if (player.ZoneTowerStardust)
                player.AddBuff(BuffID.Summoning, 600);
            else if (player.ZoneTowerSolar)
                player.AddBuff(BuffID.Titan, 600);
            else if (player.ZoneTowerVortex)
                player.AddBuff(BuffID.AmmoReservation, 600);
            else
                player.AddBuff(BuffID.DryadsWard, 600);
            if (!CalamityCompatUtil.IsInAstralInfection(player))
                return;
            if (ModLoader.HasMod("CalamityMod"))
                CalamityCompatUtil.TryAddBuff(player, "CalamityMod", "GravityNormalizerBuff", 600);
            else if (ModLoader.HasMod("CalRD"))
                CalamityCompatUtil.TryAddBuff(player, "CalRD", "GravityNormalizerBuff", 600);
            else if (ModLoader.HasMod("CalamityModClassicPreTrailer"))
                CalamityCompatUtil.TryAddBuff(player, "CalamityModClassicPreTrailer"
                    , "GravityNormalizerBuff", 600);
        }
    }
}
