using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine
{

    public struct Tile
    {



        public Tile(string tileID, bool canWalk, bool transparent)
        {
            this.canWalk = canWalk;
            //this.explored = explored;
            this.tileID = tileID;
            this.explored = false;
            this.transparent = transparent;
        }
        public bool canWalk; // can we walk through this tile?
        // public bool explored; //has been explored?
        public bool explored;
        public bool transparent;
        
        public string tileID;

    };



    public class Map
    {

        public int maxPlayerMemory = 100;
        public int minPlayerMemory = 10;
        public bool showAllTiles = false;


        public int startx, starty;
        public byte[] _light;
        public string[] _tileID;
        public bool[] _explored;
        public bool[] _transparent;
        public bool[] _canWalk;
        [NonSerialized]
        public Tile[,] tiles;

        public int width;
        public int height;

        public List<Generators.Room> rooms = new List<Generators.Room>();
        [NonSerialized]
        public Generators.MapGenerator generator;
        public int renderWidth;
        public int renderHeight;
        public int renderX, renderY;
        public int offsetX;
        public int offsetY;

        public bool updateFov;
        public bool updateDynFov;

        [NonSerialized]
        private static Engine engine = Program.engine;
        [NonSerialized]
        private Level level;
        [NonSerialized]
        public TCODMap map;

        public Map(Level level, int renderX, int renderY, int renderWidth, int renderHeight)
        {
            this.width = 101;
            this.height = 101;
            tiles = new Tile[width, height];
            map = new TCODMap(width, height);
            this.renderHeight = renderHeight;
            this.renderWidth = renderWidth;
            this.renderX = renderX;
            this.renderY = renderY;

            this.level = level;

        }
        public void load(Level level)
        {
            map = new TCODMap(width, height);
            tiles = new Tile[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].transparent = _transparent[tileIndex(x, y)];
                    tiles[x, y].canWalk = _canWalk[tileIndex(x, y)];
                    tiles[x, y].explored = _explored[tileIndex(x, y)];
                    tiles[x, y].tileID = _tileID[tileIndex(x, y)];
                    map.setProperties(x, y, tiles[x,y].transparent, tiles[x,y].canWalk);
                }
            this.level = level;
        }
        public void save()
        {
            _canWalk = new bool[tiles.GetLength(1) * tiles.GetLength(0)];
            _light = new byte[tiles.GetLength(1) * tiles.GetLength(0)];
            _explored = new bool[tiles.GetLength(1) * tiles.GetLength(0)];
            _tileID = new string[tiles.GetLength(1) * tiles.GetLength(0)];
            _transparent = new bool[tiles.GetLength(1) * tiles.GetLength(0)];

            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    tiles[x, y].canWalk = map.isWalkable(x, y);
                    tiles[x, y].transparent = map.isTransparent(x, y);

                    _canWalk[tileIndex(x,y)] = tiles[x, y].canWalk;
                    _explored[tileIndex(x, y)] = tiles[x, y].explored;
                    _tileID[tileIndex(x, y)] = tiles[x, y].tileID;
                    _transparent[tileIndex(x, y)] = tiles[x, y].transparent;
                }
            }
        }

        private int tileIndex(int x, int y)
        {
            return x + y * tiles.GetLength(0);
        }
        public static TCODColor darkWall = new TCODColor(30, 30, 50);
        public static TCODColor darkGround = new TCODColor(120, 120, 140);
        public static TCODColor lightWall = new TCODColor(30, 30, 50);
        public static TCODColor lightGround = new TCODColor(70, 70, 80);
        #region methods

        public void generate()
        {
            generate(typeof(Generators.MapGenerator));
        }
        public void generate(Type mapGenerator)
        {
            engine = Program.engine;
            engine.gameStatus = GameStatus.LOADING;
            engine.loadingGui = new GUI.LoadingGui();
            tiles = new Tile[width, height];
            map = new TCODMap(width, height);
            level.actorHandler.actors = new List<Actor>();


            Message.lines = new List<Line>();
            rooms = new List<Generators.Room>();
            generator = (Generators.MapGenerator) Activator.CreateInstance(mapGenerator, level);
            generator.generate();
            updateFov = true;
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
            if (x >= 0 && x < width && y >= 0 && y < height)
                return tiles[x, y].explored;
            else
                return false;
        }

        public bool isWall(int x, int y)
        {
            return !map.isWalkable(x, y);
        }
        public void setWall(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                tiles[x, y].canWalk = false;
                tiles[x, y].transparent = false;
                tiles[x, y].tileID = "wall";

                map.setProperties(x, y, false, false);
            }
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

        public void setExplored(int x, int y, bool explored)
        {
            tiles[x, y].explored = explored;
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
                        tiles[tilex, tiley].transparent = true;
                        tiles[tilex, tiley].canWalk = true;
                        tiles[tilex, tiley].tileID = null;
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
                    if (x + offsetX >= 0 && x + offsetX < tiles.GetLength(0) && y + offsetY >= 0 && y + offsetY < tiles.GetLength(1))
                    {
                        if (isExplored(x + offsetX, y + offsetY) || showAllTiles)
                        {

                            bool explored = tiles[x + offsetX, y + offsetY].explored;
                            TCODColor src;

                            if (isWall(x + offsetX, y + offsetY))
                            {

                                src = new TCODColor(darkWall.Red, darkWall.Green, darkWall.Blue);
                                if (explored)
                                {
                                    float v = (float)(((float)((float)darkWall.getValue())));
                                    float s = (float)(((float)((float)darkWall.getSaturation())));
                                    src.setSaturation(s);
                                    src.setValue(v);
                                }
                                TCODConsole.root.setCharBackground(x, y, src);
                            }
                            else
                            {
                                src = new TCODColor(darkGround.Red, darkGround.Green, darkGround.Blue);
                                if (explored)
                                {

                                    float v = (float)(((float)((float)darkGround.getValue())));
                                    float s = (float)(((float)((float)darkGround.getSaturation())));
                                    src.setSaturation(s);
                                    src.setValue(v);
                                }
                                TCODConsole.root.setCharBackground(x, y, src);
                            }
                        }
                        
                    }
                }




            }
            
            updateFov = false;
            updateDynFov = false;
        }
    }
}
