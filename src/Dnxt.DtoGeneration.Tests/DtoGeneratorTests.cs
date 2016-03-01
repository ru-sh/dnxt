using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dnxt.DtoGeneration.Transformations;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Dnxt.DtoGeneration.Tests
{
    public class A
    {
    }

    public class B
    {
    }

    [TestFixture]
    public class DtoGeneratorTests
    {
        readonly EntityModel _model = new EntityModel(
            "Test",
            new[]
            {
                new PropertyModel("A", typeof(string), new object[0]),
                new PropertyModel("BA", typeof(int), new object[0]),
                new PropertyModel("BB", typeof(int), new object[0])
            },
            new[]
            {
                new RefModel("RefQ", new EntityModel("Q", new[]
                {
                    new PropertyModel("X", typeof(string), new object[0]),
                    new PropertyModel("Y", typeof(string), new object[0])
                }, new RefModel[0], new object[0] ), new object[0])
            },
            new object[0]);

        [Test]
        public async Task B()
        {
            var t = new EntityMapping(_model)
                .Property("A1", "A")
                .Property<string, int>("B1", "B", s => int.Parse(s), i => i.ToString())
                .Reference("B", model => model.Property("A", "BA").Property("B", "BB"))
                .Property("RefQX", "RefQ.X")
                .Property("RefQY", "RefQ.Y")
                .Property("RefQ", "RefQ", "JsonConvert.Serialize(_)", "JsonConvert.Deserialize<{E}>(_)")
                .Property<decimal>("External")
                ;

            var destination = t.GenerateDestinationModel("Test2");
            //var mapping = t.GenerateMappingCode();

        }


        //[Test]
        public async Task A()
        {
            Expression<Func<EntityModel, EntityModel>> t = m => new EntityModel(m.Name, m.Properties, m.References, m.Attributes);

            var t1 = new Transformation<EntityModel>()
                .Set(entityModel => entityModel.Name, name => name + "Dto");

            var t2 = new Transformation<EntityModel>()
                .Set(entityModel => entityModel.Properties, entity => entity.Properties.Where(propertyModel => true).ToList())
                ;

            var transformation = t1
                .ForProperties(prop => prop.Name == "B", new ChangeType<int, string>(i => i.ToString(), s => int.Parse(s)))
                .ForProperties(prop => true, new ChangeName(name => name.Name + "1"));

            var dto = transformation.Apply(_model);


            Assert.AreEqual("TestDto", dto.Name, "entities name");
            Assert.AreEqual(_model.Properties.Count, dto.Properties.Count, "props count");

            var firstProp = dto.Properties[0];
            Assert.AreEqual("A1", firstProp.Name);
            Assert.AreEqual(typeof(string), firstProp.Type);

            var secondProp = dto.Properties[1];
            Assert.AreEqual("B1", secondProp.Name);
            Assert.AreEqual(typeof(string), secondProp.Type);



            var r = @"
namespace TestNs
{
    public class Test
    {
        public Test(string a, int b)
        {
        }

        property string A { get; }
        property int B { get; }
    }

    public class TestDto
    {
        public Test(string a1, int b1)
        {
        }

        property string A1 { get; }
        property string B1 { get; }
    }
}
";
        }

        public async Task A2()
        {
            var r = @"
namespace TestNs
{
    public static class TestConverter
    {
        public static Test(this TestDto src)
        {
            var a = src.A1;
            var b = int.Parse(src.B1);
            return new Test(a, b);
        }
    }
}
";
        }
    }

    public class PropertyMapping
    {
        public PropertyModel Prop { get; set; }
        public string F { get; set; }

        public PropertyMapping(PropertyModel prop, string f)
        {
            Prop = prop;
            F = f;
        }
    }
}