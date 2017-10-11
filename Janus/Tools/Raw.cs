using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Janus;
using Janus.Engine;
using Janus.Engine.Components;
using CColor = System.Drawing.Color;
using libtcod;
static class Raw
{

    internal static Actor actor;
    internal static List<Effect> effects;
    public static Actor DeserializeActor(string s, int id)
    {

        actor = new Actor(id);
        effects = new List<Effect>();
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '[')
            {
                int j = i + 1;
                string t = string.Empty;
                while (s[j] != ']')
                {
                    t += s[j].ToString();
                    j++;
                }
                i += j - 1 - i;
                addToken(t);

            }
            if(s[i] == '/' && s[i - 1] == '/' && i + 1  < s.Length)
            {
                while(s[i] != '\n')
                {
                    i++;
                }
               
            }
        }
        Pickable p = (Pickable)actor.getComponent(typeof(Pickable));
        if (effects.Count > 0 && p != null)
        {
            p.effects = effects;
        }

        

        return actor;

    }
    public static string SerializeActor(Actor a)
    {
        string o = "";
        string name = a.name;
        int ch = a.ch;
        byte r = a.color.Red, g = a.color.Green, b = a.color.Blue;
        bool blocks = a.blocks;
        o += token("Name", name);
        o += token("Char", ch.ToString());
        o += token("Color", r.ToString(), g.ToString(), b.ToString());
        if (blocks)
            o += token("Blocks");
        for (int i = 0; i < a.components.Count; i++)
        {

            o += componentToken(a.components[i].name, a.components[i].args);
        }

        return o;
    }
    public static void addToken(string token)
    {

        int tokenLength = int.MaxValue;
        if (token.Contains(':'))
        {
            byte valuenum = 1;
            for (int i = 0; i < token.Length; i++)
            {
                if (token[i] == ':')
                {
                    if (tokenLength == int.MaxValue)
                        tokenLength = i;
                    else
                    {
                        valuenum++;
                    }
                }
            }


            string[] values = new string[valuenum];
            int length = tokenLength;

            for (int i = 0; i < valuenum; i++)
            {
                length++;

                while (token[length] != ':' && token[length] != ']')
                {
                    values[i] += token[length];
                    length++;
                    if (token.Length <= length)
                        break;
                }


            }
            token = token.Remove(tokenLength, token.Length - tokenLength);
            if (token == "Effect")
            {
                try
                {
                    string s = "JanusRT.Engine.Components.Effects." + values[0];
                    Type effType = Type.GetType("JanusRT.Engine.Components.Effects." + values[0], true);

                    if (effType != null)
                    {
                        try
                        {

                            List<string> o = new List<string>();
                            for (int i = 1; i < values.Length; i++)
                            {
                                o.Add(values[i]);
                            }
                            Effect effect = new Effect(o.ToArray());
                            if (o.Count > 0)
                                effect = (Effect)Activator.CreateInstance(effType, false, o.ToArray());
                            else
                                effect = (Effect)Activator.CreateInstance(effType, false);

                            effects.Add(effect);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.Message);
                            throw new Exception(exc.Message);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
            if (token == "Component") //for components
            {
                try
                {
                    Type compType = Type.GetType("JanusRT.Engine.Components." + values[0], false);

                    if (compType == null)
                        compType = Type.GetType("JanusRT.Engine.Components.AIs." + values[0], false);
                    if (compType == null)
                        compType = Type.GetType("JanusRT.Engine.Components.Blocks." + values[0], false);
                    if (compType == null)
                        compType = Type.GetType("JanusRT.Engine.Components.Pickables." + values[0], true);
                    if (compType != null)
                    {
                        try
                        {
                            List<string> o = new List<string>();
                            for (int i = 1; i < values.Length; i++)
                            {
                                o.Add(values[i]);
                            }
                            Component comp = new Component(actor);
                            if (o.Count > 0)
                                comp = (Component)Activator.CreateInstance(compType, actor, o.ToArray());
                            else
                                comp = (Component)Activator.CreateInstance(compType, actor);
                            actor.components.Add(comp);
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.Message);
                            throw new Exception(exc.Message);
                        }

                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);

                }
            }

            switch (token) //tokens with values
            {
                case "Name":
                    {
                        if (values.Length > 1)
                            actor.pluralName = values[1];
                        else
                            actor.pluralName = values[0] + "s";
                        actor.name = values[0];
                        break;
                    }
                case "Char":
                    {

                        int ch = 0;
                        if (int.TryParse(values[0], out ch))
                            actor.ch = ch;
                        else
                            actor.ch = char.ConvertToUtf32(values[0].ToString(), 0);

                        if (values.Length > 1)
                        {
                            actor.chs = new int[values.Length];
                            for (int i = 0; i < values.Length; i++)
                            {

                                if (int.TryParse(values[i], out ch))
                                {
                                    actor.chs[i] = ch;

                                }
                                else
                                    actor.chs[i] = char.ConvertToUtf32(values[i].ToString(), 0);
                            }
                        }

                        break;
                    }

                case "Description":
                    {
                        actor.description = values[0];
                        break;
                    }

                case "Color":
                    {
                        if (values.Length > 1)
                            actor.color = new libtcod.TCODColor(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
                        else
                        {
                            try
                            {
                                System.Reflection.PropertyInfo property = typeof(libtcod.TCODColor).GetProperty(values[0]);
                                if (property != null)
                                {
                                    libtcod.TCODColor color = new libtcod.TCODColor();
                                    actor.color = (libtcod.TCODColor)property.GetValue(color, null);
                                }
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine(exc.Message);
                            }
                        }

                        break;
                    }
                case "Rarity":
                    {
                        if (values.Length > 0)
                            actor.rarity = int.Parse(values[0]);
                        else
                        {
                            actor.rarity = 0;
                        }
                        break;
                    }
                default:
                    break;
            }

        }
        else //for tokens without values
        {
            switch (token)
            {


                case "Blocks":
                    {
                        actor.blocks = true;
                        break;
                    }
                default:
                    break;
            }
        }

    }

    public static string token(string token, string value) { return ("[" + token + ":" + value + "]" + "\r\n"); }

    public static string token(string token, params string[] values)
    {
        string value = string.Empty;
        for (int i = 0; i < values.Length; i++)
        {
            value += values[i];
            if (i < values.Length - 1)
            {
                value += ":";
            }
        }
        return ("[" + token + ":" + value + "]" + "\r\n");
    }
    public static string componentToken(string token, object[] values)
    {
        string value = string.Empty;
        if (values.Length > 0)
        {
            value += ":";
            for (int i = 0; i < values.Length; i++)
            {
                value += values[i].ToString();
                if (i < values.Length - 1)
                {
                    value += ":";
                }
            }
        }
        return ("[" + "Component:" + token + value + "]" + "\r\n");
    }

    public static string token(string token) { return ("[" + token + "]" + "\r\n"); }
}

