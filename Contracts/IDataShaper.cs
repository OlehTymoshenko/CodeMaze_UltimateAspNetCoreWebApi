using System.Collections.Generic;
using Entities.Models;

namespace Contracts
{
    public interface IDataShaper<T> where T : class
    {
        ShapedEntity ShapeData(T entity, string reqiredProperties);
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string reqiredProperties);
    }
}
