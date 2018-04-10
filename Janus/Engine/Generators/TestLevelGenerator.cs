using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Generators
{
    class TestLevelGenerator : MapGenerator
    {
        private int MAIN_ROOM_SIZE = 40;



        public TestLevelGenerator(Level level) : base(level)
        {
            
        }


        public override void generate()
        {
            resetMap(true);

            for (int x = 0; x < map.width; x++)
            {
                map.setWall(x, 0);
                map.setWall(x, map.height - 1);
            }
            for (int y = 0; y < map.height; y++)
            {
                map.setWall(0, y);
                map.setWall(map.width - 1, y);
            }


            digLevel();

            populateLevel();

            map.startx = map.width / 2;
            map.starty = map.height / 2;
            map.offsetX = 0;
            while (map.offsetX + map.renderWidth < map.startx + 10)
            {
                map.offsetX += 10;
            }
            map.offsetY = 0;
            while (map.offsetY + map.renderHeight < map.starty + 10)
            {
                map.offsetY += 10;
            }
            if (Program.engine.player != null)
            {
                Program.engine.player.x = map.startx;
                Program.engine.player.y = map.starty;
            }
        }

        public void digLevel()
        {
            map.dig((map.width - MAIN_ROOM_SIZE) / 2, (map.height - MAIN_ROOM_SIZE) / 2, (map.width + MAIN_ROOM_SIZE) / 2, (map.height + MAIN_ROOM_SIZE) / 2);
        }

        public void populateLevel()
        {
            int c1 = (map.width - MAIN_ROOM_SIZE) / 2;
            int c2 = (map.height - MAIN_ROOM_SIZE) / 2;
            int c3 = (map.width + MAIN_ROOM_SIZE) / 2;
            int c4 = (map.height + MAIN_ROOM_SIZE) / 2;

            addActor(c1 + 1, c2 + 1, "Wall Torch");
            addActor(c1 + 1, c4 - 1, "Wall Torch");

            addActor(c3 - 1, c2 + 1, "Wall Torch");
            addActor(c3 - 1, c4 - 1, "Wall Torch");

            Actor[] actors = ActorGenerator.getAllActorsOfType("items*");
            int i = actors.Length - 1;
            int j = c1 + 2;
            int k = c2 + 2;
            while(i > 0)
            {
                addActor(j, k, actors[i].name);
                j++;
                if(j > MAIN_ROOM_SIZE)
                {
                    j = c1 + 2;
                    k++;
                }
                i--;
            }

        }

    }
}
