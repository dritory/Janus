using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
using System.Drawing;

/// 1. Place a number of randomly sized and positioned rooms. If a room
///    overlaps an existing room, it is discarded. Any remaining rooms are
///    carved out.
/// 2. Any remaining solid areas are filled in with mazes. The maze generator
///    will grow and fill in even odd-shaped areas, but will not touch any
///    rooms.
/// 3. The result of the previous two steps is a series of unconnected rooms
///    and mazes. We walk the stage and find every tile that can be a
///    "connector". This is a solid tile that is adjacent to two unconnected
///    regions.
/// 4. We randomly choose connectors and open them or place a door there until
///    all of the unconnected regions have been joined. There is also a slight
///    chance to carve a connector between two already-joined regions, so that
///    the dungeon isn't single connected.
/// 5. The mazes will have a lot of dead ends. Finally, we remove those by
///    repeatedly filling in any open tile that's closed on three sides. When
///    this is done, every corridor in a maze actually leads somewhere.
///



namespace Janus.Engine.Generators
{

    enum RoomType
    {
        empty, normal, entrance, exit
    }

    class Room
    {
        public Rectangle bounds;
        public RoomType type;
        public Room(int x, int y, int width, int height)
        {
            bounds.X = x;
            bounds.Y = y;
            bounds.Height = height;
            bounds.Width = width;
            this.type = RoomType.empty;
        }
        public Room(int x, int y, int width, int height, RoomType type)
        {
            bounds.X = x;
            bounds.Y = y;
            bounds.Height = height;
            bounds.Width = width;
            this.type = type;
        }

    }
    class DungeonGenerator : MapGenerator
    {
        public enum Direction
        {
            NORTH, WEST, EAST, SOUTH, NULL
        }


        public bool debugDraw = false;

        public List<Point> junctions = new List<Point>();

        public int roomExtraSize = 1;
        public int windingPercent = 20;
        public int deadEndLength = 3;
        public int roomTries;

        public int overlapPercent = 20;
        public int maxRoomTries = 500;
        public int minRoomTries = 100;
        public int maxExtraJunctions = 30;
        public int roomNumber;

        public const int ROOM_MAX_SIZE = 11;
        public const int ROOM_MIN_SIZE = 5;
        public const int ROOM_MAX_MONSTERS = 3;
        public const int ROOM_MAX_ITEMS = 2;
        public int extraConnectorChance = 100;
        public int[,] regions;
        public int currentRegion = 1;
        public DungeonGenerator(Level level)
            : base(level)
        {

            TCODRandom rng = TCODRandom.getInstance();
            roomTries = rng.getInt(minRoomTries, maxRoomTries);
            windingPercent = rng.getInt(5, 100);
            roomExtraSize = rng.getInt(0, 5);
            extraConnectorChance = rng.getInt(10, 100);
            regions = new int[map.width, map.height];

            if (debugDraw)
            {
                map.showAllTiles = true;

                Program.engine.update();
                TCODConsole.root.clear();
                TCODConsole.flush();
            }


            addRooms();
            Program.engine.loadingGui.loading++;
            Program.engine.render();
            roomNumber = currentRegion + 1;
            startRegion();

            for (var y = 1; y < map.height; y += 2)
            {
                for (var x = 1; x < map.width; x += 2)
                {
                    if (map.isWall(x, y)) continue;

                    growMaze(x, y);
                }
            }
            connectRegions();
            addRandomJuctions();
            Program.engine.loadingGui.loading++;
            Program.engine.render();
            removeDeadEnds();
            Program.engine.loadingGui.loading++;
            Program.engine.render();
            smooth();
            Program.engine.loadingGui.loading = 10;
            Program.engine.render();

            if (debugDraw)
            {
                map.showAllTiles = false;
                Program.engine.update();
            }

        }
        public void addRooms()
        {
            TCODRandom rng = TCODRandom.getInstance();
            for (int i = 0; i < roomTries; i++)
            {
                //  Program.engine.render();
                //  Program.engine.update();
                //  TCODConsole.flush();


                int size = rng.getInt(ROOM_MIN_SIZE, ROOM_MAX_SIZE + roomExtraSize);
                if (rng.getInt(0, 1) == 0)
                {
                    size = rng.getInt(ROOM_MIN_SIZE, ROOM_MAX_SIZE / 2);
                }
                int rectangularity = rng.getInt(0, 1 + (size / 2)) * 2;
                int width = size;
                int height = size;
                if (rng.getInt(0, 1) == 1)
                {
                    width += rectangularity;
                }
                else
                {
                    height += rectangularity;
                }

                int x = rng.getInt(0, ((map.width - width)));
                int y = rng.getInt(0, ((map.height - height)));

                if ((float)x / 2 == x / 2)
                {
                    x += 1;
                }
                if ((float)y / 2 == y / 2)
                {
                    y += 1;
                }

                if ((float)(height) / 2 != height / 2)
                {
                    height += 1;
                }

                if ((float)(width) / 2 != width / 2)
                {
                    width += 1;
                }

                Room room = new Room(x, y, width, height);

                bool overlaps = false;
                /*
                for (int j = 0; j < map.rooms.Count; j++)
                {
                    Rectangle b = new Rectangle(room.bounds.X - 1, room.bounds.Y - 1, room.bounds.Width + 1, room.bounds.Height + 1);
                    Rectangle ob = new Rectangle(map.rooms[j].bounds.X - 1, map.rooms[j].bounds.Y - 1, map.rooms[j].bounds.Width + 1, map.rooms[j].bounds.Height + 1);
                    if (b.IntersectsWith(ob))
                    {
                        overlaps = true;
                        break;
                    }
                }
                */
                if (!overlaps)
                {
                    int overlapNum = 0;
                    for (int a = room.bounds.X - 1; a < room.bounds.X + room.bounds.Width + 2; a++)
                        for (int b = room.bounds.Y - 1; b < room.bounds.Y + room.bounds.Height + 2; b++)
                        {
                            if (!map.isWall(a, b))
                            {
                                overlapNum++;
                            }

                        }
                    if (overlapPercent * overlapNum > (((room.bounds.X) * (room.bounds.Y)) / 100))
                        overlaps = true;
                }
                if (!overlaps)
                {
                    RoomType type = RoomType.exit;
                    if (currentRegion == 1)
                    {
                        type = RoomType.entrance;
                    }
                    else if (map.rooms.Last().type == RoomType.exit)
                        map.rooms.Last().type = RoomType.empty;

                    room.type = type;

                    startRegion();
                    map.rooms.Add(room);

                    if (debugDraw)
                    {
                        Program.engine.render();

                        TCODConsole.flush();
                    }
                    for (int a = room.bounds.X - 1; a < room.bounds.X + room.bounds.Width + 2; a++)
                        for (int b = room.bounds.Y - 1; b < room.bounds.Y + room.bounds.Height + 2; b++)
                        {
                            if (a < room.bounds.X || b < room.bounds.Y || a > room.bounds.X + room.bounds.Width || b > room.bounds.Y + room.bounds.Height)
                            {
                                if (map.isWall(a, b))
                                    map.setWall(a, b);
                            }
                            else
                                carve(a, b);
                        }
                }

            }
        }
        public void growMaze(int x, int y)
        {
            TCODRandom rng = TCODRandom.getInstance();
            List<Point> cells = new List<Point>();
            Direction lastDir = Direction.NULL;

            map.dig(x, y, x, y);

            cells.Add(new Point(x, y));

            while (cells.Count > 0 && !TCODConsole.isWindowClosed())
            // for(int j = 0; j < 10; j ++)
            {

                if (cells.Count > 0)
                {
                    if (cells.Count > 1000)
                    {

                    }
                    Point cell = cells.Last();
                    List<Direction> unmadeCells = new List<Direction>();
                    foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                    {
                        if (dir != Direction.NULL)
                        {
                            if (canDig(cell.X, cell.Y, dir))
                                unmadeCells.Add(dir);
                            else
                            {
                                Point p = dirToPoint(dir);
                            }
                        }
                    }
                    if (unmadeCells.Count > 0)
                    {
                        if (unmadeCells.Count > 10000)
                        {

                        }
                        Direction dir = Direction.NULL;

                        if (unmadeCells.Contains(lastDir) && rng.getInt(0, 100) > windingPercent)
                        {
                            dir = lastDir;
                        }
                        else
                        {
                            dir = unmadeCells[rng.getInt(0, unmadeCells.Count - 1)];
                        }
                        Point p = dirToPoint(dir);
                        //map.dig(cell.X + p.X, cell.Y + p.Y, cell.X + p.X * 2, cell.Y + p.Y * 2);
                        carve(cell.X + p.X, cell.Y + p.Y);
                        carve(cell.X + p.X * 2, cell.Y + p.Y * 2);
                        // cells.Add(new Point(cell.X + p.X, cell.Y + p.Y));
                        cells.Add(new Point(cell.X + p.X * 2, cell.Y + p.Y * 2));
                        lastDir = dir;
                    }
                    else
                    {

                        cells.Remove(cells.Last());
                        lastDir = Direction.NULL;
                    }


                }

            }
        }


        public void connectRegions()
        {

            List<int[]> _regions = new List<int[]>();
            List<Point> _points = new List<Point>();
            TCODRandom rng = TCODRandom.getInstance();
            List<Point> extraJunctions = new List<Point>();
            for (var y = 1; y < map.height - 1; y++)
            {
                for (var x = 1; x < map.width - 1; x++)
                {

                    if (map.isWall(x, y))
                    {

                        List<int> regs = new List<int>();

                        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                        {
                            if (dir != Direction.NULL)
                            {
                                Point p = dirToPoint(dir);
                                if (x + p.X < regions.GetLength(0) - 1 && y + p.Y < regions.GetLength(1) - 1 && x + p.X > 0 && y + p.Y > 0)
                                {
                                    int region = regions[x + p.X, y + p.Y];
                                    if (region != 0 && !regs.Contains(region))
                                        regs.Add(region);
                                }

                            }
                        }
                        if (regs.Count < 2)
                        {
                            if (rng.getInt(0, extraConnectorChance * extraConnectorChance) == 0)
                            {
                                extraJunctions.Add(new Point(x, y));


                            }
                            continue;
                        }
                        _regions.Add(regs.ToArray());
                        _points.Add(new Point(x, y));


                    }
                    else
                    {

                    }
                }

            }




            int openRegions = currentRegion;
            int tries = 0;

            while (_regions.Count > 1 && !TCODConsole.isWindowClosed())
            {




                int rnum = TCODRandom.getInstance().getInt(0, _regions.Count - 1);
                if (_regions.Count > 0)
                {
                    int[] connectorReg = _regions[rnum];
                    Point connectorPos = _points[rnum];


                    addJunction(connectorPos.X, connectorPos.Y);

                    List<int> sources = new List<int>();
                    int dest = connectorReg[0];
                    bool foo = true;

                    for (int y = 1; y < map.height - 1; y++)
                    {

                        for (int x = 1; x < map.width - 1; x++)
                        {

                            for (int j = 0; j < connectorReg.Length; j++)
                            {

                                if (regions[x, y] == connectorReg[j] && regions[x, y] != dest)
                                {
                                    regions[x, y] = dest;
                                    if (foo)
                                    {

                                        foo = false;

                                    }
                                }
                            }
                        }
                    }
                    if (!foo)
                    {

                        openRegions--;
                    }



                    _regions.Remove(connectorReg);
                    _points.Remove(connectorPos);


                    for (int i = 0; i < _regions.Count; i++)
                    {
                        if (i < _regions.Count)
                        {


                            if (Math.Sqrt(Math.Pow(connectorPos.X - _points[i].X, 2) + Math.Pow(connectorPos.Y - _points[i].Y, 2)) < 2)
                            {
                                _points.RemoveAt(i);
                                _regions.RemoveAt(i);
                                continue;
                            }
                            List<int> regs = new List<int>();
                            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                            {
                                if (dir != Direction.NULL)
                                {
                                    Point p = dirToPoint(dir);
                                    if (_points[i].X + p.X < regions.GetLength(0) - 1 && _points[i].Y + p.Y < regions.GetLength(1) - 1 && _points[i].X + p.X > 0 && _points[i].Y + p.Y > 0)
                                    {
                                        int region = regions[_points[i].X + p.X, _points[i].Y + p.Y];
                                        if (region != 0 && !regs.Contains(region))
                                            regs.Add(region);


                                    }
                                }
                            }
                            _regions[i] = regs.ToArray();
                            if (_regions[i].Length > 1)
                            {
                                continue;
                            }

                            if (rng.getInt(0, extraConnectorChance) == 0)
                            {
                                if (!extraJunctions.Contains(_points[i]))
                                    extraJunctions.Add(_points[i]);
                            }

                            _points.RemoveAt(i);
                            _regions.RemoveAt(i);
                        }
                    }



                }
                else
                {
                    break;
                }
                tries++;
            }


            for (int i = 0; i < maxExtraJunctions; i++)
            {
                if (extraJunctions.Count > 0)
                {
                    Point p = extraJunctions[rng.getInt(0, extraJunctions.Count - 1)];
                    addJunction(p.X, p.Y);
                }
            }
            /*
            int[] merged = null;
            if (currentRegion > 0)
                merged = new int[currentRegion + 1];
            List<int> openRegions = new List<int>();
            for (int i = 0; i <= currentRegion; i++)
            {
                openRegions.Add(i);
                if (merged != null)
                    merged[i] = i;
            }
            TCODRandom rng = TCODRandom.getInstance();

            while (openRegions.Count > 1 && !TCODConsole.isWindowClosed())
            {

                ConnectorCell connector = connectorRegions[rng.getInt(0, connectorRegions.Count - 1)];
                addJunction(connector.pos.X, connector.pos.Y);

                List<int> regs = new List<int>();


                regs = merged.ToList();
                int dest = regs[0];
                int[] sources = regs.Skip(1).ToArray();
                for (int i = 0; i <= currentRegion; i++)
                {
                    if (sources.Contains(merged[i]))
                    {
                        merged[i] = dest;
                    }
                }
                for (int i = 0; i < sources.Length; i++)
                {
                    openRegions.Remove(sources[i]);
                }

                for (int i = 0; i < connectorRegions.Count; i++)
                {
                    if (i < connectorRegions.Count)
                    {
                        ConnectorCell c = connectorRegions[i];
                        if (connector.pos.X - c.pos.X < 2 || connector.pos.Y - c.pos.Y < 2)
                        {
                            connectorRegions.Remove(c);
                            continue;
                        }
                        if (regions.Length > 1) { continue; }

                        if (rng.getInt(0, extraConnectorChance) > 0)
                        {
                            addJunction(c.pos.X, c.pos.Y);
                            connectorRegions.Remove(c);
                            continue;
                        }
                    }
                }

            }
     */




        }

        public void addRandomJuctions()
        {
            for (int y = 1; y < map.height - 1; y++)
            {

                for (int x = 1; x < map.width - 1; x++)
                {
                    if ((map.isWall(x - 1, y) && map.isWall(x + 1, y)) || map.isWall(x, y - 1) && map.isWall(x, y + 1))
                    {
                        int rand = TCODRandom.getInstance().getInt(0, 1000);
                        if(rand > 1000 - (1000/ extraConnectorChance))
                        {
                            addJunction(x, y);
                        }
                    }
                }
            }
        }



        public void addJunction(int x, int y)
        {
            junctions.Add(new Point(x, y));
            TCODRandom rng = TCODRandom.getInstance();
            map.dig(x, y, x, y);
            if (rng.getInt(1, 4) == 1)
            {
                if (rng.getInt(1, 3) == 1)
                {
                    addDoor(x, y, true);
                }
            }
            else
            {
                addDoor(x, y, false);

            }


        }


        public void smooth()
        {
            bool done = false;
            int tries = 0;
            int l = 0;

            int percent = windingPercent ^ 2;

            while (!done && !TCODConsole.isWindowClosed() && tries < percent)
            {
                // Program.engine.update();
                if (debugDraw)
                {
                    Program.engine.render();
                    TCODConsole.flush();
                }
                done = true;

                percent = (windingPercent ^ 2) + TCODRandom.getInstance().getInt(-50, 50);

                for (var y = 1; y < map.height; y += 1)
                {
                    for (var x = 1; x < map.width; x += 1)
                    {
                        if (!map.isWall(x, y))
                        {

                            List<Point> exits = new List<Point>();
                            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                            {
                                if (dir != Direction.NULL)
                                {
                                    Point p = dirToPoint(dir);
                                    if (!map.isWall(x + p.X, y + p.Y))
                                        exits.Add(p);
                                }
                            }
                            if (exits.Count == 2)
                            {
                                Point a = exits[0];
                                Point b = exits[1];
                                if (a.X != 0) //checks if pathway is horisontal or vertical
                                {

                                    if (map.isWall(x + (a.X * 2), y) && map.isWall(x + (b.X * 2), y))
                                    {
                                        if (!map.isWall(x + a.X, y + 1) && !map.isWall(x + b.X, y + 1))
                                        {
                                            if (map.isWall(x + a.X, y - 1) && map.isWall(x + b.X, y - 1))
                                            {

                                                map.setWall(x + a.X, y);
                                                map.setWall(x + b.X, y);
                                                map.setWall(x, y);
                                                map.dig(x, y + 1, x, y + 1);


                                                done = false;
                                            }
                                        }
                                        else
                                         if (!map.isWall(x + a.X, y - 1) && !map.isWall(x + b.X, y - 1))
                                        {
                                            if (map.isWall(x + a.X, y + 1) && map.isWall(x + b.X, y + 1))
                                            {

                                                map.setWall(x + a.X, y);
                                                map.setWall(x + b.X, y);
                                                map.setWall(x, y);
                                                map.dig(x, y - 1, x, y - 1);

                                                done = false;
                                            }
                                        }
                                    }

                                }
                                else if (a.Y != 0)
                                {
                                    if (map.isWall(x, y + (a.Y * 2)) && map.isWall(x, y + (b.Y * 2)))
                                    {
                                        if (!map.isWall(x + 1, y + a.Y) && !map.isWall(x + 1, y + b.Y))
                                        {
                                            if (map.isWall(x - 1, y + a.Y) && map.isWall(x - 1, y + b.Y))
                                            {

                                                map.setWall(x, y + a.Y);
                                                map.setWall(x, y + b.Y);
                                                map.setWall(x, y);
                                                map.dig(x + 1, y, x + 1, y);

                                                done = false;
                                            }
                                        }
                                        else

                                            if (!map.isWall(x - 1, y + a.Y) && !map.isWall(x - 1, y + b.Y))
                                        {
                                            if (map.isWall(x + 1, y + a.Y) && map.isWall(x + 1, y + b.Y))
                                            {

                                                map.setWall(x, y + a.Y);
                                                map.setWall(x, y + b.Y);
                                                map.setWall(x, y);
                                                map.dig(x - 1, y, x - 1, y);

                                                done = false;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }


                tries++;
                l++;
                if (l < (windingPercent * windingPercent) / 4)
                {
                    Program.engine.loadingGui.loading++;
                    Program.engine.render();
                    l = 0;
                }
            }
            done = false;
            tries = 0;
            while (!done && !TCODConsole.isWindowClosed() && tries < percent)
            {
                // Program.engine.update();
                //Program.engine.render();

                //TCODConsole.flush();
                if (debugDraw)
                {
                    Program.engine.render();
                    TCODConsole.flush();
                }
                done = true;

                percent = (windingPercent ^ 2) + TCODRandom.getInstance().getInt(-50, 50);

                for (var y = 1; y < map.height; y += 1)
                {
                    for (var x = 1; x < map.width; x += 1)
                    {
                        if (!map.isWall(x, y))
                        {

                            List<Point> exits = new List<Point>();
                            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                            {
                                if (dir != Direction.NULL)
                                {
                                    Point p = dirToPoint(dir);
                                    if (!map.isWall(x + p.X, y + p.Y))
                                        exits.Add(p);
                                }
                            }
                            if (exits.Count == 2)
                            {
                                Point a = exits[0];
                                Point b = exits[1];
                                if (a.X != 0) //checks if pathway is horisontal or vertical
                                {

                                    if (map.isWall(x + (a.X * 3), y) && map.isWall(x + (b.X * 3), y))
                                    {
                                        if (!map.isWall(x + (a.X * 2), y) && !map.isWall(x + (b.X * 2), y))
                                        {
                                            if (!map.isWall(x + (a.X * 2), y + 1) && !map.isWall(x + (b.X * 2), y + 1))
                                            {
                                                if (map.isWall(x + (a.X * 2), y - 1) && map.isWall(x + (b.X * 2), y - 1))
                                                {
                                                    map.setWall(x + a.X, y);
                                                    map.setWall(x + b.X, y);
                                                    map.setWall(x + (a.X * 2), y);
                                                    map.setWall(x + (b.X * 2), y);
                                                    map.setWall(x, y);
                                                    map.dig(x, y + 1, x, y + 1);
                                                    map.dig(x + a.X, y + 1, x + a.X, y + 1);
                                                    map.dig(x + b.X, y + 1, x + b.X, y + 1);

                                                    done = false;
                                                }
                                            }
                                            else

                                            if (!map.isWall(x + (a.X * 2), y - 1) && !map.isWall(x + (b.X * 2), y - 1))
                                            {
                                                if (map.isWall(x + (a.X * 2), y + 1) && map.isWall(x + (b.X * 2), y + 1))
                                                {
                                                    map.setWall(x + a.X, y);
                                                    map.setWall(x + b.X, y);
                                                    map.setWall(x + (a.X * 2), y);
                                                    map.setWall(x + (b.X * 2), y);
                                                    map.setWall(x, y);
                                                    map.dig(x, y - 1, x, y - 1);
                                                    map.dig(x + a.X, y - 1, x + a.X, y - 1);
                                                    map.dig(x + b.X, y - 1, x + b.X, y - 1);

                                                    done = false;
                                                }
                                            }



                                        }
                                    }
                                    else if (a.Y != 0)
                                    {
                                        if (map.isWall(x, y + (a.Y * 3)) && map.isWall(x, y + (b.Y * 3)))
                                        {
                                            if (!map.isWall(x, y + (a.Y * 2)) && !map.isWall(x, y + (b.Y * 2)))
                                            {
                                                if (!map.isWall(x + 1, y + (a.Y * 2)) && !map.isWall(x + 1, y + (b.Y * 2)))
                                                {
                                                    if (map.isWall(x - 1, y + (a.Y * 2)) && map.isWall(x - 1, y + (b.Y * 2)))
                                                    {
                                                        map.setWall(x, y + a.Y);
                                                        map.setWall(x, y + b.Y);
                                                        map.setWall(x, y + (a.Y * 2));
                                                        map.setWall(x, y + (b.Y * 2));
                                                        map.setWall(x, y);
                                                        map.dig(x + 1, y + 1, x, y);
                                                        map.dig(x + 1, y + a.Y, x + 1, y + a.Y);
                                                        map.dig(x + 1, y + b.Y, x + 1, y + b.Y);

                                                        done = false;
                                                    }
                                                }
                                                else

                                                 if (!map.isWall(x + (a.X * 2), y + 1) && !map.isWall(x + (b.X * 2), y + 1))
                                                {
                                                    if (map.isWall(x + (a.X * 2), y - 1) && map.isWall(x + (b.X * 2), y - 1))

                                                    {
                                                        map.setWall(x, y + a.Y);
                                                        map.setWall(x, y + b.Y);
                                                        map.setWall(x, y + (a.Y * 2));
                                                        map.setWall(x, y + (b.Y * 2));
                                                        map.setWall(x, y);
                                                        map.dig(x - 1, y - 1, x, y);
                                                        map.dig(x - 1, y + a.Y, x - 1, y + a.Y);
                                                        map.dig(x - 1, y + b.Y, x - 1, y + b.Y);

                                                        done = false;
                                                    }
                                                }



                                            }
                                        }

                                    }
                                }

                            }
                        }
                    }



                }
                tries++;
            }
        }

        public void removeDeadEnds()
        {
            bool done = false;
            int tries = 0;
            int l = 0;
            List<Point> ignores = new List<Point>();
            while (!done && !TCODConsole.isWindowClosed() && tries < 100)
            {
                // Program.engine.update();
                //Program.engine.render();
                //TCODConsole.flush();
                if (debugDraw)
                {
                    Program.engine.render();
                    TCODConsole.flush();
                }
                done = true;


                for (var y = 1; y < map.height; y += 1)
                {
                    for (var x = 1; x < map.width; x += 1)
                    {
                        if (!map.isWall(x, y))
                        {

                            List<Point> exits = new List<Point>();
                            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                            {
                                if (dir != Direction.NULL)
                                {
                                    Point p = dirToPoint(dir);
                                    if (!map.isWall(x + p.X, y + p.Y) || ignores.Contains(new Point(x + p.X, y + p.Y)))
                                        exits.Add(p);
                                }
                            }
                            if (exits.Count == 1)
                            {
                                if (TCODRandom.getInstance().getInt(0, 100) < 100 - deadEndLength)
                                {
                                    done = false;
                                    map.setWall(x, y);
                                }
                                else
                                    ignores.Add(new Point(x, y));
                            }

                        }
                    }
                }
                tries++;
                l++;
                if (l > (100 - deadEndLength) / 5)
                {
                    Program.engine.loadingGui.loading++;
                    Program.engine.render();
                    l = 0;
                }
            }

        }

        public void startRegion()
        {
            // TCODConsole.flush();
            //Program.engine.render();
            currentRegion++;
        }
        public void carve(int x, int y)
        {
            map.dig(x, y, x, y);
            if (x > 0 && y > 0 && x < regions.GetLength(0) - 1 && y < regions.GetLength(1) - 1)
            {
                regions[x, y] = currentRegion;
            }
        }

        public Point dirToPoint(Direction dir)
        {
            int x = 0, y = 0;

            switch (dir)
            {
                case Direction.NORTH:
                    {
                        y = -1;
                        break;
                    }
                case Direction.EAST:
                    {
                        x = 1;
                        break;
                    }
                case Direction.SOUTH:
                    {
                        y = 1;
                        break;
                    }
                case Direction.WEST:
                    {
                        x = -1;
                        break;
                    }
            }
            return new Point(x, y);
        }


        public bool canDig(int x, int y, Direction dir)
        {
            Point p = dirToPoint(dir);
            if (p.X == 0 && p.Y == 0)
                return false;
            if (y > 0 && x > 0)
                if (y + p.Y * 3 > 0 && x + p.X * 3 > 0)
                    if (x + p.X * 3 < map.width && y + p.Y * 3 < map.height)
                        if (map.isWall(x + p.X * 2, y + p.Y * 2) && map.isWall(x + p.X, y + p.Y))
                            return true;

            return false;
        }





    }
}











