﻿using Cosmos.System;
using Cosmos.Core;
using System.Collections.Generic;
using System.Drawing;
using System;
using BlackOpal.Utilities.Calculations;
using BlackOpal.GUI.Component;
using GrapeGL.Graphics;
using Color = GrapeGL.Graphics.Color;

namespace GUI.Component
{
    internal class Window
    {
        /* VARIABLES */
        public List<WindowElement> WindowElements = new List<WindowElement>();
        public TextButton WindowCloseButton = new TextButton(new Point(0, 0));
        public Action UpdateAction = new Action(() => { });
        public Action CloseAction = new Action(() => { });
        public Canvas Framebuffer;
        public Canvas WindowIcon = new Canvas(16, 16);
        public Color UnfocusedTitlebarColor = new Color(139, 0, 139);
        public Color FocusedTitlebarColor = new Color(147, 112, 219);
        public Color WindowBGColor = Color.StackOverflowBlack;
        public Point Position = new Point(0, 0);
        public Size Size = new Size(320, 200);
        public string Title = "New Window";
        public uint WindowID = 0;
        public bool DrawTitleBar = true;
        private bool IsBeingDragged = false;
        private int DragOffsetX = 0, DragOffsetY = 0;

        /* CONSTRUCTOR */
        public Window(string WindowTitle, Color BGColor, Size WindowSize, Point WindowPosition, uint ID)
        {
            Size = WindowSize;
            WindowBGColor = BGColor;
            WindowID = ID;
            Position = WindowPosition;
            Title = WindowTitle;

            Framebuffer = new Canvas((ushort)WindowSize.Width, (ushort)WindowSize.Height);
            WindowCloseButton.ButtonGlobalPosition = new Point((Position.X + Size.Width) - 22, Position.Y + 6);
            WindowCloseButton.ScreenCanvas = UserInterface.ScreenCanvas;
            WindowCloseButton.ButtonHighlightColor = new Color(180, 0, 0);
            WindowCloseButton.ButtonPressedColor = new Color(75, 0, 130);
            WindowCloseButton.ButtonColor = Color.Magenta;
            WindowCloseButton.PressedAction = new Action(() => { Close(); });
            WindowCloseButton.ButtonText = "X";
        }

        /* FUNCTIONS */
        // Check if the user is dragging the window
        public void CheckDrag()
        {
            // Don't drag if the left mouse button isn't held
            if (IsBeingDragged && MouseManager.MouseState != MouseState.Left || WindowManager.WindowList.IndexOf(this) != WindowManager.WindowList.Count - 1 || ShapeCollision.IsPointInsideRectangle(UserInterface.ClickPoint.X, UserInterface.ClickPoint.Y, Position.X, Position.Y, Position.X + Size.Width, Position.Y + 24) == false)
            {
                IsBeingDragged = false;
                return;
            }

            // If we just started dragging, calculate the window offsets
            if (IsBeingDragged == false && MouseManager.MouseState == MouseState.Left && ShapeCollision.IsPointInsideRectangle((int)MouseManager.X, (int)MouseManager.Y, Position.X, Position.Y, Position.X + Size.Width, Position.Y + 24))
            {
                IsBeingDragged = true;
                DragOffsetX = (int)(MouseManager.X - Position.X);
                DragOffsetY = (int)(MouseManager.Y - Position.Y);
            }

            // If we're dragging, update the window position
            else if (IsBeingDragged)
            {
                Position.X = Math.Clamp((int)MouseManager.X - DragOffsetX, 0, UserInterface.ScreenWidth - 4);
                Position.Y = Math.Clamp((int)MouseManager.Y - DragOffsetY, 0, UserInterface.ScreenHeight - 4);
                UserInterface.ClickPoint.X = (int)MouseManager.X;
                UserInterface.ClickPoint.Y = (int)MouseManager.Y;
            }
        }

        // Check to see if the user is interacting with the window controls (minimize, maximize, close, etc)
        public void CheckControls()
        {

        }

        // Check to see if the user changed focus to the window
        public bool CheckFocus()
        {
            if (WindowManager.FocusingWindow || MouseManager.LastMouseState == MouseState.Left || WindowManager.WindowList.IndexOf(this) == WindowManager.WindowList.Count - 1)
                return false;

            if (MouseManager.MouseState == MouseState.Left && ShapeCollision.IsPointInsideRectangle((int)MouseManager.X, (int)MouseManager.Y, Position.X, Position.Y, Position.X + Size.Width, Position.Y + Size.Height + 24))
            {
                int oldIndex = WindowManager.WindowList.IndexOf(this);

                WindowManager.FocusingWindow = true;
                Window item = WindowManager.WindowList[oldIndex];
                WindowManager.WindowList.RemoveAt(oldIndex);
                WindowManager.WindowList.Add(item);
                return true;
            }

            return false;
        }

        // Close the window
        public void Close(bool CallCloseAction = false)
        {
            if (CallCloseAction)
            {
                CloseAction.Invoke();
            }

            UserInterface.HideMouse = false;
            WindowManager.WindowList.RemoveAt(WindowManager.WindowList.IndexOf(this));

            // Free objects from memory
            foreach(var Element in WindowElements)
            {
                //GCImplementation.Free(Element);
            }

            GCImplementation.Free(Framebuffer);
        }
    }
}
