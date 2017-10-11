using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components
{
    [Serializable]
    class AI : Component
    {
        public AI(Actor owner)
            : base(owner)
        {
            this.owner = owner;
        }
        public override void update(bool validate)
        {
            base.update(validate);
        }
        public override void render()
        {
            base.render();
        }


    }
}
