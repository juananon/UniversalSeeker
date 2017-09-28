using POC_UniversalSeeker.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POC_UniversalSeeker.Utils.Helpers
{
    public class ExpressionBuilderHelper
    {
        
        public static Expression<Func<T, bool>> CreateExpression<T>(string searchField, string searchString, OperatorEnum searchOper) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "p");

            Expression propertyAccess;
            if (!searchField.Contains("."))
                propertyAccess = Expression.Property(parameter, searchField);
            else
            {
                propertyAccess = parameter;
                foreach (var member in searchField.Split('.'))
                {
                    propertyAccess = Expression.PropertyOrField(propertyAccess, member);
                }
            }
            
            var obj = GetPropertyObject(propertyAccess.Type, searchString);

            var expression = GetExpression(searchOper, obj, propertyAccess);
            
            return (Expression<Func<T, bool>>)Expression.Lambda(expression, parameter);
        }

        private static Expression GetExpression(OperatorEnum oper, Object obj, Expression propertyAccess)
        {
            Expression expression;

            switch (oper)
            {
                case OperatorEnum.StartsWith:
                    expression = Expression.Call(propertyAccess, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(obj));
                    break;

                case OperatorEnum.Contains:

                    if (propertyAccess.Type == typeof(DateTime) || propertyAccess.Type == typeof(DateTime?))
                    {
                        Expression toStringCall = Expression.Call(
                            propertyAccess, "ToString",
                            null,
                            new[] { Expression.Constant("D") });

                        Expression containsCall = Expression.Call(
                            toStringCall, "Contains",
                            null,
                            new[] { Expression.Constant(obj) });

                        expression = containsCall;
                    }
                    else
                    {
                        expression = Expression.Call(propertyAccess, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(obj));
                    }
                    break;
                case OperatorEnum.EndsWith:
                    expression = Expression.Call(propertyAccess, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), Expression.Constant(obj));
                    break;
                case OperatorEnum.GreaterThan:
                    expression = Expression.GreaterThan(propertyAccess, Expression.Constant(obj, propertyAccess.Type));
                    break;
                case OperatorEnum.GreaterThanOrEqual:
                    expression = Expression.GreaterThanOrEqual(propertyAccess, Expression.Constant(obj, propertyAccess.Type));
                    break;
                case OperatorEnum.LessThan:
                    expression = Expression.LessThan(propertyAccess, Expression.Constant(obj, propertyAccess.Type));
                    break;
                //case OperatorEnum.LessThanOrEqual:
                //    expression = Expression.LessThanOrEqual(propertyAccess, Expression.Constant(searchString, propertyAccess.Type));
                //    break;
                case OperatorEnum.Equal:
                    expression = Expression.Equal(propertyAccess, Expression.Constant(obj, propertyAccess.Type));
                    break;
                case OperatorEnum.NotEqual:
                    expression = Expression.NotEqual(propertyAccess, Expression.Constant(obj, propertyAccess.Type));
                    break;
                default:
                    return null;
            }

            return expression;
        }

        private static Object GetPropertyObject(Type type, string searchString)
        {
            // TODO: Averiguar la forma de evaluar tipos nulables con el fin de no usar strings y poder usar el enum TypeCode. (ver método GetPropertyObjectNEW)
            object obj = new object();
            switch (type.ToString())
            {
                case "System.String":
                    obj = searchString;
                    break;

                case "System.DateTime":
                case "System.Nullable`1[System.DateTime]":
                    obj = DateTime.ParseExact(searchString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    break;
                case "System.Nullable`1[System.Int32]":
                case "System.Int":
                case "System.Int32":
                    obj = int.Parse(searchString);
                    break;
                case "System.Nullable`1[System.Decimal]":
                case "System.Decimal":
                    obj = decimal.Parse(searchString);
                    break;
                case "System.Double":
                    obj = double.Parse(searchString);
                    break;
                case "System.Boolean":
                    obj = searchString == "1";
                    break;
                case "System.Nullable`1[System.Single]":
                    obj = float.Parse(searchString);
                    break;
                default:
                    obj = searchString;
                    break;
            }

            return obj;
        }

        private static Object GetPropertyObjectNEW(Type type, string searchString)
        {
            object obj = new object();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    obj = searchString;
                    break;

                case TypeCode.DateTime:
                    obj = DateTime.ParseExact(searchString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    obj = int.Parse(searchString);
                    break;

                case TypeCode.Decimal:
                    obj = decimal.Parse(searchString);
                    break;
                case TypeCode.Double:
                    obj = double.Parse(searchString);
                    break;
                case TypeCode.Boolean:
                    obj = searchString == "1";
                    break;
                
                default:
                    obj = searchString;
                    break;
            }

            return obj;
        }
    }
}
