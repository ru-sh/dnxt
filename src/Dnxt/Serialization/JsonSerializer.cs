using Newtonsoft.Json;

namespace Dnxt.Serialization
{
    public class JsonSerializer<TA> : ISerializer<TA>, IDeserializer<TA>
    {
        public string Serialize(TA obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public TA Deserialize(string obj)
        {
            return JsonConvert.DeserializeObject<TA>(obj);
        }
    }
}