using System.Collections.Generic;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    public class Filter : AggregateFunction<IList<object>>
    {
        protected override void AggregateValue(object value, AggregationState aggregationState, object[] args)
        {
            if (!aggregationState.ContainsKey("results"))
                aggregationState["results"] = new List<object>();

            var pass = TypeNormalizer.EnsureType<bool>(value);
            if (pass && aggregationState.DataItem != null)
                aggregationState.GetValue<List<object>>("results").Add(aggregationState.DataItem);
            
        }

        protected override IList<object> ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<List<object>>("results");
        }
    }
}
