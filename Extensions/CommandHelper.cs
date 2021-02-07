using System;
using System.Collections.Generic;

namespace MyNotesConsoleApp.Extensions
{
    public static class CommandHelper // extension class
    {
        public static void List(Dictionary<string, string> commands, bool isData = false)
        {
            string commandName = isData ? "Data" : "Initial";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n {commandName} commands to use this program: \n");
            Console.ForegroundColor = ConsoleColor.White;

            foreach (var (key, value) in commands)
            {
                Console.WriteLine($"    {(isData ? "{entity-name}" : "")} {key} ( {value} )");
            }
        }

        public static string Reader()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n~command/ ");
            Console.ForegroundColor = ConsoleColor.White;
            string command = Console.ReadLine()?.Trim();

            return command;
        }

        public static string AskAndReturnVariable(string question, bool isRequired = false)
        {
            string required = isRequired ? "required" : "optional";
            Console.Write($"{question} ({required}): ");
            string answer = Console.ReadLine()?.Trim();

            return answer;
        }

        public static string YesOrNo(string question)
        {
            Console.Write($"{question}? (y/n): ");
            string answer = Console.ReadLine()?.Trim().ToLower();

            return answer;
        }

        public static void Success(string successMsg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  SUCCESS: {successMsg} successfully ----------");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Warning(string warnMsg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  WARNING: {warnMsg}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Wrong(string command)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{command} is invalid command. Try again!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(string content)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ERROR: {content}!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // special methods for data below

        public static string EditStringProp(string propName, string prop)
        {
            string askForProp = YesOrNo($"\nDo you want to change {propName}");
            switch (askForProp)
            {
                case "y":
                    {
                        Console.Write($"{propName}: ");
                        string editedProp = ReadLine(prop);

                        return editedProp;
                    }
                case "n":
                    {
                        Warning($"Default is keeping for {propName}");
                        return prop;
                    }
                default:
                    {
                        Wrong(askForProp);
                        Warning("We are keeping default data for it");
                        return prop;
                    }
            }
        }

        public static string ReadLine(string givenString = "default prop content")
        {
            int pos = Console.CursorLeft;
            int initPos = pos;
            List<char> chars = new List<char>(givenString.ToCharArray());

            while (true)
            {
                Console.CursorLeft = initPos;
                foreach (var c in chars)
                {
                    Console.Write(c);
                }
                Console.Write(' ');

                Console.CursorLeft = pos + 1;

                ConsoleKeyInfo info = Console.ReadKey(true);

                if (info.Key == ConsoleKey.Backspace && Console.CursorLeft > initPos
                                                     && Console.CursorLeft <= initPos + chars.Count)
                {
                    chars.RemoveAt(pos - initPos);
                    if (pos >= initPos) pos--;
                }
                else if (info.Key == ConsoleKey.Spacebar)
                {
                    pos++;
                    chars.Insert(pos - initPos, ' ');
                }
                else if (char.IsLetterOrDigit(info.KeyChar))
                {
                    pos++;
                    chars.Insert(pos - initPos, info.KeyChar);
                }
                else if (info.Key == ConsoleKey.LeftArrow && Console.CursorLeft > initPos) pos--;
                else if (info.Key == ConsoleKey.RightArrow && Console.CursorLeft >= initPos
                                                           && Console.CursorLeft < initPos + chars.Count) pos++;
                else if (info.Key == ConsoleKey.Enter) break;
            }

            string finalRes = new string(chars.ToArray());

            return finalRes;
        }
    }
}