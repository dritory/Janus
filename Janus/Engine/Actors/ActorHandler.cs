using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;

namespace Janus.Engine
{
    class ActorHandler
    {
        public List<Actor> actors = new List<Actor>();

        public Level level;
        private Map map { get { return level.map; } }
        public TCODMouseData mousedata;
        
        public ActorHandler(Level level)
        {
            this.level = level;
        }
        public void initialize(bool restarting)
        {
            actors = new List<Actor>();
            if (!restarting)
            {
                ActorLoader.getAllActorDirectories();
                Generators.ActorGenerator.getReferenceActors();
                
            }
        }
        public void update()
        {
            update(false);
        }
        public void update(bool validate)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (i < actors.Count)
                    if (actors[i] != Program.engine.player)
                    {
                        actors[i].update(validate);
                    }
            }
        }
        public void sendToBack(Actor actor)
        {
            actors.Remove(actor);
            actors.Insert(0, actor);
        }

        public Actor getClosestMonster(int x, int y, float range)
        {
            Actor closest = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < Program.engine.currentLevel.actorHandler.actors.Count; i++)
            {
                Actor actor = Program.engine.currentLevel.actorHandler.actors[i];
                Components.Destructible d = actor.getDestructible();
                if (actor != Program.engine.player && d != null && !d.isDead())
                {
                    float distance = actor.getDistance(x, y);
                    if (distance < bestDistance && (distance <= range || range == 0.0f))
                    {
                        bestDistance = distance;
                        closest = actor;
                    }
                }
            }
            return closest;

        }
        public bool pickATile(out int x, out int y)
        {
            return pickATile(out x, out y, 0.0f);
        }

        public bool pickATile(out int x, out int y, float maxRange)
        {
            int pickX = Program.engine.player.x - map.offsetX, pickY = Program.engine.player.y - map.offsetY;
            for (int cx = map.renderX; cx < map.renderWidth; cx++)
            {
                for (int cy = map.renderY; cy < map.renderHeight; cy++)
                {
                    if (Program.engine.player.fov.isInFov(cx + map.offsetX, cy + map.offsetY)
                        && (maxRange == 0 || Program.engine.player.getDistance(cx + map.offsetX, cy + map.offsetY) <= maxRange))
                    {
                        TCODColor col = TCODConsole.root.getCharBackground(cx, cy);
                        col = col.Multiply(1.2f);
                        TCODConsole.root.setCharBackground(cx, cy, col);
                    }
                }
            }
            int oldX = pickX, oldY = pickY;
            TCODColor oldC = TCODConsole.root.getCharBackground(pickX, pickY);
            while (!TCODConsole.isWindowClosed())
            {
                render();
                

                mousedata = TCODMouse.getStatus();
                Program.engine.key = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);
                if (Engine.useMouse)
                {
                    if (Program.engine.player.fov.isInFov(mousedata.CellX + map.offsetX, mousedata.CellY + map.offsetY))
                        if ((maxRange == 0 || Program.engine.player.getDistance(mousedata.CellX + map.offsetX, mousedata.CellY + map.offsetY) <= maxRange))
                        {
                            pickX = mousedata.CellX;
                            pickY = mousedata.CellY;
                            if (mousedata.LeftButtonPressed)
                            {
                                x = mousedata.CellX + map.offsetX;
                                y = mousedata.CellY + map.offsetY;
                                return true;
                            }
                        }
                    if (mousedata.RightButtonPressed)
                    {
                        x = 0;
                        y = 0;
                        return false;
                    }
                }
                if (pickX != oldX || pickY != oldY)
                {
                    TCODConsole.root.setCharBackground(oldX, oldY, oldC);
                    oldC = TCODConsole.root.getCharBackground(pickX, pickY);
                    TCODConsole.root.setCharBackground(pickX, pickY, TCODColor.white);
                    oldX = pickX;
                    oldY = pickY;
                }
                if (Program.engine.player.fov.isInFov(pickX + map.offsetX, pickY + map.offsetY))
                    if ((maxRange == 0 || Program.engine.player.getDistance(pickX + map.offsetX, pickY + map.offsetY) <= maxRange))
                    {

                        switch (Program.engine.key.KeyCode)
                        {
                            case TCODKeyCode.Enter:
                                {
                                    x = pickX + map.offsetX;
                                    y = pickY + map.offsetY;

                                    return true;
                                }
                            case TCODKeyCode.Up:
                                {
                                    if (Program.engine.player.fov.isInFov(pickX + map.offsetX, pickY - 1 + map.offsetY))
                                        if (Program.engine.player.getDistance(pickX + map.offsetX, pickY - 1 + map.offsetY) <= maxRange)
                                            pickY--;
                                    break;
                                }
                            case TCODKeyCode.Down:
                                if (Program.engine.player.fov.isInFov(pickX + map.offsetX, pickY + 1 + map.offsetY))
                                    if (Program.engine.player.getDistance(pickX + map.offsetX, pickY + 1 + map.offsetY) <= maxRange)
                                        pickY++;
                                break;
                            case TCODKeyCode.Left:
                                if (Program.engine.player.fov.isInFov(pickX - 1 + map.offsetX, pickY + map.offsetY))
                                    if (Program.engine.player.getDistance(pickX - 1 + map.offsetX, pickY + map.offsetY) <= maxRange)
                                        pickX--;
                                break;
                            case TCODKeyCode.Right:
                                if (Program.engine.player.fov.isInFov(pickX + 1 + map.offsetX, pickY + map.offsetY))
                                    if (Program.engine.player.getDistance(pickX + 1 + map.offsetX, pickY + map.offsetY) <= maxRange)
                                        pickX++;
                                break;

                        }
                    }

                TCODConsole.flush();
                Program.engine.lastKey = Program.engine.key;
            }
            x = 0;
            y = 0;
            return false;
        }


        public void addActor(Actor actor)
        {
           
            actor.setActorHandler(this);
            actors.Add(actor);
            if (actor.blocks)
                map.updateFov = true;
        }


        public Actor getActor(int ID)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i].id == ID)
                {
                    return actors[i];
                }
            }
            return null;

        }

        public int getUniqueId()
        {
            int id = actors.Count + 1;


            return id;
        }


        public Actor getActor(int x, int y)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i].x == x && actors[i].y == y)
                {
                    return actors[i];
                }
            }
            return null;

        }
        public Actor[] getActors(int x, int y)
        {
            List<Actor> a = new List<Actor>();
            for (int i = 0; i < actors.Count; i++)
            {
                if (this.actors[i].x == x && actors[i].y == y)
                {
                    a.Add(actors[i]);
                }
            }
            if (a.Count > 0)
                return a.ToArray();
            else
                return null;

        }
        public void render()
        {
            foreach (Actor actor in actors)
            {
                if (Program.engine.player.fov.isInFov(actor.x, actor.y) &&
                    actor.x < map.renderWidth + Program.engine.currentLevel.map.offsetX &&
                    actor.y < map.renderHeight + Program.engine.currentLevel.map.offsetY &&
                    actor.x > Program.engine.currentLevel.map.offsetX &&
                    actor.y > Program.engine.currentLevel.map.offsetY)
                    actor.render(true);
                else
                    actor.render(false);
            }
        }
    }
}
