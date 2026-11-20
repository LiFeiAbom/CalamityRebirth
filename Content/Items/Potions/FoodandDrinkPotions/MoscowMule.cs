using CalamityRebirth.Content.ModBuffs.Debuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Potions.FoodandDrinkPotions
{
    /// <summary>莫斯科骡子：八分钟内提高伤害、暴击和击退，降低再生。</summary>
    public class MoscowMule : AlcoholPotionItem
    {
        /// <summary>注册莫斯科骡子浅绿色饮用粒子。</summary>
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            SetAlcoholVisuals(new Color(194, 252, 192), new Color(226, 252, 192), new Color(250, 225, 222));
        }
        /// <summary>通用伤害加成。</summary>
        public const float DamageBoost = 0.09f;
        /// <summary>通用暴击加成。</summary>
        public const int CritBoost = 3;
        /// <summary>武器击退加法乘数。</summary>
        public const float KnockbackBoost = 0.5f;
        /// <summary>采用现代浅红稀有度、显式 2 银价格和 9999 堆叠。</summary>
        public override void SetDefaults() => SetAlcoholDefaults(32, 34
            , ModContent.BuffType<MoscowMuleBuff>(), 8 * 60 * 60, ItemRarityID.LightRed, 9999);
        /// <summary>注册 ScoriaOre → CruptixBar → ChaoticOre 的双配方。</summary>
        public override void AddRecipes()
        {
            foreach (string modName in new[] { "CalamityMod", "CalamityModClassicPreTrailer", "CalRD" })
            {
                if (!ModLoader.TryGetMod(modName, out Mod mod))
                    continue;
                ModItem material = null;
                if (mod.TryFind<ModItem>("ScoriaOre", out ModItem scoria))
                    material = scoria;
                else if (mod.TryFind<ModItem>("CruptixBar", out ModItem cruptix))
                    material = cruptix;
                else if (mod.TryFind<ModItem>("ChaoticOre", out ModItem chaotic))
                    material = chaotic;
                if (material == null)
                    continue;
                CreateRecipe().AddIngredient(ItemID.Ale).AddIngredient(material.Type, 3)
                    .AddTile(TileID.Kegs).Register();
                if (mod.TryFind<ModItem>("BloodOrb", out ModItem bloodOrb))
                {
                    CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient(bloodOrb.Type, 5)
                        .AddIngredient(material.Type).AddTile(TileID.AlchemyTable)
                        .Register().DisableDecraft();
                }
            }
        }
    }
}
