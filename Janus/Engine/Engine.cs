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
        STARTUP, IDLE, NEW_TURN, VICTORY, DEFEAT, LOADING, MENU
    }

    public class Engine
    {

        public int screenWidth = 180;
        public int screenHeight = 90;
        public static string MAINDIRECTORY = "\\data";
        public static bool useMouse = true;

        private const int FIRST_LEVEL = int.MaxValue - 1;



        [NonSerialized]
        public GUI.Gui gui = new GUI.Gui();
        [NonSerialized]
        public GUI.MessageGui messageGui = new GUI.MessageGui();
        [NonSerialized]
        public GUI.LoadingGui loadingGui = new GUI.LoadingGui();
        [NonSerialized]
        public GUI.DefeatGui defeatGui = new GUI.DefeatGui();
        [NonSerialized]
        public GUI.MenuGui menuGui;

        public Dictionary<int, Level> levels = new Dictionary<int, Level>();

        public Level currentLevel
        {
            get
            {
                if (levels.ContainsKey(levelnr))
                    return levels[levelnr];
                else
                    return null;
            }
        }
        public int Levelnr { get { return levelnr; } }
        private int levelnr;
        public int _levelnr;

        public Map map { get { return currentLevel.map; } }
        public ActorHandler actorHandler { get { return currentLevel.actorHandler; } }

        public GameStatus gameStatus = GameStatus.STARTUP;
        public Player player;
        [NonSerialized]
        public TCODKey key = new TCODKey();
        [NonSerialized]
        public TCODKey lastKey = new TCODKey();
        [NonSerialized]
        public TCODMouseData mousedata = new TCODMouseData();
        public bool validate;
        [NonSerialized]
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

            menuGui = new GUI.MenuGui(screenWidth, screenHeight);

            levels = new Dictionary<int, Level>();

            gui = new GUI.Gui();
            loadingGui = new GUI.LoadingGui();
            messageGui = new GUI.MessageGui();
            defeatGui = new GUI.DefeatGui();
            debugCommands.initialize(this);

            levels.Add(FIRST_LEVEL, new Level());

            levelnr = FIRST_LEVEL;
            //currentLevel = new Level();
            if (FIRST_LEVEL == int.MaxValue)
                currentLevel.initialize(restarting, FIRST_LEVEL, typeof(Generators.TestLevelGenerator));
            else
                currentLevel.initialize(restarting, FIRST_LEVEL);

            changeLevel(FIRST_LEVEL);

            player = new Player();
            player.getDestructible().ressurect();

            player.x = map.startx; //assign player position
            player.y = map.starty;

            player.fov.update();
            if(actorHandler.getActor(0) != null)
            {
                actorHandler.actors.Remove(actorHandler.getActor(0));
            }
            actorHandler.addActor(player);

            Saver.load();

            Console.WriteLine("Initializing Complete");


            render();

            lastKey = new TCODKey();
            gameStatus = GameStatus.STARTUP;
        }
        public void load()
        {
            levelnr = _levelnr;
            menuGui = new GUI.MenuGui(screenWidth, screenHeight);

            debugCommands.initialize(this);
            //currentLevel =
            foreach (Level level in levels.Values)
            {
                level.load();
            }
            player = (Player)currentLevel.actorHandler.getActor(0);
            gameStatus = GameStatus.STARTUP;
        }
        public void save()
        {
            _levelnr = levelnr;
            foreach (Level level in levels.Values)
            {
                level.save();
            }
           
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
            else if (gameStatus == GameStatus.MENU)
            {
                menuGui.focused = true;
                menuGui.update();

                if (key.KeyCode == (TCODKeyCode.Escape))
                {
                    menuGui.focused = false;
                    gameStatus = GameStatus.IDLE;
                }
            }
            else
            {

                gameStatus = GameStatus.IDLE;

                player.update();
                gui.update();
                messageGui.update();
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
            if (number >= 0)
            {
                int lastLevel = levelnr;
                if (levels.ContainsKey(number))
                {
                    levelnr = number;
                    //currentLevel = levels[number];
                    map.updateFov = true;
                    if (player != null)
                    {
                        if (gameStatus != GameStatus.STARTUP && gameStatus != GameStatus.LOADING)
                        {
                            if (levels.ContainsKey(lastLevel))
                                levels[lastLevel].actorHandler.actors.Remove(player);
                            currentLevel.actorHandler.addActor(player);

                        }
                        player.fov.update();
                    }
                }
                else if (levels.Count > 0)//The level does not exist yet
                {
                    Level newLevel = new Level();

                    //currentLevel = newLevel;
                    levelnr = number;
                    levels.Add(number, newLevel);
                    if (number == int.MaxValue)
                        newLevel.initialize(levels.Count > 0, number, typeof(Generators.TestLevelGenerator)); //Generate new level
                    else
                        newLevel.initialize(levels.Count > 0, number); //Generate new level

                    if (levels.ContainsKey(lastLevel))
                        levels[lastLevel].actorHandler.actors.Remove(player);
                    currentLevel.actorHandler.addActor(player);
                    player.x = map.startx; //assign player position
                    player.y = map.starty;
                    map.updateFov = true;
                    player.fov.update();
                }
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
                currentLevel.renderGui();

                menuGui.render();
            }

            TCODConsole.flush();
        }
    }
}
