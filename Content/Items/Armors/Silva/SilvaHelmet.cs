using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Silva
{
    /// <summary>始源林海头盔：召唤向头部，提供仆从栏、鞭子面板与叶棱晶套装效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class SilvaHelmet : SilvaArmorItem
    {
        /// <summary>远古叶棱晶的基础召唤伤害。</summary>
        public const int CrystalBaseDamage = 1500;
        /// <summary>设置 13 防御和 90 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(13, 90);
        }
        /// <summary>三件套要求胸甲与护胫均为始源林海部件。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SilvaArmor>()
                && legs.type == ModContent.ItemType<SilvaLeggings>();
        }
        /// <summary>套装期间显示角色残影。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        /// <summary>置位通用与召唤套装标记，增加召唤伤害并维护叶棱晶和增益。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.silvaSet = true;
            modPlayer.silvaSummon = true;
            player.GetDamage(DamageClass.Summon) += 0.75f;
            player.setBonus = this.GetLocalization("SetBonus").Value;
            if (player.whoAmI != Main.myPlayer)
                return;
            int buffType = ModContent.BuffType<Content.ModBuffs.Buffs.SilvaCrystal>();
            if (player.FindBuffIndex(buffType) < 0)
                player.AddBuff(buffType, 3600, true);
            int crystalType = ModContent.ProjectileType<Content.Projectiles.Summon.SilvaCrystal>();
            if (player.ownedProjectileCounts[crystalType] > 0)
                return;
            int damage = (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(CrystalBaseDamage);
            int crystal = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, -Vector2.UnitY
                , crystalType, damage, 0f, Main.myPlayer);
            if (Main.projectile.IndexInRange(crystal))
                Main.projectile[crystal].originalDamage = CrystalBaseDamage;
        }
        /// <summary>增加 5 个仆从栏以及召唤伤害、鞭子范围和鞭子攻速。</summary>
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 5;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.whipRangeMultiplier += 0.12f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
        }
        /// <summary>注册三版本配方；现代头部额外消耗铅芯。</summary>
        public override void AddRecipes()
        {
            AddSilvaRecipes(30, 8, 2, 5, 5, 5, 6, 14, 14, 2, true, false, false);
        }
    }
}
