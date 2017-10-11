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
    class ActorGenerator
    {
        public static Dictionary<string, SortedList<int, Actor>> referenceList = new Dictionary<string, SortedList<int, Actor>>();
        
        
        public static void getReferenceActors()
        {
            foreach (string s in ActorLoader.actorDirectoriesByType.Keys)
            {
                List<string> actorNames = ActorLoader.actorDirectoriesByType[s].Keys.ToList();
                SortedList<int,Actor> references = new SortedList<int, Actor>();
                for (int i = 0; i < actorNames.Count; i++)
                {
                    Actor a = ActorLoader.getActor(actorNames[i]);
                    references.Add(a.rarity, a);
                }
                referenceList.Add(s, references);
            }
            
        }

        public static Actor generateActor(string type)
        {
            return null;
        }

    }
}
