using System.Collections.Generic;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [Tokens("sum")]
    public class Sum : AggregateFunction<double>
    {
        protected override void AggregateValue(object value, AggregationState aggregationState, params object[] args)
        {
            var sum = aggregationState.GetValue<double>("value");
            var nextVal = TypeNormalizer.EnsureType<double>(value);

            aggregationState["value"] = sum + nextVal;
        }

        protected override double ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<double>("value");
        }
    }
}
