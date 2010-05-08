using SimpleExpressionEvaluator.Evaluation;

namespace SimpleExpressionEvaluator.Compilation
{
    [NodeType(ExpressionNodeType.QualifiedName)]
    public class QualifiedName : ExpressionNode<object>
    {
        private IExpression _context;

        public QualifiedName(params string[] name) : this(null, name)
        {
        }

        public QualifiedName(IExpression context,params string[] name) : base(ExpressionNodeType.QualifiedName)
        {
            Name = name;
            _context = context;
        }

        public string[] Name { get; protected set; }

        public override object Evaluate(IExpressionContext context)
        {
            if (context != null)
            {
                object contextObject = null;

                if (_context != null)
                    contextObject = _context.Evaluate(context);
                if (contextObject == null)
                    contextObject = context.ContextObject;

                return context.ResolveQualifiedName(contextObject, Name);
            }
            return null;
        }
    }
}