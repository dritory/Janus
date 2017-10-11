using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components.Effects
{
    class HealthEffect : Effect
    {
        public float amount;
        public string message;
        
        public HealthEffect(bool foo,string [] s) : base (s)
        {
            if (s.Length > 0)
            this.amount = float.Parse(s[0]);
            if(s.Length > 1)
            this.message = s[1];
        }
        
        public HealthEffect(float amount, string message) : base()
        {
            this.amount = amount;
            this.message = message;
        }
        public override bool applyTo(Actor a)
        {
            Destructible d = a.getDestructible();
            if (d == null)
                return false;
            if (amount > 0)
            {
                float pointsHealed = d.heal(amount);
                if (pointsHealed > 0)
                {
                    if (message != string.Empty)
                    {
                        Message.WriteLineC(TCODColor.lightGrey, message);

                    }
                    return true;
                }
            }
            else {
            if ( message != string.Empty && -amount-d.defense > 0 ) {
                Message.WriteLineC(TCODColor.lightGrey,message,a.name,
                    -amount - d.defense);
            }
            if (d.takeDamage(a, -amount) > 0)
            {
                return true;
            }
    }
            return base.applyTo(a);
        }

    }
}
