using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components
{
    public class DynamicFov : Fov
    {
        public DynamicFov() : base()
        {
            mapx = 0;
            mapy = 0;
            height = map.renderHeight + (mapy);
            width = map.renderWidth + (mapx);

            fovMaps = new TCODMap[1];
            fovMaps[0] = new TCODMap(width, height);
            lightmap = new TCODConsole(map.renderWidth, map.renderHeight);
            lightmap.setBackgroundColor(TCODColor.black);
            lightmap.setKeyColor(TCODColor.black);
            update();
        }
        public DynamicFov(Actor owner, params object[] args) : base(owner, args)
        {
            mapx = 0;
            mapy = 0;
            height = map.renderHeight + (mapy);
            width = map.renderWidth + (mapx);

            fovMaps = new TCODMap[1];
            fovMaps[0] = new TCODMap(width, height);
            lightmap = new TCODConsole(map.renderWidth, map.renderHeight);
            lightmap.setBackgroundColor(TCODColor.black);
            lightmap.setKeyColor(TCODColor.black);
            update();
        }
        public override void load(Actor owner)
        {
            base.load(owner);
            mapx = 0;
            mapy = 0;
            height = map.renderHeight + (mapy);
            width = map.renderWidth + (mapx);

            fovMaps = new TCODMap[1];
            fovMaps[0] = new TCODMap(width, height);
            lightmap = new TCODConsole(map.renderWidth, map.renderHeight);
            lightmap.setBackgroundColor(TCODColor.black);
            lightmap.setKeyColor(TCODColor.black);
            initializeFov = true;
            update();
        }
        public override void update()
        {
            if ((map.updateDynFov || map.updateFov || initializeFov) && owner != null)
            {

                int ox = owner.x - map.offsetX + mapx;
                int oy = owner.y - map.offsetY + mapy;
                if (ox >= -mapx && ox < width && oy >= -mapy && oy < height)
                {
                    lightmap.clear();
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            int mx = x + map.offsetX - mapx;
                            int my = y + map.offsetY - mapy;
                            fovMaps[0].setProperties(x, y, map.map.isTransparent(mx, my), map.map.isWalkable(mx, my));

                        }

                    updateFov();
                    if (owner.getActorHandler() != null)
                        initializeFov = false;
                }
                else
                    initializeFov = true;

            }

        }
        public override void updateFov()
        {
            computeFov();
        }
        public override void computeFov()
        {
            int x = owner.x - map.offsetX + mapx;
            int y = owner.y - map.offsetY + mapy;
            if (x >= -mapx && x < width && y >= -mapy && y < height)
                fovMaps[0].computeFov(x, y, fovRadius, true, TCODFOVTypes.ShadowFov);
        }

        public override void render(bool insight)
        {
            if (owner != null)
            {
                Map m = map;
                lightmap.clear();
                int ox = owner.x - m.offsetX + mapx;
                int oy = owner.y - m.offsetY + mapy;
                if (ox >= -mapx && ox < width && oy >= -mapy && oy < height)
                    if (owner.x >= 0 && owner.x < m.tiles.GetLength(0) && owner.y >= 0 && owner.y < m.tiles.GetLength(1))
                    {
                        for (int x = m.renderX; x < m.renderWidth + m.renderX; x++)
                        {
                            for (int y = m.renderY; y < m.renderHeight + m.renderY; y++)
                            {
                                int mx = x + m.offsetX;
                                int my = y + m.offsetY;
                                if (isInFov(mx, my, 0, m) && (m.isExplored(mx, my) || m.showAllTiles))
                                {

                                    try
                                    {
                                        double distance = Math.Sqrt(Math.Pow(owner.x - (mx), 2) + Math.Pow(owner.y - (my), 2));
                                        if (distance <= fovRadius)
                                        {
                                            byte c = (byte)(strenght - (((float)strenght / ((float)fovRadius)) * ((byte)distance)));
                                            TCODColor color = new TCODColor(c, c, c);
                                            TCODColor newColor = color.Plus(level.lightMapConsole.getCharBackground(x, y));
                                            lightmap.setCharBackground(x, y, newColor, TCODBackgroundFlag.Set);
                                        }

                                    }
                                    catch (Exception e)
                                    {

                                        Console.WriteLine(e.Message);
                                        throw new Exception(e.Message);
                                    }
                                }
                                else
                                {

                                    //lightmap.setCharBackground(x, y, TCODColor.black, TCODBackgroundFlag.Set);
                                }
                            }
                        }
                        TCODConsole.blit(lightmap, 0, 0,
                               lightmap.getWidth(), lightmap.getHeight(), level.lightMapConsole, 0, 0, (float)strenght / (255 * 2), (float)strenght / (255 * 2));
                    }
            }
        }
        public override bool isInFov(int mx, int my, int fov)
        {
            return isInFov(mx, my, fov, this.map);
        }
        public bool isInFov(int mx, int my, int fov, Map m)
        {
            
            int x = mx - m.offsetX + mapx;
            int y = my - m.offsetY + mapy;
            if (mx < 0 || mx >= m.width || my < 0 || my >= m.height)
            {
                return false;
            }
            if (fovMaps[0].isInFov(x, y))
            {
                if (owner == Program.engine.player)
                    map.tiles[mx, my].explored = true;

                return true;
            }
            return false;
        }
    }
}
