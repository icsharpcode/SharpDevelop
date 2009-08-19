// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IMethodOrProperty : IMember
	{
		IList<IParameter> Parameters {
			get;
		}
		
		bool IsExtensionMethod {
			get;
		}
	}
	
	public interface IMethod : IMethodOrProperty
	{
		IList<ITypeParameter> TypeParameters {
			get;
		}
		
		bool IsConstructor {
			get;
		}
		
		IList<string> HandlesClauses {
			get;
		}
	}
	
	public interface IMethod2 : IMethod
	{
		bool IsOperator {
			get;
		}
	}
	
	public static class Method2Compatibility
	{
		public static bool GetIsOperator(this IMethod method)
		{
			IMethod2 method2 = method as IMethod2;
			if (method2 != null)
				return method2.IsOperator;
			else
				return false;
		}
	}
}
