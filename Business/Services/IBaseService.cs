using System.Collections.Generic;
using System.Threading.Tasks;
using MyNotesConsoleApp.Data.Entities;

namespace MyNotesConsoleApp.Business.Services
{
    public interface IBaseService<TEntity> where TEntity: IBaseEntity, new()
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int entityId);
        Task<bool> CreateAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
    }
}