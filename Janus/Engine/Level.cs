using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine
{
    class Level
    {
        public int levelnr { get; }
        public ActorHandler actorHandler;
        public Map map;
        public TCODConsole lightMapConsole;


        public Level(int levelnr)
        {
            map = new Map(this, 8, 8, 80, 45);
            this.levelnr = levelnr;
        }
        public void initialize(bool restarting)
        {
            Program.engine.gameStatus = GameStatus.LOADING;
            map = new Map(this, 4, 4, 100, 50);
            lightMapConsole = new TCODConsole(map.renderWidth, map.renderHeight);
            
            lightMapConsole.setKeyColor(TCODColor.white);
            actorHandler = new ActorHandler(this);
            actorHandler.initialize(restarting);

            Console.WriteLine("Generating Map...");
            map.generate();
            
            Program.engine.gameStatus = GameStatus.STARTUP;
        }

        public void update()
        {
            
            actorHandler.update();
            map.update();
        }
        public void render()
        {


            
            map.render();
            TCODConsole.blit(lightMapConsole, 0, 0,
                   lightMapConsole.getWidth(), lightMapConsole.getHeight(), TCODConsole.root, 0, 0, 0.5F, 0.5F);
            lightMapConsole.clear();
            actorHandler.render();
        }
    }
}
