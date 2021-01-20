using System;
using MoreLinq;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp
{
    public static class NotesHelper
    {
        public static void NoteLogger(Note note)
        {
            string updatedTime = note.UpdatedAt == null
                ? ""
                : $"\nUpdated : {note.UpdatedAt}";
            Console.WriteLine($"\n---------------\nID : {note.Id}\nTitle : {note.Title}\nNote : {note.Content}\nCreated : {note.CreatedAt}{updatedTime}");
            note.NoteTags.ForEach(nt => Console.Write($"#{nt.Tag.Name} "));
            Console.WriteLine("\n---------------");
        }
    }
}