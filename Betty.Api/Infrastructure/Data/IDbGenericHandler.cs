using System.Linq.Expressions;

namespace Betty.Api.Infrastructure.Data
{
    public interface IDbGenericHandler : ICloneable
    {
        public Task<IEnumerable<T>> Query<T>(Expression<Func<T, bool>> where, int rows = 1000) where T : class, new();
        public Task<IEnumerable<T>> QueryDictionary<T>() where T : class, new();
        public Task<IEnumerable<T>> Update<T>(T model, Expression<Func<T, bool>> where) where T : notnull, new();
        public Task<int> Insert<T>(T model) where T : class, new();
        public Task<int> Delete<T>(Expression<Func<T, bool>> where, int rows = 1000) where T : class, new();

    }
}
