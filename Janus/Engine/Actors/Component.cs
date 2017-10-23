using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine {
    [Serializable]
    class Component {

        public string name;

        public object[] args;
        public Actor owner;
        public Component(Actor owner, params object [] args)
        {
            this.owner = owner;
            this.args = args;
           name = this.GetType().Name;
        }

        public virtual void destroy() {
            owner.components.Remove(this);
        }
        public virtual void update()
        {
           
        }
        public virtual void update(bool validate) {
            if(!validate)
                this.update();
        }
        public virtual void render(bool inSight)
        {
            if(inSight)
                this.render();
        }
        public virtual void render() {
        }
    }
}
