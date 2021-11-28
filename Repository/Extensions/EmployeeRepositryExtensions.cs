using Entities.Models;
using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Dynamic.Core;
using Repository.Extensions.Utility;

namespace Repository.Extensions
{
    public static class EmployeeRepositryExtensions
    {
        public static IQueryable<Employee> FilterByAge(this IQueryable<Employee> employees,
            uint minAge, uint maxAge) =>
            employees.Where(e => e.Age >= minAge && e.Age <= maxAge);

        public static IQueryable<Employee> SearchByName(this IQueryable<Employee> employees,
            string nameToSearch)
        {
            if (string.IsNullOrWhiteSpace(nameToSearch))
                return employees;

            var lowerCaseName = nameToSearch.Trim().ToLower();

            return employees.Where(e => e.Name.ToLower().Contains(lowerCaseName));
        }

        public static IQueryable<Employee> Sort(this IQueryable<Employee> employees,
            string orderByQueryString)
        {
            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);

            if(string.IsNullOrWhiteSpace(orderQuery))
            {
                return employees.OrderBy(e => e.Name);
            }

            return employees.OrderBy(orderQuery);
        }
    }
}
