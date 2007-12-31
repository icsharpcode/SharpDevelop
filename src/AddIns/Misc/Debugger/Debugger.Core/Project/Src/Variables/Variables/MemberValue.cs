// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

using Ast = ICSharpCode.NRefactory.Ast;

namespace Debugger
{
	/// <summary>
	/// Represents a member of class or value type - 
	/// that is, a field or a property
	/// </summary>
	public class MemberValue: NamedValue
	{	
		MemberInfo memberInfo;
		
		/// <summary>
		/// Gets an MemberInfo object which can be used to obtain
		/// metadata information about the member.
		/// </summary>
		public MemberInfo MemberInfo {
			get {
				return memberInfo;
			}
		}
		
		internal MemberValue(MemberInfo memberInfo,
		                     Process process,
		                     IExpirable[] expireDependencies,
		                     IMutable[] mutateDependencies,
		                     CorValueGetter corValueGetter)
			:base (memberInfo.Name,
			       process,
			       new Ast.MemberReferenceExpression(
			           new Ast.IdentifierExpression("parent"), // TODO
			           memberInfo.Name
			       ),
			       expireDependencies,
			       mutateDependencies,
			       corValueGetter)
		{
			this.memberInfo = memberInfo;
		}
	}
}
