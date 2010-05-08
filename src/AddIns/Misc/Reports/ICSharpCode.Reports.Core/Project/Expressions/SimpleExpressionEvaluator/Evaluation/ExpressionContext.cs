using System;
using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Evaluation
{
    public class ExpressionContext : IExpressionContext
    {
        public ExpressionContext() : this(null)
        {
        }
       
        public ExpressionContext(object contextObject)
        {
            ContextObject = contextObject;
            Variables = new VariableResolutionService();
            QualifiedNames = new QualifiedNameResolutionService();
            Functions = new FunctionResolutionService();
        }


        public VariableResolutionService Variables { get; private set; }
        public QualifiedNameResolutionService QualifiedNames { get; private set; }
        public FunctionResolutionService Functions { get; private set; }

        public object ContextObject { get; set; }

        public event EventHandler<UnknownVariableEventArgs> ResolveUnknownVariable;
        public event EventHandler<UnknownQualifiedNameEventArgs> ResolveUnknownQualifiedName;
        public event EventHandler<UnknownFunctionEventArgs> ResolveMissingFunction;

        public object ResolveVariableValue(string variableName)
        {
            
            object value = Variables != null ? Variables.ResolveVariableValue(variableName) : null;

            if (value == null)
            {
                var args = new UnknownVariableEventArgs(variableName, ContextObject);
                var evt = ResolveUnknownVariable;
                if (evt != null)
                    evt(this, args);
                value = args.VariableValue;
            }

            return value;
        }

        public object ResolveQualifiedName(object target,string[] qualifiedName)
        {
            object value = QualifiedNames != null ? QualifiedNames.ResolveQualifiedName(target, qualifiedName) : null;

            if (value == null)
            {
                var args = new UnknownQualifiedNameEventArgs(qualifiedName, target);
                var evt = ResolveUnknownQualifiedName;
                if (evt != null)
                    evt(this, args);
                value = args.Value;
            }

            return value;
        }

        public Func<object[],object> ResolveFunction(string functionName)
        {
            Func<object[],object> function = Functions != null ? Functions.ResolveFunction(functionName) : null;

            if (function == null)
            {
                var args = new UnknownFunctionEventArgs(functionName, ContextObject);
                var evt = ResolveMissingFunction;
                if (evt != null)
                    evt(this, args);
                function = args.Function;
            }
            return function;
        }
    }

    public class UnknownVariableEventArgs:EventArgs
    {
        public UnknownVariableEventArgs(string variableName,object contextObject)
        {
            VariableName = variableName;
            ContextObject = contextObject;
        }

        public string VariableName { get; set; }
        public object ContextObject { get; set; }
        public object VariableValue { get; set; }
    }

    public class UnknownQualifiedNameEventArgs : EventArgs
    {
        public UnknownQualifiedNameEventArgs(string[] qualifiedName,object contextObject)
        {
            QualifiedName = qualifiedName;
            ContextObject = contextObject;
        }

        public string[] QualifiedName { get; set; }
        public object ContextObject { get; set; }
        public object Value { get; set; }

    }

    public class UnknownFunctionEventArgs : EventArgs
    {
        public UnknownFunctionEventArgs(string functionName,object contextObject)
        {
            FunctionName = functionName;
            ContextObject = contextObject;
        }

        public string FunctionName { get; set; }
        public object ContextObject { get; set; }
        public Func<object[],object> Function { get; set; }
    }
}