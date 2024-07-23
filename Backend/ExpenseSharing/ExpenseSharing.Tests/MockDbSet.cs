using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;

public class MockDbSet<T> : Mock<DbSet<T>> where T : class
{
    public MockDbSet(IEnumerable<T> data)
    {
        var queryableData = data.AsQueryable();

        As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
    }
}
