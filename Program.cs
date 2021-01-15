using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MyNotesConsoleApp.Business.Managers;
using MyNotesConsoleApp.Business.Services;
using MyNotesConsoleApp.Data;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Database Access
            await using var context = new NoteAppDbContext();
            INotesService notesService = new NotesManager(context);
            ITagsService tagsService = new TagsManager(context);
            INoteTagsService noteTagsService = new NoteTagsManager(context);

            // Global Variables
            bool isMenuOpen = true;
            string greetingName = "Nuran";
            Dictionary<string, string> initialCommands = new Dictionary<string, string>
            {
                { "--info", "brief info about this program" },
                { "--commands", "indicate valid commands to use" },
                { "--data", "list, add, update or delete data from database" },
                { "--clear", "clear console" },
                { "--exit", "close this program" }
            };
            Dictionary<string, string> dataCommands = new Dictionary<string, string>
            {
                { "{entity-name} --get", "get all data from database" },
                { "{entity-name} --get-id", "get specific entity by its unique ID" },
                { "{entity-name} --add", "add new item to database" },
                { "{entity-name} --upd", "update exist item on database" },
                { "{entity-name} --del", "delete specific entity" }
            };

            // Greeting
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"~~ Welcome to MyNotesApp, {greetingName}!");

            CommandHelper.List(initialCommands);

            while (isMenuOpen)
            {
                string initCmd = CommandHelper.Reader();

                switch (initCmd)
                {
                    case "--info":
                        Console.WriteLine("Brief info about this program : MyNotesApp is created to store your notes in database. Help of this program you can add, update, delete and list (filter provided) your whole data. You can add note with tags. With this feature you can sort your notes by tag name. Best wishes!");
                        break;
                    case "--data":
                        CommandHelper.List(dataCommands, true);
                        string[] dataCmd = CommandHelper.Reader().Split(" ");
                        if (dataCmd.Length == 1)
                        {
                            CommandHelper.Wrong(dataCmd[0]);
                            break;
                        }

                        string entity = dataCmd[0].ToLower(), cmdForEntity = dataCmd[1];
                        if (entity == "notes")
                        {
                            if (cmdForEntity == "--get")
                            {
                                string filterAnswer = CommandHelper.YesOrNo("Do you want to filter by tag name");
                                var notes = new List<Note>();
                                switch (filterAnswer)
                                {
                                    case "y":
                                        string tagName =
                                            CommandHelper.AskAndReturnVariable(
                                                "Enter tag name you want to sort your notes");
                                        var tag = await tagsService.GetByNameAsync(tagName);
                                        if (tag != null) notes = await notesService.GetByTagNameAsync(tagName);
                                        else
                                        {
                                            CommandHelper.Error($"\n{tagName} doesn't match any record tag in db!");
                                            Console.WriteLine("\nWe give you notes without any sorting!");
                                            notes = await notesService.GetAllAsync();
                                        }
                                        break;
                                    case "n":
                                        notes = await notesService.GetAllAsync();
                                        break;
                                    default:
                                        CommandHelper.Wrong(filterAnswer);
                                        Console.WriteLine("\nWe give you notes without any sorting!");
                                        break;

                                }
                                if (notes.Count == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("\nNotes list is empty! Create new one with notes --add");
                                }
                                else
                                {
                                    foreach (var note in notes)
                                    {
                                        Console.WriteLine("---------------");
                                        Console.WriteLine($"ID : {note.Id}\nTitle : {note.Title}\nNote : {note.Content}");
                                        var tags = await notesService.GetTags(note.Id);
                                        foreach (var tag in tags)
                                        {
                                            Console.Write($"|{tag}|");
                                        }
                                        Console.WriteLine("\n---------------");
                                    }
                                }
                            }
                            else if (cmdForEntity == "--get-id")
                            {
                                Console.Write("ID of desired note integer : ");
                                int noteId = int.Parse(Console.ReadLine() ?? string.Empty);
                                var note = await notesService.GetByIdAsync(noteId);

                                if (note == null) CommandHelper.Error("Database doesn't include note with this ID");
                                else
                                {
                                    Console.WriteLine($"\nID : {note.Id}\nTitle : {note.Title}\nNote : {note.Content}");
                                }
                            }
                            else if (cmdForEntity == "--add")
                            {
                                string title = String.Empty;
                                do
                                {
                                    title = CommandHelper.AskAndReturnVariable("Title of new note", true);
                                    if (title == String.Empty)
                                    {
                                        CommandHelper.Error("Title should take value. Enter required data");
                                    }
                                } while (title == String.Empty);

                                string content = CommandHelper.AskAndReturnVariable("Note content");
                                try
                                {
                                    var newNote = new Note
                                    {
                                        Title = title,
                                        Content = content,
                                        CreatedAt = DateTime.Now
                                    };
                                    bool noteAdded = await notesService.CreateAsync(newNote);
                                    if (noteAdded) Console.WriteLine($"Note#{newNote.Id} is added db successfully!");
                                    else CommandHelper.Error("Problem occured in db");

                                    string tagAnswer = CommandHelper.YesOrNo("Do you want to add tags to your note");

                                    switch (tagAnswer)
                                    {
                                        case "y":
                                            string[] writtenTags = CommandHelper.AskAndReturnVariable("Note tags (ex: tag1 tag2)").Split(" ");

                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine("Tags are process ...");
                                            foreach (var writtenTag in writtenTags)
                                            {
                                                var tag = await tagsService.GetByNameAsync(writtenTag);
                                                if (tag == null)
                                                {
                                                    try
                                                    {
                                                        var newTag = new Tag
                                                        {
                                                            Name = writtenTag,
                                                            CreatedAt = DateTime.Now,
                                                        };
                                                        bool tagAdded = await tagsService.CreateAsync(newTag);
                                                        if (tagAdded)
                                                        {
                                                            Console.WriteLine($"{writtenTag} is added db successfully!");
                                                            bool notesTagAdded = await noteTagsService.CreateAsync(
                                                                new NoteTag
                                                                {
                                                                    NoteId = newNote.Id,
                                                                    TagId = newTag.Id
                                                                });
                                                            if (notesTagAdded) Console.WriteLine($"{writtenTag} is added Note#{newNote.Id} successfully!");
                                                            else CommandHelper.Error("Problem occured in db while adding tag to note");
                                                        }
                                                        else CommandHelper.Error("Problem occured in db while storing tag in database");
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine("Tags exception");
                                                        Console.WriteLine(e);
                                                    }
                                                }
                                                else
                                                {
                                                    bool notesExistTagAdded = await noteTagsService.CreateAsync(
                                                        new NoteTag
                                                        {
                                                            NoteId = newNote.Id,
                                                            TagId = tag.Id
                                                        });
                                                    if (notesExistTagAdded) Console.WriteLine($"{writtenTag} is added Note#{newNote.Id} successfully!");
                                                    else CommandHelper.Error("Problem occured in db while adding tag to note");
                                                }
                                            }
                                            break;
                                        case "n":
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine("Okey, no tag!");
                                            break;
                                        default:
                                            CommandHelper.Error("You miss your chance. Try to add tags when you update the note");
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Notes exception");
                                    Console.WriteLine(e);
                                }
                            }
                            else if (cmdForEntity == "--upd")
                            {
                                // update note
                            }
                            else if (cmdForEntity == "--del")
                            {
                                // delete note
                            }
                            else CommandHelper.Wrong(cmdForEntity);
                        }
                        else if (entity == "tags")
                        {
                            // logical code blocks about tags' processes
                        }
                        break;
                    case "--commands":
                        CommandHelper.List(initialCommands);
                        break;
                    case "--clear":
                        Console.Clear();
                        break;
                    case "--exit":
                        isMenuOpen = false;
                        break;
                    default:
                        CommandHelper.Wrong(initCmd);
                        break;
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}