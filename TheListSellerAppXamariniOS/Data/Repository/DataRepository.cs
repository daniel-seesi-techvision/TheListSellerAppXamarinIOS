using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TheListSellerAppXamariniOS.Data.Database;
using System.Linq;

namespace TheListSellerAppXamariniOS.Data.Repository
{
    public class DataRepository<T> : IDataRepository<T> where T : new()
    {
        protected AppDbContext _context;

        public DataRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Get
        public async Task<List<T>> FindAllAsync()
        {
            var results = await Task.FromResult(_context._database.Table<T>().ToList());
            return results;
        }

        public List<T> FindAll()
        {
            var results = _context._database.Table<T>().ToList();
            return results;
        }

        public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            var results = await Task.FromResult(_context._database.Table<T>().Where(expression).ToList());
            return results;
        }

        public List<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _context._database.Table<T>().Where(expression).ToList();
        }

        public async Task<T> FindSingleByConditionAsync(Expression<Func<T, bool>> expression)
        {
            var results = await Task.FromResult(_context._database.Table<T>().FirstOrDefault(expression));
            return results;
        }

        public Task<T> FindFirstOrDefaultAsync()
        {
            var results = FindAll().ToList();
            return Task.FromResult(results.FirstOrDefault());
        }

        public async Task<int> CountAsync()
        {
            var results = await Task.FromResult(_context._database.Table<T>().ToList());
            return results.Count;
        }
        #endregion

        #region Create
        public async Task<int> CreateAsync(T entity)
        {
            var results = await Task.FromResult(_context._database.Insert(entity));
            return results;
        }

        public async Task<int> CreateMultipleAsync(IEnumerable<T> entities)
        {
            var results = await Task.FromResult(_context._database.InsertAll(entities));
            return results;
        }
        #endregion

        #region Update
        public async Task<int> UpdateAsync(T entity)
        {
            var results = await Task.FromResult(_context._database.Update(entity));
            return results;
        }
        #endregion

        #region Delete
        public async Task<int> DeleteAsync(T entity)
        {
            var results = await Task.FromResult(_context._database.Delete(entity));
            return results;
        }
        #endregion

        public async Task TearDownAndRecreateAsync()
        {
            int count = await CountAsync();

            if (count > 0)
            {
                _context._database.DropTable<T>();
                _context._database.CreateTable<T>();
            }
        }
    }
}
