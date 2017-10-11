using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine;
using Janus;
using libtcod;
namespace Janus.Engine.GUI
{
    class DefeatGui : Gui
    {
        public int loading = 0;

        private List<string> texts = new List<string>() { "YOU WERE DEFEATED", "YOU WERE DEFEATED", "YOU WERE DEFEATED", "YOU WERE DEFEATED", "YOU DEAD", "YOU DEAD", "YOU DEAD", "OOPSY", "THAT HURT","ME STUPID", "YOU WERE DEFEATED", "OUCH!", "MY BRAIN HURTS", "REST IN PEACE", "R.I.P", "R.I.P", "R.I.P", "R.I.P", "R.I.P", "R.I.P", "THE END", "THE END", "THE END", "THE END", "THE END", "GAME OVER", "GAME OVER", "GAME OVER", "GAME OVER", "GAME OVER", "GAME OVER", "YOU SUCK AT THIS" };

        public override void update()
        {

        }
        bool flag;
        int i;
        public override void render()
        {
            base.render();
            for(int x = (Program.engine.map.renderWidth / 2) - 10; x < (Program.engine.map.renderWidth / 2) + 14; x++)
                for (int y =  (Program.engine.map.renderHeight / 2) - 3; y <  (Program.engine.map.renderHeight / 2) + 3; y++)
                {
                    TCODConsole.root.setChar(x, y, '+');
                    TCODConsole.root.setCharBackground(x, y, TCODColor.black);
                }
            for (int x = (Program.engine.map.renderWidth / 2) - 9; x < (Program.engine.map.renderWidth / 2) + 13; x++)
                for (int y = (Program.engine.map.renderHeight / 2) - 2; y <  (Program.engine.map.renderHeight / 2) + 2; y++)
                {
                    TCODConsole.root.setChar(x, y, 0);
                    TCODConsole.root.setCharBackground(x, y, TCODColor.black);
                }
            if (!flag) {
                i = TCODRandom.getInstance().getInt(0, texts.Count - 1);
                flag = true;
                    }
            TCODConsole.root.print(( Program.engine.map.renderWidth / 2) - 8,  (Program.engine.map.renderHeight / 2) - 1, texts[i]);
           
        }
    }
}
