using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using libtcod;
namespace Janus.Engine.Generators
{
    class CaveGenerator :MapGenerator
    {
        
        public CaveGenerator(Level level) : base(level)
        {
            generatecave();
        }

        private TCODRandom random;
        public int PercentInbetweenWalls = 10;
        public int PercentAreWalls = 40;
        public int steps = 3;
        public bool InBounds(int x, int y)
        {
            if (x < 1 || x > map.width - 1) { return false; }
            else if (y < 1 || y > map.height - 1) { return false; }
            else { return true; }
        }

        public void dig(int x, int y)
        {
            map.dig(x, y, x, y);
        }
        public void dig(Point p)
        {
            map.dig(p.X, p.Y, p.X, p.Y);
        }
        public void generatecave()
        {
            random = TCODRandom.getInstance();
            map.generator.resetMap(false);
            
            RandomFillMap();
           
            for (int i = 0; i < steps; i++)
            {
                MakeCaverns();
               
            }

           
            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    if (InBounds(x, y))
                    {
                       
                        if (GetAdjacentCount(x, y, true) > 6)
                        {
                          
                            map.setWall(x, y);
                           
                        }
                        else
                        {
                            if (GetAdjacentCount(x, y, false) > 5)
                            {
                                // dig out most walls inbetween
                                if(random.getInt(0,100) > PercentInbetweenWalls)
                                dig(x, y);

                            }
                        }
                    }
                }
            }
           
        }
        public void MakeCaverns()
        {
            // By initilizing column in the outter loop, its only created ONCE
            for (int x = 0, y = 0; x <= map.width - 1; x++)
            {
                for (y = 0; y <= map.height - 1; y++)
                {
                    if (PlaceWallLogic(x, y))
                        map.setWall(x, y);
                }
            }
        }
        public bool PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentCount(x, y, true);


            if (map.isWall(x,y))
            {
                if (numWalls >= 4)
                {
                    return true;
                }
                if (numWalls < 2)
                {
                    return false;
                }

            }
            else
            {
                if (numWalls >= 5)
                {
                    return true;
                }
            }
            return false;
        }
        public void RandomFillMap()
        {

            int mapMiddle = 0; // Temp variable
            for (int y = 0, x = 0; x < map.width; x++)
            {
                for (y = 0; y < map.height; y++)
                {
                   if(!InBounds(x,y))
                        map.setWall(x, y);
                    // Else, fill with a wall a random percent of the time
                    else
                    {
                        mapMiddle = (map.height / 2);

                        if (y == mapMiddle)
                        {
                            map.setWall(x, y);
                        }
                        else
                        {
                           if(RandomPercent(PercentAreWalls))
                            {
                                map.setWall(x, y);
                            }
                        }
                    }
                }
            }
        }
        bool RandomPercent(int percent)
        {
            if (percent >= random.getInt(1, 101))
            {
                return true;
            }
            return false;
        }
        public int GetAdjacentCount(int x, int y, int scopeX, int scopeY, bool wall)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;

            int iX = startX;
            int iY = startY;

            int wallCounter = 0;

            for (iY = startY; iY <= endY; iY++)
            {
                for (iX = startX; iX <= endX; iX++)
                {
                    if (!(iX == x && iY == y))
                    {
                        if (wall)
                        {
                            if (map.isWall(iX, iY))
                            { wallCounter++; }
                        }
                        else
                           if (!map.isWall(iX, iY))
                        { wallCounter++; }
                    }
                }
            }
            return wallCounter;
        }
      
        private int GetAdjacentCount(int x, int y, bool wall)
        {
            return GetAdjacentCount(x, y, 1, 1, wall);
        }
    }
}
