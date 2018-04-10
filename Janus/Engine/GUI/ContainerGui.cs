using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine.Components;
using libtcod;
namespace Janus.Engine.GUI
{
    public  class ContainerGui : Gui
    {

        Container container;
        Actor owner;

        public int x, y, width, height;
        public string name;
        private TCODColor color = TCODColor.darkYellow;
        private TCODColor selectedColor = TCODColor.yellow;
        private TCODColor barColor = TCODColor.darkestYellow;

        public bool selected = false;
        private bool moreUp = false;
        private bool moreDown = false;

        private int maxLineSpace;

        private int scrollOffset;

        private int lineNum
        {
            get
            {
                if (container != null)
                {
                    return container.inventory.Count;
                }
                else
                    return 0;
            }
        }

        public int selectedLineNum = 0;

        public ContainerGui(Actor owner, string name, int x, int y, int width, int height) {
            this.owner = owner;
            this.name = name;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            maxLineSpace = height - 4;

            container = owner.getContainer();
            
            selectedColor = color.Plus(TCODColor.darkGrey);
            barColor = color.Minus(TCODColor.darkGrey); 
        }
        private float upTimer;
        private float maxInterval = 3;
        private float upInterval;

        private float downTimer;
        private float downInterval;

        public override void update() {

            if (selected)
            {
                if(scrollOffset > 0)
                {
                    moreUp = true;
                }
                else
                {
                    moreUp = false;
                }

                if ( scrollOffset + maxLineSpace < lineNum)
                {
                    moreDown = true;
                }
                else
                {
                    moreDown = false;
                }

                upTimer++;
                if (upTimer > upInterval) {
                    if (TCODConsole.isKeyPressed(TCODKeyCode.Up))
                    {
                        if(upInterval > 0)
                        {
                            upInterval--;
                        }
                        if (selectedLineNum > 0)
                        {
                            selectedLineNum--;
                        }
                        scrollUp();
                        
                    }
                    else
                    {
                        upInterval = maxInterval;
                    }
                    upTimer = 0;
                }
                downTimer++;
                if (downTimer > downInterval)
                {
                    if (TCODConsole.isKeyPressed(TCODKeyCode.Down))
                    {
                        if (downInterval > 0)
                        {
                            downInterval--;
                        }
                        if (selectedLineNum < lineNum - 1)
                        {
                            selectedLineNum++;

                            scrollDown();
                        }
                    }
                    else
                    {
                        downInterval = maxInterval;
                    }
                    downTimer = 0;
                }
            }
            if(selectedLineNum >= lineNum && lineNum > 0)
            {
                selectedLineNum = lineNum - 1;
            }
        }
        private void scrollUp()
        {
            if (selectedLineNum < scrollOffset + maxLineSpace / 2 && scrollOffset > 0)
            {
                
                scrollOffset--;

            }
            else
            {
                
            }
        }
        private void scrollDown()
        {
            if (selectedLineNum > scrollOffset + maxLineSpace / 2 && scrollOffset + maxLineSpace < lineNum)
            {
                
                scrollOffset++;

            }
            else
            {
                
            }
        }
        public override void render()
        {

            drawRectangle(x, y, width, height, (selected ? selectedColor : color));
            drawText(x + 2, y, name,(selected ? selectedColor : color));



            if(container != null)
            {
                if (lineNum > 0)
                {
                    drawHorizontalBackground(y + 2 + selectedLineNum - scrollOffset, x + 1, x + width - 1, color.Minus(TCODColor.grey));
                }
                for (int i = scrollOffset; i < scrollOffset + (maxLineSpace >= lineNum ? lineNum : maxLineSpace); i++)
                {
                    string text = container.inventory[i].name;
                    drawText(x + 2, y + 2 + i - scrollOffset, text, (selected ? selectedColor : color).Plus( ( selectedLineNum == i ? TCODColor.grey: TCODColor.black) ));
                }
            }
            drawUpperBar();
            drawLowerBar();
        }

        private void drawUpperBar()
        {
            if (moreUp)
            {
                drawHorizontalBackground(y + 1, x + 1, x + width - 1, (moreUp ? barColor.Plus(TCODColor.darkestGrey) : barColor));
           
                string text = "^ ^ ^";
                drawTextCenteredBetween(y + 1, x, x + width, text, color);
            }
        }
        private void drawLowerBar()
        {
            if (moreDown)
            {
                drawHorizontalBackground(y + height - 1, x + 1, x + width - 1, (moreDown ? barColor.Plus(TCODColor.darkestGrey) : barColor));
           
                string text = "v v v";
                drawTextCenteredBetween(y + height - 1, x, x + width, text, color);
            }
        }
    }
}
