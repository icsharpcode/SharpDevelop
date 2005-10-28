// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using Boo.Lang.Compiler.Ast;

namespace Grunwald.BooBinding.CodeCompletion
{
	/// <summary>
	/// Return type that is inferred from an expression.
	/// </summary>
	public class InferredReturnType : ProxyReturnType
	{
		Expression expression;
		Block block;
		IReturnType cachedType;
		IClass context;
		
		public InferredReturnType(Expression expression, IClass context)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			this.context = context;
			this.expression = expression;
		}
		
		public InferredReturnType(Block block, IClass context)
		{
			if (block == null) throw new ArgumentNullException("block");
			this.block = block;
			this.context = context;
		}
		
		public override IReturnType BaseType {
			get {
				// clear up references to method/expression after the type has been resolved
				if (block != null) {
					GetReturnTypeVisitor v = new GetReturnTypeVisitor(context);
					v.Visit(block);
					block = null;
					if (v.noReturnStatement)
						cachedType = ReflectionReturnType.Void;
					else if (v.result is NullReturnType)
						cachedType = ReflectionReturnType.Object;
					else
						cachedType = v.result;
				} else if (expression != null) {
					cachedType = new BooResolver().GetTypeOfExpression(expression, context);
					expression = null;
				}
				return cachedType;
			}
		}
		
		class GetReturnTypeVisitor : DepthFirstVisitor
		{
			IClass context;
			public GetReturnTypeVisitor(IClass context)
			{
				this.context = context;
			}
			public IReturnType result;
			public bool noReturnStatement = true;
			
			public override void OnReturnStatement(ReturnStatement node)
			{
				noReturnStatement = false;
				if (node.Expression == null) {
					result = ReflectionReturnType.Void;
				} else {
					result = new BooResolver().GetTypeOfExpression(node.Expression, context);
				}
			}
			
			public override void OnYieldStatement(YieldStatement node)
			{
				noReturnStatement = false;
				result = ReflectionReturnType.CreatePrimitive(typeof(System.Collections.IEnumerable));
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
