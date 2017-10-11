using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components
{
  
    class DestructibleMonster : Destructible
    {


        public DestructibleMonster(Actor owner, string[] o)
            : base(owner,o)
        {
           
        }
        public DestructibleMonster(Actor owner, float maxHp, float defense, string corpseName)
            : base(owner, maxHp, defense, corpseName)
        {

        }
        public override void die(Actor owner)
        {
            // transform it into a nasty corpse! it doesn't block, can't be
            // attacked and doesn't move
            Message.WriteLine("The {0} died", owner.name);
            base.die(owner);
        }


    }
}
