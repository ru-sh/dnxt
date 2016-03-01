using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration.Transformations
{
    public class ChangeName : IPropTransformation
    {
        [NotNull] private readonly Expression<Func<PropertyModel, string>> _a;

        public ChangeName([NotNull] Expression<Func<PropertyModel, string>> a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            _a = a;
        }

        public PropertyModel Apply(PropertyModel src)
        {
            var name = _a.Compile().Invoke(src);
            return src.Set(model => model.Name, name).Apply();
        }

        public Expression<Func<PropertyModel, PropertyModel>> Build()
        {
            Expression<Func<PropertyModel, PropertyModel>> expression = model => new PropertyModel(model.Name, model.Type, model.Attributes);
            return expression;
        }
    }
}