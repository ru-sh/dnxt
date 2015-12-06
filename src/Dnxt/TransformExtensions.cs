using System;
using System.Linq.Expressions;
using Dnxt.DtoGeneration;
using JetBrains.Annotations;

namespace Dnxt
{
#if DOTNET5_4
    public delegate TB Converter<in TA, out TB>(TA a);
#endif

    public static class TransformExtensions
    {
        [NotNull]
        public static TransformationContext<T> Set<T, TF>([NotNull] this T obj, Expression<Func<T, TF>> fieldGetter, TF newValue)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var t = new Transformation<T>().Set(fieldGetter, newValue);
            return new TransformationContext<T>(t, obj);
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
                _transformation.Set(propGetter, val);
                return this;
            }

            public T Apply()
            {
                return _transformation.Apply(_original);
            }
        }
    }

    public class ConvertersCollection
    {
        public static ConvertersCollection<string> PrimitiveToString = GetPrimitiveToString();

        private static ConvertersCollection<string> GetPrimitiveToString()
        {
            var collection = new ConvertersCollection<string>();

            collection.Add(ToString, bool.Parse);
            collection.Add(ToString, byte.Parse);

            collection.Add(ToString, ushort.Parse);
            collection.Add(ToString, uint.Parse);
            collection.Add(ToString, ulong.Parse);

            collection.Add(ToString, short.Parse);
            collection.Add(ToString, int.Parse);
            collection.Add(ToString, long.Parse);
            collection.Add(s => s, s => s);

            return collection;
        }

        private static string ToString<T>(T obj)
        {
            return obj.ToString();
        }
    }
}