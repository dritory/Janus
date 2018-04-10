using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine.Components;
using Janus.Engine.Components.Pickables;
namespace Janus.Engine.Components.Pickables
{
    public class Tool : Wearable
    {
        public Tool() : base()
        {

        }
        public Tool(Actor owner) : base(owner)
        {

        }
        public Tool(Actor owner, string[] s) : base(owner)
        {
            if (s.Length > 0)
            {
                TargetSelector.SelectorType type = (TargetSelector.SelectorType)Enum.Parse(typeof(TargetSelector.SelectorType), s[0]);
                if (type != null)
                {
                    float range = 0.0f;
                    if (s.Length > 1)
                        range = float.Parse(s[1]);

                    selector = new TargetSelector(type, range);
                }
            }
        }
        
    }
}
