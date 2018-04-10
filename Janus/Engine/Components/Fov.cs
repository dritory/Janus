using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components
{
    public class Fov : Component
    {

        public int height, width;
        public int mapx, mapy;
        public byte strenght = 100;
        [NonSerialized]
        public TCODConsole lightmap;

        public TCODColor color = TCODColor.lightestOrange;

        [NonSerialized]
        public TCODMap[] fovMaps;

        [NonSerialized]
        public byte[,,] calculatedMaps;
        public Level level
        {
            get
            {
                if (owner != null && owner.getActorHandler() != null)
                    return owner.getActorHandler().level;
                else
                    return Program.engine.currentLevel;
            }
        }
        public Map map { get { return level.map; } }
        public int fovRadius = 25;

        public bool initializeFov = true;
        private int oldx, oldy;
        //Args: fov radius : strenght : color
        public Fov () : base()
        {

        }
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
            if (args.Length > 2)
            {

                try
                {
                    System.Reflection.PropertyInfo property = typeof(libtcod.TCODColor).GetProperty(args[2].ToString().ToLower());
                    if (property != null)
                    {
                        color = (libtcod.TCODColor)property.GetValue(color, null);
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }

            }
            mapx = (fovRadius);
            mapy = (fovRadius);
            height = mapx;
            width = mapy;

            fovMaps = new TCODMap[2];
            fovMaps[0] = new TCODMap(width * 2, height * 2);
            fovMaps[1] = new TCODMap(width * 2, height * 2);

            calculatedMaps = new byte[width * 2, height * 2, fovMaps.Length];
            lightmap = new TCODConsole(map.renderWidth, map.renderHeight);
            lightmap.setBackgroundColor(TCODColor.black);
            lightmap.setKeyColor(TCODColor.black);
            update();
        }

        public override void load(Actor owner)
        {
            base.load(owner);
            lightmap = new TCODConsole(map.renderWidth, map.renderHeight);
            lightmap.setBackgroundColor(TCODColor.black);
            lightmap.setKeyColor(TCODColor.black);
            fovMaps = new TCODMap[2];
            fovMaps[0] = new TCODMap(width * 2, height * 2);
            fovMaps[1] = new TCODMap(width * 2, height * 2);

            calculatedMaps = new byte[width * 2, height * 2, fovMaps.Length];
            initializeFov = true;
        }

        public override void update()
        {
            if (map.updateFov || initializeFov || owner.x != oldx || owner.y != oldy)
            {
                Map m = map;
                int ox = owner.x - m.offsetX;
                int oy = owner.y - m.offsetY;
                if (ox >= -fovRadius * 2 && ox < m.renderWidth + fovRadius * 2 && oy >= -fovRadius * 2 && oy < m.renderHeight + fovRadius * 2)
                {
                    lightmap.clear();
                    for (int x = -mapx; x < width; x++)
                        for (int y = -mapx; y < height; y++)
                        {
                            int mx = owner.x + x;
                            int my = owner.y + y;
                            for (int i = 0; i < fovMaps.Length; i++)
                                fovMaps[i].setProperties(x + mapx, y + mapy, m.map.isTransparent(mx, my), m.map.isWalkable(mx, my));

                        }


                    if (owner.getActorHandler() != null)
                        initializeFov = false;
                }
                else
                    initializeFov = true;
                updateFov();
                base.update();
            }

            oldx = owner.x;
            oldy = owner.y;
        }

        public override void render(bool insight)
        {
            if (!insight && owner != null)
            {
                Map m = map;
                int ox = owner.x - m.offsetX;
                int oy = owner.y - m.offsetY;
                if (ox >= -fovRadius && ox < m.renderWidth + fovRadius && oy >= -fovRadius && oy < m.renderHeight + fovRadius)
                    if (owner.x >= 0 && owner.x < m.tiles.GetLength(0) && owner.y >= 0 && owner.y < m.tiles.GetLength(1))
                    {
                        lightmap.clear();
                        int OFX = m.renderX - ox + mapx <= 0 ? 0 : m.renderX - ox + mapx;
                        int OFY = m.renderY - oy + mapy <= 0 ? 0 : m.renderY - oy + mapy;
                        int OFW = ox + width - m.renderWidth <= 0 ? 0 : ox + width - m.renderWidth;
                        int OFH = oy + height - m.renderHeight <= 0 ? 0 : oy + height - m.renderHeight;
                        for (int x = -mapx + OFX; x < width - OFW; x++)
                        {
                            for (int y = -mapy + OFY; y < height - OFH; y++)
                            {
                                int mx = owner.x + x;
                                int my = owner.y + y;
                                double distance = Math.Sqrt(Math.Pow(owner.x - (mx), 2) + Math.Pow(owner.y - (my), 2));
                                if (distance <= fovRadius)
                                {

                                    if ((m.isExplored(mx, my) || m.showAllTiles))
                                    {
                                        try
                                        {


                                            byte rs = (byte)((strenght * color.Red) / 255);
                                            byte gs = (byte)((strenght * color.Green) / 255);
                                            byte bs = (byte)((strenght * color.Blue) / 255);

                                            byte r = (byte)(rs - (((float)rs / ((float)fovRadius)) * ((byte)distance)));
                                            byte g = (byte)(gs - (((float)gs / ((float)fovRadius)) * ((byte)distance)));
                                            byte b = (byte)(bs - (((float)bs / ((float)fovRadius)) * ((byte)distance)));

                                            TCODColor lcolor = level.lightMapConsole.getCharBackground(mx - m.offsetX, my - m.offsetY);
                                            if ((int)r + (int)lcolor.Red < 255)
                                                r += lcolor.Red;
                                            else
                                                r = 255;
                                            //r += (byte)(lcolor.Red - ((int)r + (int)lcolor.Red - 255));
                                            if ((int)g + (int)lcolor.Green < 255)
                                                g += lcolor.Green;
                                            else
                                                g = 255;
                                            //g += (byte)(lcolor.Green - ((int)g + (int)lcolor.Green - 255));
                                            if ((int)b + (int)lcolor.Blue < 255)
                                                b += lcolor.Blue;
                                            else
                                                b = 255;
                                            //b += (byte)(lcolor.Blue - ((int)b + (int)lcolor.Blue - 255));
                                            byte tile = calculatedMaps[x + mapx, y + mapy, 0];
                                            /*
                                            if (tile == 2)
                                            {
                                                if (r > (byte)(rs / 3))
                                                    r -= (byte)(rs / 3);
                                                if (g > (byte)(gs / 3))
                                                    g -= (byte)(gs / 3);
                                                if (b > (byte)(bs / 3))
                                                    b -= (byte)(bs / 3);
                                            }
                                            */
                                            TCODColor col = new TCODColor(r, g, b);
                                            if (tile != 0)
                                            {
                                                lightmap.setCharBackground(mx - m.offsetX, my - m.offsetY, col, TCODBackgroundFlag.Set);
                                            }
                                            else if (false)
                                            {

                                                tile = calculatedMaps[x + mapx, y + mapy, 1];
                                                if (tile != 0)
                                                {
                                                    if (r > (byte)(rs / 3))
                                                        r -= (byte)(rs / 3);
                                                    if (g > (byte)(gs / 3))
                                                        g -= (byte)(gs / 3);
                                                    if (b > (byte)(bs / 3))
                                                        b -= (byte)(bs / 3);
                                                    if (tile == 2)
                                                    {
                                                        if (r > (byte)(rs / 5))
                                                            r -= (byte)(rs / 5);
                                                        if (g > (byte)(gs / 5))
                                                            g -= (byte)(gs / 5);
                                                        if (b > (byte)(bs / 5))
                                                            b -= (byte)(bs / 5);
                                                    }
                                                    col = new TCODColor(r, g, b);
                                                    lightmap.setCharBackground(mx - m.offsetX, my - m.offsetY, col, TCODBackgroundFlag.Set);
                                                }
                                            }
                                        }

                                        catch (Exception e)
                                        {

                                            Console.WriteLine(e.Message);
                                            throw new Exception(e.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    //if (lightmap.getCharBackground(mx - m.offsetX, my - m.offsetY)!= TCODColor.black)
                                    //lightmap.setCharBackground(mx - m.offsetX, my - m.offsetY, TCODColor.black, TCODBackgroundFlag.Set);
                                }
                            }
                        }
                        TCODConsole.blit(lightmap, 0, 0,
                              lightmap.getWidth(), lightmap.getHeight(), level.lightMapConsole, 0, 0, (float)strenght / (255 * 2), (float)strenght / (255 * 2));

                    }
            }
        }

        public virtual void updateFov()
        {
            Map map = this.map;
            computeFov();
            int ox = owner.x - map.offsetX;
            int oy = owner.y - map.offsetY;
            if (ox >= -fovRadius * 2 && ox < map.renderWidth + fovRadius * 2 && oy >= -fovRadius * 2 && oy < map.renderHeight + fovRadius * 2)
            {
                for (int x = -mapx; x < width; x++)
                {
                    for (int y = -mapx; y < height; y++)
                    {

                        int mx = owner.x + x;
                        int my = owner.y + y;
                        for (int i = 0; i < fovMaps.Length; i++)
                        {
                            if (isInFov(mx, my, i))
                            {
                                if (map.isWall(mx, my))
                                    calculatedMaps[x + mapx, y + mapy, i] = 0;
                                else
                                    calculatedMaps[x + mapx, y + mapy, i] = 1;
                            }
                            else
                            {
                                calculatedMaps[x + mapx, y + mapy, i] = 0;
                            }
                        }
                    }
                }

            }
        }
        public virtual void computeFov()
        {
            int ox = owner.x - map.offsetX;
            int oy = owner.y - map.offsetY;
            if (ox >= -fovRadius * 2 && ox < map.renderWidth + fovRadius * 2 && oy >= -fovRadius * 2 && oy < map.renderHeight + fovRadius * 2)
                for (int i = 0; i < fovMaps.Length; i++)
                    fovMaps[i].computeFov(mapx + 1, mapy + 1, fovRadius, true, (TCODFOVTypes)(i + 3));

        }
        public virtual bool isInFov(int mx, int my)
        {
            return isInFov(mx, my, 0);
        }
        public virtual bool isInFov(int mx, int my, int fov)
        {
            int x = mx - owner.x + mapx;
            int y = my - owner.y + mapy;
            Map map = this.map;
            if (mx < 0 || mx >= map.width || my < 0 || my >= map.height)
            {
                return false;
            }
            if (fov < fovMaps.Length)
                if (fovMaps[fov].isInFov(x, y))
                {
                    if (owner == Program.engine.player)
                        map.tiles[mx, my].explored = true;

                    return true;
                }
            return false;
        }
    }
}
