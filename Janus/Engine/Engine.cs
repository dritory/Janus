using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using libtcod;

namespace Janus.Engine
{
    public enum GameStatus
    {
        STARTUP, IDLE, NEW_TURN, VICTORY, DEFEAT, LOADING
    }

    class Engine
    {

        public int screenWidth = 160;
        public int screenHeight = 80;
        public static string MAINDIRECTORY = "\\data";
        public static bool useMouse = true;





        public GUI.Gui gui;
        public GUI.MessageGui messageGui;
        public GUI.ContainerGui containerGui;
        public GUI.LoadingGui loadingGui;
        public GUI.DefeatGui defeatGui;
        public Dictionary<int, Level> levels = new Dictionary<int, Level>();
        public Level currentLevel = new Level(0);
        public int Levelnr { get { return levelnr; } }
        private int levelnr;

        public Map map { get { return currentLevel.map; } }
        public ActorHandler actorHandler { get { return currentLevel.actorHandler; } }

        public GameStatus gameStatus = GameStatus.STARTUP;
        public Player player;
        public TCODKey key;
        public TCODKey lastKey;
        public TCODMouseData mousedata;
        public bool validate;
        public Janus.Tools.Commands debugCommands = new Tools.Commands();
        public Engine()
        {
        }
        public void initialize(bool restarting)
        {

            Console.WriteLine("Initializing...");
            gameStatus = GameStatus.LOADING;
            TCODConsole.initRoot(screenWidth, screenHeight, "Janus Roguelike", false);

            TCODMouse.showCursor(true);


            
            levels = new Dictionary<int, Level>();

            gui = new GUI.Gui();
            loadingGui = new GUI.LoadingGui();
            messageGui = new GUI.MessageGui();
            defeatGui = new GUI.DefeatGui();
            containerGui = new GUI.ContainerGui();
            debugCommands.initialize(this);

            currentLevel = new Level(0);
            currentLevel.initialize(false);
            levels.Add(0, currentLevel);
            changeLevel(0);

            player = new Player(this);
            player.getDestructible().ressurect();

            player.x = map.startx; //assign player position
            player.y = map.starty;

            player.fov.update();

            actorHandler.addActor(player);

            Saver.load();

            Console.WriteLine("Initializing Complete");


            render();

            lastKey = new TCODKey();
            gameStatus = GameStatus.STARTUP;
        }
        public void update()
        {
            update(false);
        }
        public void update(bool validate)
        {
            this.validate = validate;
            if (validate == true)
            {
                currentLevel.actorHandler.update(true);
            }
            mousedata = TCODMouse.getStatus();

            key = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);
            debugCommands.update();

            if (gameStatus == GameStatus.LOADING)
            {
                loadingGui.update();
            }
            else if (gameStatus == GameStatus.DEFEAT)
            {
                if (key.Pressed)
                {
                    Console.WriteLine("Restarting game...");
                    initialize(true);

                }
            }
            else
            {
                
                gameStatus = GameStatus.IDLE;

                player.update();
                gui.update();
                messageGui.update();
                containerGui.update();
                if (gameStatus == GameStatus.NEW_TURN)
                {
                    currentLevel.update();
                    //Message.flush();
                }

                Message.update();

            }
            lastKey = key;
        }

        public void changeLevel(int number)
        {
            int lastLevel = levelnr;
            if (levels.ContainsKey(number))
            {
                levelnr = number;
                currentLevel = levels[number];
                map.updateFov = true;
                if (player != null && gameStatus != GameStatus.STARTUP && gameStatus != GameStatus.LOADING)
                {
                    if (levels.ContainsKey(lastLevel))
                        levels[lastLevel].actorHandler.actors.Remove(player);
                    currentLevel.actorHandler.addActor(player);
                    //player.fov.update();
                }
            }
            else if (levels.Count > 0)//The level does not exist yet
            {
                Level newLevel = new Level(number);
                currentLevel = newLevel;
                levelnr = number;
                newLevel.initialize(levels.Count > 0); //Generate new level
                levels.Add(number, newLevel);
                if (levels.ContainsKey(lastLevel))
                    levels[lastLevel].actorHandler.actors.Remove(player);
                currentLevel.actorHandler.addActor(player);
                player.x = map.startx; //assign player position
                player.y = map.starty;
                map.updateFov = true;
                player.fov.update();
            }
            
        }
       
        public void render()
        {
            render(true);
        }
        public void render(bool flush)
        {



            TCODConsole.root.clear();
            if (gameStatus == GameStatus.LOADING)
            {
                loadingGui.render();
               
            }
            else if (gameStatus == GameStatus.DEFEAT)
            {
                defeatGui.render();
            }
            else
            {
                currentLevel.render();
                if (player != null)
                    player.render();
                if (player != null)
                    TCODConsole.root.print(1, screenHeight - 2, string.Format("HP : {0}/{1}",
        (int)player.getDestructible().hp, (int)player.getDestructible().maxHp));
                gui.render();
                messageGui.render();
                containerGui.render();


            }
            TCODConsole.flush();
        }
    }
}
