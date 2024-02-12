using Betty.Api.Infrastructure.Data.Attributes;
using Betty.Api.Infrastructure.Utils;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Linq.Expressions;

namespace Betty.Api.Infrastructure.Data
{
    public class CommandGenericHandler<T> : GenericHandler<T>
        where T : class, new()
    {
        private string _connectionstring;

        public CommandGenericHandler(string connectionstring)
        {
            _connectionstring = connectionstring;
            base.Initialize();
        }


        /// <summary>
        /// Build an command using the class and its properties and then execute the command
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Number of rows affected</returns>
        public async Task<int> RunInsert(T model)
        {

            List<Type> attributesToIgnore = new List<Type>() { typeof(SqlIgnoreInsertAttribute), typeof(SqlPrimaryKeyAttribute) };

            ReflectionUtils.GetNamesAndValuesFromObject(model, out IEnumerable<string> fieldNames, out IEnumerable<string?> fieldValues, attributesToIgnore);

            var query =
                $"INSERT INTO {_tableName}(\n" +
                $"{string.Join(",\n", fieldNames.Select(f => $"{f}"))})\n\n" +
                $"VALUES ({string.Join(",\n", fieldValues)})\n" +
                $"{""};";

            //var sql = new SQLGenericHandler(_connectionstring);
            //return await sql.ExecuteMySqlCommandAsync(query);
            //Task<int> task_response;

            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                connection.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand(query, connection))
                {
                    return await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        private string BuildQuery(int rows = 1000, bool executeQuery = true, bool isInternalQuery = false)
        {
            if (wheres.Count > 0)
                _where = BuildWhere(wheres);

            return
                $"SELECT \n" +
                //  If its internal query, {HYPOTHETICAL FIELDS PENDING} will be replaced by the fields called in the expression
                $"{(isInternalQuery ? "{HYPOTHETICAL FIELDS PENDING}" : _fields)} \n" +
                $"FROM {_tableName}\n" +
                $"{(string.IsNullOrEmpty(_where) ? "" : $"WHERE {_where} ")} \n" +
                $"{(executeQuery && rows > -1 ? $"LIMIT {rows}" : "")}";
        }

        public async Task<Dictionary<string, object>[]> RunQuery<R, Result>(Expression<Func<R, Result>> selectExpression, int rows = 1000)
        {

            var dictionary = Array.Empty<Dictionary<string, object>>();
            
            var fieldsQuery = ReflectionUtils.GetMembersFromExpression(selectExpression).ToArray();

            _fields = "";   //  Crear fields previously set in contructor
            for (int i = 0; i < fieldsQuery.Length; i++)
                _fields += (i > 0 ? ",\n" : "") + $"{fieldsQuery[i]}";

            var query = BuildQuery(rows, true, false);

            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                connection.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand(query, connection))
                {
                    MySqlDataAdapter adapter = new(sqlCommand);
                    GenericMapper mapper = new();
                    dictionary = await mapper.MapObjectsFromQueryAsync(adapter, _tableName);

                }
                connection.Close();
            }

            return dictionary;
        }

        public async Task<SqlResultCollection<T>> RunQuery(int rows = 1000, bool executeQuery = true, bool isInternalQuery = false)
        {

            var modelList = new T[] { };

            var query = BuildQuery(rows, executeQuery, isInternalQuery);

            //  If it's internal query, is not necesary to execute in this call
            if (isInternalQuery || !executeQuery)
                return new SqlResultCollection<T>(new List<T>() { }) { Query = query };


            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                connection.Open();
                using(MySqlCommand sqlCommand = new MySqlCommand(query, connection))
                {
                    MySqlDataAdapter adapter = new(sqlCommand);
                    GenericMapper mapper = new();
                    modelList = await mapper.MapObjectsFromQueryAsync<T>(modelList, adapter, _tableName, _type.GetProperties().Where(p => !p.CustomAttributes.Where(a => a.AttributeType == typeof(SqlIgnoreAttribute)).Any()).ToArray());

                }
                connection.Close();
            }

            var result = new SqlResultCollection<T>(modelList.AsEnumerable());
            result.Query = query;

            return result;
        }

        public async Task<int> RunDelete()
        {
            if (wheres.Count > 0)
                _where = BuildWhere(wheres);

            var query =
                $"Delete from {_tableName} \n" +
                $"WHERE {_where} ";

            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                connection.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand(query, connection))
                {
                    return await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> RunUpdate(T model)
        {

            var attributesToIgnore = new List<Type>() { typeof(SqlIgnoreUpdateAttribute), typeof(SqlPrimaryKeyAttribute) };



            ReflectionUtils.GetNamesAndValuesFromObject(model, out IEnumerable<string> fieldNames, out IEnumerable<string?> fieldValues, attributesToIgnore);



            string sets = string.Join(", \n", fieldNames.Zip(fieldValues, (name, value) => $"{name}={value}"));

            var query =
                $"Update {_tableName} \n" +
                $"SET {string.Join("\n,", sets)} \n" +
                $"WHERE {_where} ";

            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                connection.Open();
                using (MySqlCommand sqlCommand = new MySqlCommand(query, connection))
                {
                    return await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
