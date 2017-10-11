using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine {


    

    class DestructiblePlayer : Components.Destructible {

        public DestructiblePlayer(Player player,float maxHp, float defense, string corpseName, float healingRate)
            : base(player, maxHp, defense, corpseName, healingRate)
        {
            
        }
        public override void die(Actor owner) {
            Message.WriteLine ("You died!");
            base.die(owner);
            owner.ch = '@';
            Program.engine.gameStatus=GameStatus.DEFEAT;
          
        }
    }
    class Player : Actor {


       
        public int fovRadius = 25;
        public bool computeFov = true;
        private Engine engine;
        private Components.PlayerAI playerAi;
        

        public Player(Engine engine)
            : base(0, 40, 25, '@', TCODColor.white) {

            this.components.Add(new DestructiblePlayer(this, 100, 4, "your cadaver", 0.1F));
            this.components.Add(new Components.Attacker(this,5));
            this.components.Add(new Components.PlayerAI(this));
            this.components.Add(new Components.Container(this, 26));
            
            name = "you";
            this.engine = engine;
            this.playerAi = (Components.PlayerAI)getComponent(typeof(Components.PlayerAI));
            
        }


        public void update() {

            if (playerAi != null)
            {
                playerAi.update();
            }
                if (engine.gameStatus == GameStatus.NEW_TURN)
                {
                    if (computeFov)
                    {

                        engine.map.computeFov();
                        computeFov = false;
                    }

                    foreach (Component c in components)
                    {
                        if(c.GetType() != typeof(Components.PlayerAI))
                        c.update();
                    }
                }
        }



        public override void render() {
            base.render();
        }

    }
}
