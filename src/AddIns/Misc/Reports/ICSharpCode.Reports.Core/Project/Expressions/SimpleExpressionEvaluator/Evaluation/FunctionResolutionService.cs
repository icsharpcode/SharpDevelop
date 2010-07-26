using System;
using System.Collections.Generic;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Evaluation
{
    public class FunctionResolutionService : IFunctionResolutionService
    {
        private Dictionary<string, Func<object[], object>> _functions;
        private List<IFunctionResolutionService> _resolutionServices;

        #region IFunctionResolutionService Members

        public Func<object[], object> ResolveFunction(string functionName)
        {
            Func<object[], object> found = null;
            if (_resolutionServices != null)
            {
                foreach (IFunctionResolutionService service in _resolutionServices)
                {
                    found = service.ResolveFunction(functionName);
                    if (found != null)
                        return found;
                }

                if (_functions != null)
                {
                    _functions.TryGetValue(functionName, out found);
                }
            }

            return found;
        }

        #endregion

        public void AddResolutionService(IFunctionResolutionService service)
        {
            if (_resolutionServices == null)
                _resolutionServices = new List<IFunctionResolutionService>();
            _resolutionServices.Add(service);
        }

        public void AddFunction(string functionName, Func<object[], object> function)
        {
            if (_functions == null)
                _functions = new Dictionary<string, Func<object[], object>>(StringComparer.OrdinalIgnoreCase);
            _functions[functionName] = function;
        }
    }
}