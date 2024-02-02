using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace Betty.Api.Infrastructure.Data
{
    public class SQLGenericHandler
    {
        private string _connectionstring;
        public SQLGenericHandler(string connectonstring)
        {
            _connectionstring = connectonstring;
        }

        public Task<int> ExecuteMySqlCommandAsync(string command)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                connection.Open();
                MySqlCommand sqlCommand = new MySqlCommand(command, connection);

                return sqlCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<MySqlDataAdapter> GetDataFromMySqlQueryAsync(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                await connection.OpenAsync();
                MySqlCommand sqlCommand = new MySqlCommand(query, connection);

                MySqlDataAdapter adapter = new(sqlCommand);

                return adapter;
            }
        }

        public MySqlDataAdapter GetDataFromSqlQuery(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionstring))
            {
                connection.Open();
                MySqlCommand sqlCommand = new MySqlCommand(query, connection);

                MySqlDataAdapter adapter = new(sqlCommand);

                return adapter;
            }
        }
    }
}
