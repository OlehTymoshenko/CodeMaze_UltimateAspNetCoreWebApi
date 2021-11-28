using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Repository.Extensions.Utility
{
    public static class OrderQueryBuilder
    {

        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            if(orderByQueryString is null)
            {
                return string.Empty;
            }

            var orderParams = orderByQueryString.Trim()
                .Split(",", options: StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim());
            
            var geneticTypeProperties = typeof(T).GetProperties(BindingFlags.Public |
                BindingFlags.Instance);
            
            StringBuilder orderByQueryBuilder = new();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param)) continue;

                var propertyFromQueryName = param.Split(' ', options: StringSplitOptions.RemoveEmptyEntries)
                    ?[0];

                var objectProperty = geneticTypeProperties.FirstOrDefault(pi =>
                    pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty is null) continue;

                var orderingDirection = param.EndsWith(" desc") ? "descending" : "ascending";

                orderByQueryBuilder.Append($"{objectProperty.Name} {orderingDirection}, ");
            }

            var finalOderQuery = orderByQueryBuilder.ToString().Trim(' ', ',');

            return finalOderQuery;
        }

    }
}
