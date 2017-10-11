using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components
{
    class Effect
    {
        public Effect()
        {

        }
        public Effect(string[] s)
        {
        }
        public virtual bool applyTo(Actor a) { return false; }
    }
}
