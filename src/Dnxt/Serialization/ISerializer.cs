using System;
using JetBrains.Annotations;

namespace Dnxt.Serialization
{
    public interface ISerializer<in T>
    {
        string Serialize(T obj);
    }

    public interface IDeserializer<out T>
    {
        T Deserialize(string obj);
    }
}