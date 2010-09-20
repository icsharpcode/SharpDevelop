// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Boo.Lang.Compiler.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding.CodeCompletion
{
	/// <summary>
	/// Return type that is inferred from an expression.
	/// </summary>
	public class BooInferredReturnType : ProxyReturnType
	{
		Expression expression;
		Block block;
		IReturnType cachedType;
		IClass context;
		
		public BooInferredReturnType(Expression expression, IClass context)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			this.context = context;
			this.expression = expression;
		}
		
		bool useLastStatementIfNoReturnStatement;
		
		public BooInferredReturnType(Block block, IClass context, bool useLastStatementIfNoReturnStatement)
		{
			if (block == null) throw new ArgumentNullException("block");
			this.useLastStatementIfNoReturnStatement = useLastStatementIfNoReturnStatement;
			this.block = block;
			this.context = context;
		}
		
		public override IReturnType BaseType {
			get {
				// clear up references to method/expression after the type has been resolved
				if (block != null) {
					GetReturnTypeVisitor v = new GetReturnTypeVisitor(this);
					Block b = block;
					block = null; // reset block before calling Visit to prevent StackOverflow
					v.Visit(b);
					if (v.noReturnStatement) {
						if (useLastStatementIfNoReturnStatement && v.lastExpressionStatement != null) {
							cachedType = new BooResolver().GetTypeOfExpression(v.lastExpressionStatement.Expression, context);
						} else {
							cachedType = ParserService.CurrentProjectContent.SystemTypes.Void;
						}
					} else if (v.result is NullReturnType) {
						cachedType = ConvertVisitor.GetDefaultReturnType(ParserService.CurrentProjectContent);
					} else {
						cachedType = v.result;
					}
				} else if (expression != null) {
					Expression expr = expression;
					expression = null;
					cachedType = new BooResolver().GetTypeOfExpression(expr, context);
				}
				return cachedType;
			}
		}
		
		class GetReturnTypeVisitor : DepthFirstVisitor
		{
			IClass context;
			BooInferredReturnType parentReturnType;
			public GetReturnTypeVisitor(BooInferredReturnType parentReturnType)
			{
				this.context = parentReturnType.context;
				this.parentReturnType = parentReturnType;
			}
			
			public IReturnType result;
			public bool noReturnStatement = true;
			public ExpressionStatement lastExpressionStatement;
			
			public override void OnReturnStatement(ReturnStatement node)
			{
				noReturnStatement = false;
				if (node.Expression == null) {
					result = ParserService.CurrentProjectContent.SystemTypes.Void;
				} else {
					result = new BooResolver().GetTypeOfExpression(node.Expression, context);
				}
			}
			
			public override void OnExpressionStatement(ExpressionStatement node)
			{
				base.OnExpressionStatement(node);
				lastExpressionStatement = node;
			}
			
			public override void OnYieldStatement(YieldStatement node)
			{
				noReturnStatement = false;
				
				IProjectContent pc = context != null ? context.ProjectContent : ParserService.CurrentProjectContent;
				IReturnType enumerable = new GetClassReturnType(pc, "System.Collections.Generic.IEnumerable", 1);
				
				// Prevent creating an infinite number of InferredReturnTypes in inferring cycles
				parentReturnType.expression = new NullLiteralExpression();
				IReturnType returnType;
				if (node.Expression == null)
					returnType = ConvertVisitor.GetDefaultReturnType(pc);
				else
					returnType = new BooResolver().GetTypeOfExpression(node.Expression, context);
				if (returnType != null) {
					returnType.GetUnderlyingClass(); // force to infer type
				}
				if (parentReturnType.expression == null) {
					// inferrence cycle with parentReturnType
					returnType = new GetClassReturnType(pc, "?", 0);
				}
				parentReturnType.expression = null;
				
				result = new ConstructedReturnType(enumerable, new IReturnType[] { returnType });
			}
			
			public override void OnBlockExpression(BlockExpression node)
			{
				// ignore return statements in callable blocks
			}
			
			public override bool Visit(Node node)
			{
				if (result != null && !(result is NullReturnType))
					return false;
				else
					return base.Visit(node);
			}
		}
	}
}
