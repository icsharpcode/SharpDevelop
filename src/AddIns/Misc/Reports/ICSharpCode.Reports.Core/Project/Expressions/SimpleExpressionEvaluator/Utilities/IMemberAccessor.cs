using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Utilities
{
    public interface  IMemberAccessor
    {
        object GetValue(object target);
        bool IsStatic { get; }
        Type MemberType { get; }
    }
}
