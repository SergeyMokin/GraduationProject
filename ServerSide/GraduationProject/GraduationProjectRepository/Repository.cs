using GraduationProjectInterfaces.Repository;
using GraduationProjectModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProjectRepositories
{
    // IRepository implementation to work with database.
    public class Repository<T> : IRepository<T>
        where T: class, IEntity<T>
    {
        private readonly GraduationProjectContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(GraduationProjectContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> AddAsync(T param)
        {
            param = param
              ?? throw new ArgumentNullException();

            await _dbSet.AddAsync(param);

            await _context.SaveChangesAsync();

            return param;
        }

        public async Task<T> EditAsync(T param)
        {
            param = param
                ?? throw new ArgumentNullException();

            var entity = await GetAsync(param.Id);

            entity.Edit(param);

            await _context.SaveChangesAsync();

            return entity;
        }

        public IQueryable<T> Get()
        {
            return _dbSet;
        }

        public async Task<T> GetAsync(long id)
        {
            return await _dbSet.FindAsync(id)
                ?? throw new ArgumentNullException();
        }

        public async Task<T> RemoveAsync(long id)
        {
            var entity = await GetAsync(id);

            _dbSet.Remove(entity);

            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
