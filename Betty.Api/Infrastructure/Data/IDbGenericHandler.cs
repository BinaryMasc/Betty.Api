using System.Linq.Expressions;

namespace Betty.Api.Infrastructure.Data
{
    public interface IDbGenericHandler : ICloneable
    {
        /// <summary>
        /// This overload return limit of 1000 results
        /// </summary>
        /// <typeparam name="T">Type model</typeparam>
        /// <param name="where">Condition</param>
        /// <returns>Collection of models</returns>
        public Task<SqlResultCollection<T>> Query<T>(Expression<Func<T, bool>> where) where T : class, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type model</typeparam>
        /// <param name="where">Condition</param>
        /// <param name="selectLambda">Select members</param>
        /// <returns>Collection of models</returns>
        public Task<Dictionary<string, object>[]> Query<T, Result>(Expression<Func<T, bool>> where, Expression<Func<T, Result>> selectExpression, int rows = 1000) where T : class, new();
                
        public Task<SqlResultCollection<T>> Query<T>(Expression<Func<T, bool>> where, bool isInternalQuery = false, bool executeQuery = true, int rows = 1000) where T : class, new();
        
        public Task<IEnumerable<T>> QueryDictionary<T>() where T : class, new();
        
        public Task<int> Update<T>(T model, Expression<Func<T, bool>> where) where T : class, new();
        
        public Task<int> Insert<T>(T model) where T : class, new();
        
        public Task<int> Delete<T>(Expression<Func<T, bool>> where, int rows = 1) where T : class, new();

    }
}
