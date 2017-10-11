using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components.Blocks
{
    class Stair : Portal
    {

        private bool downward;

        
        public Stair(Actor owner, string[] s) : base(owner, s)
        {





            if (downward)
                nextLevelNumber = levelNumber - 1;
            else
                nextLevelNumber = levelNumber + 1;
        }

        public override void update()
        {


            base.update();
        }
    }
}
