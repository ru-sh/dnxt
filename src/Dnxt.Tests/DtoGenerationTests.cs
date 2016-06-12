using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dnxt.DtoGeneration;
using Dnxt.Parsing;
using NUnit.Framework;

namespace Dnxt.Tests
{
    [TestFixture]
    public class DtoGenerationTests
    {
        [Test]
        public void T()
        {
            Expression<Func<Domain, string>> a1 = domain => domain.Namespace;
            Expression<Func<Domain, object>> a2 = domain => (object)domain.Namespace;

            var param = Expression.Parameter(typeof(Domain), "domain");
            Expression<Func<Domain, object>> a3 =
                Expression.Lambda<Func<Domain, object>>(
                Expression.Convert(
                    Expression.Property(param, "Namespace"),
                    typeof(object))
                , param);

            var bclTypesParser = new BclTypesParser();
            var domainBuilder = new DomainBuilder("Test.Dto", type => bclTypesParser.GetParser(type) == null);
            var foo = new EntityModel("Foo", new[] { new PropertyModel("Index", typeof(int)), });
            var bar = new EntityModel("Bar", new List<PropertyModel>(), new List<RefModel>()
            {
                new RefModel("Foo", foo, new List<object>())
            }, new List<object>());

            domainBuilder
                .AddEntity(foo)
                .AddEntity(bar);

            var transformed = domainBuilder
                .Transform(
                model => model.Name == foo.Name,
                new Transformation<EntityModel>().Map(
                    model => model.Properties,
                    model => model.Properties.Concat(new[] { new PropertyModel<string>("Name", null, Visibility.Public) }).ToList()))
                    .ToList();

            var tr = transformed.Select(result => result.Value.Mapping).ToList();

            var transformedDomain = domainBuilder.GetDomain();
            var csCodeGenerator = new CsCodeGenerator(transformedDomain);

            var barCode = string.Join(Environment.NewLine, csCodeGenerator.GetClass(bar.Name));
            var fooCode = string.Join(Environment.NewLine, csCodeGenerator.GetClass(foo.Name));

            var csMappingGenerator = new CsMappingGenerator(transformed);
            var mappingLines = csMappingGenerator.GetMappingCode(bar.Name).ToList();
            var mappingCode = string.Join(Environment.NewLine, mappingLines);
        }
    }

    public class CsMappingGenerator
    {
        private readonly IReadOnlyCollection<KeyValuePair<EntityModel, TransformationResult<EntityModel>>> _transformed;

        public CsMappingGenerator(IReadOnlyCollection<KeyValuePair<EntityModel, TransformationResult<EntityModel>>> transformed)
        {
            this._transformed = transformed;
        }

        public IEnumerable<string> GetMappingCode(string name)
        {
            var tr = _transformed.FirstOrDefault(pair => pair.Key.Name == name);

            yield return "object Map(object x){";
            foreach (var mapping in tr.Value.Mapping)
            {
                yield return $"\t{mapping.Key} = ({GetMappingCode(mapping.Value)})(x);";
            }
            yield return "}";
        }

        private string GetMappingCode(Expression<Func<EntityModel, object>> e)
        {
            var parameters = string.Join(", ", e.Parameters);
            return $"({parameters}) => " + GetMappingCode(e.Body);
        }

        private string GetMappingCode(Expression e)
        {
            var unaryExpression = e as UnaryExpression;
            if (unaryExpression != null)
            {
                return GetCode(unaryExpression);
            }

            var memberExpression = e as MemberExpression;
            if (memberExpression != null)
            {
                return GetCode(memberExpression);
            }

            return e.ToString();
        }

        private string GetCode(MemberExpression memberExpression)
        {
            return memberExpression.ToString();
        }

        private string GetCode(UnaryExpression e)
        {
            if (e.NodeType == ExpressionType.Convert)
            {
                return $"({e.Type}) {e.Operand}";
            }

            return e.ToString();
        }
    }
}