﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.UI;
using Terraria.DataStructures;

namespace BuilderEssentials.Utilities
{
    internal class CoordsSelection
    {
        public int itemType = -1;
        public bool shiftDown;

        public bool RMBDown;
        public Vector2 RMBStart = Vector2.Zero;
        public Vector2 RMBEnd = Vector2.Zero;

        public bool LMBDown;
        public Vector2 LMBStart = Vector2.Zero;
        public Vector2 LMBEnd = Vector2.Zero;

        public bool MMBDown;
        public Vector2 MMBStart = Vector2.Zero;
        public Vector2 MMBEnd = Vector2.Zero;

        public CoordsSelection(int itemToWorkWith, UIState instance)
        {
            itemType = itemToWorkWith;
            
            instance.OnRightMouseDown += OnRightMouseDown;
            instance.OnRightMouseUp += OnRightMouseUp;
            instance.OnMouseDown += OnMouseDown;
            instance.OnMouseUp += OnMouseUp;
            instance.OnMiddleMouseDown += OnMiddleMouseDown;
            instance.OnMiddleMouseUp += OnMiddleMouseUp;
        }

        private void OnRightMouseDown(UIMouseEvent evt, UIElement listeningelement)
        {
            if (Main.LocalPlayer.HeldItem.type != itemType) return;

            RMBDown = true;
            RMBStart = RMBEnd = new Vector2(Player.tileTargetX, Player.tileTargetY);
        }

        private void OnRightMouseUp(UIMouseEvent evt, UIElement listeningelement)
        {
            RMBDown = false;
        }

        private void OnMouseDown(UIMouseEvent evt, UIElement listeningelement)
        {
            if (Main.LocalPlayer.HeldItem.type != itemType) return;

            LMBDown = true;
            LMBStart = LMBEnd = new Vector2(Player.tileTargetX, Player.tileTargetY);
        }

        private void OnMouseUp(UIMouseEvent evt, UIElement listeningelement)
        {
            LMBDown = false;
        }
        
        private void OnMiddleMouseDown(UIMouseEvent evt, UIElement listeningelement)
        {
            if (Main.LocalPlayer.HeldItem.type != itemType) return;

            MMBDown = true;
            MMBStart = MMBEnd = new Vector2(Player.tileTargetX, Player.tileTargetY);
        }
        
        private void OnMiddleMouseUp(UIMouseEvent evt, UIElement listeningelement)
        {
            MMBDown = false;
        }

        internal void SquareCoords(ref Vector2 start, ref Vector2 end)
        {
            int distanceX = (int) (end.X - start.X);
            int distanceY = (int) (end.Y - start.Y);

            //Turning rectangles into a square
            if (Math.Abs(distanceX) < Math.Abs(distanceY)) //Horizontal
            {
                if (distanceX > 0) //I. and IV. Quadrant
                    end.X = start.X + Math.Abs(distanceY);
                else //II. and III. Quadrant
                    end.X = start.X - Math.Abs(distanceY);
            }
            else //Vertical
            {
                if (distanceY > 0) //III. and IV. Quadrant
                    end.Y = start.Y + Math.Abs(distanceX);
                else //I. and II. Quadrant
                    end.Y = start.Y - Math.Abs(distanceX);
            }
        }

        internal void MirrorCoords(ref Vector2 start, ref Vector2 end)
        {
            MirrorCoordsHorizontally(ref start, ref end);
            MirrorCoordsVertically(ref start, ref end);
        }
        
        internal void MirrorCoordsHorizontally(ref Vector2 start, ref Vector2 end)
        {
            Vector2 temp = start;
            start.X = end.X;
            end.X = temp.X;
        }
        
        internal void MirrorCoordsVertically(ref Vector2 start, ref Vector2 end)
        {
            Vector2 temp = start;
            start.Y = end.Y;
            end.Y = temp.Y;
        }

        /// <summary>0:TopLeft; 1:TopRight; 2:BottomLeft; 3:BottomRight</summary>
        internal int SelectedQuad(int x0, int y0, int x1, int y1)
        {
            //0:TopLeft; 1:TopRight; 2:BottomLeft; 3:BottomRight;
            int selectedQuarter = -1;

            if (x0 <= x1 && y0 <= y1)
                selectedQuarter = 3;
            else if (x0 <= x1 && y0 >= y1)
                selectedQuarter = 1;
            else if (x0 >= x1 && y0 >= y1)
                selectedQuarter = 0;
            else if (x0 >= x1 && y0 <= y1)
                selectedQuarter = 2;

            return selectedQuarter;
        }

        internal void UpdateCoords(bool bezierSelection = false)
        {
            if (Main.LocalPlayer.HeldItem.type != itemType)
            { RMBDown = LMBDown = MMBDown = false; return; }

            if (RMBDown)
                RMBEnd = new Vector2(Player.tileTargetX, Player.tileTargetY);

            if (LMBDown)
                LMBEnd = new Vector2(Player.tileTargetX, Player.tileTargetY);
            
            if (MMBDown)
                MMBEnd = new Vector2(Player.tileTargetX, Player.tileTargetY);
            
            //Centering the control point
            if (bezierSelection && LMBDown)
                RMBEnd = new Vector2((LMBStart.X + LMBEnd.X) / 2, (LMBStart.Y + LMBEnd.Y) / 2);

            shiftDown = Keyboard.GetState().IsKeyDown(Keys.LeftShift);
            if (!shiftDown) return;

            if (RMBDown) 
                SquareCoords(ref RMBStart, ref RMBEnd);
            else if (LMBDown)
                SquareCoords(ref LMBStart, ref LMBEnd);
            else if (MMBDown)
                SquareCoords(ref MMBStart, ref MMBEnd);
        }
    }
}