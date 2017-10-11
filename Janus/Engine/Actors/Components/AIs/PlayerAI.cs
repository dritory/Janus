using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using libtcod;
namespace Janus.Engine.Components
{
    [Serializable]
    class PlayerAI : Components.AI
    {
        Destructible destructible;
        Player player;

        //settings
        public float speed = 1F;
        public bool autoOpen = true;
        public bool noclip = false;

        public Dictionary<char, string> controls = new Dictionary<char, string>() { { 'g', "Grab" }, { 'T', "Dig" }, { 'o', "Open" }, { 'c', "Close" }, { 'i', "Inventory" }, { 'h', "Heal" }, { 'u', "Use" } };

        public bool inventory = false;
        public float[] gainedSpeed = new float[4];
        public bool dig;
        public bool open;


        const int MAX_LENGTH_TO_MAP_EDGE = 10; //yeah, I couldn't find a better name for it

        public PlayerAI(Player player)
            : base(player)
        {
            try {
                Dictionary<char, string> c = fastJSON.JSON.ToObject< Dictionary<char, string>>(File.ReadAllText("data/config/controls.json"));
                
                if(c.Count >= controls.Count)
                {
                    controls = c;
                }
                else
                {
                    Dictionary<char, string> missingControls = c;
                    
                    foreach (char ch in controls.Keys)
                    {
                        if(!c.ContainsValue(controls[ch]))
                        {
                            missingControls.Add(ch, controls[ch]);
                        }
                    }
                    if(missingControls.Count > 0)
                    {
                        File.WriteAllText("data/config/controls.json", fastJSON.JSON.ToJSON(missingControls));
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Controls.json not found, using default values");
                
                File.WriteAllText("data/config/controls.json", fastJSON.JSON.ToJSON(controls));
            }
            
            this.player = player;
            this.owner = player;
            destructible = (Destructible)player.getComponent(typeof(Destructible));
            }

        public override void update()
        {

            if (player.isDead())
            {
                return;
            }


            handleActionKey(Program.engine.key);
            if (!inventory)
            {
                float dx = 0, dy = 0;
                switch (Program.engine.key.KeyCode)
                {
                    case TCODKeyCode.Up:
                    case TCODKeyCode.Down:
                    case TCODKeyCode.Left:
                    case TCODKeyCode.Right:
                        if (TCODConsole.isKeyPressed(TCODKeyCode.Up))
                        {
                            dy = -speed;
                        }
                        if (TCODConsole.isKeyPressed(TCODKeyCode.Down))
                        {
                            dy = speed;
                        }
                        if (TCODConsole.isKeyPressed(TCODKeyCode.Left))
                        {
                            dx = -speed;
                        }
                        if (TCODConsole.isKeyPressed(TCODKeyCode.Right))
                        {
                            dx = speed;
                        }
                        break;

                    default: break;
                }
                if (dx != 0 || dy != 0)
                {
                    float _speed = speed;


                    if (dx > 0)
                        gainedSpeed[0] += dx;
                    if (dx < 0)
                        gainedSpeed[1] += -dx;
                    if (dy > 0)
                        gainedSpeed[2] += dy;
                    if (dy < 0)
                        gainedSpeed[3] += -dy;
                    int x = 0, y = 0;
                    if (gainedSpeed[0] >= 1)
                    {
                        x += 1;
                        gainedSpeed[0] -= 1;
                    }
                    if (gainedSpeed[1] >= 1)
                    {
                        x -= 1;
                        gainedSpeed[1] -= 1;
                    }
                    if (gainedSpeed[2] >= 1)
                    {
                        y += 1;
                        gainedSpeed[2] -= 1;
                    }
                    if (gainedSpeed[3] >= 1)
                    {
                        y -= 1;
                        gainedSpeed[3] -= 1;
                    }
                    Program.engine.gameStatus = GameStatus.NEW_TURN;

                    if (x != 0 || y != 0)
                        if (moveOrAttack(player, player.x + x, player.y + y))
                        {
                            player.computeFov = true;
                            if (player.x - Program.engine.map.offsetX - Program.engine.map.renderX < MAX_LENGTH_TO_MAP_EDGE)
                            {
                                Program.engine.map.offsetX -= MAX_LENGTH_TO_MAP_EDGE - 1;

                            }
                            if (player.y - Program.engine.map.offsetY - Program.engine.map.renderY < MAX_LENGTH_TO_MAP_EDGE)
                            {
                                Program.engine.map.offsetY -= MAX_LENGTH_TO_MAP_EDGE - 1;

                            }
                            if (player.x - Program.engine.map.offsetX - Program.engine.map.renderX > Program.engine.map.renderWidth - MAX_LENGTH_TO_MAP_EDGE)
                            {
                                Program.engine.map.offsetX += MAX_LENGTH_TO_MAP_EDGE - 1;

                            }
                            if (player.y - Program.engine.map.offsetY - Program.engine.map.renderY > Program.engine.map.renderHeight - MAX_LENGTH_TO_MAP_EDGE)
                            {
                                Program.engine.map.offsetY += MAX_LENGTH_TO_MAP_EDGE - 1;

                            }
                        }
                        else
                        {

                        }
                }
            }
            base.update();
        }

        public void handleActionKey(TCODKey key)
        {

            bool nextTurn = false;

            switch (key.KeyCode)
            {
                case TCODKeyCode.Escape:
                    {
                        if (dig)
                            dig = false;
                        if (inventory)
                            inventory = false;
                        break;
                    }
            }
            if (controls.ContainsKey(key.Character))
            {
                string control = controls[key.Character].ToLower();

                if (!inventory)
                {
                    switch (control)
                    {

                        case "use":
                            {
                                Container c = (Container)player.getComponent(typeof(Container));
                                if (c.inventory.Count > 0)
                                {
                                    Pickable p = (Pickable)c.inventory.First<Actor>().getComponent(typeof(Pickable));
                                    if (p != null)
                                    {
                                        p.use(player);
                                        nextTurn = true;
                                        break;

                                    }

                                    if (nextTurn == true) //item has been consumed
                                        break;
                                }

                                Message.WriteLine("You have no items to use");
                                break;
                            }




                        case "dig":
                            {
                                dig = true;
                                break;
                            }
                        case "open":
                            {
                                foreach (Actor a in Program.engine.actorHandler.actors)
                                {
                                    double distance = Math.Sqrt(Math.Pow(owner.x - a.x, 2) + Math.Pow(owner.y - a.y, 2));
                                    if (distance < 2)
                                    {
                                        Blocks.Door door = (Blocks.Door)a.getComponent(typeof(Blocks.Door));
                                        if (door != null && !door.open)
                                        {
                                            if (door.lockedLevel == 0)
                                            {
                                                door.open = true;
                                                door.update(false);
                                            }
                                            else
                                            {
                                                Message.WriteLineC(TCODColor.lightGrey, "The door is locked");
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case "close":
                            {
                                foreach (Actor a in Program.engine.actorHandler.actors)
                                {
                                    double distance = Math.Sqrt(Math.Pow(owner.x - a.x, 2) + Math.Pow(owner.y - a.y, 2));
                                    if (distance < 2)
                                    {
                                        Blocks.Door door = (Blocks.Door)a.getComponent(typeof(Blocks.Door));
                                        if (door != null && door.open)
                                        {
                                            door.lockedLevel = 0;
                                            door.open = false;
                                            door.update(false);

                                        }


                                    }
                                }



                                break;
                            }
                        case "inventory":
                            {
                                inventory = !inventory;
                                break;
                            }
                        case "heal":
                            {

                                Container c = (Container)player.getComponent(typeof(Container));
                                for (int i = 0; i < c.inventory.Count; i++)
                                {

                                    Pickable p = (Pickable)c.inventory[i].getComponent(typeof(Pickable));
                                    if (p != null)
                                    {
                                        if (p.effects != null && p.effects.Count > 0)
                                            foreach (Effect e in p.effects)
                                            {
                                                if (e.GetType() == typeof(Effects.HealthEffect))
                                                {
                                                    Effects.HealthEffect h = (Effects.HealthEffect)e;
                                                    if (h.amount > 0)
                                                    {
                                                        nextTurn = true;
                                                        player.getDestructible().heal(h.amount);
                                                        Message.WriteLine("You used the " + c.inventory[i].name);
                                                        c.remove(c.inventory[i]);
                                                        break;
                                                    }

                                                }
                                            }
                                        break;
                                    }
                                }
                                if (nextTurn == true) //item has been consumed
                                    break;
                                else
                                    Message.WriteLine("You have no healing consumables to use");

                                break;
                            }
                        case "grab":
                            {

                                nextTurn = true;
                                bool found = false;
                                for (int i = 0; i < Program.engine.actorHandler.actors.Count; i++)
                                {
                                    if (i < Program.engine.actorHandler.actors.Count)
                                    {
                                        Actor a = Program.engine.actorHandler.actors[i];
                                        Pickable c = (Pickable)a.getComponent(typeof(Pickable));
                                        if (c != null && a.x == owner.x && a.y == owner.y)
                                        {
                                            if (c.pick(owner))
                                            {
                                                found = true;
                                                Message.WriteLineC(TCODColor.lightGrey, "You pick up the {0}.",
                                                    a.name);
                                                break;
                                            }
                                            else if (!found)
                                            {
                                                found = true;
                                                Message.WriteLineC(TCODColor.red, "Your inventory is full.");
                                            }
                                        }
                                    }
                                }
                                if (!found)
                                {
                                    Message.WriteLineC(TCODColor.lightGrey, "There's nothing here that you can pick.");

                                    break;
                                }

                                break;
                            }

                        default: break;
                    }
                }
                else if (control == "inventory")
                {
                    inventory = !inventory;
                }
            }
            if (nextTurn)
                Program.engine.gameStatus = GameStatus.NEW_TURN;

        }

        public bool moveOrAttack(Actor owner, int targetx, int targety)
        {

            if (Program.engine.map.isWall(targetx, targety))
            {
                if (dig)
                {
                    if (targetx < Program.engine.map.width - 1 && targety < Program.engine.map.height - 1 && targetx > 1 && targety > 1)
                        Program.engine.map.dig(targetx, targety, targetx, targety);
                    else if (!noclip)
                        return false;
                }
                else if (!noclip)
                    return false;
            }
            else if (!noclip)
            {
                dig = false;
            }
            for (int i = 0; i < Program.engine.actorHandler.actors.Count; i++)
            {
                Actor a = Program.engine.actorHandler.actors[i];
                Destructible d = (Destructible)a.getComponent(typeof(Destructible));
                if (a.x == targetx && a.y == targety)
                {
                    if (d != null && !d.isDead() && a.blocks)
                    {
                        Attacker att = (Attacker)a.getComponent(typeof(Attacker));
                        att.attack(owner, a);
                        return false;
                    }

                    if ((d != null && !d.isDead()) || a.getComponent(typeof(Components.Pickable)) != null)
                    {
                        Message.WriteLine("There's a {0} here", a.name);
                        
                    }
                    Components.Blocks.Activatable activatable = (Blocks.Activatable)a.getComponent(typeof(Components.Blocks.Activatable));
                    if (activatable != null)
                    {
                        bool exit = activatable.activate(player);
                        if (exit)
                            return false;
                    }
                    if (a.blocks)
                    {
                        if (autoOpen)
                        {
                            Blocks.Door door = (Blocks.Door)a.getComponent(typeof(Blocks.Door));
                            if (door != null && !door.open)
                            {
                                if (door.lockedLevel == 0)
                                {
                                    door.open = true;

                                }
                                else
                                {
                                    Message.WriteLineC(TCODColor.lightGrey, "The door is locked");
                                }
                            }
                            open = false;

                        }
                        if (!noclip)
                            return false;
                    }
                    else
                    {


                    }

                }

            }
            // look for corpses

            player.x = targetx;
            player.y = targety;
            open = false;
            return true;
        }


    }
}
