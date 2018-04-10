using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.Components.Blocks
{
    public class Door : Component
    {
        public bool open = false;
        private bool oldState = false;
        public bool transparent = false;
        public Door() : base()
        {

        }

        public Door(Actor owner, string [] s)
            : base(owner)
        {
            owner.blocks = true;
            if (s.Length > 0)
                if (s[0] == "Transparent")
                    transparent = true;

        }
        public Door(Actor owner)
            : base(owner)
        {
            owner.blocks = true;
            
        }
        private int oldX = 0, oldY = 0;

        public int lockedLevel = 0;
       
        private bool previousTileWasOpaque = true;

        public void checkIfValid()
        {
            if (!((Program.engine.map.isWall(owner.x - 1, owner.y) && Program.engine.map.isWall(owner.x + 1, owner.y))
                ^ (Program.engine.map.isWall(owner.x, owner.y - 1) && Program.engine.map.isWall(owner.x, owner.y + 1))))
            {
                    Program.engine.map.setWall(owner.x, owner.y);
                    Program.engine.map.updateFov = true;
                     owner.destroy();
            }
        }

        public override void update(bool validate)
        {
            if (!validate)
            {
                if (owner.x != oldX || owner.y != oldY)
                {
                    if (!previousTileWasOpaque)
                        Program.engine.map.map.setProperties(oldX, oldY, true, !Program.engine.map.isWall(oldX, oldY));
                    if (!transparent)
                        if (Program.engine.map.map.isTransparent(owner.x, owner.y))
                        {
                            previousTileWasOpaque = false;
                            Program.engine.map.map.setProperties(owner.x, owner.y, false, !Program.engine.map.isWall(owner.x, owner.y));
                        }
                        else
                            previousTileWasOpaque = true;
                    if (Program.engine.map.isWall(owner.x, owner.y))
                    {
                        Program.engine.map.map.setProperties(owner.x, owner.y, transparent, true);
                    }
                    oldX = owner.x;
                    oldY = owner.y;
                }

                if (open != oldState)
                {
                    if (open)
                    {
                        if (owner.chs != null && owner.chs.Length > 1)
                            owner.ch = owner.chs[1];
                        owner.blocks = false;
                        Program.engine.map.map.setProperties(owner.x, owner.y, true, !Program.engine.map.isWall(owner.x, owner.y));
                        Program.engine.map.updateFov = true;
                    }
                    else
                    {
                        if (owner.chs != null)
                            owner.ch = owner.chs[0];
                        owner.blocks = true;
                        Program.engine.map.map.setProperties(owner.x, owner.y, transparent, !Program.engine.map.isWall(owner.x, owner.y));
                        Program.engine.map.updateFov = true;
                    }
                }

                oldState = open;
            }
            else
                checkIfValid();
            base.update(false);

           
        }
    }
}
