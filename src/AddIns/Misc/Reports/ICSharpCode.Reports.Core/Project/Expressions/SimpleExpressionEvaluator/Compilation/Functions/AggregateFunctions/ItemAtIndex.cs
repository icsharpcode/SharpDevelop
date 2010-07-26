using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions
{
    [Tokens("item_at_index","ItemAtIndex")]
    public class ItemAtIndex : AggregateFunction<object>
    {
        protected override void AggregateValue(object value, AggregationState aggregationState, params object[] args)
        {
            var index = TypeNormalizer.EnsureType<int>(value);
            if (index == aggregationState.CurrentIndex)
            {
                aggregationState["found"] = true;
                aggregationState.CanReturn = true;
            }
        }

        protected override object ExtractAggregateValue(AggregationState aggregationState)
        {
            if (aggregationState.ContainsKey("found"))
                return aggregationState.DataItem;
            return null;
        }

        protected override int ExpectedArgumentCount
        {
            get
            {
                return 2;
            }
        }
    }
}
