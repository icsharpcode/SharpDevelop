// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2285 $</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger;

namespace ICSharpCode.NRefactory.Ast
{
	/// <summary>
	/// Reference to a class property
	/// </summary>
	public class PropertyReferenceExpression: MemberReferenceExpression
	{
		PropertyInfo propertyInfo;
		
		public PropertyInfo PropertyInfo {
			get { return propertyInfo; }
		}
		
		public PropertyReferenceExpression(Expression targetObject, PropertyInfo propertyInfo)
			:base (targetObject, propertyInfo.Name)
		{
			this.propertyInfo = propertyInfo;
		}
		
		public override string ToString() {
			return string.Format("[PropertyReferenceExpression TargetObject={0} FieldName={1} TypeArguments={2}]", TargetObject, FieldName, GetCollectionString(TypeArguments));
		}
	}
}
