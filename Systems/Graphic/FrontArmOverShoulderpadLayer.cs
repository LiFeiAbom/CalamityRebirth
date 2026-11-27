using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRebirth.Systems.Graphic
{
    /// <summary>在原版手臂层之后重绘胸甲的前臂贴图。</summary>
    internal sealed class FrontArmOverShoulderpadLayer : PlayerDrawLayer
    {
        /// <summary>固定在手臂盖住手持物的原版层之后绘制。</summary>
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);
        /// <summary>正常角色和未死亡残影都允许显示前臂附加层。</summary>
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.shadow == 0f || !drawInfo.drawPlayer.dead;
        }
        /// <summary>解析当前胸甲并按相同身体帧重绘前臂。</summary>
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Item bodyItem = drawPlayer.armor[1];
            if (drawPlayer.armor[11].type > ItemID.None)
                bodyItem = drawPlayer.armor[11];
            if (ModContent.GetModItem(bodyItem.type) is not IDrawFrontArmOverShoulderpad frontArm)
                return;
            string slotName = frontArm.EquipSlotName(drawPlayer);
            int equipSlot = slotName != ""
                ? EquipLoader.GetEquipSlot(Mod, slotName, EquipType.Body)
                : bodyItem.bodySlot;
            if (drawPlayer.body != equipSlot)
                return;
            Vector2 position = drawInfo.Position - Main.screenPosition;
            position += new Vector2(
                (drawPlayer.width - drawPlayer.bodyFrame.Width) / 2f,
                drawPlayer.height - drawPlayer.bodyFrame.Height + 4f);
            position = new Vector2((int)position.X, (int)position.Y);
            position += drawPlayer.bodyPosition + drawInfo.bodyVect;
            Texture2D texture = ModContent.Request<Texture2D>(frontArm.FrontArmTexture).Value;
            Rectangle frame = texture.Frame(1, 20, 0,
                drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
            DrawData drawData = new(
                texture, position, frame, drawInfo.colorArmorBody,
                drawPlayer.fullRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0)
            {
                shader = drawPlayer.dye?[1].dye ?? 0
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}
