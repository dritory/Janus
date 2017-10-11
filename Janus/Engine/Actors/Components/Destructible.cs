using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components {



    [Serializable]
    class Destructible : Component {


        public float maxHp; // maximum health points
        public float hp; // current health points
        public float defense; // hit points deflected
        public string corpseName; // the actor's name once dead/destroyed
        public float healingRate = 0; //HP regenerated per turn 


        private Actor initialActor;
        public Destructible(Actor owner, string[] o)
            : base(owner) {
           
            this.maxHp = float.Parse(o[0]);
            this.hp = maxHp;
            if (o.Length > 1)
            this.defense = float.Parse(o[1]);
            if(o.Length > 2)
            this.corpseName = o[2];
            if (o.Length > 3)
            this.healingRate = float.Parse(o[3]);

        }
        public Destructible(Actor owner, float maxHp, float defense, string corpseName)
            : base(owner, maxHp, defense, corpseName) {
            this.corpseName = corpseName;
            this.maxHp = maxHp;
            this.hp = maxHp;
            this.defense = defense;

        }
        public Destructible(Actor owner, float maxHp, float defense, string corpseName,float healingRate)
            : base(owner, maxHp, defense, corpseName)
        {
            this.corpseName = corpseName;
            this.maxHp = maxHp;
            this.hp = maxHp;
            this.defense = defense;
            this.healingRate = healingRate;
        }
        public bool isDead() {
            return hp < 1;
        }
        public float heal(float amount) {
            hp += amount;
            if (hp > maxHp) {
                amount -= hp - maxHp;
                hp = maxHp;
            }
            return amount;
        }
        public float takeDamage(Actor owner, float damage) {

            damage -= defense;
            if (damage > 0) {
                hp -= damage;
                if (hp < 1) {
                    die(owner);
                }
            }
            else {
                damage = 0;
            }
            return damage;
        }

        public virtual void die(Actor owner) {

            initialActor = owner;
            // transform the actor into a corpse!
            owner.ch = '%';
            owner.color = new TCODColor(TCODColor.darkRed.Red, TCODColor.darkRed.Green, TCODColor.darkRed.Blue);
            owner.name = corpseName;
            owner.blocks = false;
            // make sure corpses are drawn before living actors
            Program.engine.actorHandler.sendToBack(owner);
        }

        public virtual void ressurect() {
            this.hp = maxHp;
            owner = initialActor;
        }
        public int corpseTimer;
        public override void update() {

            if (owner != null && owner.getComponent(typeof(Destructible)) != null)
            if (isDead() )
            {
                corpseTimer++;
                if (owner.color.Green < 250)
                {
                   
                    if (corpseTimer > 10)
                    {
                        if (owner.color.Green <= 250)
                            owner.color.Green += 5;
                        if (owner.color.Red <= 254)
                            owner.color.Red += 1;
                        if (owner.color.Blue <= 250)
                            owner.color.Blue += 5;
                        corpseTimer = 0;
                    }
                }
                else
                {
                    if(corpseTimer > 100)
                    owner.destroy();
                }
            }

            if (healingRate != 0)
            {
                if (hp < maxHp)
                {
                    hp += healingRate;
                }
                else if (hp >= maxHp - healingRate)
                    {
                        hp = maxHp;
                    }
            }
        }
        public override void render() {

        }
    }
}
