using System.Collections.Generic;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [Tokens("average","avg")]
    public class Average : Sum
    {
        protected override double ExtractAggregateValue(AggregationState aggregationState)
        {
            double sum = base.ExtractAggregateValue(aggregationState);
            double count = aggregationState.CurrentIndex;
            if (count > 0)
                return sum/count;
            return 0.0;
        }
    }
}
