using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Projectiles.Summon;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Summon
{
    /// <summary>
    /// 真菌团块：克鲁布龙宝藏袋中的召唤专家饰品。
    /// 装备后维持一只不占召唤栏的真菌团块，命中敌人时按 25% 伤害生成吸血治疗珠。
    /// </summary>
    internal sealed class FungalClump : ModItem
    {
        /// <summary>基础召唤伤害。</summary>
        public const int BaseMinionDamage = 10;
        /// <summary>设置尺寸、现代绿色稀有度、专家标记和饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.Green);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>置位召唤源，并在拥有者客户端维持唯一一只真菌团块。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.fungalClump = true;
            if (player.whoAmI != Main.myPlayer)
                return;
            if (player.FindBuffIndex(ModContent.BuffType<FungalClumpBuff>()) == -1)
                player.AddBuff(ModContent.BuffType<FungalClumpBuff>(), 3600, true);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FungalClumpMinion>()] < 1)
            {
                int damage = (int)(BaseMinionDamage
                    * player.GetDamage(DamageClass.Summon).Multiplicative);
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, -Vector2.UnitY
                    , ModContent.ProjectileType<FungalClumpMinion>(), damage, 1f, player.whoAmI);
            }
        }
        /// <summary>真菌团块与已包含其效果的顶点融合之脑互斥。</summary>
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => incomingItem.type != ModContent.ItemType<FungalClump>()
            && incomingItem.type != ModContent.ItemType<TheAmalgam>();
    }
}
