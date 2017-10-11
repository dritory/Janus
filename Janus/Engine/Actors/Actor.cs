using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using libtcod;

namespace Janus.Engine
{
    [Serializable()]
    class A
    {
        public int x;
        public A()
        {
        }
    }

    
    class Actor
    {

        public int x;
        public int y;
        public int ch;
        public int[] chs;
        public int id;
        public int depth;
        public TCODColor color;

        private ActorHandler actorHandler;

        public int rarity;

        public int LogRarity
        {
            get { return 2 ^ (rarity); }

            set { rarity = (int)Math.Log(value, 2); }

        }

        public string name;
        public string pluralName;

        public string description;

        public List<Component> components = new List<Component>();

        public ActorHandler getActorHandler ()
        {
            return actorHandler;
        }
        public void setActorHandler(ActorHandler actorHandler)
        {
            this.actorHandler = actorHandler;
        }

        public Actor(int id)
        {
            this.id = id;
        }
        public virtual void destroy()
        {
            Program.engine.currentLevel.actorHandler.actors.Remove(this);
        }
        public virtual void nullify()
        {
            this.color = null;
            this.description = string.Empty;
            this.name = string.Empty;
            this.x = 0;
            this.y = 0;
            this.ch = 0;
        }
        public Actor(int id, int x, int y, int ch, TCODColor col)
        {
            this.color = col;
            this.id = id;
            this.name = string.Empty;
            this.x = x;
            this.y = y;
            this.ch = ch;
        }

        public Actor(int id, int x, int y, int ch, string name, TCODColor col)
        {
            this.color = col;
            this.name = name;
            this.x = x;
            this.y = y;
            this.ch = ch;
            this.id = id;
        }
        public Actor(int id, int x, int y, int ch, string name, string description, TCODColor col)
        {
            this.color = col;
            this.description = description;
            this.name = name;
            this.x = x;
            this.y = y;
            this.ch = ch;
            this.id = id;
        }

        public bool blocks = false;

        public bool isDead()
        {

            Components.Destructible d = (Components.Destructible)getComponent(typeof(Components.Destructible));

            if (d != null && !d.isDead())
            {
                return false;
            }
            return true;
        }

        public float getDistance(int cx, int cy)
        {
            int dx = x - cx;
            int dy = y - cy;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        
        public Components.Destructible getDestructible()
        {
            return (Components.Destructible)getComponent(typeof(Components.Destructible));
        }
        public Components.Container getContainer()
        {
            return (Components.Container)getComponent(typeof(Components.Container));
        }
        public Component getComponent(Type type)
        {
            foreach (Component c in components)
            {
                Type pt = c.GetType();
                while (pt != typeof(Component))
                {
                    if (pt == type)
                        return c;
                    pt = pt.BaseType;
                }

            }
            return null;
        }
        public void update()
        {
            update(false);
        }
        public virtual void update(bool validate)
        {

            foreach (Component c in components)
            {
                c.update(validate);
            }
        }
        public virtual void render()
        {

            if (x - Program.engine.map.offsetX > Program.engine.map.renderX && y - Program.engine.map.offsetY > Program.engine.map.renderY)
                if (x - Program.engine.map.offsetX < Program.engine.map.renderWidth && y - Program.engine.map.offsetY < Program.engine.map.renderHeight)
                {
                    TCODConsole.root.setChar(x - Program.engine.map.offsetX, y - Program.engine.map.offsetY, ch);
                    if (color != null)
                        TCODConsole.root.setCharForeground(x - Program.engine.map.offsetX, y - Program.engine.map.offsetY, color);
                    else
                    {
                        Console.WriteLine("Color of " + this.name + " was null");
                        TCODConsole.root.setCharForeground(x - Program.engine.map.offsetX, y - Program.engine.map.offsetY, TCODColor.white);
                    }
                    foreach (Component c in components)
                    {
                        c.render();
                    }
                }
        }
    }
}
