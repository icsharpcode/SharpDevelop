using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Evaluation;
using SimpleExpressionEvaluator.Parser;

namespace SimpleExpressionEvaluator
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly Dictionary<string, IExpression> _expressionCache =
            new Dictionary<string, IExpression>(StringComparer.InvariantCultureIgnoreCase);

        private readonly ReaderWriterLock _rwLock = new ReaderWriterLock();


        public ExpressionEvaluator() : this(false)
        {
        }

        public ExpressionEvaluator(bool disableExpressionCaching) : this(new ExpressionCompiler(),disableExpressionCaching)
        {
        }

        public ExpressionEvaluator(IExpressionCompiler compiler) : this(compiler, false)
        {
        }

        public ExpressionEvaluator(IExpressionCompiler compiler,bool disableExpressionCaching)
        {
            if (compiler == null)
                throw new ArgumentNullException("compiler");

            UseExpressionCache = !disableExpressionCaching;

            Compiler = compiler;
        }

        public bool UseExpressionCache { get; set; }
        public IExpressionCompiler Compiler { get; private set; }

        public T EvaluateExpression<T>(string expression, object context)
        {
            return EvaluateExpression<T>(expression, new ExpressionContext(context));
        }

        public T EvaluateExpression<T>(string expression, IExpressionContext context)
        {
            IExpression<T> compiled = CompileExpression<T>(expression);
            return compiled.Evaluate(context);

        }

        public IExpression<T> CompileExpression<T>(string expression)
        {
            if (String.IsNullOrEmpty(expression))
                throw new ArgumentNullException("expression");

            if (!UseExpressionCache)
                return Compiler.CompileExpression<T>(expression);

            _rwLock.AcquireReaderLock(-1);
            try
            {
                string cacheKey = GetCacheKey(typeof (T), expression);

                if (_expressionCache.ContainsKey(cacheKey))
                    return (IExpression<T>) _expressionCache[cacheKey];

                IExpression<T> compiled = Compiler.CompileExpression<T>(expression);
                LockCookie cookie = _rwLock.UpgradeToWriterLock(-1);
                _expressionCache[cacheKey] = compiled;
                _rwLock.DowngradeFromWriterLock(ref cookie);
                return compiled;
            }
            finally
            {
                _rwLock.ReleaseReaderLock();
            }
        }

        protected static string GetCacheKey(Type expressionType,string expression)
        {
            return expression + ":" + expressionType.FullName;
        }
    }
}
