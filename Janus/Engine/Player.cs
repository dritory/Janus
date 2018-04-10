using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine
{




    public class DestructiblePlayer : Components.Destructible
    {
        public DestructiblePlayer() : base()
        {
            
        }
        public DestructiblePlayer(Player player, float maxHp, float defense, string corpseName, float healingRate)
            : base(player, maxHp, defense, corpseName, healingRate)
        {
            hp = maxHp;
        }
        public override void die(Actor owner)
        {
            Message.WriteLine("You died!");
            base.die(owner);
            owner.ch = '@';
            Program.engine.gameStatus = GameStatus.DEFEAT;

        }
    }
    public class Player : Actor
    {


        public bool paused = false;
        public int fovRadius = 25;
        [NonSerialized]
        private Engine engine;
        [NonSerialized]
        public Components.PlayerAI playerAi;
        [NonSerialized]
        public Components.DynamicFov fov;
        [NonSerialized]
        public Components.Slot mainToolSlot;


        public GUI.ContainerGui containerGui;

        public Player()
            : base(0, 40, 25, '@', TCODColor.white)
        {

            this.engine = Program.engine;
            this.components.Add(new DestructiblePlayer(this, 100, 4, "your cadaver", 0.1F));
            this.components.Add(new Components.Attacker(this, 5));
            this.components.Add(new Components.PlayerAI(this));
            this.components.Add(new Components.Container(this, 100));

            mainToolSlot = new Components.Slot(this, "Tool");
            this.components.Add(mainToolSlot);
            this.components.Add(new Components.Slot(this, "Mail"));
            this.components.Add(new Components.Slot(this, "Gauntlet"));
            this.components.Add(new Components.Slot(this, "Legging"));
            this.components.Add(new Components.Slot(this, "Boot"));


            fov = new Components.DynamicFov(this, fovRadius, 50);
            this.components.Add(fov);

            containerGui = new GUI.ContainerGui(this, "Inventory", engine.screenWidth - 44, 4, 40, 50);

            name = "you";
            this.playerAi = (Components.PlayerAI)getComponent(typeof(Components.PlayerAI));

        }

        public override void load(ActorHandler actorhandler)
        {

            base.load(actorhandler);
            this.engine = Program.engine;
            containerGui = new GUI.ContainerGui(this, "Inventory", engine.screenWidth - 44, 4, 40, 50);
            this.playerAi = (Components.PlayerAI)getComponent(typeof(Components.PlayerAI));
            this.fov = (Components.DynamicFov)getComponent(typeof(Components.DynamicFov));
            this.mainToolSlot = (Components.Slot)getComponent(typeof(Components.Slot));
        }
        public void update()
        {

            if (playerAi != null && !paused)
            {
                playerAi.update();
                containerGui.update();
            }
            if (engine.gameStatus == GameStatus.NEW_TURN)
            {
                foreach (Component c in components)
                {
                    if (c.GetType() != typeof(Components.PlayerAI))
                        c.update();
                }
            }
        }
        
        public override void render()
        {
            base.render();
        }
        public override void renderGui()
        {
            containerGui.render();
            base.render();
        }
    }
}
