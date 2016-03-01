using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Dnxt
{
    public static class TransformExtensions
    {
        [NotNull]
        public static TransformationContext<T> Set<T, TF>([NotNull] this T obj, Expression<Func<T, TF>> fieldGetter, TF newValue)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return new TransformationContext<T>(new Transformation<T>().Set(fieldGetter, o => newValue), obj);
        }

        [NotNull]
        public static TransformationContext<T> Set<T, TF>([NotNull] this T obj, Expression<Func<T, TF>> fieldGetter, Func<T, TF> newValue)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return new TransformationContext<T>(new Transformation<T>().Set(fieldGetter, newValue), obj);
        }

        public class TransformationContext<T>
        {
            [NotNull]
            private readonly Transformation<T> _transformation;

            [NotNull]
            private readonly T _original;

            public TransformationContext([NotNull] Transformation<T> transformation, [NotNull] T original)
            {
                if (transformation == null) throw new ArgumentNullException(nameof(transformation));
                if (original == null) throw new ArgumentNullException(nameof(original));

                _transformation = transformation;
                _original = original;
            }

            [NotNull]
            public TransformationContext<T> Set<TF>(Expression<Func<T, TF>> propGetter, TF val)
            {
                return new TransformationContext<T>(_transformation.Set(propGetter, o => val), _original);
            }

            [NotNull]
            public TransformationContext<T> Set<TF>(Expression<Func<T, TF>> propGetter, Func<T, TF> f)
            {
                return new TransformationContext<T>(_transformation.Set(propGetter, f), _original);
            }

            public T Apply()
            {
                return _transformation.Apply(_original);
            }
        }
    }
}