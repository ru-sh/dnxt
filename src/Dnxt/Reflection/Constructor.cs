using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Dnxt.Reflection
{
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