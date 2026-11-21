using CalamityRebirth.Content.Items.Accessories.Combat;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Players
{
	/// <summary>星云核心的玩家属性、星星存活标记与致命伤保护结算。</summary>
	public partial class CalamityRebirthPlayer : ModPlayer
	{
		/// <summary>本帧是否装备星云核心。</summary>
		public bool nebulousCore;
		/// <summary>本帧是否显示星云核心外观；隐藏时停止生成并清除已有星星。</summary>
		public bool nebulousCoreVisible;
		/// <summary>清空星云核心的逐帧装备状态。</summary>
		public void ResetNebulousCoreEffects()
		{
			nebulousCore = false;
			nebulousCoreVisible = false;
		}
		/// <summary>结算星云核心的通用伤害与通用暴击。</summary>
		public void ApplyNebulousCoreEffects()
		{
			if (!nebulousCore)
				return;
			Player.GetDamage(DamageClass.Generic) += NebulousCore.DamageBonus;
			Player.GetCritChance(DamageClass.Generic) += NebulousCore.CritBonus;
		}
		/// <summary>
		/// 致命伤前进行十分之一概率判定，成功时播放紫黑光束粒子并回复一百点生命。
		/// 返回 false 取消本次死亡，且该效果没有额外冷却。
		/// </summary>
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound
			, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (!nebulousCore
				|| !Main.rand.NextBool(NebulousCore.ReviveChanceDenominator))
				return true;
			SoundEngine.PlaySound(SoundID.Item67, Player.position);
			for (int i = 0; i < 25; i++)
			{
				int dustIndex = Dust.NewDust(Player.position, Player.width, Player.height
					, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
				Dust dust = Main.dust[dustIndex];
				dust.position.X += Main.rand.Next(-20, 21);
				dust.position.Y += Main.rand.Next(-20, 21);
				dust.velocity *= 0.9f;
				dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
				dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
				if (Main.rand.NextBool(2))
					dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
			}
			Player.statLife += NebulousCore.ReviveHeal;
			Player.HealEffect(NebulousCore.ReviveHeal);
			if (Player.statLife > Player.statLifeMax2)
				Player.statLife = Player.statLifeMax2;
			return false;
		}
	}
}
