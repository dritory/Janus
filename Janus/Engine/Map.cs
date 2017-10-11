using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine
{
    [Serializable]
    struct Tile
    {



        public Tile(string tileID, bool canWalk)
        {
            this.canWalk = canWalk;
            //this.explored = explored;
            this.tileID = tileID;
            this.memory = 0;
        }
        public bool canWalk; // can we walk through this tile?
        // public bool explored; //has been explored?
        public int memory;
        public string tileID;

    };



    class Map
    {

        public int maxPlayerMemory = 100;
        public int minPlayerMemory = 20;
        public bool showAllTiles = false;


        public int startx, starty;

        public Tile[,] tiles;
        public int width;
        public int height;
        public List<Generators.Room> rooms = new List<Generators.Room>();
        public Generators.MapGenerator generator;
        public int renderWidth;
        public int renderHeight;
        public int renderX, renderY;
        public int offsetX;
        public int offsetY;

        private static Engine engine = Program.engine;
        private Level level;
        public TCODMap map;
        public TCODMap torchMap;
        public Map(Level level, int renderX, int renderY, int renderWidth, int renderHeight)
        {
            this.width = 101;
            this.height = 101;
            tiles = new Tile[width, height];
            map = new TCODMap(width, height);
            torchMap = new TCODMap(renderWidth, renderHeight);
            this.renderHeight = renderHeight;
            this.renderWidth = renderWidth;
            this.renderX = renderX;
            this.renderY = renderY;

            this.level = level;


        }

        static TCODColor darkWall = new TCODColor(5, 5, 30);
        static TCODColor darkGround = new TCODColor(40, 40, 60);
        static TCODColor lightWall = new TCODColor(60, 60, 80);
        static TCODColor lightGround = new TCODColor(120, 120, 140);
        #region methods
        public void generate()
        {
            engine = Program.engine;
            engine.gameStatus = GameStatus.LOADING;
            engine.loadingGui = new GUI.LoadingGui();
            tiles = new Tile[width, height];
            map = new TCODMap(width, height);
            level.actorHandler.actors = new List<Actor>();
            

            Message.lines = new List<Line>();
            rooms = new List<Generators.Room>();
            generator = new Generators.MapGenerator(level);
            generator.generate();
            generator.debugRender();
            
            Console.WriteLine("..Succeed!");/*
            TCODConsole console = new TCODConsole(width, height);
            console.clear();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    console.setCharBackground(x, y,
              isWall(x, y) ? darkWall : darkGround);
                }
            }
            while (true)
            {
                
            
            
                TCODConsole.blit(console, 0, 0,
                   console.getWidth(), console.getHeight(), TCODConsole.root, 0, 0, 1F,1F);
                Program.engine.update();
                TCODConsole.flush();
            }
            */


        }
        public bool isExplored(int x, int y)
        {
            if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
                return tiles[x, y].memory != 0;
            else
                return false;
        }
        public bool isWall(int x, int y)
        {
            return !map.isWalkable(x, y);
        }
        public void setWall(int x, int y)
        {
            map.setProperties(x, y, false, false);
        }
        public bool isInFov(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return false;
            }
            if (map.isInFov(x, y))
            {
                tiles[x, y].memory = maxPlayerMemory;
                return true;
            }
            return false;
        }
        public bool isInTorchFov(int x, int y)
        {
            if (x < renderX || x  >= renderWidth || y  < renderY || y  >= renderHeight)
            {
                return false;
            }
            if (torchMap.isInFov(x, y))
            {
                return true;
            }
            return false;
        }
        public bool canWalk(int x, int y)
        {
            if (isWall(x, y))
                return false;
            foreach (Actor a in level.actorHandler.actors)
            {
                if (a.x == x && a.y == y && a.blocks)
                {
                    return false;
                }
            }
            return true;
        }


        public void computeFov()
        {
            if (engine.player.x >= 0 && engine.player.x < tiles.GetLength(0) && engine.player.y >= 0 && engine.player.y < tiles.GetLength(1))
                map.computeFov(engine.player.x, engine.player.y, engine.player.fovRadius, true, TCODFOVTypes.ShadowFov);
            //computeTorchFov();
        }
        public void computeTorchFov()
        {
            if (engine.player.x + offsetX >= 0 && engine.player.x + offsetX < tiles.GetLength(0) && engine.player.y + offsetY >= 0 && engine.player.y + offsetY < tiles.GetLength(1))
                torchMap.computeFov(engine.player.x+ offsetX, engine.player.y + offsetY, engine.player.fovRadius, true, TCODFOVTypes.Permissive2Fov);
        }
        public void dig(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1)
            {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }
            if (y2 < y1)
            {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }
            for (int tilex = x1; tilex <= x2; tilex++)
            {
                for (int tiley = y1; tiley <= y2; tiley++)
                {
                    if (tilex >= 0 && tiley >= 0 && tilex < width && tiley < height)
                    {
                        map.setProperties(tilex, tiley, true, true);

                    }
                }
            }
        }

        int i;
        int time;
        #endregion

        TCODNoise n = new TCODNoise(3, TCODRandom.getInstance());

        public void validate()
        {
            Program.engine.update(true);


        }

        public void update()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y].memory > minPlayerMemory)
                        tiles[x, y].memory--;
                }
            }
        }

        public void render()
        {
            if (i > 10000)
                i = 0;
            if (time > 10)
            {
                i++;
                time = 0;
            }
            time++;
            for (int x = renderX; x < renderWidth; x++)
            {
                for (int y = renderY; y < renderHeight; y++)
                {

                    if (isInFov(x + offsetX, y + offsetY))
                    {
                        
                            if (engine.player.x >= 0 && engine.player.x < tiles.GetLength(0) && engine.player.y >= 0 && engine.player.y < tiles.GetLength(1))
                                try
                                {
                                    double distance = Math.Sqrt(Math.Pow(Program.engine.player.x - (x + offsetX), 2) + Math.Pow(Program.engine.player.y - (y + offsetY), 2));

                                    TCODColor color = new TCODColor(200 - ((200 / (engine.player.fovRadius)) * ((byte)distance - 1)), 200 - ((200 / (engine.player.fovRadius)) * ((byte)distance - 1)), 200 - ((200 / (engine.player.fovRadius)) * ((byte)distance)));

                                    level.lightMapConsole.setCharBackground(x, y, color, TCODBackgroundFlag.Set);

                                }
                                catch (Exception e)
                                {

                                    Console.WriteLine(e.Message);

                                    throw new Exception(e.Message);
                                }
                        
                        if (isExplored(x + offsetX, y + offsetY) || showAllTiles)
                        {
                            TCODConsole.root.setCharBackground(x, y, isWall(x + offsetX, y + offsetY) ? lightWall : lightGround);
                        }
                    }
                    else
                    {
                        level.lightMapConsole.setCharBackground(x, y, TCODColor.black, TCODBackgroundFlag.Burn);
                        if (isExplored(x + offsetX, y + offsetY) || showAllTiles)
                        {
                            if (x + offsetX >= 0 && x + offsetX < tiles.GetLength(0) && y + offsetY >= 0 && y + offsetY < tiles.GetLength(1))
                            {
                                int memory = tiles[x + offsetX, y + offsetY].memory;
                                TCODColor src;

                                if (isWall(x + offsetX, y + offsetY))
                                {

                                    src = new TCODColor(darkWall.Red, darkWall.Green, darkWall.Blue);
                                    if (memory > 0)
                                    {
                                        float v = (float)(((float)((float)darkWall.getValue() / (float)maxPlayerMemory) * memory));
                                        float s = (float)(((float)((float)darkWall.getSaturation() / (float)maxPlayerMemory) * memory));
                                        src.setSaturation(s);
                                        src.setValue(v);
                                    }
                                    TCODConsole.root.setCharBackground(x, y, src);
                                }
                                else
                                {
                                    src = new TCODColor(darkGround.Red, darkGround.Green, darkGround.Blue);
                                    if (memory > 0)
                                    {
                                        
                                        float v = (float)(((float)((float)darkGround.getValue() / (float)maxPlayerMemory) * memory));
                                        float s = (float)(((float)((float)darkGround.getSaturation() / (float)maxPlayerMemory) * memory));
                                        src.setSaturation(s);
                                        src.setValue(v);
                                    }
                                    TCODConsole.root.setCharBackground(x, y, src);
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
