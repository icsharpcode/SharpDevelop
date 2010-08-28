// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Runtime.Remoting.Contexts;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of ParamRangeCheck.
	/// </summary>
	public class ParamRangeCheck : ParamCheck
	{
		public override string Title {
			get { return "Add range check"; }
		}
		
		public override bool IsAvailable(IReturnType parameterType)
		{
			IClass parameterTypeClass = parameterType.GetUnderlyingClass();
			return (parameterTypeClass != null && parameterTypeClass.FullyQualifiedName == "System.Int32");
		}
		
		public override string GetCodeToInsert(string parameterName)
		{
			return "if (" + parameterName + " < 0 || " + parameterName + " > upper_bound)\n" +
				"throw new ArgumentOutOfRangeException(\"" + parameterName + "\", " + parameterName + ", \"Value must be between 0 and \" + upper_bound);";
		}
	}
}
