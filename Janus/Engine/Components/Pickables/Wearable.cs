using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine.Components;
namespace Janus.Engine.Components.Pickables
{


    public class Wearable : Pickable
    {
        public List<Effect> passiveEffects;

        public Wearable() : base()
        {

        }
        public Wearable(Actor owner) : base(owner)
        {

        }
        public Wearable(Actor owner, string[] s) : base(owner)
        {

        }
        //wears item into first compatible slot
        public virtual bool wear(Actor wearer)
        {
            List<Slot> slots = wearer.getSlots().ToList<Slot>();

            foreach (Slot slot in slots)
            {
                if (wear(wearer, slot))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool wear(Actor wearer, Slot slot)
        {
            Container c = wearer.getContainer();
            if (c != null)
            {
                if (slot.item == null)
                {
                    
                    if (this.GetType().Name == slot.type || this.GetType().BaseType.Name == slot.type || this.GetType().BaseType.BaseType.Name == slot.type)
                    {

                        slot.item = owner;
                        slot.component = this;
                        c.remove(owner);
                        if (passiveEffects != null)
                        {
                            for (int i = 0; i < passiveEffects.Count; i++)
                            {
                                passiveEffects[i].applyTo(wearer);
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool unwear(Actor wearer, Slot slot)
        {
            Container c = wearer.getContainer();
            if (c != null)
            {
                slot.item = null;
                slot.component = null;
                c.add(owner);
                if (passiveEffects != null)
                {
                    for (int i = 0; i < passiveEffects.Count; i++)
                    {
                        passiveEffects[i].reverseEffect(wearer);
                    }
                }
                return true;
            }
            return false;
        }

        public virtual bool wearing(Actor wearer)
        {
            bool succeed = false;
            for (int j = 0; j < passiveEffects.Count; j++)
            {
                if (j < passiveEffects.Count)
                    if (passiveEffects[j].applyTo(wearer))
                    {
                        succeed = true;
                    }
            }
            return succeed;
        }

        public override bool use(Actor wearer)
        {
            if (activeEffects.Count > 0)
            {
                List<Actor> list = new List<Actor>();
                if (selector != null)
                {
                    selector.selectTargets(wearer, out list);
                }
                else
                {
                    list.Add(wearer);
                }
                bool succeed = false;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i < list.Count)
                        for (int j = 0; j < activeEffects.Count; j++)
                        {
                            if (j < activeEffects.Count)
                                if (activeEffects[j].applyTo(list[i]))
                                {
                                    succeed = true;
                                }
                        }
                }
                return succeed;
            }
            return false;
        }

    }

    

}
