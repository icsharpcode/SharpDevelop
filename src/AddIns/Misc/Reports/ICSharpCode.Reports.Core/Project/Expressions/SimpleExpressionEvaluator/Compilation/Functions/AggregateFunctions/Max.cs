using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [Tokens("max","maximum")]
    public class Max : AggregateFunction<double>
    {
        protected override void AggregateValue(object value, AggregationState aggregationState, params object[] args)
        {
            var val = TypeNormalizer.EnsureType<double>(value);
            if (!aggregationState.ContainsKey("value"))
                aggregationState["value"] = val;
            else
            {
                var max = aggregationState.GetValue<double>("value");

                if (val > max)
                    max = val;

                aggregationState["value"] = max;
            }
        }

        protected override double ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<double>("value");
        }
    }
}
