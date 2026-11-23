using Terraria;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Armors.Bloodflare
{
    /// <summary>血炎胸甲：增加生命与通用面板，浸入岩浆时额外增加防御和再生。</summary>
    [AutoloadEquip(EquipType.Body)]
    internal sealed class BloodflareBodyArmor : BloodflareArmorItem
    {
        /// <summary>设置 35 防御与 48 金价格。</summary>
        public override void SetDefaults()
        {
            SetArmorDefaults(35, 48);
        }
        /// <summary>增加 40 生命、14% 通用伤害和暴击；岩浆中额外获得 30 防御与 5 HP/s 再生。</summary>
        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.GetDamage(DamageClass.Generic) += 0.14f;
            player.GetCritChance(DamageClass.Generic) += 14f;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }
        }
        /// <summary>注册三版本配方。</summary>
        public override void AddRecipes()
        {
            AddBloodflareRecipes(50, 20, 16, 4);
        }
    }
}
