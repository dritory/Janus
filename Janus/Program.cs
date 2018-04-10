using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus
{
    public class Program
    {

        public static Engine.Engine engine = new Engine.Engine();
        static int exitCode = 0;
        static bool exit = false;
        public static void Exit(int code) {
            exitCode = code;
            exit = true;
        }
        public static int Main()
        {
            Console.WriteLine("Started Game");
            TCODSystem.setFps(30);
            engine.initialize(false);
            while (!TCODConsole.isWindowClosed() && !exit)
            {
                
                engine.update();
                engine.render();

               
            }
            


            Console.WriteLine("Game exited with code " + exitCode.ToString());
            return exitCode;
        }

    }
}
