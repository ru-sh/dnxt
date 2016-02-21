using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Dnxt.Parsing
{
    public class BclTypesParser
    {
        private static int IntParser(string s)
        {
            int result;
            if (!int.TryParse(s, out result))
            {
                throw new Exception($"{s} is not a number.");
            }

            return result;
        }

        private static double DoubleParser(string s)
        {
            double result;
            if (!double.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
            {
                throw new Exception($"{s} is not a double.");
            }

            return result;
        }

        private static decimal DecimalParser(string s)
        {
            decimal result;
            if (!decimal.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
            {
                throw new Exception($"{s} is not a double.");
            }

            return result;
        }

        public static Func<string, object> GetParser(Type type)
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

        public static T Parse<T>(string s)
        {
            var type = typeof (T);
            var parser = GetParser(type);
            return (T) parser(s);
        }

        public static Func<string, T> GetParser<T>()
        {
            var type = typeof (T);
            var parser = GetParser(type);
            return s => (T) parser(s);
        }
    }
}