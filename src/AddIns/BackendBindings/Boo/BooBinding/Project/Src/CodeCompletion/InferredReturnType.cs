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
		
		public InferredReturnType(Expression expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			this.expression = expression;
		}
		
		public InferredReturnType(Block block)
		{
			if (block == null) throw new ArgumentNullException("block");
			this.block = block;
		}
		
		public override bool IsDefaultReturnType {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.IsDefaultReturnType : false;
			}
		}
		
		public override IReturnType BaseType {
			get {
				// clear up references to method/expression after the type has been resolved
				if (block != null) {
					GetReturnTypeVisitor v = new GetReturnTypeVisitor();
					v.Visit(block);
					block = null;
					if (v.noReturnStatement)
						cachedType = ReflectionReturnType.Void;
					else if (v.result is NullReturnType)
						cachedType = ReflectionReturnType.Object;
					else
						cachedType = v.result;
				} else if (expression != null) {
					cachedType = new BooResolver().GetTypeOfExpression(expression);
					expression = null;
				}
				return cachedType;
			}
		}
		
		class GetReturnTypeVisitor : DepthFirstVisitor
		{
			public IReturnType result;
			public bool noReturnStatement = true;
			
			public override void OnReturnStatement(ReturnStatement node)
			{
				noReturnStatement = false;
				if (node.Expression == null) {
					result = ReflectionReturnType.Void;
				} else {
					result = new BooResolver().GetTypeOfExpression(node.Expression);
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
