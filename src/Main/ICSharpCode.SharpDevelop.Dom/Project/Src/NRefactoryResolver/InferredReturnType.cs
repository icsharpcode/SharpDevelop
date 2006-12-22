// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	/// <summary>
	/// Used for the type of implicitly typed local variables in C# 3.0 or VB 9.
	/// </summary>
	public sealed class InferredReturnType : ProxyReturnType
	{
		NRefactoryResolver _resolver;
		Expression _expression;
		IReturnType _baseType;
		
		internal InferredReturnType(Expression expression, NRefactoryResolver resolver)
		{
			if (resolver == null)
				throw new ArgumentNullException("resolver");
			
			_expression = expression;
			_resolver = resolver;
		}
		
		public override IReturnType BaseType {
			get {
				if (_expression != null) {
					// prevent infinite recursion:
					Expression expr = _expression;
					_expression = null;
					
					ResolveResult rr = _resolver.ResolveInternal(expr, ExpressionContext.Default);
					if (rr != null) {
						_baseType = rr.ResolvedType;
					}
					
					_resolver = null;
				}
				return _baseType;
			}
		}
	}
}
