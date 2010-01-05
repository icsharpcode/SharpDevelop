using System.Collections.Generic;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [Tokens("MatchesAll", "matches_all")]
    public class MatchesAll : AggregateFunction<bool>
    {
        protected override void AggregateValue(object value, AggregationState aggregationState, object[] args)
        {
            var val = TypeNormalizer.EnsureType<bool>(value);
            aggregationState["value"] = val;
            if (val == false)
                aggregationState.CanReturn = true;
        }

        protected override bool ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<bool>("value");
        }
    }
}