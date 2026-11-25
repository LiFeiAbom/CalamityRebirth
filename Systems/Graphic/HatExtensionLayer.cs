using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems.Graphic
{
    /// <summary>在原版头部层之后绘制 IExtendedHat 提供的附加贴图。</summary>
    internal sealed class HatExtensionLayer : PlayerDrawLayer
    {
        /// <summary>固定在原版头部层之后绘制。</summary>
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
        /// <summary>正常角色和未死亡残影都允许显示附加层。</summary>
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;
        }
        /// <summary>解析当前头部装备并按同一身体帧绘制附加贴图。</summary>
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Item headItem = drawPlayer.armor[0];
            if (drawPlayer.armor[10].type > ItemID.None)
                headItem = drawPlayer.armor[10];
            if (ModContent.GetModItem(headItem.type) is not IExtendedHat extendedHat
                || !extendedHat.PreDrawExtension(drawInfo) || drawPlayer.dead)
                return;
            string slotName = extendedHat.EquipSlotName(drawPlayer);
            int equipSlot = slotName != ""
                ? EquipLoader.GetEquipSlot(Mod, slotName, EquipType.Head)
                : headItem.headSlot;
            if (equipSlot != drawPlayer.head)
                return;
            Vector2 position = drawInfo.Position - Main.screenPosition;
            position += new Vector2(
                (drawPlayer.width - drawPlayer.bodyFrame.Width) / 2f,
                drawPlayer.height - drawPlayer.bodyFrame.Height + 4f);
            position = new Vector2((int)position.X, (int)position.Y);
            position += drawPlayer.headPosition + drawInfo.headVect;
            position += extendedHat.ExtensionSpriteOffset(drawInfo);
            Texture2D texture = ModContent.Request<Texture2D>(extendedHat.ExtensionTexture).Value;
            Rectangle frame = texture.Frame(1, 20, 0,
                drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
            DrawData drawData = new(
                texture, position, frame, drawInfo.colorArmorHead,
                drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0)
            {
                shader = drawPlayer.dye?[0].dye ?? 0
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}
