using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
    public class Initializer<T>
    {
        public readonly IReadOnlyCollection<Constructor> Constructors;

        public Initializer()
        {
            var type = typeof(T);
            var constructors = type.GetConstructors();
            Constructors = constructors.Select(info => new Constructor(info, info.GetParameters())).ToList();
        }

        public T Initialize(Dictionary<string, object> parameters)
        {
            var matchCtor = Constructors.FirstOrDefault(tuple =>
                tuple.Parameters.All(paramInfo => parameters.ContainsKey(paramInfo.Name)));

            if (matchCtor != null)
            {
                var enumerable = matchCtor.Parameters.Select(param => parameters[param.Name]);
            }

            throw new NotImplementedException();
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
            public ConstructorInfo ConstructorInfo { get; private set; }

            [NotNull]
            public IReadOnlyCollection<ParameterInfo> Parameters { get; private set; }
        }
    }
}