using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using libtcod;
using Janus;
using Janus.Engine;
using Janus.Engine.Components;
using System.Reflection;
public class    Saver
{
    public static TCODMap map;
    public static List<Tile> tiles;
    private const string FOLDER = "\\saves\\";
    private const string EXTENSION = ".sav";

    public static void load()
    {
        string s = Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER;
        if (!Directory.Exists(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER))
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER);
        }
    }

    public static bool saveGame(string name)
    {
        FileStream stream = File.Open(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER + name + EXTENSION, FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);
        fastJSON.JSONParameters pars = new fastJSON.JSONParameters();
        pars.SerializerMaxDepth = 20;
        Program.engine.save();
        string s = fastJSON.JSON.ToJSON(Program.engine, pars);

        writer.Write(fastJSON.JSON.Beautify(s));
        writer.Close();

        stream.Close();
        return true;
    }
    public static bool loadGame(string name) // does not work
    {
        if (File.Exists(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER + name + EXTENSION))
        {
            FileStream stream = File.Open(Directory.GetCurrentDirectory() + Engine.MAINDIRECTORY + FOLDER + name + EXTENSION, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string file = reader.ReadToEnd();
            //object s = fastJSON.JSON.Parse(file);
            Program.engine = (Engine)fastJSON.JSON.ToObject<Engine>(file);
            //fastJSON.JSON.RegisterCustomType(typeof(Level))
            //if (s != null)
            //deSerializeEngine((Dictionary<string, object>)s,file);
            Program.engine.load();
            reader.Close();
            stream.Close();
            return true;
        }
        return false;
    }

    public static void deSerializeEngine(Dictionary<string, object> dict, string raw)
    {
        fastJSON.JSON.FillObject(Program.engine, raw);
        foreach (KeyValuePair<string, object> k in dict)
        {

            FieldInfo field = Program.engine.GetType().GetField(k.Key);
            if(field != null)
            {
                string typeName = field.FieldType.Name;




                if (typeName == Program.engine.levels.GetType().Name)
                {
                   // fastJSON.JSON.ToObject<Dictionary<int, Level>>(k.Value);
                    
                }
                else if (typeName == Program.engine.gameStatus.GetType().Name)
                {
                    field.SetValue(Program.engine, (GameStatus)Enum.Parse(typeof(GameStatus),(string)k.Value));
                }
                else if (typeName == Program.engine.player.GetType().Name)
                {


                }
                else
                {
                    field.SetValue(Program.engine, Convert.ChangeType(k.Value, field.GetValue(Program.engine).GetType()));
                }
            }
        }

    }
    //public static void deSerializeActor
    public static void deSerializeLevel(Object lvl)
    {

    }


}

