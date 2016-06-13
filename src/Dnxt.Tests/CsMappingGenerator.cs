using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dnxt.DtoGeneration;
using Dnxt.Extensions;
using Dnxt.Parsing;

namespace Dnxt.Tests
{
    public class CsMappingGenerator
    {
        public string GetMappingCode(ICollection<IEntityMapping> mappings)
        {
            var methods = mappings.Select(mapping => GetMappingCode(
                mapping.GetSourceType(), 
                mapping.GetTargetEntity(), 
                mapping.GetPropsMapping()));

            return string.Join(Environment.NewLine + Environment.NewLine, methods.Select(lines => string.Join(Environment.NewLine, lines)));
        }

        private IEnumerable<string> GetMappingCode(
            Type sourceType,
            EntityModel dstModel,
            IReadOnlyDictionary<PropertyModel, LambdaExpression> propsMappings)
        {
            var dstTypeName = dstModel.Name;

            yield return $"public static {dstTypeName} Map(this {sourceType.Name} x){{";
            foreach (var mapping in propsMappings)
            {
                yield return $"\tvar {mapping.Key.Name.ToCamelCase()} = ({GetMappingCode(mapping.Value)})(x);";
            }


            var args = dstModel.Properties.Select(model => model.Name.ToCamelCase());
            yield return $"\treturn new {dstTypeName}({string.Join(", ", args)})";
            yield return "}";
        }
        
        private string GetMappingCode(Expression e)
        {
            var unaryExpression = e as UnaryExpression;
            if (unaryExpression != null)
            {
                return GetCode(unaryExpression);
            }

            var memberExpression = e as MemberExpression;
            if (memberExpression != null)
            {
                return GetCode(memberExpression);
            }

            return e.ToString();
        }

        private string GetCode(MemberExpression memberExpression)
        {
            return memberExpression.ToString();
        }

        private string GetCode(UnaryExpression e)
        {
            if (e.NodeType == ExpressionType.Convert)
            {
                return $"({e.Type}) {e.Operand}";
            }

            return e.ToString();
        }
    }
}