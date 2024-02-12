using Betty.Api.Commons;
using Betty.Api.Infrastructure.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

#pragma warning disable CS8604 // Posible argumento de referencia nulo

namespace Betty.Api.Infrastructure.Data
{
    public class GenericMapper
    {
        
        public async Task<T[]> MapObjectsFromQueryAsync<T>(T[] modelT, MySqlDataAdapter adapter, string tableName, PropertyInfo[] properties)
            where T : new()
        {
            if (modelT == null)
                throw new ArgumentNullException("modelT");

            using (var ds = new DataSet())
            {
                await Task.Run(() =>
                {
                    adapter.Fill(ds, tableName);
                });


                modelT = new T[ds.Tables[0].Rows.Count];


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var tempT = new T();
                    foreach (var propertie in properties)
                    {
                        var memberName = propertie.Name;

                        if (ds.Tables[0].Columns.Contains(memberName))
                        {
                            var value = ds.Tables[0].Rows[i][memberName];
                            Helpers.SetProperty(tempT, memberName, value.GetType() != typeof(DBNull) ? value : null);
                        }
                    }
                    modelT[i] = tempT;
                }
                

            }

            return modelT;
        }

        public async Task<Dictionary<string, object>[]> MapObjectsFromQueryAsync(MySqlDataAdapter adapter, string tableName)
        {
            using (var ds = new DataSet())
            {
                await Task.Run(() =>
                {
                    adapter.Fill(ds, tableName);
                });

                var result = new Dictionary<string, object>[ds.Tables[0].Rows.Count];

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var row = ds.Tables[0].Rows[i];
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        dict[col.ColumnName] = row[col.ColumnName];
                    }

                    result[i] = dict;
                }

                return result;
            }
        }


    }
}
#pragma warning restore CS8604 
