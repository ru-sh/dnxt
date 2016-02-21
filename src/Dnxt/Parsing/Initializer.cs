using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dnxt.Reflection;
using Dnxt.RxAsync;
using JetBrains.Annotations;

namespace Dnxt.Parsing
{
    public class Initializer<T> : Initializer, 
        IRouter<IDictionary<string, string>, T>, 
        IAsyncFunc<IDictionary<string, string>, T>
    {
        public Initializer() : base(typeof(T))
        {
        }

        public async Task<T> InvokeAsync(IDictionary<string, string> arg, CancellationToken cancellation)
        {
            var asyncFunc = await this.FindHandlerAsync(arg, cancellation);
            if (asyncFunc == null)
            {
                var ctors = Constructors
                    .Select(constructor => constructor.Parameters.Select(
                        info => info.Name + (Nullable.GetUnderlyingType(info.ParameterType) != null ? "?" : ""))
                        .ToList())
                    .ToList();

                throw new MatchConstructorNotFound(arg.Keys.ToList(), ctors);
            }

            return await asyncFunc(cancellation);
        }
        
        public Task<AsyncFunc<T>> FindHandlerAsync(IDictionary<string, string> args, CancellationToken cancellation)
        {
            var ctor = Constructors
              .Select(constructor => new
              {
                  constructor,
                  parameters = constructor.Parameters.Select(info =>
                  {
                      var underlyingType = Nullable.GetUnderlyingType(info.ParameterType);
                      var parameterType = underlyingType ?? info.ParameterType;
                      var value = args.ContainsKey(info.Name) ? args[info.Name] : null;
                      var isNullable = underlyingType != null;
                      var isFilled = isNullable || value != null;
                      return new
                      {
                          name = info.Name,
                          type = parameterType,
                          isNullable,
                          value,
                          isFilled
                      };
                  }).ToList()
              })
              .Where(arg => arg.parameters.All(pair => pair.isFilled))
              .OrderByDescending(arg => arg.parameters.Count)
              .FirstOrDefault();

            if (ctor != null)
            {
                AsyncFunc<T> builder = (token) =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        var parameters = ctor.parameters
                            .Select(info => Convert.ChangeType(info.value, info.type)).ToArray();

                        var instance = (T)ctor.constructor.ConstructorInfo.Invoke(parameters);
                        return instance;
                    }, token);
                };

                return Task.FromResult(builder);
            }

            return Task.FromResult<AsyncFunc<T>>(null);
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
}