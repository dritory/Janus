using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components {
    [Serializable]
    class Attacker : Component{


        public float power;

        public Attacker(Actor owner,float power)
            : base(owner,power)
        {
            this.power = power;
        }
        public Attacker(Actor owner, string[] o)
            : base(owner,o)
        {
            this.power = float.Parse(o[0]);
        }
        public void attack(Actor owner, Actor target) {
            if (target != null)
            {
                Destructible destructible = (Destructible)target.getComponent(typeof(Destructible));
                if (destructible != null && !destructible.isDead())
                {
                    if (power - destructible.defense > 0)
                    {
                        if (owner != Program.engine.player)
                        {
                            if (target != Program.engine.player)
                                Message.WriteLineC(TCODColor.red, "The {0} attacks the {1} for {2} hit points.", owner.name, target.name,
                                power - destructible.defense);
                            else
                                Message.WriteLineC(TCODColor.red, "The {0} attacks {1} for {2} hit points.", owner.name, target.name,
                               power - destructible.defense);
                        }
                        else
                        {
                            Message.WriteLineC(TCODColor.green, "You attack the {1} for {2} hit points.", owner.name, target.name,
                                power - destructible.defense);
                        }

                    }
                    else
                    {
                        if (owner != Program.engine.player)
                        {

                            Message.WriteLineC(TCODColor.red, "the {0} attacks the {1} but it has no effect!\n", owner.name, target.name);
                        }
                        else
                            Message.WriteLineC(TCODColor.green, "You attack the {1} for {2} hit points.\n", owner.name, target.name,
                            power - destructible.defense);
                    }
                    destructible.takeDamage(target, power);
                    if (destructible.hp < 0)
                        destructible.hp = 0;
                }
                else
                {
                    if (owner != Program.engine.player)
                        Message.WriteLineC(TCODColor.red, "The {0} attacks {1} in vain.\n", owner.name, target.name);
                    else
                        Message.WriteLineC(TCODColor.green, "You attack the {1} for {2} hit points.\n", owner.name, target.name,
                            power - destructible.defense);
                }
            }
        }

        public override void update() {
            base.update();
        }

        public override void render() {
            base.render();
        }
        
    }
}
