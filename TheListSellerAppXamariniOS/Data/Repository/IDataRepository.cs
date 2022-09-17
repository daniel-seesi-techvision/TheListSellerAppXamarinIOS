using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TheListSellerAppXamariniOS.Data.Repository
{
    public interface IDataRepository<T> where T : new()
    {
        Task<IEnumerable<T>> FindAllAsync();
        IEnumerable<T> FindAll();
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T> FindSingleByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T> FindFirstOrDefaultAsync();
        Task<int> CreateAsync(T entity);
        Task<int> CreateMultipleAsync(IEnumerable<T> entities);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<int> CountAsync();
        Task TearDownAndRecreateAsync();
    }
}