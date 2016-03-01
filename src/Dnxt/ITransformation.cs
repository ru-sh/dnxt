using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Dnxt
{
    public interface ITransformation<T>
    {
        [NotNull]
        Expression<Func<T, T>> Build();
    }
}