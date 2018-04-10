using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus.Engine;
using Janus.Engine.Components;
using Janus;
using libtcod;
namespace Janus.Engine.Generators
{
    public class ActorGenerator
    {
        //structure that sorts all known actors in the game
        //first by type
        //and then by rarity
        public static Dictionary<string, SortedList<float, List<Actor>>> referenceList = new Dictionary<string, SortedList<float, List<Actor>>>();

        public static float maxRarity = 10f;


        public static void getReferenceActors()
        {
            foreach (string s in ActorLoader.actorDirectoriesByType.Keys)
            {
                if (!referenceList.ContainsKey(s))
                {
                    List<string> actorNames = ActorLoader.actorDirectoriesByType[s].Keys.ToList();
                    SortedList<float, List<Actor>> references = new SortedList<float, List<Actor>>();
                    for (int i = 0; i < actorNames.Count; i++)
                    {
                        Actor a = ActorLoader.getActor(actorNames[i]);

                        if (!references.ContainsKey(a.rarity))
                            references.Add(a.rarity, new List<Actor>());

                        references[a.rarity].Add(a);


                    }

                    referenceList.Add(s, references);
                }
            }

        }


        public static float getRarity(float min, float max, double rand)
        {
            if (max != 0 && min < max)
            {
                double e = Math.Pow(max + 1, (rand - min) / (max + 1)) - 1;

                return (float)e;
            }
            return 0;
        }

        public static float getRandomRarity(float min, float max)
        {
            return getRarity(min, max, (double)TCODRandom.getInstance().getFloat(min, max));

        }

        public static Actor[] getAllActorsOfType(string type)
        {
            SortedList<float, List<Actor>> sortedList = getActorsOfType(type);
            return sortedList.Values.SelectMany(x => x).ToArray();
        }

        public static SortedList<float, List<Actor>> getActorsOfType(string type)
        {
            type = type.ToLower();
            SortedList<float, List<Actor>> actors = new SortedList<float, List<Actor>>();
            if (referenceList.ContainsKey(type))
            {
                actors = referenceList[type];
            }
            if (type.Last() == '*') //Include all subtypes too
            {
                type = type.Remove(type.Length - 1, 1);
                foreach (string k in referenceList.Keys)
                {
                    if (k.Contains(type))
                    {
                        foreach (float f in referenceList[k].Keys)
                        {
                            if (actors.ContainsKey(f))
                            {
                                actors[f].AddRange(referenceList[k][f]);
                            }
                            else
                            {
                                actors.Add(f, referenceList[k][f]);
                            }
                        }
                    }
                }
            }
            return actors;
        }



        public static Actor getRandomActorOfType(string type)
        {
            return getRandomActorOfType(type, 0, maxRarity);
        }
        public static Actor getRandomActorOfType(string type, float minRarity, float maxRarity)
        {
            SortedList<float, List<Actor>> actors = getActorsOfType(type);
            if (actors != null)
            {
                float randInt = getRandomRarity(minRarity > actors.Keys.Min() ? minRarity : actors.Keys.Min(), maxRarity > actors.Keys.Max() ? maxRarity : actors.Keys.Max());
                float index = float.MaxValue;

                foreach (float k in actors.Keys)
                {
                    if (Math.Abs(randInt - k) < Math.Abs(randInt - index))
                        index = k;
                }
                if (actors.ContainsKey(index))
                {
                    int randIndex = TCODRandom.getInstance().getInt(0, actors[index].Count - 1);
                    return ActorLoader.getActor(actors[index][randIndex].name);
                }
            }
            return null;
        }

    }
}
