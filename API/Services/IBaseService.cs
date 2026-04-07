using System.Linq.Expressions;

namespace UnitTests_ExpenseAPI.Services
{
    public interface IBaseService<T>
    {
        public Task<T?> GetByID(int id);
        public Task<List<T>> GetAll(Expression<Func<T, bool>>? where = null, params string[] includes);

        public Task<bool> Create(T dto);

        public Task<bool> Update(T dto);

        public Task<bool> Delete(int id);

        public IQueryable<T> ApplyIncludes(IQueryable<T> query, params string[] includes);
    }
}
