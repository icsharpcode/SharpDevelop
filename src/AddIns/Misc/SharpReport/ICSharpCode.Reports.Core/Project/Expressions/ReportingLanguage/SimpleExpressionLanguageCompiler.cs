using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleExpressionEvaluator;
using Irony.CompilerServices;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions;
using SimpleExpressionEvaluator.Compilation.Functions.AggregateFunctions;
using SimpleExpressionEvaluator.Utilities;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
    public class SimpleExpressionLanguageCompiler : IExpressionCompiler
    {
        protected DefaultExpressionNodeFactory ExpressionNodeFactory { get; set; }

        public SimpleExpressionLanguageCompiler(params Assembly[] assembliesToRegister)
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

        public virtual IExpression<T> CompileExpression<T>(string expression)
        {
        	/*
        	var lang = new SimpleExpressionLanguage();
        	var parser = new Compiler(lang);
        	ParseTree node = parser.Parse(expression);
        	return CompileExpression<T>(node.Root);
        	*/
        	return null;
        }

        #endregion

        protected virtual IExpression<T> CompileExpression<T>(ParseTreeNode root)
        {
            ParseTreeNode expr = root.ChildNodes[0];
            IExpression rootNode = CompileExpressionNode(ExpressionNodeFactory,expr);
            return new Expression<T>(rootNode);
            //Expression<T> exp = new Expression<T>();
        }

        
        protected virtual IExpression CompileExpressionNode(IExpressionNodeFactory factory,ParseTreeNode astNode)
        {
            switch (astNode.Term.Name)
            {
                case "Number":
                    var num = TypeNormalizer.EnsureType<double>(astNode.Token.Value);
                    return factory.CreateLiteral(num);
                case "String":
                    var str = TypeNormalizer.EnsureType<string>(astNode.Token.Value);
                    return factory.CreateLiteral(str);
                case "Boolean":
                    var bln = TypeNormalizer.EnsureType<bool>(astNode.Token.Value);
                    return factory.CreateLiteral(bln);
                case "Identifier":
                    var variable = astNode.Token.Value;
                    if (variable != null)
                        return factory.CreateVariable(variable.ToString());
                    break;
                case "Expr":
                    if (astNode.ChildNodes.Count == 3 &&
                        astNode.ChildNodes[1].Term is SymbolTerminal)
                    {
                        IExpression left = CompileExpressionNode(factory, astNode.ChildNodes[0]);
                        IExpression right = CompileExpressionNode(factory, astNode.ChildNodes[2]);
                        return factory.CreateBinaryOperator(astNode.ChildNodes[1].Term.Name, left, right);
                    }
                    if (astNode.ChildNodes.Count == 2 &&
                        astNode.ChildNodes[0].Term is SymbolTerminal)
                    {
                        IExpression arg = CompileExpressionNode(factory, astNode.ChildNodes[1]);
                        return factory.CreateUnaryOperator(astNode.ChildNodes[0].Term.Name, arg);
                    }
                    if (astNode.ChildNodes.Count == 2 &&
                        astNode.ChildNodes[1].Term is SymbolTerminal)
                    {
                        IExpression arg = CompileExpressionNode(factory, astNode.ChildNodes[0]);
                        return factory.CreateUnaryOperator(astNode.ChildNodes[1].Term.Name, arg);
                    }
                    break;
                case "QualifiedName":
                    var parts = new List<string>();

                    if (astNode.ChildNodes.Count == 2)
                        return new QualifiedName(new[] {astNode.ChildNodes[1].Token.ValueString});

                    //Condition ought to be impossible
                    if (astNode.ChildNodes.Count != 3 )
                        throw new Exception("Malformed QualifiedName - should have 3 child nodes");

                    ExtractQualifiedName(astNode, parts);
                    return new QualifiedName(parts.ToArray());
                case "FunctionExpression":
                    string functionName = (astNode.ChildNodes[0].Token.ValueString);
                    var args = new IExpression[astNode.ChildNodes[1].ChildNodes.Count];
                    for (int i = 0; i < astNode.ChildNodes[1].ChildNodes.Count; i++)
                    {
                        args[i] = CompileExpressionNode(factory, astNode.ChildNodes[1].ChildNodes[i]);
                    }

                    return factory.CreateFunction(functionName, args);
                case "IfThen":
                    IExpression condition = CompileExpressionNode(factory,astNode.ChildNodes[1]);
                    IExpression trueExpr = CompileExpressionNode(factory, astNode.ChildNodes[3]);
                    IExpression falseExpr = null;
                    if (astNode.ChildNodes.Count == 6)
                        falseExpr = CompileExpressionNode(factory, astNode.ChildNodes[5]);
                    var func = new IfThen();
                    if (falseExpr != null)
                        func.AcceptArguments(condition, trueExpr, falseExpr);
                    else
                        func.AcceptArguments(condition, trueExpr);
                    return func;
                case "ArrayExpression":
                    IExpression context = CompileExpressionNode(factory, astNode.ChildNodes[0]);
                    IExpression index = CompileExpressionNode(factory, astNode.ChildNodes[1]);
                    var indexer = new ItemAtIndex();
                    indexer.AcceptArguments(context, index);
                    return indexer;


            }
            return null;
        }

        public static void ExtractQualifiedName(ParseTreeNode currentNode,ICollection<string> parts)
        {
        	if (currentNode.ChildNodes.Count != 3){
        		parts.Add(currentNode.ChildNodes[0].Token.ValueString);
                return;
        	}

            //The inner most QualifiedName, both arguments are parts of the name
            //Not sure why a \b character started showing up on this guy???
            if (currentNode.ChildNodes[0].Term.Name != "QualifiedName")
            {
            	parts.Add(currentNode.ChildNodes[0].Token.ValueString);
                parts.Add(currentNode.ChildNodes[2].Token.ValueString);
            }
            else
            {
                ExtractQualifiedName(currentNode.ChildNodes[0], parts);
                parts.Add(currentNode.ChildNodes[2].Token.ValueString);
            }
        }
        
        public static void original_ExtractQualifiedName(ParseTreeNode currentNode,ICollection<string> parts)
        {
            if (currentNode.ChildNodes.Count != 3)
                return;

            //The inner most QualifiedName, both arguments are parts of the name
            //Not sure why a \b character started showing up on this guy???
            if (currentNode.ChildNodes[0].Term.Name.Replace("\b","") != "QualifiedName")
            {
            	parts.Add(currentNode.ChildNodes[0].Token.ValueString);
                parts.Add(currentNode.ChildNodes[2].Token.ValueString);
            }
            else
            {
                ExtractQualifiedName(currentNode.ChildNodes[0], parts);
                parts.Add(currentNode.ChildNodes[2].Token.ValueString);
            }
        }
    }
}