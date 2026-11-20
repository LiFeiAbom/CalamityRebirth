using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using CalamityRebirth.Content.Items.Accessories.Combat.Melee;
using CalamityRebirth.Content.Items.Accessories.HealthandMana;
using CalamityRebirth.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 血神核心：由血腥蠕虫围巾、血契、血肉图腾和血炎晶核合成的综合型专家饰品。
    /// 属性为复合生存与近战加成，价格、稀有度和掉落基准按现代档位校准。
    /// </summary>
    public class CoreOfTheBloodGod : ModItem
    {
        /// <summary>最大生命提高比例。</summary>
        public const float MaxLifeBonus = 0.10f;
        /// <summary>通用伤害提高比例。</summary>
        public const float DamageBonus = 0.12f;
        /// <summary>通用暴击率提高值。</summary>
        public const int CritBonus = 12;
        /// <summary>近战攻速提高比例；按用户要求从旧版 10% 调整为 12%。</summary>
        public const float MeleeSpeedBonus = 0.12f;
        /// <summary>伤害减免提高比例，对应 Player.endurance。</summary>
        public const float DamageReduction = 0.10f;
        /// <summary>防御低于 100 时追加的通用伤害比例。</summary>
        public const float LowDefenseDamageBonus = 0.15f;
        /// <summary>灵魂虹吸光环弹幕的基础伤害。</summary>
        public const int AuraBaseDamage = 40;
        /// <summary>同一光环弹幕对 NPC 的本地命中冷却帧数。</summary>
        public const int AuraHitCooldown = 5;
        /// <summary>
        /// 设置贴图尺寸、现代版两铂金售价、CosmicPurple 稀有度、专家标记和饰品类型。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            ModernRarityValueUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>());
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>
        /// 点亮血神核心本体效果，并同时继承血肉图腾的接触伤害减半标记。
        /// 数值和光环的实际结算统一放在 CalamityRebirthPlayer，避免物品 Hook 分散。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.coreOfTheBloodGod = true;
            modPlayer.fleshTotem = true;
        }
        /// <summary>
        /// 注册三版本合成分支：现代版使用宇宙砧与死灵质，两个经典版本使用嘉登熔炉与幻影质。
        /// </summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                && calamity.TryFind<ModItem>("CosmiliteBar", out ModItem modernCosmilite)
                && calamity.TryFind<ModItem>("Necroplasm", out ModItem necroplasm)
                && calamity.TryFind<ModTile>("CosmicAnvil", out ModTile cosmicAnvil))
            {
                Recipe modernRecipe = CreateRecipe();
                modernRecipe.AddIngredient<BloodyWormScarf>();
                modernRecipe.AddIngredient<BloodPact>();
                modernRecipe.AddIngredient<FleshTotem>();
                modernRecipe.AddIngredient<BloodflareCore>();
                modernRecipe.AddIngredient(modernCosmilite.Type, 5);
                modernRecipe.AddIngredient(necroplasm.Type, 5);
                modernRecipe.AddTile(cosmicAnvil.Type);
                modernRecipe.Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicCosmilite)
                && classic.TryFind<ModItem>("Phantoplasm", out ModItem classicPhantoplasm)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile classicForge))
            {
                Recipe classicRecipe = CreateRecipe();
                classicRecipe.AddIngredient<BloodyWormScarf>();
                classicRecipe.AddIngredient<BloodPact>();
                classicRecipe.AddIngredient<FleshTotem>();
                classicRecipe.AddIngredient<BloodflareCore>();
                classicRecipe.AddIngredient(classicCosmilite.Type, 5);
                classicRecipe.AddIngredient(classicPhantoplasm.Type, 5);
                classicRecipe.AddTile(classicForge.Type);
                classicRecipe.Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("CosmiliteBar", out ModItem calRDCosmilite)
                && calRD.TryFind<ModItem>("Phantoplasm", out ModItem calRDPhantoplasm)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDForge))
            {
                Recipe calRDRecipe = CreateRecipe();
                calRDRecipe.AddIngredient<BloodyWormScarf>();
                calRDRecipe.AddIngredient<BloodPact>();
                calRDRecipe.AddIngredient<FleshTotem>();
                calRDRecipe.AddIngredient<BloodflareCore>();
                calRDRecipe.AddIngredient(calRDCosmilite.Type, 5);
                calRDRecipe.AddIngredient(calRDPhantoplasm.Type, 5);
                calRDRecipe.AddTile(calRDForge.Type);
                calRDRecipe.Register();
            }
        }
    }
}
