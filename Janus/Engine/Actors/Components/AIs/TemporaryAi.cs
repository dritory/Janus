using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components.AIs
{
    class TemporaryAi : AI
    {
        public AI oldAi;
        public int nbTurns;

        public TemporaryAi(Actor owner, int nbTurns)
            : base(owner)
        {
            this.nbTurns = nbTurns;
        }

       public void update()
        {
            nbTurns--;
            if (nbTurns == 0)
            {
                owner.components.Add(oldAi);
                this.destroy();
            }
        }

       public void applyTo(Actor actor)
        {
            oldAi = (AI)actor.getComponent(typeof(AI));
            owner.components.Remove(oldAi);
            owner.components.Add(this);
        }
    }
}

