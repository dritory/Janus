using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components.Blocks
{
    class Portal : Activatable
    {

        public int levelNumber { get { return owner.getActorHandler().level.levelnr; } }
        public List<Actor> nextActors = new List<Actor>();
        public Actor nextActor
        {
            get
            {
                if (nextActors.Count > 0)
                {
                    Actor a = nextActors[TCODRandom.getInstance().getInt(0, nextActors.Count - 1)];
                    return a;
                }
                else
                    return null;
            }

        }


        private int nln;
        public int nextLevelNumber
        {
            get { return nln; }
            set
            {
                if (this.levelNumber - value > 0)
                {
                    if (owner.chs != null && owner.chs.Length > 1)
                        owner.ch = owner.chs[1];
                }
                else
                if (owner.chs != null && owner.chs.Length > 0)
                    owner.ch = owner.chs[0];
                nln = value;
            }
        }

        public Portal(Actor owner) : base(owner, null)
        {
            this.owner = owner;
            owner.setActorHandler(Program.engine.currentLevel.actorHandler);
            nextLevelNumber = Program.engine.Levelnr;
        }

        public Portal(Actor owner, string[] strings)
            : base(owner, strings)
        {
            this.owner = owner;
            owner.setActorHandler(Program.engine.currentLevel.actorHandler);
            nextLevelNumber = Program.engine.Levelnr;
            if (strings != null && strings.Length > 0)
            {
                try
                {
                    int levelNr;
                    if (strings[0].Contains("d"))
                    {
                        string s = strings[0];
                        s = s.Replace('d', ' ');
                        levelNr = levelNumber + int.Parse(s);
                    }
                    else
                        levelNr = int.Parse(strings[0]);

                    if (strings.Length > 1)
                    {
                        int Id = int.Parse(strings[1]);
                        nextActors.Add(Program.engine.currentLevel.actorHandler.getActor(Id));
                    }
                    else
                    {

                        nextLevelNumber = levelNr;


                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }


        }
        public override void update(bool validate)
        {
            base.update();
        }


        public void connectPortal(Actor nextActor, int levelNumber)
        {
            this.nextLevelNumber = levelNumber;
            this.nextActors.Add(nextActor);
            Portal portal = (Portal)nextActor.getComponent(typeof(Portal));
            portal.nextActors.Add(owner);
            portal.nextLevelNumber = this.levelNumber;
            

        }

        public override bool activate(Actor activator)
        {
            if (nextActors.Count > 0)
            {
                Portal nextPortal = (Portal)nextActor.getComponent(typeof(Portal));
                if (true)
                {
                    if (nextLevelNumber != levelNumber) //actor needs to be transported over multible levels
                    {
                        if (!Program.engine.levels.ContainsKey(nextLevelNumber))
                        {
                            Program.engine.levels.Add(nextLevelNumber, Program.engine.generateLevel(nextLevelNumber));
                        }

                        Program.engine.actorHandler.actors.Remove(activator);
                        Program.engine.levels[nextLevelNumber].actorHandler.addActor(activator);

                        if (activator == Program.engine.player)
                        {
                            Program.engine.changeLevel(nextLevelNumber);
                        }

                    }
                    activator.setActorHandler(Program.engine.actorHandler);
                    activator.x = nextActor.x;
                    activator.y = nextActor.y;
                }
            }
            else
            {
                if (nextLevelNumber != levelNumber) //next actor does not exist
                {
                    if (!Program.engine.levels.ContainsKey(nextLevelNumber))
                    {
                        Program.engine.levels.Add(nextLevelNumber, Program.engine.generateLevel(nextLevelNumber));
                    }
                    Level level = Program.engine.levels[nextLevelNumber];
                    Program.engine.actorHandler.actors.Remove(activator);
                    level.actorHandler.addActor(activator);

                    if (activator == Program.engine.player)
                    {
                        Program.engine.changeLevel(nextLevelNumber);
                    }

                    activator.x = level.map.startx;
                    activator.y = level.map.starty;

                    Actor[] actors = level.actorHandler.getActors(activator.x, activator.y);
                    if (actors != null && actors.Length > 0)
                    {
                        for (int i = 0; i < actors.Length; i++)
                        {

                            if ((Portal)actors[i].getComponent(typeof(Portal)) != null) //We found our portal!
                            {
                                connectPortal(actors[i], nextLevelNumber);
                            }
                        }
                    }
                    if (nextActors.Count == 0)
                    {
                        //Adds an identical actor here
                        connectPortal(level.map.generator.addActor(activator.x, activator.y, owner.name), nextLevelNumber);
                    }

                }

            }
            activated = true;
            return true;
        }


    }
}
