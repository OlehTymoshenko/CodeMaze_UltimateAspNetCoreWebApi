using Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Entities.Models;

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


        public ShapedEntity ShapeData(T entity, string reqiredPropertiesString)
        {
            var requiredProperties = GetRequiredProperties(reqiredPropertiesString);

            return FetchData(entity, requiredProperties);
        }

        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string reqiredPropertiesString)
        {
            List<ShapedEntity> shapedEntities = new();
            
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

        private ShapedEntity FetchData(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            ShapedEntity shapedEntity = new();

            foreach (var property in requiredProperties)
            {
                var propertyValue = property.GetValue(entity);
                shapedEntity.Entity.TryAdd(property.Name, propertyValue);
            }

            var idProperty = entity.GetType().GetProperty("Id");
            shapedEntity.Id = (Guid)idProperty.GetValue(entity);

            return shapedEntity;
        }

    }
}
