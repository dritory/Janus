using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine;
using Janus.Engine.Components;
using Janus;
using libtcod;
class TargetSelector
{
    public SelectorType type;
    public float range;
    public TargetSelector(SelectorType type, float range)
    {
        this.type = type;
        this.range = range;
    }
    public enum SelectorType
    {
        CLOSEST_MONSTER,
        SELECTED_MONSTER,
        WEARER_RANGE,
        SELECTED_RANGE
    };
   public void selectTargets(Actor wearer, out List<Actor> list)
    {
        list = new List<Actor>();
        switch (type)
        {
            case SelectorType.CLOSEST_MONSTER:
                {
                    Actor closestMonster = Program.engine.actorHandler.getClosestMonster(wearer.x, wearer.y, range);
                    if (closestMonster != null && closestMonster != wearer)
                    {
                        list.Add(closestMonster);
                    }
                }
                break;
            case SelectorType.SELECTED_MONSTER:
                {
                    int x = 0, y  =0;
                    Message.WriteLineC(TCODColor.cyan, "Left-click to select an enemy, or right-click to cancel.");
                    if (Program.engine.actorHandler.pickATile(out x, out y, range))
                    {
                        Actor actor = Program.engine.actorHandler.getActor(x, y);
                        if (actor != null)
                        {
                            list.Add(actor);
                        }
                    }

                }
                break;
            case SelectorType.WEARER_RANGE:
                for (int i = 0; i < Program.engine.actorHandler.actors.Count; i++)
                {
                    Actor actor = Program.engine.actorHandler.actors[i];
                    Destructible d = actor.getDestructible();
                    if (actor != wearer && d != null && !d.isDead()
                        && actor.getDistance(wearer.x, wearer.y) <= range)
                    {
                        list.Add(actor);
                    }
                }
                break;

            case SelectorType.SELECTED_RANGE:
                {
                    int x, y;
                    Message.WriteLineC(TCODColor.cyan, "Left-click to select a tile,\nor right-click to cancel.");
                    if (Program.engine.actorHandler.pickATile(out x, out y))
                    {
                        for (int i = 0; i < Program.engine.actorHandler.actors.Count; i++)
                        {
                            Actor actor = Program.engine.actorHandler.actors[i];
                            Destructible d = actor.getDestructible();
                            if (d != null && !d.isDead()
                                && actor.getDistance(x, y) <= range)
                            {
                                list.Add(actor);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        if (list.Count == 0)
        {
            Message.WriteLineC(TCODColor.lightGrey, "No enemy is close enough");
        }
    }
}

