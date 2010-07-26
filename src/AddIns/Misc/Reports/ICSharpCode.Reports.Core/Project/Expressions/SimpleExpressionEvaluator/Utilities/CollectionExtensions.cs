using System.Collections.Generic;

namespace SimpleExpressionEvaluator.Utilities
{
    public static class CollectionExtensions
    {
        public static T GetValue<T>(this IDictionary<string,object> container, string key)
        {
            if (container == null)
                return (default(T));

            object val;
            container.TryGetValue(key, out val);

            return TypeNormalizer.EnsureType<T>(val);
        }
    }
}
