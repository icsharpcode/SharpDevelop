// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			return (parameterTypeClass == null || 
			        (parameterTypeClass.ClassType != ClassType.Enum && parameterTypeClass.ClassType != ClassType.Struct));
		}
		
		public override string GetCodeToInsert(string parameterName)
		{
			return "if (" + parameterName + " == null)\n" +
				"throw new ArgumentNullException(\"" + parameterName + "\");";
		}
	}
}
