using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace Dnxt.DtoGeneration
{
    public class ConvertersCollection<TB>
    {
        [NotNull]
        readonly ConcurrentDictionary<Type, Tuple<Converter<TB, object>, Converter<object, TB>>> _converters
            = new ConcurrentDictionary<Type, Tuple<Converter<TB, object>, Converter<object, TB>>>();

        public void Add<TA>([NotNull] Converter<TA, TB> a, [NotNull] Converter<TB, TA> b)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            Converter<TB, object> toA = o => b(o);
            Converter<object, TB> toB = i => a((TA)i);

            var type = typeof(TA);
            var succesful = _converters.TryAdd(type, Tuple.Create(toA, toB));
            if (!succesful)
            {
                throw new ArgumentException($"Converters for '{type.FullName}' are registered already.");
            }
        }

        public Tuple<Converter<TB, TA>, Converter<TA, TB>> Get<TA>()
        {
            var type = typeof(TA);
            var tuple = _converters[type];
            if (tuple == null)
            {
                throw new ArgumentException($"Converters for '{type.FullName}' are not registered.");
            }

            Converter<TB, TA> toA = b => (TA)tuple.Item1(b);
            Converter<TA, TB> toB = a => tuple.Item2(a);
            return Tuple.Create(toA, toB);
        }
    }
}