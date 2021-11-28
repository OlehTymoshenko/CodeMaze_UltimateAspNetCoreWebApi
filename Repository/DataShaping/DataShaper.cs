using Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Repository.DataShaping
{
    public class DataShaper<T> : IDataShaper<T> where T : class 
    {
        public PropertyInfo[] TypeProperties { get; private set; }

        public DataShaper()
        {
            TypeProperties = typeof(T).GetProperties(BindingFlags.Public |
                BindingFlags.Instance);
        }


        public ExpandoObject ShapeData(T entity, string reqiredPropertiesString)
        {
            var requiredProperties = GetRequiredProperties(reqiredPropertiesString);

            return FetchData(entity, requiredProperties);
        }

        public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string reqiredPropertiesString)
        {
            List<ExpandoObject> shapedEntities = new();
            
            var requiredProperties = GetRequiredProperties(reqiredPropertiesString);

            foreach(var entity in entities)
            {
                shapedEntities.Add(FetchData(entity, requiredProperties));
            }

            return shapedEntities;
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string requiredPropertiesString)
        {
            if(string.IsNullOrWhiteSpace(requiredPropertiesString))
            {
                return TypeProperties;
            }

            var requiredProperties = new List<PropertyInfo>();
            var propertiesName = requiredPropertiesString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var propertyName in propertiesName)
            {
                var property = TypeProperties?.FirstOrDefault(pi => pi.Name.Equals(
                    propertyName.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (property is null) continue;

                requiredProperties.Add(property);
            }

            return requiredProperties;
        }

        private ExpandoObject FetchData(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            ExpandoObject shapedEntity = new();

            foreach (var property in requiredProperties)
            {
                var propertyValue = property.GetValue(entity);
                shapedEntity.TryAdd(property.Name, propertyValue);
            }

            return shapedEntity;
        }

    }
}
