using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions;
using SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions;
using SimpleExpressionEvaluator.Utilities;

namespace SimpleExpressionEvaluator.Parser
{
    public class ExpressionCompiler : IExpressionCompiler
    {
        protected DefaultExpressionNodeFactory ExpressionNodeFactory { get; set; }
        

        public ExpressionCompiler(params Assembly[] assembliesToRegister)
        {
            ExpressionNodeFactory = new DefaultExpressionNodeFactory();
            if (assembliesToRegister != null)
                Array.ForEach(assembliesToRegister,RegisterAssembly);
        }

        public void RegisterAssembly(Assembly asm)
        {
            ExpressionNodeFactory.RegisterAssembly(asm);
        }

        #region IExpressionCompiler Members

        public IExpression<T> CompileExpression<T>(string expression)
        {
            var s = new Scanner();
            var parser = new Parser(s);

            ParseTree ast = parser.Parse(expression);

            return CompileExpression<T>(ast);
        }

        #endregion

        protected virtual IExpression<T> CompileExpression<T>(ParseNode root)
        {
            ParseNode expr = root.Nodes[0].Nodes[0];
            IExpression rootNode = CompileExpressionNode(ExpressionNodeFactory,expr);
            return new Expression<T>(rootNode);
            //Expression<T> exp = new Expression<T>();
        }

        protected virtual IExpression CompileExpressionNode(IExpressionNodeFactory factory,ParseNode astNode)
        {
            switch (astNode.Token.Type)
            {
                case TokenType.Atom:
                    if (astNode.Nodes[0].Token.Type == TokenType.BROPEN)
                        return CompileExpressionNode(factory, astNode.Nodes[1]);
                    return CompileExpressionNode(factory, astNode.Nodes[0]);
                case TokenType.NUMBER:
                case TokenType.INTEGER:
                    var num = TypeNormalizer.EnsureType<double>(astNode.Token.Text);
                    return factory.CreateLiteral(num);
                case TokenType.STRING:
                    var str = TypeNormalizer.EnsureType<string>(astNode.Token.Text.Replace("\"",String.Empty));
                    return factory.CreateLiteral(str);
                case TokenType.TRUE:
                case TokenType.FALSE:
                    var bln = TypeNormalizer.EnsureType<bool>(astNode.Token.Text);
                    return factory.CreateLiteral(bln);
                case TokenType.NULL:
                    return null;
                case TokenType.BooleanExpr:
                case TokenType.CompareExpr:
                case TokenType.AddExpr:
                case TokenType.MultExpr:
                case TokenType.PowerExpr:
                    return CompileBinaryOperator(factory,astNode);
                case TokenType.QualifiedName:
                    return CompileQualifiedName(factory, null, Select(astNode.Nodes, TokenType.Identifier), 0);
                case TokenType.Conditional:
                    var pieces = Select(astNode.Nodes, TokenType.BooleanExpr);
                    IExpression condition = CompileExpressionNode(factory,pieces[0]);
                    IExpression trueExpr = CompileExpressionNode(factory,pieces[1]);
                    IExpression falseExpr = (pieces.Count == 3) ? CompileExpressionNode(factory, pieces[2]) : null;
                    var func = new IfThen();
                    if (falseExpr != null)
                        func.AcceptArguments(condition, trueExpr, falseExpr);
                    else
                    {
                        func.AcceptArguments(condition, trueExpr);
                    }
                    return func;
                case TokenType.UnaryExpr:
                    string token = astNode.Nodes[0].Token.Text;
                    IExpression left = CompileBinaryOperator(factory, astNode.Nodes[1]);
                    return factory.CreateUnaryOperator(token, left);

            }
            return null;
        }

        private IExpression CompileBinaryOperator(IExpressionNodeFactory factory,ParseNode opNode)
        {
            if (opNode.Nodes.Count == 0)
                return null;
            if (opNode.Nodes.Count == 1)
                return CompileExpressionNode(factory, opNode.Nodes[0]);
            if (opNode.Nodes.Count >= 3)
            {
                int curIndex = 1;
                IExpression curLeft = CompileExpressionNode(factory, opNode.Nodes[0]);

                while (curIndex < opNode.Nodes.Count)
                {
                    string token = opNode.Nodes[curIndex].Token.Text;
                    IExpression right = CompileExpressionNode(factory, opNode.Nodes[curIndex + 1]);
                    curLeft = factory.CreateBinaryOperator(token, curLeft, right);
                    curIndex += 2;
                }
                return curLeft;
            }
            throw new Exception(opNode.Text + " has two children. Operators must have three or more children.");



        }
        private static IList<ParseNode> Select(IEnumerable<ParseNode> source,params TokenType[] types)
        {
            var found = new List<ParseNode>();
            foreach (ParseNode node in source)
            {
                foreach(TokenType type in types)
                {
                    if (node.Token.Type == type)
                        found.Add(node);
                }
            }
            return found;
        }

        private IExpression CompileQualifiedName(IExpressionNodeFactory factory,IExpression curContext,IList<ParseNode> identifiers,int startIndex)
        {
            if (startIndex >= identifiers.Count)
                return curContext;

            int curIndex = startIndex;

            ParseNode curNode = identifiers[curIndex].Nodes[0];
            var nameParts = new List<string>();

            while(curNode != null && curNode.Token.Type == TokenType.IDENTIFIER)
            {
                nameParts.Add(curNode.Token.Text);
                curIndex += 1;
                curNode = (curIndex < identifiers.Count) ? identifiers[curIndex].Nodes[0] : null;
            }

            if (curNode != null)
            {
                if (curNode.Token.Type == TokenType.Array)
                {
                    nameParts.Add(curNode.Nodes[0].Token.Text);
                    IExpression arrayContext = new QualifiedName(curContext, nameParts.ToArray());
                    
                    var arrayFunc = new ItemAtIndex();
                    IExpression index = CompileExpressionNode(factory, curNode.Nodes[2]);
                    arrayFunc.AcceptArguments(arrayContext,index);
                    return CompileQualifiedName(factory, arrayFunc, identifiers, curIndex+1);
                }
                
                if (curNode.Token.Type == TokenType.Method)
                {
                    var methodContext = nameParts.Count > 0 ? new QualifiedName(curContext,nameParts.ToArray()) : curContext;
                    string methodName = curNode.Nodes[0].Token.Text;

                    IList<ParseNode> argNodes = Select(curNode.Nodes[2].Nodes, TokenType.BooleanExpr);
                    var args = new List<IExpression>();
                    if (methodContext != null)
                        args.Add(methodContext);
                    foreach (ParseNode argNode in argNodes)
                        args.Add(CompileExpressionNode(factory, argNode));

                    return CompileQualifiedName(factory,factory.CreateFunction(methodName, args.ToArray()),identifiers,curIndex+1);
                }
            }

            if (curContext == null && nameParts.Count == 1)
                return factory.CreateVariable(nameParts[0]);
            return new QualifiedName(curContext, nameParts.ToArray());
        }
        
    }
}
