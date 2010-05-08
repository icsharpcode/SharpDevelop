using SimpleExpressionEvaluator;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
    public class SimpleExpressionEval : ExpressionEvaluator
    {
        public SimpleExpressionEval() : this(false)
        {
        }

        public SimpleExpressionEval(bool disableExpressionCaching)
            : base(new SimpleExpressionLanguageCompiler(), disableExpressionCaching)
        {
        }
    }
}
