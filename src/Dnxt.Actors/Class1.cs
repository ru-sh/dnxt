using System;
using System.Threading.Tasks;

namespace Dnxt.Actors
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Class1
    {
        public Class1()
        {
        }

        public void A(GeneratedTestActorService actorService)
        {
            actorService.GetActor();
        }
    }

    public class GeneratedTestActorService : IActorService<ITestActor, Guid>
    {
        public ITestActor GetActor()
        {
            throw new NotImplementedException();
        }

        public ITestActor GetActor(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    public interface IActorService<out T, in TId> where T : IActor<TId>
    {
        T GetActor();
        T GetActor(TId id);
    }

    public interface ITestActor : IActor<Guid>
    {
        Task<int> Foo(string str, bool b);
        Task Bar(decimal d);
    }

    public interface IActor<TId>
    {
    }
}
