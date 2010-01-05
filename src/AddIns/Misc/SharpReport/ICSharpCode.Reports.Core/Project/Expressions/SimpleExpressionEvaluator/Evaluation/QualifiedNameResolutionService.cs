using System;
using System.Collections.Generic;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Evaluation
{
    public class QualifiedNameResolutionService : IQualifiedNameResolutionService
    {
        private readonly Dictionary<string, PropertyPath> _propCache = new Dictionary<string, PropertyPath>();
        private List<IQualifiedNameResolutionService> _resolutionServices;

        #region IQualifiedNameResolutionService Members

        public object ResolveQualifiedName(object target, string[] qualifiedName)
        {
            if (_resolutionServices != null)
            {
                object found = null;
                foreach (IQualifiedNameResolutionService service in _resolutionServices)
                {
                    found = service.ResolveQualifiedName(target, qualifiedName);
                    if (found != null)
                        return found;
                }
            }

            Type targetType = null;
            if (target != null)
                targetType = target.GetType();
            else if (qualifiedName.Length > 1)
            {
                try
                {
                    targetType = Type.GetType(qualifiedName[0]);
                    var temp = new string[qualifiedName.Length - 1];
                    Array.Copy(qualifiedName, 1, temp, 0, qualifiedName.Length - 1);
                    qualifiedName = temp;
                }
                catch
                {
                }
            }

            if (targetType == null)
                return null;

            return ResolveQualifiedName(targetType, target, qualifiedName);
        }

        #endregion

        public void AddResolutionService(IQualifiedNameResolutionService resolutionService)
        {
            if (_resolutionServices == null)
                _resolutionServices = new List<IQualifiedNameResolutionService>();
            _resolutionServices.Add(resolutionService);
        }

        protected virtual object ResolveQualifiedName(Type targetType, object target, string[] name)
        {
            string cacheKey = PropertyPath.GetCacheKey(targetType, name);
            PropertyPath path = null;
            _propCache.TryGetValue(cacheKey, out path);

            if (path == null)
            {
                path = PropertyPath.Compile(targetType, name);
                if (path != null)
                    _propCache[cacheKey] = path;
            }

            if (path != null)
                return path.Evaluate(target);
            return null;
        }
    }
}