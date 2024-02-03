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

                //if (ds.Tables[0].Rows.Count < 1)
                //    throw new DataNotFoundException("The query didn't return data.");


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


    }
}
#pragma warning restore CS8604 // Posible argumento de referencia nulo
