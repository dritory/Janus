using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine.Components.Pickables;
using Janus.Engine.Components.Effects;
using Janus.Engine.Components;

namespace Janus.Engine.Components
{
    public class Slot : Component
    {

        public Actor item;

        public string type = "Wearable";
        [NonSerialized]
        public Component component;

        public string slotName = "Slot";

        public Slot() : base()
        {

        }
        public Slot (Actor owner) : base(owner)
        {

        }
        public override void load(Actor owner)
        {
            base.load(owner);
            if(item != null)
            {
                component = item.getComponent(type);
            }
        }
        public Slot(Actor owner, params object[] args) : base(owner)
        {
            if(args.Length > 0)
            {
                if(args[0].GetType() == typeof(string))
                {
                    type = (string)args[0];
                }
                else
                {
                    type = args[0].ToString();
                }
            }
        }
    }
}
