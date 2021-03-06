﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components {
    public class Container : Component {

        public int size = 0; //0 = infinite inventory
        public List<Actor> inventory = new List<Actor>();

        public Container() : base()
        {

        }
        public Container(Actor owner, int size)
            : base(owner) {
                this.size = size;
        }
        public Container(Actor owner, string[] s)
            : base(owner) {
            if (s.Length > 0)
                this.size = int.Parse(s[0]);
            if(s.Length > 1)
            {
                Actor a = ActorLoader.getActor(s[1]);
                if (a != null)
                    inventory.Add(a);
            }

        }
        public bool add(Actor actor) {
            if (size > 0 && inventory.Count >= size) {
                return false; //inventory is full
            }
            inventory.Add(actor);
            return true;
        }
        public void remove(Actor actor) {
            inventory.Remove(actor);
        }
    }
}
