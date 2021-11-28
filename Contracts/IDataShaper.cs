using System.Collections.Generic;
using System.Dynamic;

namespace Contracts
{
    public interface IDataShaper<T> where T : class
    {
        ExpandoObject ShapeData(T entity, string reqiredProperties);
        IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string reqiredProperties);
    }
}
