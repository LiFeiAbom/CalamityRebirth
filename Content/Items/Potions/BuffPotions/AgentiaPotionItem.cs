using CalamityRebirth.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.BuffPotions
{
    /// <summary>
    /// Agentia 系列药水的公共面板与血珠兜底配方。
    /// 子类只负责指定增益、持续时间、现代价格档位和特有材料配方。
    /// </summary>
    public abstract class AgentiaPotionItem : ModItem
    {
        /// <summary>统一设置现代研究解锁数量。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }
        /// <summary>设置 Agentia 药水的通用 28×18、17 帧进食面板。</summary>
        /// <param name="buffType">药水提供的 ModBuff 类型。</param>
        /// <param name="buffTime">增益持续帧数。</param>
        /// <param name="rarity">稀有度；未指定时使用橙色默认值。</param>
        /// <param name="explicitValue">现代版显式价格；为空时使用稀有度标准价。</param>
        /// <param name="maxStack">最大堆叠，默认为 999。</param>
        protected void SetAgentiaDefaults(int buffType, int buffTime
            , int rarity = ItemRarityID.Orange, int? explicitValue = null, int maxStack = 999)
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = maxStack;
            ModernRarityValueUtil.Apply(Item, rarity, explicitValue);
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = buffType;
            Item.buffTime = buffTime;
        }
        /// <summary>
        /// 为当前药水注册三版本共用的瓶装水 + 血珠兜底配方。
        /// 每个已加载灾厄分支独立查找，避免跨程序集复用 ModItem 类型。
        /// </summary>
        protected void AddBloodOrbRecipe(int bloodOrbCount)
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod)
                    || !mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                    continue;
                CreateRecipe()
                    .AddIngredient(ItemID.BottledWater)
                    .AddIngredient(bloodOrb.Type, bloodOrbCount)
                    .AddTile(TileID.AlchemyTable)
                    .Register();
            }
        }
    }
}
