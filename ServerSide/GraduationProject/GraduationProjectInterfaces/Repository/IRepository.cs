using GraduationProjectModels;
using System.Linq;
using System.Threading.Tasks;

namespace GraduationProjectInterfaces.Repository
{
    // Universal repository to work with database.
    public interface IRepository<T>
        where T : class, IEntity<T>
    {
        IQueryable<T> Get();

        Task<T> GetAsync(long id);

        Task<T> EditAsync(T param);

        Task<T> AddAsync(T param);

        Task<T> RemoveAsync(long id);
    }
}
