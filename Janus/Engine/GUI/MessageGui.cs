using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine.GUI {
    public  class MessageGui : Gui {


        public MessageGui()
        {
            
        }
       
        public override void update() {


            base.update();
        }
        public int MSG_X = 5;
        public int MSG_Y = 55;
        public int MAX_LINES = 10;

        private int x = 0, y = 0;
        private int offset;
        public override void render() {

            /*
            if (Message.newLines.Count >0)
            {
                for (int i = 0; i < Message.newLines.Count; i++)
                {
                    Line line = Message.newLines[i];
                    for (int j = 0; j < Message.newLines[i].text.Count; j++)
                    {
                        TCODConsole.root.setForegroundColor(line.text[j].color);
                        TCODConsole.root.setChar(x + j, i, line.text[j].ch);
                    }
                    

                }
            }
    */
            if (Message.lines.Count > 0)
            {
                int count = Message.lines.Count >= MAX_LINES ? MAX_LINES : Message.lines.Count;
                if (Message.lines.Count > MAX_LINES)
                {
                    offset = Message.lines.Count - MAX_LINES;
                }
                else
                    offset = 0;

                for (int i = 0; i < count; i++)
                {

                    Line line = Message.lines[i + offset];
                    for (int j = 0; j < line.text.Count; j++)
                    {
                        TCODConsole.root.setCharForeground(MSG_X + j, i + MSG_Y,line.text[j].color);
                       
                        TCODConsole.root.setChar(MSG_X + j, i + MSG_Y, line.text[j].ch);
                    }



                }
            }
            base.render();
        }
    }
}
