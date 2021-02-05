using System.Threading.Tasks;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Business.Services
{
    public interface INoteTagsService
    {

        Task<bool> CreateAsync(NoteTag noteTag);
    }
}