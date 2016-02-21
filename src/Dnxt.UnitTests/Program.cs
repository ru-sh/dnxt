using System;
using System.Linq;
using Dnxt.DtoGeneration;
using Dnxt.Parsing;

namespace Dnxt.UnitTests
{
    public class Program
    {
        static readonly Predicate<Type> PostgresReference = type => type != typeof(Type) && BclTypesParser.GetParser(type) == null;

        public static void Main(string[] args)
        {
            var domain = new Domain(PostgresReference);
            domain.AddEntityType<BankEvent>();


            var sqlSchemaGenerator = new SqlSchemaGenerator(domain);
            var sqlBuilder = sqlSchemaGenerator.GetSchemaSql();
            var sql = sqlBuilder.ToString();


            var parser = new RegexParser<BankEvent>(
    @"Pokupka, (?<agent>.*), karta \*(?<cardId>\d+), (?<date>\d+\.\d+\.\d+ \d+:\d+), (?<delta>\d+.\d+) rub. Dostupno = (?<available>\d+\.\d+)\s* rub");

        }

        private void TestDomainTransforming()
        {
            var domain = new Domain(PostgresReference);
            var foo = domain.AddEntityType<IFoo>();

            var dtos = domain
                .Transform(model => true, new Transformation<EntityModel>()
                    .Set(
                        model => model.Properties,
                        model => model.Properties.Select(propModel => propModel.Type == typeof (Type)
                            ? propModel
                            : propModel.Set(m => m.Type, typeof (string)).Apply()).ToList())
                    .Set(
                        model => model.Name,
                        model => model.Name.TrimStart('I') + "Dto")
                );

            foo = foo.Set(model => model.Name, "Foo").Apply();
            if (foo.Name != "Foo")
            {
                throw new Exception();
            }
        }

        private static void TestSetter()
        {
            IFoo foo = new Foo(0, string.Empty, typeof(bool), null);

            var updated = foo
                .Set(f => f.A, 1)
                .Set(f => f.B, "test")
                .Apply();

            Console.WriteLine(foo.A);
        }
    }
}
