using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions;

namespace SimpleExpressionEvaluator.Compilation
{
    public class DefaultExpressionNodeFactory : IExpressionNodeFactory
    {
        private readonly ReaderWriterLock _lock = new ReaderWriterLock();

        private readonly Dictionary<ExpressionNodeType, Dictionary<string, Type>> _nodeTable =
            new Dictionary<ExpressionNodeType, Dictionary<string, Type>>();

        public DefaultExpressionNodeFactory()
        {
            RegisterAssembly(typeof (IExpression).Assembly);
        }

        #region IExpressionNodeFactory Members

        public IExpression CreateBinaryOperator(string token, IExpression leftArgument, IExpression rightArgument)
        {
            IExpression op = CreateExpressionNode(ExpressionNodeType.BinaryOperator, token);
            if (op == null)
            {
                //some error handling to allow end user to provide value would be nice here
                throw new Exception(token + " is not a valid BinaryOperator");
            }
            ((IExpressionNodeWithArguments) op).AcceptArguments(leftArgument, rightArgument);
            return op;
        }

        public IExpression CreateUnaryOperator(string token, IExpression argument)
        {
            IExpression op = CreateExpressionNode(ExpressionNodeType.UnaryOperator, token);
            if (op == null)
            {
                //some error handling to allow end user to provide value would be nice here
                throw new Exception(token + " is not a valid UnaryOperator");
            }
            ((IExpressionNodeWithArguments) op).AcceptArguments(argument);
            return op;
        }

        public IExpression CreateFunction(string functionName, params IExpression[] arguments)
        {
            IExpression function = CreateExpressionNode(ExpressionNodeType.Function, functionName);
            if (function == null)
            {
                return new UnknownFunction(functionName, arguments);
            }
            ((IExpressionNodeWithArguments) function).AcceptArguments(arguments);
            return function;
        }

        public Literal<T> CreateLiteral<T>(T literalValue)
        {
            return new Literal<T>(literalValue);
        }

        public Variable CreateVariable(string variableName)
        {
            return new Variable(variableName);
        }

        public QualifiedName CreateQualifiedName(string[] qualifiedName)
        {
            return new QualifiedName(qualifiedName);
        }

        #endregion

        public void RegisterAssembly(Assembly asm)
        {
            _lock.AcquireWriterLock(TimeSpan.FromSeconds(10));
            try
            {
                foreach (Type type in asm.GetTypes())
                {
                    var attribute = NodeTypeAttribute.Find(type);
                    if (attribute != null)
                    {
                        string[] tokenNames = GetTokenNamesFromType(type);
                        RegisterExpressionNodeType(type, attribute.NodeType, tokenNames);
                    }
                }
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        public void RegisterExpressionNodeType(Type expressionType, ExpressionNodeType nodeType,
                                               params string[] tokenNames)
        {
            _lock.AcquireWriterLock(TimeSpan.FromSeconds(10));
            try
            {
                Dictionary<string, Type> tokens;
                _nodeTable.TryGetValue(nodeType, out tokens);
                if (tokens == null)
                {
                    tokens = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);
                    _nodeTable[nodeType] = tokens;
                }

                foreach (string tokenName in tokenNames)
                {
                    tokens[tokenName] = expressionType;
                }
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        public Type GetExpressionNodeType(ExpressionNodeType nodeType, string tokenName)
        {
            _lock.AcquireReaderLock(TimeSpan.FromSeconds(10));
            try
            {
                Dictionary<string, Type> tokens;
                _nodeTable.TryGetValue(nodeType, out tokens);
                if (tokens == null) return null;
                if (tokens.ContainsKey(tokenName))
                    return tokens[tokenName];
                return null;
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

        protected IExpression CreateExpressionNode(ExpressionNodeType nodeType, string tokenName)
        {
            Type expressionType = GetExpressionNodeType(nodeType, tokenName);
            if (expressionType != null)
                return (IExpression) Activator.CreateInstance(expressionType);
            return null;
        }
 		
        
        public static string[] GetTokenNamesFromType(Type expressionNodeType)
        {
            List<string> tokenNames = TokensAttribute.FindTokens(expressionNodeType);
            if (tokenNames.Count < 1)
                return new[] {expressionNodeType.Name};
            return tokenNames.ToArray();
        }
      
    }
}