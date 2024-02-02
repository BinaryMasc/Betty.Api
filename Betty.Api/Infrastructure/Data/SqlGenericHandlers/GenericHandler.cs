using Betty.Api.Infrastructure.Utils;
using System.Linq.Expressions;

namespace Betty.Api.Infrastructure.Data
{

#pragma warning disable CS8604 // Posible argumento de referencia nulo
    /// <summary>
    /// Generic abstract class that provide wheres methods for manage sql commands and querys
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericHandler<T> where T : class, new()
    {
        protected string _tableName;
        protected string _fields;

        protected Type _type;
        protected string _where;

        //protected Dictionary<string, object?> _dataFields;
        protected List<WhereClause> wheres = new List<WhereClause>();

        protected void Initialize()
        {
            

            _type = typeof(T);
            _tableName = _type.Name;
            _where = "";


            var properties = _type.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                _fields += (i > 0 ? ",\n" : "") + $"{properties[i].Name}";
            }
        }

        public string BuildWhere(List<WhereClause> WhereList)
        {
            string whereClause = _where + " ";

            foreach (var where in WhereList)
            {
                if (string.IsNullOrEmpty(whereClause))
                    whereClause += where.FieldName + " " + where.Operator + " " + (where.Type == typeof(string) ? $"'{where.Right.ToString()}'" : where.Right.ToString());

                else if (where.InnerWhere != null)
                    whereClause +=
                        (string.IsNullOrEmpty(whereClause) ? "" : where.LogicOperator) + " (" + BuildWhere(where.InnerWhere) + ") ";

                else
                    whereClause += " " + where.LogicOperator + " " + where.FieldName + " " + where.Operator + " " + (where.Type == typeof(string) ? $"'{where.Right.ToString()}'" : where.Right.ToString());
            }

            return whereClause;
        }

        public GenericHandler<T> Where(Expression<Func<T, bool>> expression)
        {
            string sqlWhere = "";
            ReflectionUtils.ExpressionToString<T>(expression, ref sqlWhere);

            _where += sqlWhere;

            return this;
        }



        public class WhereClause
        {
            public WhereClause(string fieldname, object right, Type type, string @operator)
            {
                FieldName = fieldname;
                Right = right;
                Type = type;
                Operator = @operator;
            }

            public List<WhereClause> InnerWhere { get; set; }

            public object Left { get; private set; }
            public string FieldName { get; private set; }
            public object Right { get; private set; }
            public Type Type { get; private set; }
            public string Operator { get; private set; }
            public string LogicOperator { get; set; } = "AND";

        }

        protected void ValidateFieldName(string name)
        {
            const string chars = "abcdefghijklmnoprstuvwxyz1234567890.[]";

            foreach (var nameChar in name)
            {
                if (!chars.Contains(nameChar, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Column name '{name}' is invalid.");
            }
        }

        protected string ValidateStringSecurity(string value)
        {
            if (!value.Contains('\'')) return value;

            else return value.Replace("'", "''");
        }

    }

#pragma warning disable CS8604 // Posible argumento de referencia nulo
}
