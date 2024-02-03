using Betty.Api.Infrastructure.Utils;
using System.Linq.Expressions;

namespace Betty.Api.Infrastructure.Data
{
    public class DbGenericHandler : IDbGenericHandler
    {
        private string _connectionstring;
        public DbGenericHandler(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public Task<IEnumerable<T>> Query<T>(Expression<Func<T,bool>> where, int rows = 1000) where T : class, new()
        {
            var query = new CommandGenericHandler<T>(_connectionstring);
            query.Where(where);

            return query.RunQuery(rows);
        }

        public Task<int> Insert<T>(T model) where T : class, new()
        {
            return new CommandGenericHandler<T>(_connectionstring).RunInsert(model);
        }


        public Task<int> Update<T>(T model, Expression<Func<T, bool>> where) where T : class, new()
        {
            var query = new CommandGenericHandler<T>(_connectionstring);
            query.Where(where);

            return query.RunUpdate(model);
        }

        public Task<int> Delete<T>(Expression<Func<T, bool>> where, int rows = 1000) where T : class, new()
        {
            var query = new CommandGenericHandler<T>(_connectionstring);
            query.Where(where);

            return query.RunDelete();
        }

        public object Clone() => new DbGenericHandler(_connectionstring);

        public async Task<IEnumerable<T>> QueryDictionary<T>() where T : class, new()
        {
            var query = new CommandGenericHandler<T>(_connectionstring);
            

            return await query.RunQuery(1000);
        }
    }
}
