using POC_UniversalSeeker.Entities.Shared;
using POC_UniversalSeeker.Services.Shared.Interfaces;
using POC_UniversalSeeker.Utils.Attributes;
using POC_UniversalSeeker.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POC_UniversalSeeker.Services.Shared
{
    public class SeekerService : ISeekerService
    {
        public IEnumerable<Filter> GetFilters<T>(string navigationProperties) where T : class
        {
            List<Filter> filters = new List<Filter>();
            IDictionary<string, PropertyInfo> columns = ExtractClassAttributesHelper.GetAllColumnsFromClass<T>();
            foreach (var column in columns)
            {
                var attribute = column.Value.GetCustomAttributes<FilterAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    if (column.Value.PropertyType.FullName.Contains("POC_UniversalSeeker.Entities"))
                    {

                        // Si la propiedad compleja tiene descripción consiederamos que se ha de filtrar por la entidad
                        if (attribute.Description != string.Empty)
                        {
                            var propertyName = string.Empty;
                            if (navigationProperties == string.Empty)
                                propertyName = column.Value.Name;
                            else
                                propertyName = navigationProperties + "." + column.Value.Name;

                            filters.Add(new Filter
                            {
                                Name = propertyName,
                                Description = attribute.Description,
                                Type = column.Value.PropertyType,
                                Property = column.Value,
                                IsEntity = true
                            });

                        }

                        if (navigationProperties == string.Empty)
                            navigationProperties = column.Value.Name;
                        else
                            navigationProperties = navigationProperties + string.Empty + column.Value.Name;

                        Type ex = typeof(SeekerService);
                        MethodInfo mi = ex.GetMethod("GetFilters"); // En el caso de que sea una propiedad compleja empleamos la recursividad utilizando un tipo genérico.
                        Type type = column.Value.PropertyType;
                        MethodInfo miConstructed = mi.MakeGenericMethod(type);
                        object[] obj = new object[1];
                        obj[0] = navigationProperties;
                        Object result = miConstructed.Invoke(this, obj);
                        filters.AddRange((List<Filter>)result);

                        navigationProperties = string.Empty;
                    }
                    else
                    {
                        var propertyName = string.Empty;
                        if (navigationProperties == string.Empty)
                            propertyName = column.Value.Name;
                        else
                            propertyName = navigationProperties + "." + column.Value.Name;

                        filters.Add(new Filter
                        {
                            Name = propertyName,
                            Description = attribute.Description,
                            Type = column.Value.PropertyType,
                            Property = column.Value,
                            IsEntity = false
                        });
                    }
                }
            }
            return filters;
        }
    }
}
