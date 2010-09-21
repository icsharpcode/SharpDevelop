// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
