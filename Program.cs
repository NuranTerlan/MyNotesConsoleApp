using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyNotesConsoleApp.Business.Managers;
using MyNotesConsoleApp.Business.Services;
using MyNotesConsoleApp.Data;
using MyNotesConsoleApp.Data.Entities;
using MyNotesConsoleApp.Extensions;

namespace MyNotesConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Notes Manager";

            // Database Access
            var context = new NoteAppDbContext();

            // Service Instance
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
            var dataCommands = new Dictionary<string, string>
            {
                { "--get", "get all data from database" },
                { "--get-id", "get specific entity by its unique ID" },
                { "--add", "add new item to database" },
                { "--upd", "update exist item on database" },
                { "--del", "delete specific entity" }
            };

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
                    ";
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
                        {
                            Console.WriteLine("Brief info about this program :\nMyNotesApp is created to store your notes in database. \nHelp of this program you can add, update, delete and list (filter provided) your whole data. \nYou can add note with tags. With this feature you can sort your notes by tag name. Best wishes!");
                            break;
                        }
                    case "--data":
                        {
                            CommandHelper.List(dataCommands, true);
                            string[] dataCmd = CommandHelper.Reader().Split(" ");
                            if (dataCmd.Length == 1)
                            {
                                CommandHelper.Wrong(dataCmd[0]);
                                break;
                            }

                            string entity = dataCmd[0], cmdForEntity = dataCmd[1];

                            if (entity == "notes")
                            {
                                switch (cmdForEntity)
                                {
                                    case "--get": // ready to use
                                        {
                                            string filteredAnswer = CommandHelper
                                                .YesOrNo("Do you want to filter by tag name");
                                            bool isMatched = false;
                                            var notes = new List<Note>();
                                            switch (filteredAnswer)
                                            {
                                                case "y":
                                                {
                                                    string tagName = CommandHelper.AskAndReturnVariable(
                                                        "Enter tag name you want to sort your notes", true);
                                                    var tag = await tagsService.GetByNameAsync(tagName);
                                                    if (tag != null)
                                                    {
                                                        notes = await notesService.GetByTagNameAsync(tagName);
                                                        isMatched = true;
                                                    }
                                                    else
                                                    {
                                                        CommandHelper.Error(
                                                            $"{tagName} doesn't match any tag in db");
                                                        CommandHelper.Warning(
                                                            $"Any note isn't found with #{tagName} tag");
                                                    }

                                                    break;
                                                }
                                                case "n":
                                                {
                                                    notes = await notesService.GetAllAsync();
                                                    break;
                                                }
                                                default:
                                                {
                                                    CommandHelper.Wrong(filteredAnswer);
                                                    CommandHelper.Error(
                                                        "\nWe'll give you notes without any sorting!");
                                                    break;
                                                }
                                            }

                                            if (notes.Count == 0 && isMatched)
                                                CommandHelper.Warning(
                                                    "Notes list is empty! Create new one with 'notes --add' command");
                                            else if (notes.Count != 0)
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
                                    case "--get-id": // ready to use
                                        {
                                            int noteId = int.Parse(CommandHelper.AskAndReturnVariable(
                                                "ID of desired note as an integer", true));
                                            var note = await notesService.GetByIdAsync(noteId);

                                            if (note != null) NotesHelper.NoteLogger(note);
                                            else CommandHelper.Error($"Database doesn't include note with ID#{noteId}");

                                            break;
                                        }
                                    case "--add": // ready to use
                                        {
                                            string title; // required field
                                            do
                                            {
                                                title = CommandHelper.AskAndReturnVariable("Title of new note", true);
                                                if (title == String.Empty)
                                                    CommandHelper.Error("Title should take a value. Enter required data");
                                            } while (title == String.Empty);

                                            // optional field
                                            string content = CommandHelper.AskAndReturnVariable("Note content: ");

                                            try
                                            {
                                                var newNote = new Note
                                                {
                                                    Title = title,
                                                    Content = content,
                                                    CreatedAt = DateTime.Now
                                                };

                                                bool noteAdded = await notesService.CreateAsync(newNote);
                                                if (!noteAdded) CommandHelper.Error("Problem occured in db while creating a new element");
                                                else
                                                {
                                                    CommandHelper.Success($"Note#{newNote.Id} is added db");

                                                    string tagAnswer =
                                                        CommandHelper.YesOrNo("Do you want to add tags to your note");
                                                    switch (tagAnswer)
                                                    {
                                                        case "y":
                                                            {
                                                                string[] writtenTags = CommandHelper.AskAndReturnVariable(
                                                                    "Note tags (ex: tag1 tag2)").Split(" ");

                                                                CommandHelper.Warning("Tags are processing");
                                                                foreach (var writtenTag in writtenTags)
                                                                {
                                                                    var tag = await tagsService.GetByNameAsync(writtenTag);
                                                                    if (tag == null)
                                                                    {
                                                                        var newTag = new Tag
                                                                        {
                                                                            Name = writtenTag,
                                                                            CreatedAt = DateTime.Now
                                                                        };
                                                                        bool tagAdded = await tagsService.CreateAsync(newTag);
                                                                        if (!tagAdded) CommandHelper.Error(
                                                                            "Problem occured in db while storing tag in database");
                                                                        else
                                                                        {
                                                                            CommandHelper.Success($"{writtenTag} is added db");
                                                                            bool notesTagAdded =
                                                                                await noteTagsService.CreateAsync(
                                                                                    new NoteTag
                                                                                    {
                                                                                        NoteId = newNote.Id,
                                                                                        TagId = newTag.Id
                                                                                    });
                                                                            if (!notesTagAdded) CommandHelper.Error("Problem occured in db while adding tag to note");
                                                                            else CommandHelper.Success($"{writtenTag} is added Note#{newNote.Id}");
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
                                                                        if (!notesExistTagAdded) CommandHelper.Error("Problem occured in db while adding existing tag to note");
                                                                        else CommandHelper.Success($"{writtenTag} is added Note#{newNote.Id}");
                                                                    }
                                                                }

                                                                break;
                                                            }
                                                        case "n":
                                                            {
                                                                CommandHelper.Warning("Oka, no tag");
                                                                break;
                                                            }
                                                        default:
                                                            {
                                                                CommandHelper.Error(
                                                                    "You miss your chance. Try to add tags when you update the note");
                                                                break;
                                                            }
                                                    }
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                CommandHelper.Error($"Notes exception => {e}");
                                                throw;
                                            } // adding try

                                            break;
                                        }
                                    case "--upd": // ready to use
                                        {
                                            int noteId = int.Parse(CommandHelper.AskAndReturnVariable(
                                                "Id of destination note", true));
                                            var note = await notesService.GetByIdAsync(noteId);
                                            if (note == null) CommandHelper.Error($"Note isn't found with {noteId} ID");
                                            else
                                            {
                                                CommandHelper.Success($"Note#{noteId} is fetched");
                                                string newTitle = CommandHelper.EditStringProp("Title", note.Title);
                                                string newContent = CommandHelper.EditStringProp("Content", note.Content);

                                                note.Title = newTitle;
                                                note.Content = newContent;
                                                note.UpdatedAt = DateTime.Now;

                                                bool isUpdated = await notesService.UpdateAsync(note);
                                                if (!isUpdated) CommandHelper.Error("Problem happened in db while updating existing element");
                                                else CommandHelper.Success($"Note#{noteId} is updated");
                                            }

                                            break;
                                        }
                                    case "--del": // ready to use
                                        {
                                            int noteId = int.Parse(CommandHelper.AskAndReturnVariable(
                                                "Id of note you want to delete", true));

                                            var note = await notesService.GetByIdAsync(noteId);
                                            if (note == null) CommandHelper.Error($"Note isn't found with {noteId} ID");
                                            else
                                            {
                                                bool isRemoved = await notesService.DeleteAsync(note);
                                                if (!isRemoved) CommandHelper.Error("We have a problem in our db");
                                                else CommandHelper.Success($"Note#{noteId} is deleted");
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
                                CommandHelper.Warning("Tags related operations should be here, but implement them first!");
                                // tags related code
                            }
                            else CommandHelper.Wrong(entity);

                            break;
                        }
                    case "--commands":
                        {
                            CommandHelper.List(initialCommands);
                            break;
                        }
                    case "--clear":
                        {
                            Console.Clear();
                            break;
                        }
                    case "--exit":
                        {
                            isMenuOpen = false;
                            break;
                        }
                    default:
                        {
                            CommandHelper.Wrong(initCmd);
                            break;
                        }
                }
            }
        }
    }
}