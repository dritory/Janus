using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using libtcod;
using Janus;
using Janus.Engine;
using Janus.Engine.Components;

class Saver
{
    public static TCODMap map;
    public static List<Tile> tiles;
    private const string FOLDER = "\\saves\\";
    private const string EXTENSION = ".sav";
    public static List<object> save = new List<object>();

    public static void load()
    {
        string s = Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER;
        if (!Directory.Exists(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER))
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER);
        }
    }

    public static void saveGame(string name)
    {

        serializeMap(Program.engine.map);
        FileStream stream = File.Open(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER + name + EXTENSION, FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);

        string s = fastJSON.JSON.ToJSON(save);

        writer.Write(fastJSON.JSON.Beautify(s));
        writer.Close();

        stream.Close();
    }
    public static void loadGame(string name) // does not work
    {
        if (File.Exists(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER + name + EXTENSION))
        {
            FileStream stream = File.Open(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER + name + EXTENSION, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string file = reader.ReadToEnd();
            List<object> s = ( List<object>)fastJSON.JSON.Parse(file);
            
            if(s != null)
                for (int i = 0; i < s.Count; i++)
                {
                    if (s[i].GetType() == typeof(Map))
                    {
                        deSerializeMap((Map)s[i]);
                    }
                }




            reader.Close();
            stream.Close();
        }
    }
    public static void serializeMap(Map map)
    {

        save.Add(map);
    }
    public static void deSerializeMap(Map map)
    {
        Program.engine.map.tiles = map.tiles;
    }
}

