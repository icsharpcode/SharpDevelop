// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ParamCheckForNull.
	/// </summary>
	public class ParamCheckForNull : ParamCheck
	{
		public override string Title {
			get { return "Add check for null"; }
		}
		
		public override bool IsAvailable(IReturnType parameterType)
		{
			var parameterTypeClass = parameterType.GetUnderlyingClass();
			return (parameterTypeClass == null || parameterTypeClass.ClassType != ClassType.Enum && parameterTypeClass.ClassType != ClassType.Struct);
		}
		
		public override string GetCodeToInsert(string parameterName)
		{
			return "if (" + parameterName + " == null)\n" +
				"throw new ArgumentNullException(\"" + parameterName + "\");";
		}
	}
}
