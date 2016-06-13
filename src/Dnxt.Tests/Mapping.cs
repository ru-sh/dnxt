using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dnxt.DtoGeneration;
using JetBrains.Annotations;

namespace Dnxt.Tests
{
    public interface IEntityMapping
    {
        IReadOnlyDictionary<PropertyModel, LambdaExpression> GetPropsMapping();
        EntityModel GetTargetEntity();
        Type GetSourceType();
    }

    public class EntityMapping<T> : IEntityMapping
    {
        [NotNull]
        private readonly string _entityName;

        [NotNull]
        public IReadOnlyDictionary<string, PropMapping> Props { get; }

        public EntityMapping(string entityName, IReadOnlyDictionary<string, PropMapping> props = null)
        {
            _entityName = entityName;
            Props = props ?? new Dictionary<string, PropMapping>();
        }

        public EntityMapping<T> Map<TProp>(string propName, Expression<Func<T, TProp>> expr)
        {
            var props = Props.ToDictionary(pair => pair.Key, pair => pair.Value);

            var propertyModel = new PropertyModel(propName, typeof(TProp));
            props.Add(propName, new PropMapping(propertyModel, expr));

            return new EntityMapping<T>(_entityName, props);
        }

        public EntityModel GetTargetEntity()
        {
            var props = Props.Select(pair => pair.Value.Property).ToList();
            return new EntityModel(_entityName, props);
        }

        public Type GetSourceType()
        {
            return typeof(T);
        }

        public IReadOnlyDictionary<PropertyModel, LambdaExpression> GetPropsMapping()
        {
            return Props.ToDictionary(pair => pair.Value.Property, pair => (LambdaExpression) pair.Value.Expression);
        }
    }
}