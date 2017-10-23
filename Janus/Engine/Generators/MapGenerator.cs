using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Generators
{

    class MapGenerator
    {
        public Level level;
        public Map map { get { return level.map; } }
        public DungeonGenerator dungeonGenerator;
        public CaveGenerator caveGenerator;
        public MapGenerator(Level level)
        {
            this.level = level;
        }
        public void resetMap(bool walls)
        {
            for (int x = 0; x < map.width; x++)
                for (int y = 0; y < map.height; y++)
                {

                    map.tiles[x, y].canWalk = !walls;
                    if (map.showAllTiles == true)
                        map.tiles[x, y].explored = true;
                    map.tiles[x, y].tileID = "wall";
                    map.map.setProperties(x, y, !walls, !walls);
                }
        }

        public void generate()
        {

            resetMap(true);

            //caveGenerator = new CaveGenerator(map);
            dungeonGenerator = new DungeonGenerator(level);

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

            for (int i = 0; i < map.rooms.Count; i++)
            {
                Room room = map.rooms[i];
                createRoom(room.type, room.bounds.X, room.bounds.Y, room.bounds.X + room.bounds.Width, room.bounds.Y + room.bounds.Height);
            }


            map.validate();
        }


        public Actor addActor(int x, int y, string name)
        {
            Actor actor = ActorLoader.getActor(name);
            if (actor != null)
            {
                actor.x = x;
                actor.y = y;
                level.actorHandler.addActor(actor);
                level.actorHandler.sendToBack(actor);
                return actor;
            }
            return null;
        }


        public Actor addDoor(int x, int y, bool open)
        {
            TCODRandom rng = TCODRandom.getInstance();
            if (rng.getInt(0, 100) < 50)
            {
                Actor door = ActorLoader.getActor("Wooden Door");
                if (door != null)
                {
                    Components.Blocks.Door d = (Components.Blocks.Door)door.getComponent(typeof(Components.Blocks.Door));
                    if (d != null)
                    {
                        d.open = open;
                    }
                    if (door != null)
                    {
                        door.x = x;
                        door.y = y;
                        level.actorHandler.addActor(door);
                        level.actorHandler.sendToBack(door);
                        return door;
                    }
                }
            }
            return null;
        }

        public void addPortal(int x, int y, int deltaLevels)
        {
            Actor stair = addActor(x, y, "stair");
            if (stair != null)
            {
                Components.Blocks.Portal portal = (Components.Blocks.Portal)stair.getComponent(typeof(Components.Blocks.Portal));
                if (portal != null)
                {
                    portal.nextLevelNumber = portal.levelNumber + deltaLevels;
                }
            }


        }



        public void addRandomActorOfType(int x, int y, string type)
        {
            Actor a = ActorGenerator.getRandomActorOfType(type);
            if (a != null)
            {
                a.x = x;
                a.y = y;
                level.actorHandler.addActor(a);
                level.actorHandler.sendToBack(a);
            }
        }

        public void addMonster(int x, int y)
        {
            TCODRandom rng = TCODRandom.getInstance();
            if (rng.getInt(0, 100) < 70)
            {
                if (rng.getInt(0, 100) < 40)
                {
                    Actor kobold = ActorLoader.getActor("kobold");


                    if (kobold != null)
                    {
                        kobold.x = x; kobold.y = y;
                        level.actorHandler.addActor(kobold);
                    }

                }
                else if (rng.getInt(0, 100) < 70)
                {

                    Actor orc = ActorLoader.getActor("orc");


                    if (orc != null)
                    {
                        orc.x = x; orc.y = y;
                        level.actorHandler.addActor(orc);
                    }

                }
                else
                {
                    Actor troll = ActorLoader.getActor("troll");

                    if (troll != null)
                    {
                        troll.x = x; troll.y = y;
                        level.actorHandler.addActor(troll);
                    }
                }
            }
        }

        public void debugRender(int iterations)
        {
            for (int i = 0; i < iterations; i++)
                TCODConsole.flush();
            level.render();
            level.update();
        }
        public void debugRender()
        {
            debugRender(1);
        }

        public void createRoom(RoomType type, int x1, int y1, int x2, int y2)
        {
            if (type == RoomType.entrance)
            {
                map.startx = x1 + ((x2 - x1) / 2);
                map.starty = y1 + ((y2 - y1) / 2);
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
                addActor(map.startx + 2, map.starty, "Torch");
                addPortal(map.startx, map.starty, 1);

            }
            else if (type == RoomType.exit)
            {
                addPortal(x1 + ((x2 - x1) / 2), y1 + ((y2 - y1) / 2), -1);
            }
            else
            {
                TCODRandom rng = TCODRandom.getInstance();
                int nbMonsters = rng.getInt(0, DungeonGenerator.ROOM_MAX_MONSTERS);
                while (nbMonsters > 0)
                {
                    int x = rng.getInt(x1, x2);
                    int y = rng.getInt(y1, y2);
                    if (map.canWalk(x, y))
                    {
                        addRandomActorOfType(x, y, "creature*");
                    }
                    nbMonsters--;
                }
                int nbItems = rng.getInt(0, DungeonGenerator.ROOM_MAX_ITEMS);
                while (nbItems > 0)
                {
                    int x = rng.getInt(x1, x2);
                    int y = rng.getInt(y1, y2);
                    if (map.canWalk(x, y))
                    {
                        addRandomActorOfType(x, y,"items*");
                    }
                    nbItems--;
                }
            }

        }
    }

}
