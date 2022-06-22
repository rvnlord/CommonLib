using System;
using CommonLib.Source.Common.Extensions;
using org.mariuszgromada.math.mxparser;

namespace CommonLib.Source.Common.Converters
{
    public static class MathExpressionConverter
    {
        public static Expression ToMathExpression(this object o)
        {
            var strExpr = o?.ToStringInvariant();
            if (strExpr.IsNullOrWhiteSpace())
                throw new NullReferenceException(nameof(o));
            return new Expression(strExpr);
        }
    }
}
