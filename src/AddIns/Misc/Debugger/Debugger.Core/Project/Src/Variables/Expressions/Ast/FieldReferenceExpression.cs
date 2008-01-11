// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger;

namespace ICSharpCode.NRefactory.Ast
{
	/// <summary>
	/// Reference to a class field
	/// </summary>
	public class FieldReferenceExpression: MemberReferenceExpression
	{
		FieldInfo fieldInfo;
		
		public FieldInfo FieldInfo {
			get { return fieldInfo; }
		}
		
		public FieldReferenceExpression(Expression targetObject, FieldInfo fieldInfo)
			:base (targetObject, fieldInfo.Name)
		{
			this.fieldInfo = fieldInfo;
		}
		
		public override string ToString() {
			return string.Format("[FieldReferenceExpression TargetObject={0} FieldName={1} TypeArguments={2}]", TargetObject, FieldName, GetCollectionString(TypeArguments));
		}
	}
}
