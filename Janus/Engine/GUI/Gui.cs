using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.GUI {
    class Gui {

      
        public Gui() {
           
        }

        public virtual void update() {
            
        }
        public virtual void render() {
            
            for (int x = Program.engine.map.renderX - 1; x < Program.engine.map.renderWidth + 1; x++) {
                TCODConsole.root.setChar(x, Program.engine.map.renderX - 1, 196);
                TCODConsole.root.setCharForeground(x, Program.engine.map.renderX - 1, TCODColor.green);
                        TCODConsole.root.setChar(x, Program.engine.map.renderHeight, 196);
                        TCODConsole.root.setCharForeground(x, Program.engine.map.renderHeight, TCODColor.green);
            }
            for (int y = Program.engine.map.renderY - 1; y < Program.engine.map.renderHeight + 1; y++) {
                TCODConsole.root.setChar(Program.engine.map.renderX - 1, y, 179);
                TCODConsole.root.setCharForeground(Program.engine.map.renderX - 1, y, TCODColor.green);
                TCODConsole.root.setChar(Program.engine.map.renderWidth, y, 179);
                TCODConsole.root.setCharForeground(Program.engine.map.renderWidth, y, TCODColor.green);
            }
            TCODConsole.root.setChar(Program.engine.map.renderX - 1, Program.engine.map.renderY - 1, 218);
            TCODConsole.root.setCharForeground(Program.engine.map.renderX - 1, Program.engine.map.renderY - 1, TCODColor.green);
            TCODConsole.root.setChar(Program.engine.map.renderX - 1, Program.engine.map.renderHeight, 192);
            TCODConsole.root.setCharForeground(Program.engine.map.renderX - 1, Program.engine.map.renderHeight, TCODColor.green);
            TCODConsole.root.setChar(Program.engine.map.renderWidth, Program.engine.map.renderHeight, 217);
            TCODConsole.root.setCharForeground(Program.engine.map.renderWidth, Program.engine.map.renderHeight, TCODColor.green);
            TCODConsole.root.setChar(Program.engine.map.renderWidth, Program.engine.map.renderY - 1, 191);
            TCODConsole.root.setCharForeground(Program.engine.map.renderWidth, Program.engine.map.renderY - 1, TCODColor.green);
        
             }
    }

}
