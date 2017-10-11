using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine.Components {
    class Container : Component {

        public int size = 0; //0 = infinite inventory
        public List<Actor> inventory = new List<Actor>();
        public Container(Actor owner, int size)
            : base(owner) {
                this.size = size;
        }
        public Container(Actor owner, string[] s)
            : base(owner) {
                this.size = int.Parse(s[0]);
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
