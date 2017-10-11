using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components {
    [Serializable]
    class MonsterAI : AI {
        public MonsterAI(Actor owner)
            : base(owner)
        {
            this.owner = owner;
        }
        public MonsterAI(Actor owner, string [] s)
            : base(owner)
        {
            this.owner = owner;
            this.speed = float.Parse(s[0]);
            
        }

        public MonsterAI(Actor owner, float speed)
            : base(owner) {

            this.owner = owner;
        }
        public float speed = 1f;
        private float[] gainedSpeed = new float[2];
        private int x, y, ox, oy;
        protected int moveCount;
        // how many turns the monster chases the player
        // after losing his sight
        const int TRACKING_TURNS = 50;
        public void moveOrAttack(int targetx, int targety) {
            int dx = targetx - owner.x;
            int dy = targety - owner.y;
            int stepdx = (dx >0 ?1:-1);
            int stepdy = (dy >0 ?1:-1);
            
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            if (distance >= 2) {
                dx = (int)(Math.Round(dx / distance));
                dy = (int)(Math.Round(dy / distance));
                if (Program.engine.map.canWalk(owner.x + dx, owner.y + dy)) {
                    x += dx;
                    y += dy;
                }else if(Program.engine.map.canWalk(owner.x + stepdx, owner.y)) {
                    x += stepdx;
                }else if(Program.engine.map.canWalk(owner.x, owner.y + stepdy)) {
                    y += stepdy;
                }
            }
            else {
                Attacker a = (Attacker)owner.getComponent(typeof(Attacker));
                if (a != null) {
                    a.attack(owner, Program.engine.player);

                }
            }

        }
        public override void update(bool validate) {
            if (!validate)
            {
                if (owner.isDead())
                {
                    return;
                }
                if (Program.engine.map.isInFov(owner.x, owner.y))
                {
                    // we can see the player. move towards him
                    moveCount = TRACKING_TURNS;


                }
                else
                {
                    moveCount--;
                }
                
                if (moveCount > 0)
                {

                    moveOrAttack(Program.engine.player.x, Program.engine.player.y);

                    if (Program.engine.map.isWall(owner.x, owner.y))
                    {
                        for (int j = -1; j < 2; j++)
                            for (int i = -1; i < 2; i++)
                            {
                                if (Program.engine.map.canWalk(owner.x + j, owner.y + i))
                                {
                                    owner.x = owner.x + j;
                                    owner.y = owner.y + i;
                                    break;
                                }
                            }
                    }
                    if (x != 0 || y != 0)
                    {
                        float _speed = speed;
                        if (x != 0 && y != 0)
                        {
                            //  _speed *= 0.71F; //pythagoras //sucks
                        }

                        if (x != 0)
                        {
                            if (x != ox)
                                gainedSpeed[0] = 0;
                            gainedSpeed[0] += _speed;
                            ox = x;
                        }
                        if (y != 0)
                        {
                            if (y != oy)
                                gainedSpeed[1] = 0;
                            gainedSpeed[1] += _speed;
                            oy = y;
                        }

                        if (gainedSpeed[0] >= 1)
                        {
                            if (x > 0)
                            {
                                if (Program.engine.map.canWalk(owner.x + 1, owner.y))
                                    owner.x += 1;
                            }
                            else
                                if (Program.engine.map.canWalk(owner.x - 1, owner.y))
                                owner.x -= 1;

                            gainedSpeed[0] -= 1;
                        }
                        if (gainedSpeed[1] >= 1)
                        {
                            if (y > 0)
                            {
                                if (Program.engine.map.canWalk(owner.x, owner.y + 1))
                                    owner.y += 1;
                            }
                            else
                                if (Program.engine.map.canWalk(owner.x, owner.y - 1))
                                owner.y -= 1;
                            gainedSpeed[1] -= 1;
                        }

                        x = 0;
                        y = 0;
                    }

                }
            }
            base.update();
           
        }
    }
}
