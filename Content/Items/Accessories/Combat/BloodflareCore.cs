using CalamityRebirth.Players;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 血炎晶核：提供伤害性吸血光环，并按低防御、低生命档位追加进攻与生存属性。
    /// </summary>
    public class BloodflareCore : ModItem
    {
        /// <summary>防御低于 100 时追加的通用伤害比例。</summary>
        public const float LowDefenseDamageBonus = 0.15f;
        /// <summary>生命低于 50% 档位的伤害减免。</summary>
        public const float HalfLifeDamageReduction = 0.15f;
        /// <summary>生命低于 50% 档位的通用伤害。</summary>
        public const float HalfLifeDamageBonus = 0.10f;
        /// <summary>生命低于 50% 档位的通用暴击。</summary>
        public const int HalfLifeCritBonus = 10;
        /// <summary>生命低于 15% 档位的伤害减免；按用户要求从 30% 调整为 20%。</summary>
        public const float LowLifeDamageReduction = 0.20f;
        /// <summary>生命低于 15% 档位的通用伤害。</summary>
        public const float LowLifeDamageBonus = 0.20f;
        /// <summary>生命低于 15% 档位的通用暴击。</summary>
        public const int LowLifeCritBonus = 20;
        /// <summary>灵魂虹吸光环弹幕的基础伤害。</summary>
        public const int AuraBaseDamage = 40;
        /// <summary>同一光环弹幕对 NPC 的本地命中冷却帧数。</summary>
        public const int AuraHitCooldown = 5;
        /// <summary>
        /// 设置贴图尺寸、现代版一铂金五十金售价和 Turquoise 稀有度。
        /// 现代版不再是专家物品，因此这里不再写入 expert 标记。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            ModernRarityValueUtil.Apply(Item, ModContent.RarityType<TurquoiseRarity>());
            Item.accessory = true;
        }
        /// <summary>点亮本帧的血炎晶核状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().bloodflareCore = true;
        }
    }
}
