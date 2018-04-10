using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.GUI
{
    public class MenuGui : Gui
    {

        struct Entry
        {
            public Action function;
            public string name;
            public Entry(string name, Action function)
            {
                this.function = function;
                this.name = name;
            }
        }

        List<Entry> entries = new List<Entry>();

        int selectedEntry;
        public bool focused;

        public int x, y, width, height;
        

        public MenuGui(int screenWidth,int screenHeight) : base()
        {
            this.x = screenWidth / 2 - 10;
            this.y = screenHeight / 2 - 20;
            this.width = 20;
            this.height = 40;

            entries.Add(new Entry("Save", save));

            entries.Add(new Entry("Load", load));

            entries.Add(new Entry("Restart", restart));

            entries.Add(new Entry("Load Test Level",loadTest));

            entries.Add(new Entry("Settings", settings));

            entries.Add(new Entry("Exit", exit));

        }

        bool oldup;
        bool olddown;
        public override void update()
        {
            if (focused)
            {
                
                if (TCODConsole.isKeyPressed(TCODKeyCode.Enter))
                {
                    if (entries.Count > selectedEntry)
                    {
                        entries[selectedEntry].function();
                    }
                }

                if (TCODConsole.isKeyPressed(TCODKeyCode.Up))
                {
                    if (!oldup)
                    {
                        if (selectedEntry > 0)
                        {
                            selectedEntry--;
                        }
                    }
                    oldup = true;
                }
                else
                {
                    oldup = false;
                }

                if (TCODConsole.isKeyPressed(TCODKeyCode.Down))
                {
                    if (!olddown)
                    {
                        if (selectedEntry < entries.Count - 1)
                        {
                            selectedEntry++;
                        }
                    }
                    olddown = true;
                }
                else
                {
                    olddown = false;
                }
            }
            base.update();
        }

        public override void render()
        {

            if (focused)
            {
                drawFilledRectangle(x, y, width, height, TCODColor.black);
                drawRectangle(x, y, width, height, TCODColor.green);

                drawTextCenteredBetween(y, x, x + width + 1, "< Menu >", TCODColor.green);

                for (int i = 0; i < entries.Count; i++)
                {
                    drawTextCenteredBetween(y + 3 + i, x, x + width + 1, entries[i].name, 
                        (i == selectedEntry ? TCODColor.white : TCODColor.grey) );
                }

            }
            
        }

        private void load()
        {
            Saver.loadGame("save.sav");
            Program.engine.gameStatus = GameStatus.IDLE;
        }

        private void restart()
        {
            Program.engine.initialize(true);
        }

            private void save()
        {
            Saver.saveGame("save.sav");
            Program.engine.gameStatus = GameStatus.IDLE;
            focused = false;
        }

        private void exit()
        {
            Program.Exit(0);
        }

        private void settings()
        {

        }

        private void loadTest()
        {
            Level newLevel = new Level();
            newLevel.initialize(true, int.MaxValue,typeof(Generators.TestLevelGenerator));
            if (!Program.engine.levels.ContainsKey(int.MaxValue))
            {
                Program.engine.levels.Add(int.MaxValue, newLevel);
            }

            Program.engine.changeLevel(int.MaxValue);
            focused = false;
        }
    }
}
