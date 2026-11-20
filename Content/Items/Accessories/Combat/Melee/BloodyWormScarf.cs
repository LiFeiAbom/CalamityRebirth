using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat.Melee
{
    /// <summary>
    /// 血腥蠕虫围巾：近战伤害与攻速各提高 10%，伤害减免按用户要求调整为 10%。
    /// </summary>
    [AutoloadEquip(EquipType.Neck)]
    public class BloodyWormScarf : ModItem
    {
        /// <summary>近战伤害提高比例。</summary>
        public const float MeleeDamageBonus = 0.10f;
        /// <summary>近战攻速提高比例。</summary>
        public const float MeleeSpeedBonus = 0.10f;
        /// <summary>伤害减免提高比例；按用户要求从 15% 调整为 10%。</summary>
        public const float DamageReduction = 0.10f;
        /// <summary>设置颈部贴图尺寸、现代版 10 金售价、浅红稀有度和专家标记。</summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.LightRed);
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>点亮本帧的围巾状态。</summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CalamityRebirthPlayer>().bloodyWormScarf = true;
        }
        /// <summary>
        /// 现代版在铁砧使用血腥蠕虫牙、原版蠕虫围巾和暗影之魂合成；
        /// 两个经典分支沿用秘银砧，且不要求暗影之魂。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                Recipe modernRecipe = CreateRecipe();
                modernRecipe.AddIngredient<BloodyWormTooth>();
                modernRecipe.AddIngredient(ItemID.WormScarf);
                modernRecipe.AddIngredient(ItemID.SoulofNight, 3);
                modernRecipe.AddTile(TileID.Anvils);
                modernRecipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out _))
            {
                Recipe classicRecipe = CreateRecipe();
                classicRecipe.AddIngredient<BloodyWormTooth>();
                classicRecipe.AddIngredient(ItemID.WormScarf);
                classicRecipe.AddTile(TileID.MythrilAnvil);
                classicRecipe.Register();
            }
            if (ModLoader.TryGetMod("CalRD", out _))
            {
                Recipe calRDRecipe = CreateRecipe();
                calRDRecipe.AddIngredient<BloodyWormTooth>();
                calRDRecipe.AddIngredient(ItemID.WormScarf);
                calRDRecipe.AddTile(TileID.MythrilAnvil);
                calRDRecipe.Register();
            }
        }
    }
}
