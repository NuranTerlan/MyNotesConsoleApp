using System;
using System.Collections.Generic;

namespace MyNotesConsoleApp
{
    public static class CommandHelper
    {
        public static void List(Dictionary<string, string> commands, bool isData = false)
        {
            string commandName = isData ? "Data" : "Initial";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n {commandName} commands to use this program : \n");
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
            string command = Console.ReadLine();

            return command;
        }

        public static string AskAndReturnVariable(string question, bool isRequired = false)
        {
            string required = isRequired ? "required" : "optional";
            Console.Write($"{question} ({required}) : ");
            string answer = Console.ReadLine();

            return answer;
        }

        public static string YesOrNo(string question)
        {
            Console.Write($"{question}? (y/n) : ");
            string answer = Console.ReadLine()?.ToLower();

            return answer;
        }

        public static void Success(string successMsg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{successMsg} successfully --------");
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
            Console.WriteLine($"{content}!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string EditProp(string propName, string prop)
        {
            string askForProp = YesOrNo($"\nDo you want to change {propName}");
            string editedString;
            switch (askForProp)
            {
                case "y":
                    Console.Write($"{propName} : ");
                    editedString = ReadLine(prop);
                    break;
                case "n":
                    Console.WriteLine("Default is keeping!");
                    editedString = prop;
                    break;
                default:
                    Error($"{askForProp} is incorrect answer type!");
                    Console.WriteLine("But we are keeping default!");
                    editedString = prop;
                    break;
            }

            return editedString;
        }

        public static string ReadLine(string defaultStr = "default")
        {
            int pos = Console.CursorLeft;
            int initPos = pos;
            List<char> chars = new List<char>();
            if (string.IsNullOrEmpty(defaultStr) == false)
            {
                chars.AddRange(defaultStr.ToCharArray());
            }

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

                if (info.Key == ConsoleKey.Backspace 
                    && Console.CursorLeft > initPos
                    && Console.CursorLeft <= (initPos + chars.Count))
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
                else if (info.Key == ConsoleKey.RightArrow 
                         && Console.CursorLeft >= initPos 
                         && Console.CursorLeft < (initPos + chars.Count)) pos++;
                else if (info.Key == ConsoleKey.Enter) break;
            }

            string finalRes = new string(chars.ToArray());

            return finalRes;
        }
    }
}