using System;
using System.Linq.Expressions;

namespace Dnxt.DtoGeneration.Transformations
{
    public class ChangeType<T1, T2> : IPropTransformation
    {
        private readonly Expression<Func<T1, T2>> _a;
        private readonly Expression<Func<T2, T1>> _b;

        public ChangeType(Expression<Func<T1, T2>> a, Expression<Func<T2, T1>> b)
        {
            _a = a;
            _b = b;
        }

        public PropertyModel Apply(PropertyModel src)
        {
            var t1 = typeof(T1);
            if (src.Type != t1)
            {
                throw new Exception($"Type of {src.Name} property is not {t1}.");
            }

            return src.Set(model => model.Type, typeof(T2)).Apply();
        }

        public Expression<Func<PropertyModel, PropertyModel>> Build()
        {
            throw new NotImplementedException();
        }
    }
}