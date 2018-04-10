using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
namespace Janus.Engine
{

    public class ColoredChar
    {
        public char ch;
        public TCODColor color;

        public ColoredChar(char ch)
        {
            this.ch = ch;
            this.color = TCODColor.white;
        }
        public ColoredChar(char ch, TCODColor color)
        {
            this.ch = ch;
            this.color = color;
        }
    }

    public class Line
    {

        public List<ColoredChar> text = new List<ColoredChar>();

        public int number = 1;
        public Line(string text)
        {
            this.text = stringToLine(text);
        }
        public Line(string text, TCODColor color)
        {
            this.text = stringToLine(text, color);
        }
        public Line(string text, TCODColor color, int number)
        {
            this.text = stringToLine(text, color);
            this.number = number;
        }

        public string getString()
        {
            string s = "";
            for (int i = 0; i < text.Count; i++)
            {
                s += text[i].ch;
            }
            return s;
        }

        public void setString(string text)
        {

            for (int i = 0; i < text.Length; i++)
            {
                if (i < this.text.Count)
                {
                    this.text[i] = new ColoredChar(text[i], this.text[i].color);
                }
                else
                    this.text.Add(new ColoredChar(text[i], this.text.Last().color));
            }

        }
        public void setStringAt(string text, int index)
        {

            for (int i = index; i < text.Length + index; i++)
            {
                if (i < this.text.Count)
                {
                    this.text[i] = new ColoredChar(text[i - index], this.text[i - index].color);
                }
                else
                    this.text.Add(new ColoredChar(text[i - index], this.text.Last().color));
            }

        }
        public void setStringAt(string text, int index, TCODColor color)
        {

            for (int i = index; i < text.Length + index; i++)
            {
                if (i < this.text.Count)
                {
                    this.text[i] = new ColoredChar(text[i - index], color);
                }
                else
                    this.text.Add(new ColoredChar(text[i - index], color));
            }

        }
        public void setString(string text, TCODColor color)
        {

            for (int i = 0; i < text.Length; i++)
            {
                if (i < this.text.Count)
                {
                    this.text[i] = new ColoredChar(text[i], color);
                }
                else
                    this.text.Add(new ColoredChar(text[i], color));
            }

        }

        public List<ColoredChar> stringToLine(string text)
        {
            List<ColoredChar> line = new List<ColoredChar>();
            for (int i = 0; i < text.Length; i++)
            {
                line.Add(new ColoredChar(text[i]));
            }
            return line;
        }
        public List<ColoredChar> stringToLine(string text, TCODColor color)
        {
            List<ColoredChar> line = new List<ColoredChar>();
            for (int i = 0; i < text.Length; i++)
            {
                line.Add(new ColoredChar(text[i], color));
            }
            return line;
        }
    }

    public class Message
    {

        public static List<Line> lines = new List<Line>();



        public static int MaxLength = 50;

        public static void Write(string s, params object[] args)
        {
            Write(string.Format(s, args));
        }

        public static void update()
        {

        }

        public static Line getLastLine()
        {
            if (lines.Count > 0)
                return lines[lines.Count - 1];
            else
                return null;
        }

        public static void Write(string s)
        {
            WriteC(TCODColor.white, s);

        }
        public static void WriteLine(string s, params object[] args)
        {
            WriteLine(string.Format(s, args));
        }
        public static void WriteLine(string s)
        {

            WriteLineC(TCODColor.white, s);
        }

        public static void WriteC(TCODColor color, string s, params object[] args)
        {
            WriteC(color, string.Format(s, args));
        }



        public static void WriteC(TCODColor color, string s)
        {
            s = Lang.grammar(s); //last line of defense
            if (lines.Count > 0)
            {
                Line line = getLastLine();
                if (line.text.Count < MaxLength)
                    line.text.AddRange(line.stringToLine(s, color));
                else
                {

                    lines.Add(new Line(s, color));

                }
            }
            else
            {

                lines.Add(new Line(s, color));

            }
        }
        public static void WriteLineC(TCODColor color, string s, params object[] args)
        {
            if (s != null && color != null)
                WriteLineC(color, (string.Format(s, args)));
        }
        public static void WriteLineC(TCODColor color, string s)
        {
            s = Lang.grammar(s);
            if (s == null)
                return;

            if (lines.Count > 0)
            {
                string lineText = getLastLine().getString();
                if (lineText.Contains(s) && s != ">")
                {

                    if (getLastLine().number > 1)
                    {
                        if (lineText.Contains(" (x" + (getLastLine().number).ToString() + ")"))
                        {
                            getLastLine().setStringAt(" (x" + (getLastLine().number + 1).ToString() + ")", lineText.Length - (4 + getLastLine().number.ToString().Length), TCODColor.grey);
                            getLastLine().number++;
                            return;
                        }
                    }
                    getLastLine().number++;
                    getLastLine().text.AddRange(getLastLine().stringToLine(" (x" + (getLastLine().number).ToString() + ")", TCODColor.grey));
                    return;
                }


            }
            if (s.Length <= MaxLength)
            {
                lines.Add(new Line(s, color));

            }
            else
            {

                if (s.Length <= MaxLength * 2)
                {

                    string n = s.Substring(MaxLength, s.Length - MaxLength);
                    s = s.Substring(0, MaxLength);
                    lines.Add(new Line(s, color));
                    lines.Add(new Line(n, color));
                }
                else
                {
                    string n = s.Substring(MaxLength, MaxLength);
                    string n2 = s.Substring(MaxLength * 2, s.Length - MaxLength * 2);
                    s = s.Substring(0, MaxLength);
                    lines.Add(new Line(s, color));
                    lines.Add(new Line(n, color));
                    lines.Add(new Line(n2, color));
                }
            }

        }
        public static void unwriteLastLine()
        {
            if (lines.Count > 0)
            {
                Line line = lines[lines.Count - 1];
                lines.Remove(line);
            }
        }
        public static void unwriteLastChar()
        {
            if (lines.Count > 0)
            {
                Line line = lines[lines.Count - 1];
                if (line.text.Count == 0)
                {
                    lines.Remove(line);
                    if (lines.Count > 0)
                    {
                        line = lines[lines.Count - 1];
                    }
                    else
                        return;
                }


                unwrite(lines.Count - 1, line.text.Count - 1, 1);
            }

        }
        public static void unwrite(int lineNumber, int index, int count)
        {
            if (lineNumber >= 0 && lineNumber < lines.Count)
            {
                Line line = lines[lineNumber];

                if (index + count > 0 && index + count <= line.text.Count)
                {
                    line.text.RemoveRange(index, count);
                }
            }
        }

    }
}
