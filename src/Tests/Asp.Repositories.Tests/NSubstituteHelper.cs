using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Asp.Repositories.Tests
{
    public static class NSubstituteHelper
    {
        public static DbSet<T> CreateMockDbSet<T>(IQueryable<T> data = null) where T : class
        {
            var mockSet = Substitute.For<DbSet<T>, IQueryable<T>, IAsyncEnumerable<T>>();
            if (data != null)
            {
                ((IAsyncEnumerable<T>)mockSet).GetEnumerator().Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
                ((IQueryable<T>)mockSet).Provider.Returns(new TestAsyncQueryProvider<T>(data.Provider));
                ((IQueryable<T>)mockSet).Expression.Returns(data.Expression);
                ((IQueryable<T>)mockSet).ElementType.Returns(data.ElementType);
                ((IQueryable<T>)mockSet).GetEnumerator().Returns(data.GetEnumerator());
            }
            return mockSet;
        }
    }
}