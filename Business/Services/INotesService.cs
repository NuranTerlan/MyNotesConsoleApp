using System.Collections.Generic;
using System.Threading.Tasks;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Business.Services
{
    public interface INotesService : IBaseService<Note>
    {
        Task<List<Note>> GetByTagNameAsync(string tagName);
    }
}