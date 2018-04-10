using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using libtcod;
using Janus.Engine;
using Janus.Engine.Components;
namespace Janus.Tools
{
    public class Commands
    {
        public Engine.Engine engine = Program.engine;

        private List<string> record = new List<string>() { "" };
        private int index = 0;

        private bool locked = false;
        public void initialize(Engine.Engine engine)
        {
            this.engine = engine;
        }
        public void update()
        {
            if (engine.gameStatus == GameStatus.STARTUP || engine.gameStatus == GameStatus.IDLE)
            {
                if (engine.gameStatus == GameStatus.STARTUP)
                    engine.player.fov.updateFov();
                engine.gameStatus = GameStatus.IDLE;

                if (engine.key.Character == '|' || engine.key.Character == '`' || locked)
                {
                    Message.WriteLine(">");

                    string s = getInput(record[index]).ToLower();

                    locked = false;
                    engine.player.paused = false;
                    if(s == "|")
                    {
                        Message.unwriteLastLine();
                        return;
                    }
                    if (s != "" && s != "up" && s != "down")
                    {
                        record[index] = s;
                        record.Add("");
                        index = record.Count - 1;
                    }
                    
                    string[] coms = s.Split(' ');


                    string com = "";
                    string[] args = new string[0];
                    if (coms.Length > 0)
                    {
                        com = coms[0];
                        if (coms.Length > 1)
                            args = new string[coms.Length - 1];
                    }
                    for (int i = 0; i < args.Length; i++)
                        args[i] = coms[i + 1];



                    switch (com)
                    {

                        case "setposition":
                        case "position":
                        case "setpos":
                        case "pos":
                        case "p":
                            {
                                setPos();
                                break;
                            }

                        case "save":
                            {
                                string file = "save";
                                if (args.Length > 0)
                                {
                                    file = args[0].ToString();
                                }
                                bool success = Saver.saveGame(file);
                                if (success)
                                    Message.WriteLine("Saving complete");
                                else
                                    Message.WriteLine("Saving failed");
                                break;
                            }
                        case "load":
                            {
                                if (args.Length > 0)
                                {
                                    bool success = Saver.loadGame((args[0].ToString()));
                                    if (success)
                                        Message.WriteLine("Loading complete");
                                    else
                                        Message.WriteLine("Could not find load file");
                                }
                                break;
                            }
                        case "generatemap":
                        case "generate":
                        case "gen":
                        case "g":
                            {
                                Console.WriteLine("Generating Map...");
                                engine.map.generate();
                                engine.gameStatus = GameStatus.STARTUP;
                                break;
                            }
                        case "kill":
                            {
                                if (args.Length > 0)
                                {
                                    if (args[0] == "me")
                                    {
                                        DestructiblePlayer d = (DestructiblePlayer)engine.player.getDestructible();
                                        d.hp = 0;

                                        d.die(engine.player);
                                    }
                                    else if (args[0] == "all")
                                    {
                                        string pluralname = "";
                                        if (args.Length > 1) //Kill one type of actors
                                            pluralname = args[1];


                                        for (int i = 0; i < engine.actorHandler.actors.Count; i++)
                                        {
                                            if (i < engine.actorHandler.actors.Count)
                                            {
                                                Actor a = engine.actorHandler.actors[i];
                                                if (a != engine.player && (pluralname == "" || pluralname == a.pluralName))
                                                {

                                                    Janus.Engine.Components.Destructible d = a.getDestructible();
                                                    if (d != null && d.hp > 0)
                                                    {
                                                        d.hp = 0;
                                                        d.die(a);
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }
                                else
                                {
                                    Message.WriteLine("Missing argument");
                                }
                                break;
                            }



                        case "noclip":
                        case "ncl":
                        case "n":
                            {
                                PlayerAI ai = (PlayerAI)engine.player.getComponent(typeof(PlayerAI));
                                if (ai != null)
                                {
                                    ai.noclip = !ai.noclip;
                                    if (ai.noclip)
                                        Message.WriteLineC(TCODColor.yellow, "DEBUG: Noclip activated");
                                    else
                                        Message.WriteLineC(TCODColor.yellow, "DEBUG: Noclip deactivated");
                                }
                                break;
                            }

                        case "godmode":
                        case "gm":
                            {
                                DestructiblePlayer d = (DestructiblePlayer)engine.player.getComponent(typeof(DestructiblePlayer));
                                if (d != null)
                                {
                                    d.defense = int.MaxValue;

                                }
                                break;
                            }
                        case "spawn":
                        case "s":
                        case "create":
                            {
                                spawn(args);
                                break;
                            }
                        case "give":
                            {

                                give(args);
                                break;
                            }
                        case "showall":
                            {
                                engine.map.showAllTiles = !engine.map.showAllTiles;
                                if (engine.map.showAllTiles)
                                    Message.WriteLineC(TCODColor.yellow, "DEBUG: Showing all tiles");
                                else
                                    Message.WriteLineC(TCODColor.yellow, "DEBUG: Not showing all tiles");
                                break;
                            }

                        case "changelevel":
                        case "level":
                            {
                                Message.WriteLine("Insert level number:");
                                try
                                {
                                    int number = int.Parse(getInput());

                                    engine.changeLevel(number);
                                    engine.player.x = engine.currentLevel.map.startx;
                                    engine.player.y = engine.currentLevel.map.starty;
                                }
                                catch (Exception e)
                                {
                                    Message.WriteLine("Invalid number");
                                }
                                Message.unwriteLastLine();
                                break;
                            }
                        case "":
                            {
                                Message.unwriteLastLine();
                                break;
                            }
                        case "up":
                            {
                                if (index > 0)
                                {
                                    index--;
                                }
                                else
                                {
                                    index = record.Count - 1;
                                }
                                Message.unwriteLastLine();

                                locked = true;
                                engine.player.paused = true;
                                break;
                            }
                        case "down":
                            {
                                if (index < record.Count - 1)
                                {
                                    index++;
                                }
                                else
                                {
                                    index = 0;
                                }
                                Message.unwriteLastLine();

                                locked = true;
                                engine.player.paused = true;
                                break;
                            }

                        default:
                            {
                                Message.WriteLine("Unknown or incorrect command");
                                break;
                            }

                    }

                }




                if (engine.key.Character == 'S')
                {
                    Saver.saveGame("newSave");
                }
                if (engine.key.Character == 'L')
                {
                    Saver.loadGame("newSave");
                }
            }
        }

        private void give(string[] args)
        {
            string name = "";
            int number = 1;
            if (args.Length == 0)
            {
                Message.WriteLine("What would you like to have?");
                Message.WriteLineC(TCODColor.yellow, "(Syntax: \"Name;Number\")");
                Message.Write(":");
                name = getInput();
                if (name.Contains(';'))
                {
                    string[] s = name.Split(';');
                    name = s[0];
                    if (!int.TryParse(s[1], out number))
                    {
                        Message.WriteLine("Incorrect number");
                        number = 1;
                    }
                    if (number == 0)
                        number = 1;
                }
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (!int.TryParse(args[i], out number))
                    {
                        name += args[i].ToLower() + " ";
                    }
                }
                if (name[name.Length - 1] == ' ')
                {
                    name = name.Remove(name.Length - 1);
                }
                if (number == 0)
                {
                    number = 1;
                }
            }
            if (ActorLoader.actorDirectories.ContainsKey(name))
            {
                List<Actor> actors = new List<Actor>();
                for (int i = 0; i < number; i++)
                {
                    actors.Add(ActorLoader.getActor(ActorLoader.actorDirectories[name], engine.actorHandler.getUniqueId() + i));
                }

                if (actors.Count > 0)
                {
                    if (actors[0].getComponent(typeof(Pickable)) != null)
                    {
                        for (int i = 0; i < number; i++)
                        {
                            engine.player.getContainer().add(actors[i]);
                            engine.player.playerAi.tryWear(engine.player, actors[i]);
                        }

                        Message.WriteLine("*Gives you stuff*");
                    }
                    else
                    {
                        Message.WriteLine("You can't carry that");
                    }
                }
            }
            else
            {
                Message.WriteLine("Could not find the \"" + name + "\"");
            }
        }

        private void spawn(string[] args)
        {
            string name = "";

            int number = 1;
            if (args.Length == 0)
            {
                Message.WriteLine("What would you like to spawn?");
                Message.WriteLineC(TCODColor.yellow, "(Syntax: \"Name;Number\")");
                Message.Write(":");
                name = getInput();
                name = name.ToLower();


                if (name.Contains(';'))
                {
                    string[] s = name.Split(';');
                    name = s[0];
                    if (!int.TryParse(s[1], out number))
                    {
                        Message.WriteLine("Incorrect number");
                        number = 1;
                    }
                    if (number == 0)
                        number = 1;
                }
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (!int.TryParse(args[i], out number))
                    {
                        name += args[i].ToLower() + " ";
                    }
                }
                if (name[name.Length - 1] == ' ')
                {
                    name = name.Remove(name.Length - 1);
                }
                if (number == 0)
                {
                    number = 1;
                }
            }


            if (ActorLoader.actorDirectories.ContainsKey(name))
            {
                List<Actor> actors = new List<Actor>();
                for (int i = 0; i < number; i++)
                {
                    actors.Add(ActorLoader.getActor(ActorLoader.actorDirectories[name], engine.actorHandler.getUniqueId() + i));
                }

                if (actors.Count > 0)
                {
                    if (number < 2)
                    {
                        Message.WriteLine("Where would you like to spawn the " + actors[0].name + "?");
                        Message.WriteLine("Position: ");
                    }
                    else
                    {
                        Message.WriteLine("Where would you like to spawn the " + actors[0].pluralName + "?");
                        Message.WriteLine("Position: ");
                    }
                    Point p = new Point();
                    if (getInputPos(out p))
                    {
                        foreach (Actor actor in actors)
                        {
                            actor.x = p.X;
                            actor.y = p.Y;
                            engine.actorHandler.addActor(actor);

                        }
                        Message.unwriteLastLine();
                        Message.WriteLine("Succeeded spawning");
                    }


                }
            }
            else
            {
                Message.WriteLine("Could not identify the \"" + name + "\"");
            }
        }
        private bool getInputPos(out Point p)
        {
            string ps = "";
            p = new Point();
            int oldX = 0, oldY = 0;
            TCODColor oldC = TCODConsole.root.getCharBackground(0, 0);
            while (true && !TCODConsole.isWindowClosed())
            {
                engine.mousedata = TCODMouse.getStatus();
                engine.key = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);
                engine.render(false);

                TCODConsole.root.setCharBackground(engine.mousedata.CellX, engine.mousedata.CellY, TCODColor.white);
                if (engine.mousedata.CellX != oldX || engine.mousedata.CellY != oldY)
                {
                    TCODConsole.root.setCharBackground(oldX, oldY, oldC);
                    oldC = TCODConsole.root.getCharBackground(engine.mousedata.CellX, engine.mousedata.CellY);

                    oldX = engine.mousedata.CellX;
                    oldY = engine.mousedata.CellY;
                }
                TCODConsole.flush();
                if (Engine.Engine.useMouse)
                {

                    p.X = engine.mousedata.CellX;
                    p.Y = engine.mousedata.CellY;
                    if (engine.mousedata.LeftButtonPressed)
                    {
                        p.X = engine.mousedata.CellX + engine.map.offsetX;
                        p.Y = engine.mousedata.CellY + engine.map.offsetY;
                        return true;
                    }

                }

                if (engine.key.KeyCode == TCODKeyCode.Enter)
                {
                    break;
                }
                if (char.IsLetterOrDigit(engine.key.Character) || engine.key.Character == ',')
                {
                    Message.Write(engine.key.Character.ToString());
                    ps += engine.key.Character;
                }
                if (engine.key.KeyCode == TCODKeyCode.Backspace)
                {
                    if (ps.Length > 0)
                    {
                        ps.Remove(ps.Length - 1);
                        Message.unwriteLastChar();
                    }
                }

            }


            if (ps == "here")
            {
                p.X = engine.player.x;
                p.Y = engine.player.y;
                return true;
            }

            try
            {
                string[] coords = ps.Split(',');
                p = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Message.WriteLine("Incorrect position");
                return false;
            }

        }
        private string getInput()
        {
            return getInput("");
        }
        private string getInput(string initial)
        {
            string s = initial;
            Message.Write(s);
            while (true && !TCODConsole.isWindowClosed())
            {
                engine.key = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);
                engine.render();
                TCODConsole.flush();
                
                if (engine.key.KeyCode == TCODKeyCode.Enter)
                {
                    break;
                }
                if (engine.key.KeyCode == TCODKeyCode.Backspace)
                {
                    if (s.Length > 0)
                    {
                        s = s.Remove(s.Length - 1);
                        Message.unwriteLastChar();
                    }
                }
                if (engine.key.KeyCode == TCODKeyCode.Up)
                {
                    if (s != "")
                    {

                        record[index] = s;
                    }
                    s = "up";
                    break;
                }
                if (engine.key.KeyCode == TCODKeyCode.Down)
                {
                    if (s != "")
                    {

                        record[index] = s;
                    }
                    s = "down";
                    break;
                }
                if (!char.IsControl(engine.key.Character))
                {
                    Message.Write(engine.key.Character.ToString());
                    s += engine.key.Character;
                    if (engine.key.Character == '|')
                    {
                        break;
                    }
                    
                }

            }
            return s;
        }
        private void setPos()
        {
            Message.WriteLine("Insert Position x,y");
            Message.WriteLine("Position: ");
            Point p = new Point();

            if (getInputPos(out p))
            {
                engine.player.x = p.X;
                engine.player.y = p.Y;
                engine.map.offsetX = 0;
                while (engine.map.offsetX + engine.map.renderWidth < Program.engine.player.x + 10)
                {
                    engine.map.offsetX += 10;
                }
                engine.map.offsetY = 0;
                while (engine.map.offsetY + engine.map.renderHeight < Program.engine.player.y + 10)
                {
                    engine.map.offsetY += 10;
                }
            }

        }
    }
}
