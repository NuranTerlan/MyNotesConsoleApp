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
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var (key, value) in commands)
            {
                Console.WriteLine($"    {key} ( {value} )");
            }
        }

        public static string Reader()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n~command/ ");
            Console.ForegroundColor = ConsoleColor.White;
            string command = Console.ReadLine();
            Console.WriteLine($"\nYour command is {command}\n");
            Console.ForegroundColor = ConsoleColor.Cyan;

            return command;
        }

        public static string AskAndReturnVariable(string question, bool isRequired = false)
        {
            string required = isRequired ? "required" : "optional";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{question} ({required}) : ");
            Console.ForegroundColor = ConsoleColor.White;
            string answer = Console.ReadLine();

            return answer;
        }

        public static string YesOrNo(string question)
        {
            Console.Write($"{question}? (y/n) : ");
            string answer = Console.ReadLine()?.ToLower();

            return answer;
        }

        public static void Wrong(string command)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{command} is invalid command. Try again!");
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void Error(string content)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{content}!");
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}