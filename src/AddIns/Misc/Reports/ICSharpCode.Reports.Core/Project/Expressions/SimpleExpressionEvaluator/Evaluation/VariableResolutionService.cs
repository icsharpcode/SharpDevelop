using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Evaluation
{
    public class VariableResolutionService : IVariableResolutionService
    {
        private List<IVariableResolutionService> _resolutionServices;
        private Dictionary<string, object> _variables;

        public object ResolveVariableValue(string variableName)
        {
            object found = null;
            if (_resolutionServices != null)
            {
                foreach (IVariableResolutionService service in _resolutionServices)
                {
                    found = service.ResolveVariableValue(variableName);
                    if (found != null)
                        return found;
                }

                if (_variables != null)
                {
                    _variables.TryGetValue(variableName,out found);
                    if (found != null && found is Func<object>)
                        return ((Func<object>) found)();
                }

            }

            return found;
        }

        public void AddResolutionService(IVariableResolutionService service)
        {
            if (_resolutionServices == null)
                _resolutionServices = new List<IVariableResolutionService>();
            _resolutionServices.Add(service);
        }

        public void AddVariable(string variableName,object variableValue)
        {
            if (_variables == null)
                _variables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            _variables[variableName] = variableValue;
        }

        public void AddDynamicVariable(string variableName,Func<object> resolver)
        {
            AddVariable(variableName, resolver);
        }

        public void AddVariableTable(IDictionary<string,object> variables)
        {
            foreach (string key in variables.Keys)
            {
                AddVariable(key, variables[key]);
            }
        }
    }
}