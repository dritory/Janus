using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components.Effects
{
    public class LightEffect : Effect
    {
        private DynamicFov light;
        public LightEffect() : base()
        {
            light = new DynamicFov(null);
        }
        public LightEffect(params object[] args) : base(args)
        {
            light = new DynamicFov(null, args);
        }
        
        public override bool applyTo(Actor a)
        {
            if (!a.components.Contains(light)) {
                light.owner = a;
                a.components.Add(light);
                return true;
            }
            return false;
        }
        public override void reverseEffect(Actor a)
        {
            if (!a.components.Contains(light))
            {
                light.owner = null;
                a.components.Remove(light);
            }
            base.reverseEffect(a);
        }

    }
}
