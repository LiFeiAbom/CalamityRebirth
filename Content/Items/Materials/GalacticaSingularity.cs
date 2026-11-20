using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Content.Items.Materials
{
    /// <summary>
    /// 银河奇点：由四色月亮碎片合成的终局材料，供三种高阶药水配方使用。
    /// </summary>
    public class GalacticaSingularity : ModItem
    {
        /// <summary>注册灵魂式动画、悬浮绘制和材料排序优先级。</summary>
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 24));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.SortingPriorityMaterials[Type] = 99;
        }
        /// <summary>设置 28×28、普通堆叠、80 银价值和红色稀有度。</summary>
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Red;
        }
        /// <summary>在掉落状态发出随游戏节奏闪动的暗红微光。</summary>
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.rand.Next(90, 111) * 0.01f * Main.essScale;
            Lighting.AddLight((int)((Item.position.X + Item.width / 2f) / 16f)
                , (int)((Item.position.Y + Item.height / 2f) / 16f)
                , brightness, 0.3f * brightness, 0.3f * brightness);
        }
        /// <summary>用四种原版月亮碎片在远古操纵机合成，确保无灾厄或经典分支下仍可制作高阶药水。</summary>
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar)
                .AddIngredient(ItemID.FragmentVortex)
                .AddIngredient(ItemID.FragmentNebula)
                .AddIngredient(ItemID.FragmentStardust)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
