using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components
{
    public class Effect
    {
        public Effect()
        {

        }
        public Effect(params object[] args)
        {
        }
        public virtual bool applyTo(Actor a) { return false; }

        public virtual void reverseEffect(Actor a) { }
    }
}
