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
        //[Test]
        public void T()
        {
            var bclTypesParser = new BclTypesParser();
            var domainBuilder = new DomainBuilder("Test.Dto", type => bclTypesParser.GetParser(type) == null);
            var foo = new EntityModel("Foo", new[] { new PropertyModel("Index", typeof(int)), });
            var bar = new EntityModel("Bar", new List<PropertyModel>(){
                new PropertyModel("Foo", foo, new List<object>())
            }, new List<object>());

            domainBuilder
                .AddEntity(foo)
                .AddEntity(bar);

            domainBuilder
                .Transform(
                model => model.Name == foo.Name,
                new Transformation<EntityModel>().Map(
                    model => model.Properties,
                    model => model.Properties.Concat(new[]
                    {
                        new PropertyModel<string>("Name", null, Visibility.Public)
                    }).ToList()))
                    .ToList();

            var transformedDomain = domainBuilder.GetDomain();
            var csCodeGenerator = new CsCodeGenerator(transformedDomain);

            var barCode = string.Join(Environment.NewLine, csCodeGenerator.GetClass(bar.Name));
            var fooCode = string.Join(Environment.NewLine, csCodeGenerator.GetClass(foo.Name));
        }

        [Test]
        public void TestMapping()
        {
            var dtos = new[] {
                new EntityMapping<Foo>("FooDto")
                    .Map("BarId", foo => foo.Bar.Id)
                    .Map("BarName", foo => foo.Bar.Name)
                    .Map("Name", foo => "name"),

                new EntityMapping<Foo>("FooDb")
                    .Map("BarId", foo => foo.Bar.Id)
                    .Map("Name", foo => "name"),

            };

            var csMappingGenerator = new CsMappingGenerator();
            var mappingCode = csMappingGenerator.GetMappingCode(dtos);
        }
    }
}