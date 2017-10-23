using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Janus.Engine;
using Janus.Engine.Components;
using System.Diagnostics;
using libtcod;
namespace Janus
{
    class ActorLoader
    {

        public static Dictionary<string, Dictionary<string, string>> actorDirectoriesByType = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, string> actorDirectories = new Dictionary<string, string>();
        public const string EXTENSION = ".raw";
        public static void setActor(Actor actor, string folder)
        {

            if (actor != null && actor.name != string.Empty)
            {

                string n = (Lang.replaceAt(0, actor.name, Char.ToUpper(actor.name[0]).ToString()));


                if (File.Exists(Directory.GetCurrentDirectory() + Engine.Engine.MAINDIRECTORY + folder + n + EXTENSION))
                {

                    StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + Engine.Engine.MAINDIRECTORY + folder + n + EXTENSION);
                    string s = Raw.SerializeActor(actor);

                    writer.WriteLine(s);
                    writer.Close();
                }
            }
        }




        public static void getAllActorDirectories()
        {
            string mainDir = Directory.GetCurrentDirectory() + Engine.Engine.MAINDIRECTORY;

            string[] dirs = Directory.GetDirectories(mainDir,"*",SearchOption.AllDirectories);
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i].Remove(0, (Directory.GetCurrentDirectory() + Engine.Engine.MAINDIRECTORY).Length + 1).ToLower();
                if (!dir.Contains("data\\saves"))
                {
                    string[] files = Directory.GetFiles(mainDir +"\\"+ dir );
                    for (int j = 0; j < files.Length; j++)
                    {
                        if (!files[j].Contains("data\\creatures\\Raw.raw") && files[j].Contains(".raw"))
                        {
                            Actor a = getActor(files[j], 0);
                            if (!actorDirectories.ContainsKey(a.name.ToLower()))
                                actorDirectories.Add(a.name.ToLower(), files[j]);

                            dir = dir.Replace('\\', ':');
                            dir = dir.Replace('/', ':');
                            if (!actorDirectoriesByType.ContainsKey(dir))
                                actorDirectoriesByType.Add(dir, new Dictionary<string, string>());

                            if (!actorDirectoriesByType[dir].ContainsKey(a.name.ToLower()))
                                actorDirectoriesByType[dir].Add(a.name.ToLower(), files[j]);


                        }
                    }
                }
            }
        }


        public static Actor getActor(string name)
        {
            int id = Program.engine.actorHandler.getUniqueId();

            if (actorDirectories.ContainsKey(name))
            {
                return getActor(actorDirectories[name], id);
            }
            else
                if (actorDirectories.ContainsKey(name.ToLower()))
            {
                return getActor(actorDirectories[name.ToLower()], id);
            }
            return null;
        }

        public static Actor getActor(string name, string folder, int id)
        {
            string dir = Directory.GetCurrentDirectory() + Engine.Engine.MAINDIRECTORY + folder + name + EXTENSION;
            if (!File.Exists(dir))//if it can't be found
            {
                name = Lang.replaceAt(0, name, Char.ToUpper(name[0]).ToString()); //try this
            }
            if (!File.Exists(dir))//if it still can't be found
            {
                name = name.ToLower();//try this
            }

            if (File.Exists(dir))
            {
                getActor(dir, id);
            }
            return null;
        }

        public static Actor getActor(string filePlusFolder, int id)
        {

            Stream stream = new FileStream(filePlusFolder,
                          FileMode.Open,
                          FileAccess.Read,
                          FileShare.Read);

            StreamReader reader = new StreamReader(stream);
            string s = reader.ReadToEnd();
            if (s != string.Empty)
            {
                try
                {
                    Actor a = Raw.DeserializeActor(s, id);
                    if (a != null)
                        if (a.GetType() == typeof(Actor))
                        {
                            return (Actor)a;
                        }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);

                    Debug.Assert(true, exc.Message);
                    throw exc;
                }
            }
            reader.Close();
            return null;
        }

    }
}
