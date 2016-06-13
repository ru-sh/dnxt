using System;
using System.Linq.Expressions;
using Dnxt.DtoGeneration;

namespace Dnxt.Tests
{
    public class PropMapping
    {
        public PropMapping(PropertyModel property, LambdaExpression expression)
        {
            Property = property;
            Expression = expression;
        }

        public PropertyModel Property { get; }
        public LambdaExpression Expression { get; }
    }
}