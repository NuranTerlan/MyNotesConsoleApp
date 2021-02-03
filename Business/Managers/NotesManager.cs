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
            return await _context.Notes.Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag).ToListAsync();
        }

        public async Task<Note> GetByIdAsync(int entityId)
        {
            return await _context.Notes.Include(n => n.NoteTags).ThenInclude(nt => nt.Tag).FirstOrDefaultAsync(note => note.Id == entityId);
        }

        public async Task<bool> CreateAsync(Note entity)
        {
            await _context.Notes.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Note entity)
        {
            _context.Notes.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Note entity)
        {
            _context.Notes.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Note>> GetByTagNameAsync(string tagName)
        {
            tagName = tagName.ToLower();

            var filteredNotes = await _context.Notes
                .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
                .Where(n => n.NoteTags.Any(nt => nt.Tag.Name.ToLower() == tagName))
                .ToListAsync();

            return filteredNotes;
        }
    }
}