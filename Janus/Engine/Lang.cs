using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Janus.Engine {
    public class Lang {


        public static bool vocal(string s) {
            char ch = char.ToLower(s[0]);
            if (ch == 'o' || ch == 'i' || ch == 'a' || ch == 'e' || ch == 'u' || ch == 'y') {

                return true;
            }
            return false;
        }
        public static string grammar(string s)
        {
           
            if(s == String.Empty || s == null)
                return s;
            if (s.Contains(" a "))
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if ((s[i] == 'a' || s[i] == 'A') && s.Length > i +2)
                    {
                        if(s.Length > 1 && s[i - 1] == ' ')
                        if (vocal(s[i + 2].ToString()))
                        {
                          s= s.Insert(i+1, "n");
                        }
                    }
                }
            }

            if (Char.IsLower(s, 0))
            {

                s = replaceAt(0, s, Char.ToUpper(s[0]).ToString());
                
            }
            if (s.Contains("the you"))
            {
               s = s.Replace("the you", "you");
            }
            return s;
        }
        public static string replaceAt (int index, string s, string r)
    {
        s = s.Insert(index, r);
        s = s.Remove(index + 1, 1);
        return s;
    }
    }
}
