using CalamityRebirth.Players;
using CalamityRebirth.Enums;
using CalamityRebirth.Systems;
using CalamityRebirth.Systems.Graphic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using RedDevilBuff = CalamityRebirth.Content.ModBuffs.Buffs.RedDevil;
using RedDevilProjectile = CalamityRebirth.Content.Projectiles.Summon.RedDevil;
namespace CalamityRebirth.Content.Items.Armors.Demonshade
{
    /// <summary>魔影头盔：提供通用面板、召唤栏与魔影套装核心效果。</summary>
    [AutoloadEquip(EquipType.Head)]
    internal sealed class DemonshadeHelm : DemonshadeArmorItem, IHatExtensionDrawer
    {
        /// <summary>魔影头盔附加兜帽贴图路径。</summary>
        public string ExtensionTexture =>
            "CalamityRebirth/Content/Items/Armors/Demonshade/DemonshadeHelm_Extension";
        /// <summary>附加贴图比标准头部高四像素，因此整体上移四像素。</summary>
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new(0f, -4f);
        /// <summary>设置五十防御和五铂金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(500);
        }
        /// <summary>要求魔影胸甲与魔影护腿同时装备。</summary>
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DemonshadeBreastplate>()
                && legs.type == ModContent.ItemType<DemonshadeGreaves>();
        }
        /// <summary>套装期间绘制残影与轮廓。</summary>
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }
        /// <summary>
        /// 启用魔影命中减益、红魔、狂怒技能、弑神者冲刺与专属盾牌冲撞。
        /// 套装通用伤害必须先写入，红魔后续取值才能继承这一百百分点。
        /// </summary>
        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 1f;
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.demonshadeSetBonus = true;
            modPlayer.redDevil = true;
            modPlayer.ProvideShieldSlamDash(ShieldSlamDash.Demonshade);
            int redDevilDamage = (int)player.GetDamage(DamageClass.Generic).ApplyTo(10000);
            if (player.FindBuffIndex(ModContent.BuffType<RedDevilBuff>()) < 0)
                player.AddBuff(ModContent.BuffType<RedDevilBuff>(), 3600, true);
            int devilType = ModContent.ProjectileType<RedDevilProjectile>();
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[devilType] < 1)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero
                    , devilType, redDevilDamage, 0f, player.whoAmI);
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.type == devilType
                    && projectile.owner == player.whoAmI)
                    projectile.damage = redDevilDamage;
            }
            player.setBonus = this.GetLocalization("SetBonus").Format(
                GodslayerKeybinds.KeyDisplay, DemonshadeKeybinds.KeyDisplay);
        }
        /// <summary>提供通用伤害、暴击、近战攻速、仆从栏与哨兵栏。</summary>
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.50f;
            player.GetCritChance(DamageClass.Generic) += 25f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.25f;
            player.maxMinions += 10;
            player.maxTurrets += 5;
        }
        /// <summary>注册三版本四十枚魔影锭配方。</summary>
        public override void AddRecipes()
        {
            AddShadowspecRecipe(40);
        }
    }
}
