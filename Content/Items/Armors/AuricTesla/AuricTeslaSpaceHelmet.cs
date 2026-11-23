using CalamityRebirth.Content.Items.Armors.Bloodflare;
using CalamityRebirth.Content.Items.Armors.GodSlayer;
using CalamityRebirth.Content.Items.Armors.Silva;
using CalamityRebirth.Content.Items.Armors.Tarragon;
using CalamityRebirth.Players;
using CalamityRebirth.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.AuricTesla
{
    /// <summary>金宇星界盔：古圣金源召唤头，复合四套召唤效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class AuricTeslaSpaceHelmet : AuricTeslaArmorItem
    {
        /// <summary>古圣金源叶棱晶基础伤害。</summary>
        private const int CrystalBaseDamage = 3000;
        /// <summary>设置 12 防御和 1 铂金 80 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(12, 180);
        }
        /// <summary>要求古圣金源胸甲与腿甲。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AuricTeslaBodyArmor>()
                && legs.type == ModContent.ItemType<AuricTeslaCuisses>();
        }
        /// <summary>套装期间显示角色残影。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        /// <summary>置位四套召唤分支、额外召唤伤害与高阶叶棱晶。</summary>
        public override void UpdateArmorSet(Player player)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.tarraSet = true;
            modPlayer.tarraSummon = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareSummon = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerSummon = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaSummon = true;
            modPlayer.auricSet = true;
            player.GetDamage(DamageClass.Summon) += 1.20f;
            player.maxMinions++;
            ApplyCommonSetBonuses(player, false);
            player.setBonus = this.GetLocalization("SetBonus").Format(GodslayerKeybinds.KeyDisplay);
            if (player.whoAmI != Main.myPlayer)
                return;
            EnsureAuricCrystal(player);
        }
        /// <summary>增加 7 个仆从栏以及召唤伤害、鞭子范围和鞭子攻速。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().auricBoost = true;
            player.maxMinions += 7;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.whipRangeMultiplier += 0.12f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
        }
        /// <summary>注册三版本召唤头配方。</summary>
        public override void AddRecipes()
        {
            AddHeadRecipes<TarragonHornedHelm, BloodflareHelmet, SilvaHelmet, GodSlayerHornedHelm>();
        }
        /// <summary>确保始源林海水晶增益与基础伤害 3000 的叶棱晶同时存在。</summary>
        private void EnsureAuricCrystal(Player player)
        {
            int buffType = ModContent.BuffType<Content.ModBuffs.Buffs.SilvaCrystal>();
            if (player.FindBuffIndex(buffType) < 0)
                player.AddBuff(buffType, 3600, true);
            int crystalType = ModContent.ProjectileType<Content.Projectiles.Summon.SilvaCrystal>();
            if (player.ownedProjectileCounts[crystalType] > 0)
            {
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.owner == player.whoAmI && projectile.type == crystalType)
                    {
                        projectile.originalDamage = CrystalBaseDamage;
                        break;
                    }
                }
                return;
            }
            int damage = (int)player.GetTotalDamage(DamageClass.Summon).ApplyTo(CrystalBaseDamage);
            int crystal = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center
                , -Vector2.UnitY, crystalType, damage, 0f, Main.myPlayer);
            if (Main.projectile.IndexInRange(crystal))
                Main.projectile[crystal].originalDamage = CrystalBaseDamage;
        }
    }
}
