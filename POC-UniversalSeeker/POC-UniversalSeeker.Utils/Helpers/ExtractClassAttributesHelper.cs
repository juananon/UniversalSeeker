using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POC_UniversalSeeker.Utils.Helpers
{
    public class ExtractClassAttributesHelper
    {
        public static string GetTableNameEntity<T>()
        {
            var type = typeof(T);
            var nameTable = type.GetCustomAttributes(typeof(TableAttribute), true)
                .FirstOrDefault() as TableAttribute;

            return nameTable?.Name ?? type.Name;
        }

        public static string GetColumnFromEntity<T>(Expression<Func<T, object>> propertyEntity, bool checkPrimaryKey = false)
        {
            var type = typeof(T);
            var property = type.GetProperty(GetFullPropertyName(propertyEntity));

            if (CheckNotMapperProperty<T>(property)) return string.Empty;
            if (CheckKeyProperty<T>(property, checkPrimaryKey)) return string.Empty;

            var nameColumn = property.GetCustomAttributes(typeof(ColumnAttribute), false)
                .FirstOrDefault() as ColumnAttribute;

            return nameColumn?.Name ?? property.Name;
        }

        public static string GetColumnFromEntity<T>(string columnName, bool checkPrimaryKey = false)
        {
            var type = typeof(T);
            var property = type.GetProperty(columnName);

            if (CheckNotMapperProperty<T>(property)) return string.Empty;
            if (CheckKeyProperty<T>(property, checkPrimaryKey)) return string.Empty;

            var nameColumn = property.GetCustomAttributes(typeof(ColumnAttribute), false)
                .FirstOrDefault() as ColumnAttribute;

            return nameColumn?.Name ?? property.Name;
        }

        public static IDictionary<string, object> GetAllColumnsFromEntity<T>(T entity, bool checkPrimaryKey = false)
        {
            var properties = new Dictionary<string, object>();
            var type = typeof(T);

            foreach (var infoMiembro in type.GetMembers().Where(x => x.MemberType == MemberTypes.Property))
            {
                var propiedad = (PropertyInfo)infoMiembro;
                var nameProperty = GetColumnFromEntity<T>(propiedad.Name, checkPrimaryKey);

                if (!string.IsNullOrEmpty(nameProperty))
                    properties.Add(nameProperty, propiedad.GetValue(entity, null));
            }

            return properties;
        }

        public static IList<string> GetKeyPrimary<T>()
        {
            var keyPrimary = new List<string>();
            var type = typeof(T);

            foreach (var infoMiembro in type.GetMembers().Where(x => x.MemberType == MemberTypes.Property))
            {
                var propiedad = (PropertyInfo)infoMiembro;
                if (CheckKeyProperty<T>(propiedad, true))
                {
                    keyPrimary.Add(GetColumnFromEntity<T>(propiedad.Name));
                }
            }

            return keyPrimary;
        }

        public static IDictionary<string, PropertyInfo> GetAllColumnsFromClass<T>(bool checkPrimaryKey = false)
        {
            var properties = new Dictionary<string, PropertyInfo>();
            var type = typeof(T);

            foreach (var infoMiembro in type.GetMembers().Where(x => x.MemberType == MemberTypes.Property))
            {
                var propiedad = (PropertyInfo)infoMiembro;
                var nameProperty = GetColumnFromEntity<T>(propiedad.Name, checkPrimaryKey);

                if (!string.IsNullOrEmpty(nameProperty))
                    properties.Add(nameProperty, propiedad);
            }

            return properties;
        }

        #region Métodos privados

        private static bool CheckNotMapperProperty<T>(PropertyInfo property)
        {
            var columnNotMapped = property.GetCustomAttributes(typeof(NotMappedAttribute), false)
                .FirstOrDefault() as NotMappedAttribute;

            return columnNotMapped != null;
        }

        private static bool CheckKeyProperty<T>(PropertyInfo property, bool checkPrimaryKey)
        {
            if (!checkPrimaryKey) return false;

            var columnNotMapped = property.GetCustomAttributes(typeof(KeyAttribute), false)
                .FirstOrDefault() as KeyAttribute;

            return columnNotMapped != null;
        }

        private static string GetFullPropertyName<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsConversion(Expression exp)
        {
            return (
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked
            );
        }

        #endregion
    }
}
