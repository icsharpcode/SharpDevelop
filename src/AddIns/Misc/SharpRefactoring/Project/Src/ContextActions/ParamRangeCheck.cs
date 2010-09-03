// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
