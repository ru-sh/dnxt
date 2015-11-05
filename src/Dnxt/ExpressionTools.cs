using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Dnxt
{
    public static class ExpressionTools
    {
        public static string GetParameterName<T>(Expression<Func<T>> paramExpr)
        {
            var lambda = paramExpr as LambdaExpression;
            Debug.Assert(lambda != null);

            var paramRef = lambda.Body as MemberExpression;
            Debug.Assert(paramRef != null);
            
            // Get the parameter name
            string paramName = paramRef.Member.Name;
            return paramName;
        }
    }

}
