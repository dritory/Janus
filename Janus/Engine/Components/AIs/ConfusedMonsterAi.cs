using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components.AIs
{
    public class ConfusedMonsterAi : TemporaryAi
    {
        public ConfusedMonsterAi() : base()
        {

        }
        public ConfusedMonsterAi(Actor owner, int nbTurns)
            : base(owner, nbTurns)
        {
        }
        void update()
        {

            TCODRandom rng = TCODRandom.getInstance();
            int dx = rng.getInt(-1, 1);
            int dy = rng.getInt(-1, 1);
            if (dx != 0 || dy != 0)
            {
                int destx = owner.x + dx;
                int desty = owner.y + dy;
                if (Program.engine.map.canWalk(destx, desty))
                {
                    owner.x = destx;
                    owner.y = desty;
                }
                else
                {
                    Actor actor = Program.engine.actorHandler.getActor(destx, desty);
                    if (actor != null)
                    {
                        Attacker a = (Attacker)owner.getComponent(typeof(Attacker));
                        a.attack(owner, actor);
                    }
                }
            }
            base.update();
        }
    }


}


