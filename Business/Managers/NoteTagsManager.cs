using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task<List<NoteTag>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<NoteTag> GetByIdAsync(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> CreateAsync(NoteTag entity)
        {
            if (entity == null) return false;

            await _context.NoteTags.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<bool> UpdateAsync(NoteTag entity)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteAsync(NoteTag entity)
        {
            throw new System.NotImplementedException();
        }
    }
}