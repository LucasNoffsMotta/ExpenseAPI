using System.Linq.Expressions;
using UnitTests_ExpenseAPI.Models;

namespace UnitTests_ExpenseAPI.Repo
{
    public interface IBaseRepo<T>
    {
        public Task<T?> GetByID(int id);
        public Task<List<T>> GetAll(Expression<Func<T, bool>>? where = null, params string[] includes);

        public Task<T> Create(T dto);

        public Task<bool> Update(T dto);

        public Task<bool> Delete(int id);

        public IQueryable<T> ApplyIncludes(IQueryable<T> query, params string[] includes);
    }
}
