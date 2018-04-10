using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components {
    public class Pickable : Component {
        public TargetSelector selector;
        public List<Effect> activeEffects;
        public Pickable() : base()
        {

        }
        public Pickable(Actor owner):base (owner)
        {

        }
        public Pickable(Actor owner, string[] s) : base(owner)
        {

            if (s.Length > 0)
            {
                TargetSelector.SelectorType type = (TargetSelector.SelectorType)Enum.Parse(typeof(TargetSelector.SelectorType),s[0]);
            if(type != null)
            {
                float range = 0.0f;
                if (s.Length > 1)
                    range = float.Parse(s[1]);

                  selector = new TargetSelector(type,range);
            }
            }

              
           
        }
        public Pickable(Actor owner, TargetSelector selector, Effect effect)
            : base(owner) {
                this.selector = selector;
                activeEffects = new List<Effect>();
                this.activeEffects.Add(effect);
        }
        public Pickable(Actor owner, TargetSelector selector, List<Effect> effects)
            : base(owner)
        {
            this.selector = selector;
            this.activeEffects = effects;
        }
        public bool pick(Actor wearer) {
            Container c = wearer.getContainer();
            if (c != null && c.add((owner))) {
                owner.destroy();
                return true;
            }
            return false;

        }

        public virtual bool use(Actor wearer) {
            List<Actor> list = new List<Actor>();
            Container c = wearer.getContainer();
            if (selector != null)
            {
                selector.selectTargets(wearer, out list);
            }
            else
            {
                list.Add(wearer);
            }
            bool succeed = false;
            for (int i = 0; i < list.Count; i++ )
            {
                if (i < list.Count)
                for(int j = 0; j < activeEffects.Count; j++)
                {
                    if(j < activeEffects.Count)
                        if (activeEffects[j].applyTo(list[i]))
                    {
                        succeed = true;
                    }
                }

            }
            if(succeed)
            if (c != null) {
                c.remove(owner);
                owner.destroy();
            }

            return succeed;
        }
    }
}
