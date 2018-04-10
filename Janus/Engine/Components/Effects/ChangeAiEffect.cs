using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;

namespace Janus.Engine.Components.Effects
{
    public class ChangeAiEffect : Effect
    {
        public AIs.TemporaryAi newAi;
        public string message;

        public ChangeAiEffect() : base()
        {
            
        }
        public ChangeAiEffect(AIs.TemporaryAi newAi, string message)
            : base()
        {
            this.newAi = newAi;
            this.message = message;
        }
        public override bool applyTo(Actor actor)
        {
            newAi.applyTo(actor);
            if (message != string.Empty)
            {
                Message.WriteLineC(TCODColor.lightGrey, message, actor.name);
            }
            return true;
        }
    }
}
