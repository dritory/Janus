using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.GUI
{
    public class Gui
    {


        public Gui()
        {

        }

        public static void drawTextCentered(int x, int y, string text, TCODColor color)
        {
            drawText(x + text.Length / 2, y, text, color);
        }
        public static void drawTextCenteredBetween(int y, int start, int end, string text, TCODColor color)
        {
            drawText(start + ((end - start) - text.Length) / 2, y, text, color);
        }
        public static void drawText(int x, int y, string text, TCODColor color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                TCODConsole.root.setChar(x + i, y, text[i]);
                TCODConsole.root.setCharForeground(x + i, y, color);
            }
        }

        public static void drawHorizontalBackground(int y, int start, int end, TCODColor color)
        {
            for (int x = start; x < end; x++)
            {
                TCODConsole.root.setCharBackground(x, y, color);
            }
        }
        public static void drawVerticalBackground(int x, int start, int end, TCODColor color)
        {
            for (int y = start; y < end; y++)
            {
                TCODConsole.root.setCharBackground(x, y, color);
            }
        }
        public static void drawHorizontalLine(int y, int start, int end, char c, TCODColor color)
        {
            for (int x = start; x < end; x++)
            {
                TCODConsole.root.setChar(x, y, c);
                TCODConsole.root.setCharForeground(x, y, color);
            }
        }
        public static void drawVerticalLine(int x, int start, int end, char c, TCODColor color)
        {
            for (int y = start; y < end; y++)
            {
                TCODConsole.root.setChar(x, y, c);
                TCODConsole.root.setCharForeground(x, y, color);
            }
        }
        public static void drawFilledRectangle(int x, int y, int width, int height, TCODColor color)
        {
            for(int i = x; i < x +width; i++)
            {
                for(int j = y; j < y + height; j++)
                {
                    TCODConsole.root.setCharBackground(i, j, color);
                    TCODConsole.root.setCharForeground(i, j, color);
                    TCODConsole.root.setChar(i, j, 0);
                }
            }
        }
        //c1 = upper left corner, c2 = upper right corner, c3 = lower left corner, c4 = lower right corner
        public static void drawRectangle(int x, int y, int width, int height, int horizontal, int vertical, int c1, int c2, int c3, int c4, TCODColor color)
        {
            drawRectangle(x, y, width, height, (char)horizontal, (char)vertical, (char)c1, (char)c2, (char)c3, (char)c4, color);
        }
        public static void drawRectangle(int x, int y, int width, int height, char horizontal, char vertical, char c1, char c2, char c3, char c4, TCODColor color)
        {
            drawHorizontalLine(y, x, x + width, horizontal, color);
            drawHorizontalLine(y + height, x, x + width, horizontal, color);

            drawVerticalLine(x, y, y + height, vertical, color);
            drawVerticalLine(x + width, y, y + height, vertical, color);

            TCODConsole.root.setChar(x, y, c1);
            TCODConsole.root.setCharForeground(x, y, color);

            TCODConsole.root.setChar(x + width, y, c2);
            TCODConsole.root.setCharForeground(x + width, y, color);

            TCODConsole.root.setChar(x, y + height, c3);
            TCODConsole.root.setCharForeground(x, y + height, color);

            TCODConsole.root.setChar(x + width, y + height, c4);
            TCODConsole.root.setCharForeground(x + width, y + height, color);
        }

        public static void drawRectangle(int x, int y, int width, int height, TCODColor color)
        {
            drawRectangle(x, y, width, height, 196, 179, 218, 191, 192, 217, color);
        }

        public virtual void update()
        {

        }
        public virtual void render()
        {

            drawRectangle(Program.engine.map.renderX - 1, Program.engine.map.renderY - 1, Program.engine.map.renderWidth - Program.engine.map.renderY + 1, Program.engine.map.renderHeight - Program.engine.map.renderY + 1, TCODColor.green);

        }
    }

}
