using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [Tokens("count")]
    public class Count : AggregateFunction<int>
    {
        protected override void AggregateValue(object value, AggregationState aggregationState, params object[] args)
        {
            bool val = TypeNormalizer.EnsureType<bool>(value);
            if (val)
            {
                var count = aggregationState.GetValue<int>("count");
                aggregationState["count"] = count + 1;
            }
        }
    	
  
        protected override int ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<int>("count");
        }
    }
}
