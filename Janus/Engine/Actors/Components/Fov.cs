using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components
{
    class Fov : Component
    {
        public TCODMap fovMap;
        private int lastLevel;
        private int height, width;
        private int mapx, mapy;
        public byte strenght = 100;
        private TCODConsole lightmap;

        private Level level
        {
            get
            {
                if (owner.getActorHandler() != null)
                    return owner.getActorHandler().level;
                else
                    return Program.engine.currentLevel;
            }
        }
        private Map map { get { return level.map; } }
        public int fovRadius = 25;

        private bool initializeFov = true;
        public Fov(Actor owner, params object[] args) : base(owner, args)
        {

            if (args.Length > 0)
            {
                if (args[0].GetType() == typeof(string))
                    fovRadius = int.Parse((string)args[0]);
                else
                    fovRadius = (int)args[0];
            }
            if (args.Length > 1)
            {
                if (args[1].GetType() == typeof(string))
                    strenght = byte.Parse((string)args[1]);
                else
                    strenght = (byte)(int)args[1];
            }
            mapx = fovRadius * 2;
            mapy = fovRadius * 2;
            height = map.renderHeight + mapx;
            width = map.renderWidth + mapy;

            fovMap = new TCODMap(width, height);
            lightmap = new TCODConsole(map.renderWidth, map.renderHeight);
            lightmap.setBackgroundColor(TCODColor.black);
            lightmap.setKeyColor(TCODColor.black);
            update();
        }

        public override void update()
        {
            if (map.updateFov || initializeFov)
            {
                lightmap.clear();
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        int mx = x + map.offsetX - mapx;
                        int my = y + map.offsetY - mapy;
                        fovMap.setProperties(x, y, map.map.isTransparent(mx, my), map.map.isWalkable(mx, my));

                    }

                updateFov();
                if (owner.getActorHandler() != null)
                    initializeFov = false;
            }

            lastLevel = Program.engine.Levelnr;
            base.update();
        }

        public override void render(bool insight)
        {
            //lightmap.clear();
            int ox = owner.x - map.offsetX + mapx;
            int oy = owner.y - map.offsetY + mapy;
            if (ox >= -mapx && ox < width && oy >= -mapy && oy < height)
                for (int x = map.renderX; x < map.renderWidth; x++)
                {
                    for (int y = map.renderY; y < map.renderHeight; y++)
                    {
                        int mx = x + map.offsetX;
                        int my = y + map.offsetY;
                        if (isInFov(mx, my))//&& Program.engine.player.fov.isInFov(mx,my))
                        {

                            if (owner.x >= 0 && owner.x < map.tiles.GetLength(0) && owner.y >= 0 && owner.y < map.tiles.GetLength(1))
                                try
                                {
                                    double distance = Math.Sqrt(Math.Pow(owner.x - (mx), 2) + Math.Pow(owner.y - (my), 2));
                                    byte c = (byte)(strenght - (((float)strenght / ((float)fovRadius)) * ((byte)distance)));
                                    TCODColor color = new TCODColor(c, c, c);
                                    TCODColor newColor = color.Plus(level.lightMapConsole.getCharBackground(x, y));
                                    lightmap.setCharBackground(x, y, newColor, TCODBackgroundFlag.Set);
                                    if (map.tiles[mx, my].light + newColor.Blue <= 255)
                                        map.tiles[mx, my].light += newColor.Blue;
                                    else
                                        map.tiles[mx, my].light = 255;
                                }
                                catch (Exception e)
                                {

                                    Console.WriteLine(e.Message);
                                    throw new Exception(e.Message);
                                }
                        }
                        else
                        {
                            
                            lightmap.setCharBackground(x, y, TCODColor.black, TCODBackgroundFlag.Set);
                        }
                    }
                }
            TCODConsole.blit(lightmap, 0, 0,
                   lightmap.getWidth(), lightmap.getHeight(), level.lightMapConsole, 0, 0, (float)strenght / (255 * 2), (float)strenght / (255 * 2));


        }
        public void updateFov()
        {
            computeFov();

        }
        public void computeFov()
        {
            int x = owner.x - map.offsetX + mapx;
            int y = owner.y - map.offsetY + mapy;
            if (x >= -mapx && x < width && y >= -mapy && y < height)
                fovMap.computeFov(x, y, fovRadius, true, TCODFOVTypes.ShadowFov);

        }
        public bool isInFov(int mx, int my)
        {
            int x = mx - map.offsetX + mapx;
            int y = my - map.offsetY + mapy;
            if (mx < 0 || mx >= map.width || my < 0 || my >= map.height)
            {
                return false;
            }
            if (fovMap.isInFov(x, y))
            {
                if (owner == Program.engine.player)
                    map.tiles[mx, my].explored = true;
                
                return true;
            }
            return false;
        }
    }
}
