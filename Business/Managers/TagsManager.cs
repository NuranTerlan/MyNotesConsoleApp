using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyNotesConsoleApp.Business.Services;
using MyNotesConsoleApp.Data;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Business.Managers
{
    public class TagsManager : ITagsService
    {
        private readonly NoteAppDbContext _context;

        public TagsManager(NoteAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag> GetByIdAsync(int entityId)
        {
            return await _context.Tags.FirstOrDefaultAsync(tag => tag.Id == entityId);
        }

        public async Task<Tag> GetByNameAsync(string tagName)
        {
            return await _context.Tags.FirstOrDefaultAsync(tag => tag.Name.ToLower() == tagName.ToLower());
        }

        public async Task<bool> CreateAsync(Tag entity)
        {
            if (entity == null) return false;

            await _context.Tags.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Tag entity)
        {
            if (entity == null) return false;

            _context.Tags.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Tag entity)
        {
            if (entity == null) return false;

            _context.Tags.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}