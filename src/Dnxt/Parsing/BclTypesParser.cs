using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Dnxt.Parsing
{
    public class BclTypesParser
    {
        private readonly CultureInfo _cultureInfo;

        public BclTypesParser(CultureInfo cultureInfo = null)
        {
            _cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        private int IntParser(string s)
        {
            int result;
            if (!int.TryParse(s, NumberStyles.Integer, _cultureInfo, out result))
            {
                throw new Exception($"{s} is not a number.");
            }

            return result;
        }

        private double DoubleParser(string s)
        {
            double result;
            if (!double.TryParse(s, NumberStyles.AllowDecimalPoint, _cultureInfo, out result))
            {
                throw new Exception($"{s} is not a double.");
            }

            return result;
        }

        private decimal DecimalParser(string s)
        {
            decimal result;
            if (!decimal.TryParse(s, NumberStyles.AllowDecimalPoint, _cultureInfo, out result))
            {
                throw new Exception($"{s} is not a double.");
            }

            return result;
        }

        public Func<string, object> GetParser(Type type)
        {
            if (type == typeof(bool))
            {
                return s => bool.Parse(s);
            }

            if (type == typeof(short))
            {
                return s => short.Parse(s);
            }

            if (type == typeof(int))
            {
                return s => IntParser(s);
            }
            
            if (type == typeof(double))
            {
                return s => DoubleParser(s);
            }
            
            if (type == typeof(decimal))
            {
                return s => DecimalParser(s);
            }
            
            if (type == typeof(string))
            {
                return s => s;
            }
            
            if (type == typeof(DateTime))
            {
                return s => DateTime.Parse(s);
            }
            
            if (type == typeof(DateTimeOffset))
            {
                return s => DateTimeOffset.Parse(s);
            }
            
            if (type == typeof(bool))
            {
                return s => bool.Parse(s);
            }
            
            if (type == typeof(Guid))
            {
                return s => Guid.Parse(s);
            }

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsEnum)
            {
                return s => Enum.Parse(type, s, true);
            }

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                var itemType = type.GetGenericArguments().First();
                var subParser = GetParser(itemType);
                if (subParser != null)
                {
                    return s => string.IsNullOrWhiteSpace(s) ? null : subParser(s);
                }
            }
			
            return null;
        }

        public Func<string, T> GetParser<T>()
        {
            var type = typeof (T);
            var parser = GetParser(type);
            return s => (T) parser(s);
        }

        public  T Parse<T>(string s)
        {
            var type = typeof (T);
            var parser = GetParser(type);
            return (T) parser(s);
        }
    }
}