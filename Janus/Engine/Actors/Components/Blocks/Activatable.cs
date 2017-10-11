using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components.Blocks
{
    class Activatable : Component
    {



        public bool activated;

        public Activatable(Actor owner, string[] args) : base(owner, args)
        {
        }

        public virtual bool activate(Actor activator)
        {
            activated = true;
            return false;
        }

        public override void update()
        {
            activated = false;
            base.update();
        }

    }
}
