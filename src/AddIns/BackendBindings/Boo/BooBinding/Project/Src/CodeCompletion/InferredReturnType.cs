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
		
		public InferredReturnType(Expression expression)
		{
			this.expression = expression;
		}
		
		bool needInfer = true;
		IReturnType cachedType;
		
		public override IReturnType BaseType {
			get {
				if (needInfer) {
					needInfer = false;
					cachedType = new BooResolver().GetTypeOfExpression(expression);
				}
				return cachedType;
			}
		}
		
		public override bool IsDefaultReturnType {
			get {
				IReturnType baseType = BaseType;
				return (baseType != null) ? baseType.IsDefaultReturnType : false;
			}
		}
	}
}
