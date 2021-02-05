using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyNotesConsoleApp.Business.Services;
using MyNotesConsoleApp.Data;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Business.Managers
{
    public class NoteTagsManager : INoteTagsService
    {
        private readonly NoteAppDbContext _context;

        public NoteTagsManager(NoteAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NoteTag>> GetAllAsync()
        {
            return await _context.NoteTags.ToListAsync();
        }

        public async Task<bool> CreateAsync(NoteTag entity)
        {
            await _context.NoteTags.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}