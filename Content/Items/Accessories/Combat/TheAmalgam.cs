using CalamityRebirth.Content.Items.Accessories.Combat.Summon;
using CalamityRebirth.Content.ModBuffs.Buffs;
using CalamityRebirth.Content.Projectiles.Summon;
using CalamityRebirth.Content.Rarities;
using CalamityRebirth.Players;
using CalamityRebirth.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Accessories.Combat
{
    /// <summary>
    /// 融合之脑 / The Amalgam：聚合之脑链的顶点综合专家饰品。
    /// 集合暗影反击、硫磺火、岩浆和水下战斗、真菌团块与完全闪避效果。
    /// </summary>
    internal sealed class TheAmalgam : ModItem
    {
        /// <summary>通用伤害提高比例。</summary>
        public const float DamageBonus = 0.20f;
        /// <summary>通用暴击提高值。</summary>
        public const int CritBonus = 10;
        /// <summary>完全闪避概率分母。</summary>
        public const int DodgeChanceDenominator = 8;
        /// <summary>无敌期间反击弹幕基础伤害。</summary>
        public const int InvincibilityProjectileDamage = 500;
        /// <summary>周期硫磺火齐射基础伤害。</summary>
        public const int BarrageBaseDamage = 500;
        /// <summary>移动毒海水基础伤害。</summary>
        public const int SeawaterBaseDamage = 500;
        /// <summary>真菌团块基础伤害。</summary>
        public const int FungalMinionDamage = 250;
        /// <summary>注册六帧纵向灵魂动画。</summary>
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(9, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        /// <summary>设置尺寸、现代 CosmicPurple 稀有度、专家标记和饰品类型。</summary>
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            ModernRarityValueUtil.Apply(Item, ModContent.RarityType<CosmicPurpleRarity>());
            Item.expert = true;
            Item.accessory = true;
        }
        /// <summary>
        /// 置位顶点和真菌召唤源。隐藏外观时仍保留全部属性，但不再维持真菌团块仆从。
        /// </summary>
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityRebirthPlayer modPlayer = player.GetModPlayer<CalamityRebirthPlayer>();
            modPlayer.theAmalgam = true;
            modPlayer.fungalClump = !hideVisual;
            if (hideVisual || player.whoAmI != Main.myPlayer)
                return;
            if (player.FindBuffIndex(ModContent.BuffType<FungalClumpBuff>()) == -1)
                player.AddBuff(ModContent.BuffType<FungalClumpBuff>(), 3600, true);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FungalClumpMinion>()] >= 1)
                return;
            int damage = (int)(FungalMinionDamage * player.GetDamage(DamageClass.Summon).Multiplicative);
            Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, -Vector2.UnitY
                , ModContent.ProjectileType<FungalClumpMinion>(), damage, 1f, player.whoAmI);
        }
        /// <summary>顶点包含所有组件效果，与五个可重复叠加的组件双向互斥。</summary>
        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
            => incomingItem.type != ModContent.ItemType<AmalgamatedBrain>()
            && incomingItem.type != ModContent.ItemType<VoidofExtinction>()
            && incomingItem.type != ModContent.ItemType<CalamityRing>()
            && incomingItem.type != ModContent.ItemType<FungalClump>()
            && incomingItem.type != ModContent.ItemType<LeviathanAmbergris>();
        /// <summary>注册现代、经典和 CalRD 三版本顶点配方。</summary>
        public override void AddRecipes()
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity)
                && calamity.TryFind<ModItem>("CosmiliteBar", out ModItem modernBar)
                && calamity.TryFind<ModItem>("Necroplasm", out ModItem modernSecondary)
                && calamity.TryFind<ModTile>("CosmicAnvil", out ModTile modernAnvil))
            {
                CreateRecipe()
                    .AddIngredient<AmalgamatedBrain>()
                    .AddIngredient<VoidofExtinction>()
                    .AddIngredient<LeviathanAmbergris>()
                    .AddIngredient<FungalClump>()
                    .AddIngredient(modernBar.Type, 5)
                    .AddIngredient(modernSecondary.Type, 5)
                    .AddTile(modernAnvil.Type)
                    .Register();
            }
            if (ModLoader.TryGetMod("CalamityModClassicPreTrailer", out Mod classic)
                && classic.TryFind<ModItem>("CosmiliteBar", out ModItem classicBar)
                && classic.TryFind<ModItem>("Phantoplasm", out ModItem classicSecondary)
                && classic.TryFind<ModTile>("DraedonsForge", out ModTile classicForge))
            {
                CreateRecipe()
                    .AddIngredient<AmalgamatedBrain>()
                    .AddIngredient<VoidofExtinction>()
                    .AddIngredient<LeviathanAmbergris>()
                    .AddIngredient<FungalClump>()
                    .AddIngredient(classicBar.Type, 5)
                    .AddIngredient(classicSecondary.Type, 5)
                    .AddTile(classicForge.Type)
                    .Register();
            }
            if (ModLoader.TryGetMod("CalRD", out Mod calRD)
                && calRD.TryFind<ModItem>("CosmiliteBar", out ModItem calRDBar)
                && calRD.TryFind<ModItem>("AscendantSpiritEssence", out ModItem calRDSecondary)
                && calRD.TryFind<ModTile>("DraedonsForge", out ModTile calRDForge))
            {
                CreateRecipe()
                    .AddIngredient<AmalgamatedBrain>()
                    .AddIngredient<VoidofExtinction>()
                    .AddIngredient<LeviathanAmbergris>()
                    .AddIngredient<FungalClump>()
                    .AddIngredient(calRDBar.Type, 5)
                    .AddIngredient(calRDSecondary.Type)
                    .AddTile(calRDForge.Type)
                    .Register();
            }
        }
    }
}
