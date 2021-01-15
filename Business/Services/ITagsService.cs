using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Business.Services
{
    public interface ITagsService : IBaseService<Tag>
    {
        Task<Tag> GetByNameAsync(string tagName);
    }
}