using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components.Effects
{
    public class ArmorEffect : Effect
    {
        public int armor = 0;
        public ArmorEffect() : base()
        {

        }
        public ArmorEffect(params object[] args) : base()
        {
            if (args.Length > 0)
            {
                if (args[0].GetType() == typeof(string))
                {
                    armor = int.Parse((string)args[0]);
                }
                else
                {
                    armor = (int)args[0];
                }
            }
        }

        public override bool applyTo(Actor a)
        {
            Destructible d = a.getDestructible();
            if (d == null)
                return false;
            d.defense += armor;
            return true;
        }

        public override void reverseEffect(Actor a)
        {
            Destructible d = a.getDestructible();
            if (d.defense - armor >= 0)
            {
                d.defense -= armor;
            }
        }
    }
}
