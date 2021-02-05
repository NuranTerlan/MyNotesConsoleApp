using System;
using System.Collections.Generic;
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
            Console.Title = "Notes Manager"; // change title of opened console we work

            // Database Access
            var context = new NoteAppDbContext();
            // Service Instances
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
                { "--get", "get all data from database" },
                { "--get-id", "get specific entity by its unique ID" },
                { "--add", "add new item to database" },
                { "--upd", "update exist item on database" },
                { "--del", "delete specific entity" }
            }; // dict reference https://www.tutorialsteacher.com/csharp/csharp-dictionary

            // Greeting
            string appNameASCII = @"
                 ███▄ ▄███▓▓██   ██▓ ███▄    █  ▒█████  ▄▄▄█████▓▓█████   ██████  ▄▄▄       ██▓███   ██▓███  
                ▓██▒▀█▀ ██▒ ▒██  ██▒ ██ ▀█   █ ▒██▒  ██▒▓  ██▒ ▓▒▓█   ▀ ▒██    ▒ ▒████▄    ▓██░  ██▒▓██░  ██▒
                ▓██    ▓██░  ▒██ ██░▓██  ▀█ ██▒▒██░  ██▒▒ ▓██░ ▒░▒███   ░ ▓██▄   ▒██  ▀█▄  ▓██░ ██▓▒▓██░ ██▓▒
                ▒██    ▒██   ░ ▐██▓░▓██▒  ▐▌██▒▒██   ██░░ ▓██▓ ░ ▒▓█  ▄   ▒   ██▒░██▄▄▄▄██ ▒██▄█▓▒ ▒▒██▄█▓▒ ▒
                ▒██▒   ░██▒  ░ ██▒▓░▒██░   ▓██░░ ████▓▒░  ▒██▒ ░ ░▒████▒▒██████▒▒ ▓█   ▓██▒▒██▒ ░  ░▒██▒ ░  ░
                ░ ▒░   ░  ░   ██▒▒▒ ░ ▒░   ▒ ▒ ░ ▒░▒░▒░   ▒ ░░   ░░ ▒░ ░▒ ▒▓▒ ▒ ░ ▒▒   ▓▒█░▒▓▒░ ░  ░▒▓▒░ ░  ░
                ░  ░      ░ ▓██ ░▒░ ░ ░░   ░ ▒░  ░ ▒ ▒░     ░     ░ ░  ░░ ░▒  ░ ░  ▒   ▒▒ ░░▒ ░     ░▒ ░     
                ░      ░    ▒ ▒ ░░     ░   ░ ░ ░ ░ ░ ▒    ░         ░   ░  ░  ░    ░   ▒   ░░       ░░       
                       ░    ░ ░              ░     ░ ░              ░  ░      ░        ░  ░                  
                            ░ ░                                                                              
            "; // ascii art generator reference : http://patorjk.com/software/taag/#p=display&f=Bloody&t=MyNotesApp
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"~~ Welcome to MyNotesApp, {greetingName}!\n\n{appNameASCII}\n");

            // Opening at the very first time
            CommandHelper.List(initialCommands);

            while (isMenuOpen)
            {
                string initCmd = CommandHelper.Reader();

                switch (initCmd)
                {
                    case "--info":
                        Console.WriteLine("Brief info about this program :\nMyNotesApp is created to store your notes in database. \nHelp of this program you can add, update, delete and list (filter provided) your whole data. \nYou can add note with tags. With this feature you can sort your notes by tag name. Best wishes!");
                        break;
                    case "--data":
                    {
                            CommandHelper.List(dataCommands, true);
                            string[] dataCmd = CommandHelper.Reader().Split(" ");
                            if (dataCmd.Length == 1)
                            {
                                CommandHelper.Wrong(dataCmd[0]);
                                break;
                            }

                            string entity = dataCmd[0].ToLower(), cmdForEntity = dataCmd[1].ToLower();
                            if (entity == "notes")
                            {
                                switch (cmdForEntity)
                                {
                                    case "--get":
                                        {
                                            string filterAnswer = CommandHelper.YesOrNo("Do you want to filter by tag name");
                                            var notes = new List<Note>();
                                            switch (filterAnswer)
                                            {
                                                case "y":
                                                    string tagName =
                                                        CommandHelper.AskAndReturnVariable(
                                                            "Enter tag name you want to sort your notes", true);
                                                    var tag = await tagsService.GetByNameAsync(tagName);
                                                    if (tag != null) notes = await notesService.GetByTagNameAsync(tagName);
                                                    else
                                                    {
                                                        CommandHelper.Error($"\n{tagName} doesn't match any record tag in db!");
                                                        Console.WriteLine("\nWe'll give you notes without any sorting!");
                                                        notes = await notesService.GetAllAsync();
                                                    }
                                                    break;
                                                case "n":
                                                    notes = await notesService.GetAllAsync();
                                                    break;
                                                default:
                                                    CommandHelper.Wrong(filterAnswer);
                                                    Console.WriteLine("\nWe'll give you notes without any sorting!");
                                                    break;

                                            }
                                            if (notes.Count == 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                                Console.WriteLine("\nNotes list is empty! Create new one with notes --add command");
                                            }
                                            else
                                            {
                                                bool isPlural = notes.Count > 1;
                                                Console.Write($"{notes.Count} note{(isPlural ? "s" : "")} found ...");
                                                foreach (var note in notes)
                                                {
                                                    NotesHelper.NoteLogger(note);
                                                }
                                            }

                                            break;
                                        }
                                    case "--get-id":
                                        {
                                            Console.Write("ID of desired note integer : ");
                                            int noteId = int.Parse(Console.ReadLine() ?? string.Empty);
                                            var note = await notesService.GetByIdAsync(noteId);

                                            if (note == null) CommandHelper.Error("Database doesn't include note with this ID");
                                            else
                                            {
                                                NotesHelper.NoteLogger(note);
                                            }

                                            break;
                                        }
                                    case "--add":
                                        {
                                            // required
                                            string title;
                                            do
                                            {
                                                title = CommandHelper.AskAndReturnVariable("Title of new note", true);
                                                if (title == String.Empty)
                                                {
                                                    CommandHelper.Error("Title should take value. Enter required data");
                                                }
                                            } while (title == String.Empty);

                                            // optional
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
                                                if (noteAdded) CommandHelper.Success($"Note#{newNote.Id} is added db");
                                                else CommandHelper.Error("Problem occured in db");

                                                string tagAnswer = CommandHelper.YesOrNo("Do you want to add tags to your note");

                                                switch (tagAnswer)
                                                {
                                                    case "y":
                                                        string[] writtenTags = CommandHelper.AskAndReturnVariable("Note tags (ex: tag1 tag2)").Split(" ");

                                                        Console.WriteLine("Tags are processing ...");
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
                                                                        CommandHelper.Success($"{writtenTag} is added db");
                                                                        bool notesTagAdded = await noteTagsService.CreateAsync(
                                                                            new NoteTag
                                                                            {
                                                                                NoteId = newNote.Id,
                                                                                TagId = newTag.Id
                                                                            });
                                                                        if (notesTagAdded) CommandHelper.Success($"{writtenTag} is added Note#{newNote.Id}");
                                                                        else CommandHelper.Error("Problem occured in db while adding tag to note");
                                                                    }
                                                                    else CommandHelper.Error("Problem occured in db while storing tag in database");
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    CommandHelper.Error($"Tags exception => {e}");
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
                                                                if (notesExistTagAdded) CommandHelper.Success($"{writtenTag} is added Note#{newNote.Id}");
                                                                else CommandHelper.Error("Problem occured in db while adding tag to note");
                                                            }
                                                        }
                                                        break;
                                                    case "n":
                                                        Console.WriteLine("Ok, no tag!");
                                                        break;
                                                    default:
                                                        CommandHelper.Error("You miss your chance. Try to add tags when you update the note");
                                                        break;
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                CommandHelper.Error($"Notes exception => {e}");
                                            }

                                            break;
                                        }
                                    case "--upd":
                                        {
                                            int noteId =
                                                int.Parse(CommandHelper.AskAndReturnVariable("Id of destination note", true));
                                            var note = await notesService.GetByIdAsync(noteId);
                                            if (note == null) CommandHelper.Error($"Not found with {noteId} ID");
                                            else
                                            {
                                                CommandHelper.Success($"Note#{noteId} is fetched");
                                                string newTitle = CommandHelper.EditProp("Title", note.Title);
                                                string newContent = CommandHelper.EditProp("Content", note.Content);

                                                note.Title = newTitle;
                                                note.Content = newContent;
                                                note.UpdatedAt = DateTime.Now;

                                                bool isUpdated = await notesService.UpdateAsync(note);
                                                if (isUpdated) CommandHelper.Success($"Note#{noteId} is updated");
                                                else CommandHelper.Error("Problem happened in db");
                                            }

                                            break;
                                        }
                                    case "--del":
                                        {
                                            int noteId =
                                                int.Parse(CommandHelper.AskAndReturnVariable("Id of note you want to delete",
                                                    true));
                                            var note = await notesService.GetByIdAsync(noteId);
                                            if (note == null) CommandHelper.Error($"Not found with {noteId} ID");
                                            else
                                            {
                                                bool isRemoved = await notesService.DeleteAsync(note);
                                                if (isRemoved) CommandHelper.Success($"Note#{noteId} is deleted");
                                                else CommandHelper.Error("We have a problem in our db");
                                            }

                                            break;
                                        }
                                    default:
                                        CommandHelper.Wrong(cmdForEntity);
                                        break;
                                }
                            }
                            else if (entity == "tags")
                            {
                                // logical code blocks about tags' processes
                                CommandHelper.Error("Add commands for tags before");
                            }
                            else CommandHelper.Wrong(entity);
                            break;
                    }
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