using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine
{
    public class Level
    {
        public int levelnr { get { return _levelnr; } }
        private int _levelnr;
        public ActorHandler actorHandler = new ActorHandler();
        public Map map;
        [NonSerialized]
        public TCODConsole lightMapConsole = new TCODConsole(100, 50);


        public Level()
        {
            map = new Map(this, 8, 8, 80, 45);
        }
        public void initialize(bool restarting, int levelnr)
        {
             initialize(restarting, levelnr, typeof(Generators.MapGenerator));
        }

        public void initialize(bool restarting, int levelnr, Type mapGenerator)
        {
            this._levelnr = levelnr;
            Program.engine.gameStatus = GameStatus.LOADING;
            map = new Map(this, 4, 4, 100, 50);
            lightMapConsole = new TCODConsole(map.renderWidth, map.renderHeight);

            lightMapConsole.setKeyColor(TCODColor.white);
            actorHandler = new ActorHandler();
            actorHandler.initialize(restarting, this);

            Console.WriteLine("Generating Map...");
            map.generate(mapGenerator);

            Program.engine.gameStatus = GameStatus.IDLE;

        }
        public void load()
        {
            lightMapConsole = new TCODConsole(map.renderWidth,map.renderHeight);
            map.load(this);
            actorHandler.load(this);
        }
        public void save()
        {
            map.save();
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
                   lightMapConsole.getWidth(), lightMapConsole.getHeight(), TCODConsole.root, 0, 0, 0.7F, 0.7F);
            lightMapConsole.clear();
            actorHandler.render();

            map.updateDynFov = false;
            map.updateFov = false;
        }

        public void renderGui()
        {
            actorHandler.renderGui();
        }
    }
}
