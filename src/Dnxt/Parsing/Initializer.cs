using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
    public class Initializer<T> : Initializer
    {
        public Initializer() : base(typeof(T))
        {
        }
    }

    public class Initializer
    {
        [NotNull]
        public readonly IReadOnlyCollection<Constructor> Constructors;

        public Initializer(Type type)
        {
            var constructors = type.GetConstructors();
            Constructors = constructors.Select(info => new Constructor(info, info.GetParameters())).ToList();
        }

        [CanBeNull]
        public Constructor GetConstructor([NotNull] IReadOnlyCollection<string> paramNames)
        {
            if (paramNames == null) throw new ArgumentNullException(nameof(paramNames));

            var constructor = Constructors
                .OrderByDescending(c => c.Parameters.Count)
                .FirstOrDefault(c => c.Parameters.All(info => paramNames.Contains(info.Name)));

            return constructor;
        }
    }

    public class Constructor
    {
        public Constructor([NotNull] ConstructorInfo constructorInfo,
            [NotNull] IReadOnlyCollection<ParameterInfo> parameters)
        {
            if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            ConstructorInfo = constructorInfo;
            Parameters = parameters;
        }

        [NotNull]
        public ConstructorInfo ConstructorInfo { get; }

        [NotNull]
        public IReadOnlyCollection<ParameterInfo> Parameters { get; }
    }

}