using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine;
using Janus;
using libtcod;
namespace Janus.Engine.GUI
{
    public  class LoadingGui : Gui
    {
        public int loading = 0;
        public override void update()
        {

        }
        public override void render()
        {
            base.render();
            for(int x = Program.engine.map.renderX; x < Program.engine.map.renderWidth; x++)
                for (int y = Program.engine.map.renderY; y < Program.engine.map.renderHeight; y++)
                {
                    TCODConsole.root.setChar(x, y, 0);
                    TCODConsole.root.setCharBackground(x, y, TCODColor.black);
                }
            TCODConsole.root.print((Program.engine.map.renderWidth / 2) - 5, (Program.engine.map.renderHeight / 2),"LOADING");
            for (int i = 0; i < 10; i++)
            {
         
                TCODConsole.root.setCharBackground((Program.engine.map.renderWidth / 2) - 5 + i, (Program.engine.map.renderHeight / 2), TCODColor.grey);
                TCODConsole.root.setCharBackground((Program.engine.map.renderWidth / 2) - 5 + i, (Program.engine.map.renderHeight / 2) + 1, TCODColor.grey);
            }
            if(loading > 0)
            for(int i = 0; i < loading; i++)
            {
                    if (i < 10)
                    {
                        TCODConsole.root.setCharBackground((Program.engine.map.renderWidth / 2) - 5 + i, (Program.engine.map.renderHeight / 2), TCODColor.green);
                        TCODConsole.root.setCharBackground((Program.engine.map.renderWidth / 2) - 5 + i, (Program.engine.map.renderHeight / 2) + 1, TCODColor.green);
                    }
             }
        }
    }
}
