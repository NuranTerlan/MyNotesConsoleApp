using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyNotesConsoleApp.Business.Services;
using MyNotesConsoleApp.Data;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Business.Managers
{

    public class NotesManager : INotesService
    {
        private readonly NoteAppDbContext _context;

        public NotesManager(NoteAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Note>> GetAllAsync()
        {
            return await _context.Notes.Include(n => n.NoteTags).ThenInclude(nt => nt.Tag).ToListAsync();
        }

        public async Task<Note> GetByIdAsync(int entityId)
        {
            return await _context.Notes.Include(n => n.NoteTags).ThenInclude(nt => nt.Tag).FirstOrDefaultAsync(note => note.Id == entityId);
        }

        public async Task<bool> CreateAsync(Note entity)
        {
            if (entity == null) return false;

            await _context.Notes.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Note entity)
        {
            if (entity == null) return false;

            _context.Notes.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Note entity)
        {
            if (entity == null) return false;

            _context.Notes.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Note>> GetByTagNameAsync(string tagName)
        {
            tagName = tagName.ToLower();
            //var searchedTag = await _context.Tags.Select(t => new {t.Id, t.Name}).
            //    FirstOrDefaultAsync(t => t.Name.ToLower() == tagName.ToLower());

            //var filteredNotes = await _context.Notes.Include(n => n.NoteTags.Where(nt => nt.TagId == searchedTag.Id))
            //    .ThenInclude(nt => nt.Tag)
            //    .ToListAsync();

            var filteredNotes = await _context.NoteTags
                .Include(nt => nt.Tag.Name.ToLower() == tagName)
                .Select(nt => nt.Note)
                .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
                .ToListAsync();

            return filteredNotes;
        }

        public async Task<List<Tag>> GetTags(int noteId)
        {
            return await _context.NoteTags
                .Include(nt => nt.Note.Id == noteId)
                .Select(nt => nt.Tag)
                .ToListAsync();
        }
    }
}