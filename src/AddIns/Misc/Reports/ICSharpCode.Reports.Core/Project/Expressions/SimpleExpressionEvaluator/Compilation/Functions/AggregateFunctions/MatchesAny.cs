using System.Collections.Generic;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [Tokens("MatchesAny","matches_any")]
    public class MatchesAny : AggregateFunction<bool>
    {
        protected override void AggregateValue(object value, AggregationState aggregationState, object[] args)
        {
            var val = TypeNormalizer.EnsureType<bool>(value);
            aggregationState["value"] = val;
            if (val)
                aggregationState.CanReturn = true;
        }

        protected override bool ExtractAggregateValue(AggregationState aggregationState)
        {
            return aggregationState.GetValue<bool>("value");
        }
    }
}
