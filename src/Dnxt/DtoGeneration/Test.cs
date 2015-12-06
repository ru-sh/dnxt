namespace Dnxt.DtoGeneration
{
    public class Test
    {
        private interface IFoo
        {
            int A { get; }
            string B { get; }
        }

        private class Foo : IFoo
        {
            public Foo(int a, string b)
            {
                A = a;
                B = b;
            }

            public int A { get; }
            public string B { get; }
        }

        public void A()
        {
            IFoo foo = new Foo(0, string.Empty);

            var updated = foo
                .Set(f => f.A, 1)
                .Set(f => f.B, "test")
                .Apply();
        }
    }
}