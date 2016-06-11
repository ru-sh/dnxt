using System;
using System.Collections.Generic;
using System.Linq;
using Dnxt.DtoGeneration;
using Dnxt.Parsing;
using NUnit.Framework;

namespace Dnxt.Tests
{
    [TestFixture]
    public class DtoGenerationTests
    {
        [Test]
        public void Test()
        {
            var bclTypesParser = new BclTypesParser();
            var domain = new Domain("Test.Dto", type => bclTypesParser.GetParser(type) == null);
            var foo = new EntityModel("Foo", new[] { new PropertyModel("Index", typeof(int)), });
            var bar = new EntityModel("Bar", new List<PropertyModel>(), new List<RefModel>()
            {
                new RefModel("Foo", foo, new List<object>())
            }, new List<object>());

            domain
                .AddEntity(foo)
                .AddEntity(bar);

            var transformed = domain
                .Transform(
                model => model.Name == foo.Name, 
                new Transformation<EntityModel>().Set(
                    model => model.Properties, 
                    model => model.Properties.Concat(new[] { new PropertyModel<string>("Name") }).ToList()));

            var csCodeGenerator = new CsCodeGenerator(transformed);

            var barCode = string.Join(Environment.NewLine, csCodeGenerator.GetClass(bar.Name));
            var fooCode = string.Join(Environment.NewLine, csCodeGenerator.GetClass(foo.Name));
        }
    }

    public class CsCodeGenerator
    {
        private readonly Domain _domain;

        public CsCodeGenerator(Domain domain)
        {
            _domain = domain;
        }

        public IEnumerable<string> GetClass(string name)
        {
            yield return $"namespace {_domain.Namespace} {{";
            var entity = _domain.Entities[name];
            var visibility = GetVisibility(entity.Visibility);
            yield return $"\t{visibility} class {entity.Name} {{ ";
            var classProps = entity.Properties.Cast<IPropertyModel>().Concat(entity.References).ToList();
            foreach (var p in GenerateCtor(entity, classProps)) yield return "\t\t" + p;
            yield return "";
            foreach (var p in classProps.SelectMany(GetProperty)) yield return "\t\t" + p;
            yield return "\t}";
            yield return "}";
        }

        private static IEnumerable<string> GenerateCtor(EntityModel entity, IReadOnlyCollection<IPropertyModel> classProps)
        {
            var ctorArgs = string.Join(", ", classProps.Select(p => p.TypeFullName + " " + p.Name));
            yield return $"public {entity.Name}({ctorArgs}) {{";
            foreach (var prop in classProps)
            {
                yield return $"\tthis.{prop.Name} = {prop.Name};";
            }
            yield return "}";
        }

        private IEnumerable<string> GetProperty(IPropertyModel p)
        {
            foreach (var attribute in p.Attributes)
            {
                yield return $"[{attribute.GetType().FullName}]";
            }

            var visibility = GetVisibility(p.Visibility);
            var s = $"{visibility} {p.TypeFullName} {p.Name}";
            if (p.HasGetter || p.HasSetter)
            {
                s += " {";
                if (p.HasGetter)
                {
                    s += " get;";
                }
                if (p.HasSetter)
                {
                    s += " set;";
                }
                s += " }";
            }
            yield return s;
        }

        private string GetVisibility(Visibility v)
        {
            switch (v)
            {
                case Visibility.Public:
                    return "public";
                case Visibility.Internal:
                    return "internal";
                case Visibility.Private:
                    return "private";
                default:
                    throw new ArgumentOutOfRangeException(nameof(v), v, null);
            }
        }
    }
}