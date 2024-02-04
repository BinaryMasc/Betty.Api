using Betty.Api.Infrastructure.Data.Attributes;
using System.Linq.Expressions;
using System.Reflection;

#pragma warning disable CS8601 // Desreferencia de una referencia posiblemente NULL.
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
#pragma warning disable CS8603 // Desreferencia de una referencia posiblemente NULL.
#pragma warning disable CS8604 // Desreferencia de una referencia posiblemente NULL.
#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
namespace Betty.Api.Infrastructure.Utils
{
    public static class ReflectionUtils
    {
        public static void ExpressionToString<T>(Expression<Func<T, bool>> expression, ref string strExpression)
        {
            dynamic body = expression.Body;
            Type typeBody = expression.Body.GetType();

            ;

            switch (typeBody.Name)
            {
                case "MethodBinaryExpression":
                case "BinaryExpression":
                    strExpression += "(" + body.Left.Member.Name + " ";
                    strExpression += GetNodeTypeString(body.Method.Name);  //  Operator
                    strExpression += " " + GetRightOperandingExpression(body.Right).ToString();
                    strExpression += ")";
                    break;
                case "LogicalBinaryExpression":
                    Type typeLeft = body.Left.GetType();
                    Type typeRight = body.Right.GetType();
                    if (typeLeft.Name == "PropertyExpression")
                        strExpression += body.Left.Member.Name;
                    else if (typeLeft.Name == "LogicalBinaryExpression")
                        LogicalBinaryExpressionToString(expression, ref strExpression);
                    else ExpressionToString(body.Left, ref strExpression);

                    strExpression += GetNodeTypeString(body.NodeType.ToString());

                    if (typeRight.Name == "ConstantExpression")
                        strExpression += body.Right;
                    else if (typeRight.Name == "FieldExpression" || typeRight.Name == "PropertyExpression")
                    {
                        var fieldName = body.Right.Member.Name;
                        var dictionaryObject = typeRight.Name == "FieldExpression" ? ConvertObjectRuntimeFieldsToDictionary(body.Right.Expression.Value) : ConvertObjectRuntimeFieldsToDictionary(body.Right.Expression.Expression.Value);

                        WriteValue(ref strExpression, (Type)body.Right.Type, dictionaryObject[fieldName]);
                    }
                    else ExpressionToString(body.Right, ref strExpression);
                    break;
                case "PropertyExpression":
                    //  Assuming the property is bool in C# and bit in sql
                    strExpression += body.Member.Name + "=1 ";
                    break;
            }
        }

        private static void WriteValue(ref string strExpression, Type type, object obj)
        {
            strExpression += SanitizeValue(type, obj);
        }

        private static string SanitizeValue(Type type, object obj)
        {
            string result = "";
            if (type == typeof(string))
                result = $"'{obj}'";

            else if (type == typeof(DateTime))
                result = $"'{((DateTime)obj).ToString("u").Split('Z')[0]}'";

            else if (type == typeof(bool))
                result += (bool)obj ? '1' : '0';

            else result += obj;

            return result;
        }

        private static string GetNodeTypeString(string nodeType)
        {
            switch (nodeType)
            {
                case "op_Equality": return "=";
                case "Equal": return "=";
                case "NotEqual": return "<>";
                case "AndAlso": return " AND ";
                case "LessThan": return "<";
                case "GreaterThan": return ">";
                case "LessThanOrEqual": return "<=";
                case "GreaterThanOrEqual": return ">=";
                case "OrElse": return " OR ";
                default: return "";
            };
        }

        private static void ExpressionToString(BinaryExpression expression, ref string strExpression)
        {
            dynamic body = expression;
            strExpression += "(" + body.Left.Member.Name + " ";
            strExpression += GetNodeTypeString(body.Method?.Name ?? expression.NodeType.ToString());  //  Operator
            strExpression += " " + GetRightOperandingExpression(body.Right).ToString();
            strExpression += ")";
        }

        //  LogicalBinaryExpression
        private static void LogicalBinaryExpressionToString(dynamic expression, ref string strExpression)
        {
            dynamic body;
            try { body = expression.Body; } catch { body = expression; }
            Type typeLeft = body.Left.GetType();
            Type typeRight = body.Right.GetType();

            if (typeLeft.Name == "PropertyExpression")
                strExpression += "(" + body.Left.Member.Name;
            else
            {
                if (body.Left.GetType().Name == "LogicalBinaryExpression")
                    LogicalBinaryExpressionToString(body.Left, ref strExpression);
                
                else ExpressionToString(body.Left, ref strExpression);
            }
            strExpression += GetNodeTypeString(body.NodeType.ToString());
            if (typeRight.Name == "ConstantExpression")
                strExpression += body.Right + ")";
            else ExpressionToString(body.Right, ref strExpression);
        }

        private static void ExpressionToString(dynamic expression, ref string strExpression)
        {


            dynamic body = expression;
            try { strExpression += "(" + body.Left.Member.Name + " "; }
            catch (Exception) { strExpression += "(" + body.Member.Name + " "; }
            ;
            if (body.GetType().Name != "PropertyExpression")
            {
                strExpression += GetNodeTypeString(body.Method.Name);  //  Operator
                strExpression += " " + GetRightOperandingExpression(body.Right).ToString();
                strExpression += ")";
            }
            else strExpression += "=1) ";
            
            
        }


        private static object GetRightOperandingExpression(MemberExpression member)
        {
            try
            {
                ConstantExpression din3 = (ConstantExpression)member.Expression;
                var memberName = member.Member.Name;
                var members = din3?.Value?.GetType().GetFields() ?? throw new NullReferenceException("The expression has no fields.");
                var dictionary = members.ToDictionary(property => property.Name, property => property.GetValue(din3.Value)).Where(name => name.Key == memberName);


                return SanitizeValue(dictionary.First().Value.GetType(), dictionary.First().Value);

            }
            //  Catch if its a FieldExpression
            catch(InvalidCastException)
            {
                var expresion2 = (MemberExpression)member.Expression;

                string fieldName = member.Member.Name;
                string objectname = expresion2.Member.Name;
                var propertiesReflected = member.Member.ReflectedType.GetProperties();

                var typeRight = member.GetType();
                var dictionaryObject = typeRight.Name == "FieldExpression" ? ConvertObjectRuntimeFieldsToDictionary(((dynamic)member.Expression).Value) : ConvertObjectRuntimeFieldsToDictionary(((dynamic)member.Expression).Expression.Value);


                return SanitizeValue(dictionaryObject[fieldName].GetType(), dictionaryObject[fieldName]);
                //return dict2[fieldName].GetType() == typeof(string) ? $"'{dict2[fieldName]}'" : dict2[fieldName];

            }
        }



        private static object GetRightOperandingExpression(ConstantExpression member)
        {
            return SanitizeValue(member?.Value?.GetType(), member.Value);
        }

        //  Called on insert command
        public static void GetNamesAndValuesFromObject<T>(T model, out IEnumerable<string> fieldNames, out IEnumerable<string?> fieldValues, IEnumerable<Type> AditionalAttributesToIgnore)
        {
            var modelReflection = model?.GetType();
            var properties = modelReflection?.GetProperties()
                .Where(p => !p.CustomAttributes.Where(a => a.AttributeType == typeof(SqlIgnoreAttribute) || AditionalAttributesToIgnore.Where(aditionalAtt => aditionalAtt == a.AttributeType).Any()).Any()) ?? 
                throw new NullReferenceException("Null reference in model used for reflection.");

            fieldNames = properties.Select(p => p.Name);
            fieldValues = properties
                .Select(p => p.GetValue(model))
                .Select(v => v.GetType() == typeof(string)  ? $"'{v.ToString().Replace("'", "''")}'" : 
                v.GetType() == typeof(bool) ? (bool)v == true ? "1" : "0" :
                v.GetType() == typeof(DateTime) ? $"'{((DateTime)v).ToString("u").Split('Z')[0]}'" : v.ToString());
        }

        static Dictionary<string, object> ConvertObjectRuntimePropertiesToDictionary(object obj)
        {
            var dictionary = new Dictionary<string, object>();

            // Obtenemos todas las propiedades del objeto
            PropertyInfo[] properties = obj.GetType().GetRuntimeProperties().ToArray();

            // Recorremos las propiedades y las añadimos al diccionario
            foreach (var property in properties)
            {
                var type = property.PropertyType;
                var isClass = !type.IsPrimitive && type != typeof(string) && type != typeof(DateTime);

                if (isClass)
                {
                    var value = property.GetValue(obj);
                    if (value is null) continue;
                    var auxDictionary = type.GetRuntimeProperties().Any() ? ConvertObjectRuntimePropertiesToDictionary(value) : ConvertObjectRuntimeFieldsToDictionary(value);
                    foreach (var pair in auxDictionary)
                        dictionary[pair.Key] = pair.Value;
                }
                else
                    dictionary[property.Name] = property.GetValue(obj);
            }

            return dictionary;
        }

        static Dictionary<string, object> ConvertObjectRuntimeFieldsToDictionary(object obj)
        {
            var dictionary = new Dictionary<string, object>();

            // Obtenemos todas las propiedades del objeto
            IEnumerable<FieldInfo> fields = obj.GetType().GetRuntimeFields();

            // Recorremos las propiedades y las añadimos al diccionario
            foreach (var field in fields)
            {
                var type = field.FieldType;
                var isClass = !type.IsPrimitive && type != typeof(string) && type != typeof(DateTime);

                if(isClass)
                {
                    var value = field.GetValue(obj);
                    if (value is null) continue;
                    var auxDictionary = type.GetRuntimeProperties().Any() ? ConvertObjectRuntimePropertiesToDictionary(value) : ConvertObjectRuntimeFieldsToDictionary(value);
                    foreach (var pair in auxDictionary)
                        dictionary[pair.Key] = pair.Value;
                }
                else
                    dictionary[field.Name] = field.GetValue(obj);
            }

            return dictionary;
        }


        public static Dictionary<string, object> ConvertToDictionary(object model, Func<PropertyInfo, bool> where = null)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var dictionary = new Dictionary<string, object>();
            IEnumerable<PropertyInfo> properties;

            if (where is null)
                properties = model.GetType().GetProperties();
            else
                properties = model.GetType().GetProperties().Where(where);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(model);
                if (value != null)
                {
                    dictionary.Add(property.Name, value);
                }
            }

            return dictionary;
        }
    }
}
#pragma warning restore CS8601 // Desreferencia de una referencia posiblemente NULL.
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
#pragma warning restore CS8603 // Desreferencia de una referencia posiblemente NULL.
#pragma warning restore CS8604 // Desreferencia de una referencia posiblemente NULL.
#pragma warning restore CS8600 // Desreferencia de una referencia posiblemente NULL.
