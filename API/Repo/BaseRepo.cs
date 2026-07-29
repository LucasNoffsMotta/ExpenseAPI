using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI.Repo
{
    public class BaseRepo<T> : IBaseRepo<T> where T : BaseModel
    {
        private AppDbContext _dbContext;

        public BaseRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> ApplyIncludes(IQueryable<T> query, params string[] includes)
        {
            return includes.Aggregate(query, (current, include) => current.Include(include));
        }

        public async Task<T> Create(T model)
        {
            await _dbContext.Set<T>().AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return model;
        }

        public async Task<bool> Delete(int id)
        {
            var model = await _dbContext.Set<T>().FindAsync(id);
            if (model == null) return false;

            _dbContext.Set<T>().Remove(model);
            int changes = await _dbContext.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? where = null, params string[] includes)
        {
            var query = ApplyIncludes(_dbContext.Set<T>(), includes);

            if (where != null)
            {
                query = query.Where(where);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByID(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<bool> Update(T dto)
        {
            _dbContext.Entry(dto).State = EntityState.Modified;
            int changes = await _dbContext.SaveChangesAsync();
            return changes > 0;
        }
    }
}
