using CalamityRebirth.Content.ModBuffs.Debuffs;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>
    /// 龙舌兰日出：经典 1.4.2 面板与昼夜双层属性，按用户要求把白天防御从 +15 调整为 +10。
    /// 现代 2.2.2 提供浅红稀有度与 2 银价格；效果结构使用可结算版本。
    /// </summary>
    public class TequilaSunrise : ModItem
    {
        /// <summary>昼夜均生效的通用伤害加成。</summary>
        public const float GeneralDamageBoost = 0.03f;
        /// <summary>昼夜均生效的通用暴击加成。</summary>
        public const int GeneralCritBoost = 2;
        /// <summary>昼夜均生效的伤害减免。</summary>
        public const float GeneralDamageReduction = 0.03f;
        /// <summary>昼夜均生效的武器击退加成。</summary>
        public const float GeneralKnockbackBoost = 0.03f;
        /// <summary>白天额外伤害加成。</summary>
        public const float DayDamageBoost = 0.07f;
        /// <summary>白天额外暴击加成。</summary>
        public const int DayCritBoost = 3;
        /// <summary>白天额外防御；按用户要求设置为 10。</summary>
        public const int DayDefenseBoost = 10;
        /// <summary>白天额外伤害减免。</summary>
        public const float DayDamageReduction = 0.07f;
        /// <summary>白天额外武器击退加成。</summary>
        public const float DayKnockbackBoost = 0.07f;
        /// <summary>每帧 lifeRegen 扣减值；10 点约等于每秒损失 5 HP。</summary>
        public const int LifeRegenLoss = 10;
        /// <summary>设置研究数量、浅红稀有度、现代精确价格与五分钟饮用面板。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new[]
            {
                new Color(220, 75, 59),
                new Color(229, 147, 47),
                new Color(255, 190, 76)
            };
            ItemID.Sets.IsFood[Type] = true;
        }
        /// <summary>
        /// 设置 28×18、堆叠 30、浅红档、17 帧饮用动作和五分钟增益。
        /// 现代 2.2.2 的显式价格为 2 银，因此通过统一工具传入精确价格覆盖。
        /// </summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            ModernRarityValueUtil.Apply(Item, ItemRarityID.LightRed, Item.sellPrice(silver: 2));
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<TequilaSunriseBuff>();
            Item.buffTime = 5 * 60 * 60;
        }
        /// <summary>
        /// 注册双配方，并依次解析现代、1.4.2.101 与 CalRD 的材料：
        /// 优先 ScoriaOre，其次 CruptixBar，最后 ChaoticOre。
        /// </summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod calamity))
                    continue;
                ModItem scoriaMaterial = null;
                if (calamity.TryFind<ModItem>("ScoriaOre", out ModItem scoriaOre))
                    scoriaMaterial = scoriaOre;
                else if (calamity.TryFind<ModItem>("CruptixBar", out ModItem cruptixBar))
                    scoriaMaterial = cruptixBar;
                else if (calamity.TryFind<ModItem>("ChaoticOre", out ModItem chaoticOre))
                    scoriaMaterial = chaoticOre;
                if (scoriaMaterial == null)
                    continue;
                CreateRecipe()
                    .AddIngredient(ItemID.Ale)
                    .AddIngredient(scoriaMaterial.Type, 3)
                    .AddTile(TileID.Kegs)
                    .Register();
                if (!calamity.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                    continue;
                CreateRecipe()
                    .AddIngredient(ItemID.BottledWater)
                    .AddIngredient(bloodOrb.Type, 5)
                    .AddIngredient(scoriaMaterial.Type)
                    .AddTile(TileID.AlchemyTable)
                    .Register()
                    .DisableDecraft();
            }
        }
    }
}
