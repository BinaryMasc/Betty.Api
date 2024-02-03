using Betty.Api.Infrastructure.Data.Attributes;
using Betty.Api.Infrastructure.Utils;
using MySql.Data.MySqlClient;

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

        public async Task<IEnumerable<T>> RunQuery(int rows = 100)
        {

            var modelList = new T[] { };

            if (wheres.Count > 0)
                _where = BuildWhere(wheres);

            var query =
                $"SELECT \n" +
                $"{_fields} \n" +
                $"FROM {_tableName}\n" +
                $"{(string.IsNullOrEmpty(_where) ? "" : $"WHERE {_where} ")} \n" +
                $"LIMIT {rows}";


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
            return modelList.AsEnumerable();
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
